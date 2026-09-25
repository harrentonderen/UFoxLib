using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Landfall.TABS;
using Landfall.TABS.GameMode;
using Landfall.TABS.GameState;
using Landfall.TABS.AI.Systems;
using Unity.Entities;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Threading;

namespace UFoxLib.UI
{
    public class TeamHealthbar : GameStateListener
    {
        private void Start()
        {
            m_codeAnimation = GetComponent<CodeAnimation>();
            m_teamSystem = World.Active.GetOrCreateManager<TeamSystem>();
            m_latestDeathTimestamp = Time.unscaledTime;
            m_firstDeathTimestampAfterRecover = Time.unscaledTime;
        }

        public override void OnEnterPlacementState()
        {
            if (_spawnnedOnScene) Reset();
            if (!_spawnnedOnScene) _spawnnedOnScene = true;
            m_codeAnimation.PlayOut();
        }

        public override void OnEnterBattleState()
        {
            Init();
            m_codeAnimation.PlayIn();
        }

        private void OnEnable()
        {
            base.StartCoroutine(Recover());
        }

        public void Init()
        {
            m_teamSystem.AddOnUnitDeadListener(OnUnitDeath);

            m_healthRed = m_teamSystem.GetTeamUnits(Team.Red).Count;
            m_healthBlue = m_teamSystem.GetTeamUnits(Team.Blue).Count;
            base.StopAllCoroutines();

            var num = m_healthRed + m_healthBlue;
            m_barRed.fillAmount = m_healthRed / num;
            m_barBlue.fillAmount = m_healthBlue / num;
            base.StartCoroutine(Recover());
        }

        void Reset()
        {
            m_teamSystem.RemoveOnUnitDeadListener(OnUnitDeath);
            m_healthOverflowRed = 0f;
            m_healthOverflowBlue = 0f;
            m_latestDeathTimestamp = 0f;
            m_firstDeathTimestampAfterRecover = 0f;
        }

        public void OnUnitDeath(Unit unit)
        {
            switch (unit.Team)
            {
                case Team.Blue:
                    AttackTeam(1f, Team.Blue);
                    break;
                default:
                    AttackTeam(1f, Team.Red);
                    break;
            }
        }

        public void WhenBattleIsOver(Team whoWon)
        {
            switch (whoWon)
            {
                case Team.Blue:
                    AttackTeam(m_healthRed, Team.Red);
                    break;
                default:
                    AttackTeam(m_healthBlue, Team.Blue);
                    break;
            }
        }

        //true = red, false = blue
        public void AttackTeam(float _damage, Team m_team, bool clearOverflow = false)
        {
            m_whiteBarFlashAnim.StopFade(false);
            Team _teamToDamage = m_team;
            if (m_recovering || IsForceRecoverTimer)
            {
                switch (_teamToDamage)
                {
                    case Team.Blue:
                        m_healthOverflowBlue += 1f;
                            break;
                    default:
                        m_healthOverflowRed += 1f;
                            break;
                }
            }
            else
            {
                m_healthRed = Mathf.Max(m_healthRed - _damage, 0f);
                if (_slidingRedCoroutine != null) base.StopCoroutine(_slidingRedCoroutine);
                _slidingRedCoroutine = base.StartCoroutine(SlideAway(m_healthRed, m_healthBlueOld, m_barRed, m_team));

                m_whiteBarFlashAnim.FadeOut();

                if (clearOverflow)
                {
                    switch (_teamToDamage)
                    {
                        case Team.Blue:
                            m_healthOverflowBlue = 0f;
                            break;
                        default:
                            m_healthOverflowRed = 0f;
                            break;
                    }
                }
            }
        }

        private IEnumerator SlideAway(float health, float oppositeHealth, Image bar, Team m_team)
        {
            if (m_team == Team.Red) m_slidingRed = true;
            else m_slidingBlue = true;

            m_latestDeathTimestamp = Time.unscaledTime;

            if (!m_firstAttackAfterRecover)
            {
                m_firstAttackAfterRecover = true;
                m_firstDeathTimestampAfterRecover = Time.unscaledTime;
            }

            float targetFill = health / (health + oppositeHealth);
            float startFill = bar.fillAmount;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime * 2f;
                bar.fillAmount = FillLerp(startFill, targetFill, m_damageCurve.Evaluate(t));
                yield return new WaitForEndOfFrame();
            }

            bar.fillAmount = targetFill;

            if (m_team == Team.Red && m_slidingRed) m_slidingRed = false;
            else if (m_slidingBlue) m_slidingBlue = false;

            yield break;
        }

        private IEnumerator Recover()
        {
            m_recovering = false;
            var totalHP_old = m_healthRed + m_healthBlue;
            m_healthRedOld = m_healthRed;
            m_healthBlueOld = m_healthBlue;

            for (; ; )
            {
                if (!m_recovering)
                {
                    if (m_healthOverflowRed > 0f && !m_isSlidingRed) AttackTeam(m_healthOverflowRed, Team.Red, true);
                    if (m_healthOverflowBlue > 0f && !m_isSlidingBlue) AttackTeam(m_healthOverflowBlue, Team.Blue, true);
                }
                var totalHP_new = m_healthRed + m_healthBlue;

                bool timer = Time.unscaledTime - m_latestDeathTimestamp > 0.4f || (IsForceRecoverTimer && !m_isSlidingRed && !m_isSlidingBlue);

                if (totalHP_new != totalHP_old && !m_recovering && timer)
                {
                    m_recovering = true;
                    var oldRatio_red = m_barRed.fillAmount;
                    var oldRatio_blu = m_barBlue.fillAmount;
                    var newRatio_red = m_healthRed / totalHP_old;
                    var newRatio_blu = m_healthBlue / totalHP_old;

                    var t = 0f;
                    while (t < 1f)
                    {
                        t += Time.unscaledDeltaTime * 2f;
                        m_barRed.fillAmount = Mathf.Lerp(oldRatio_red, newRatio_red, m_recoverCurve.Evaluate(t));
                        m_barBlue.fillAmount = Mathf.Lerp(oldRatio_blu, newRatio_blu, m_recoverCurve.Evaluate(t));
                        yield return null;
                    }

                    m_barRed.fillAmount = newRatio_red;
                    m_barBlue.fillAmount = newRatio_blu;
                    totalHP_old = totalHP_new;
                    m_healthRedOld = m_healthRed;
                    m_healthBlueOld = m_healthBlue;
                    m_recovering = false;
                    m_firstAttackAfterRecover = false;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        private float FillLerp(float num1, float num2, float time)
        {
            return (1f - time) * num1 + time * num2;
        }

        private bool IsForceRecoverTimer
            { get { return m_firstAttackAfterRecover && Time.unscaledTime - m_firstDeathTimestampAfterRecover > 1.5f; } }

        TeamSystem m_teamSystem;
        public CodeAnimation m_codeAnimation;
        public SpriteColorLerp m_whiteBarFlashAnim;

        bool _spawnnedOnScene;

        [Header("---ANIMATION CURVES---")]
        [SerializeField] private AnimationCurve m_recoverCurve;
        [SerializeField] private AnimationCurve m_damageCurve;

        [FoldoutGroup("---HEALTHBAR BLUE---")]
        public Image m_barRed;
        public float m_healthValueRed;
        [SerializeField] private float m_healthRed;
        [SerializeField] private float m_healthRedOld;
        [SerializeField] private float m_healthOverflowRed;
        [SerializeField] private bool m_slidingRed;
        [SerializeField] private bool m_isSlidingRed;
        private Coroutine _slidingBlueCoroutine;

        [FoldoutGroup("---HEALTHBAR BLUE---")]
        public Image m_barBlue;
        public float m_healthValueBlue;
        [SerializeField] private float m_healthBlue;
        [SerializeField] private float m_healthBlueOld;
        [SerializeField] private float m_healthOverflowBlue;
        [SerializeField] private bool m_slidingBlue;
        [SerializeField] private bool m_isSlidingBlue;
        private Coroutine _slidingRedCoroutine;

        [SerializeField] private float m_latestDeathTimestamp;
        [SerializeField] private float m_firstDeathTimestampAfterRecover;
        [SerializeField] private bool m_recovering;
        [SerializeField] private bool m_firstAttackAfterRecover;
    }
}
