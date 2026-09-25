using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace UFoxLib.MainMenu
{
    public class UFoxLibMenuHandler : MonoBehaviour
    {
        private void Start()
        {
            if (prefab_uFoxMod.gameObject.activeInHierarchy) prefab_uFoxMod.gameObject.SetActive(false);

            var _instance = MainMenuButtons.Instance;
            var _intro = _instance.m_IntroSequence;

            if (_intro != null) _intro.OnIntroSequenceFinished.AddListener(new UnityAction(DelayCornerButton));
        }

        public void InitializeMenu()
        {
            UFoxLibPersistent ufoxlib = UFoxLibPersistent.Instance;

            bool flag_noMods = ufoxlib.uFoxMods == null || (ufoxlib.uFoxMods.Count == 0 && !infoObject_noMods.activeInHierarchy);

            if (flag_noMods)
            {
                if (!infoObject_noMods.activeInHierarchy) infoObject_noMods.SetActive(true);
                UFoxUtil.LOG("", "You have no UFoxLib mods installed as of right now. You can check out Thunderstore.io for more mods!");
                return;
            }

            if (infoObject_noMods.activeInHierarchy) infoObject_noMods.SetActive(false);
            PopulateMods();
        }

        public void DelayCornerButton()
        {
            StartCoroutine(_DelayMenuStartup());
        }

        IEnumerator _DelayMenuStartup()
        {
            yield return new WaitForSeconds(0.15f);
            //DelayedInit();
            _popUpAnimation.PlayOut();

            yield break;
        }

        private void PopulateMods()
        {
            UFoxLibPersistent ufoxlib = UFoxLibPersistent.Instance;

            for (int i = 0; i < ufoxlib.uFoxMods.Count; i++)
            {
                MainMenuModEntryPrefab modEntry = 
                    global::UnityEngine.Object.Instantiate<MainMenuModEntryPrefab>
                    (
                        prefab_uFoxMod, 
                        prefab_uFoxMod.gameObject.transform.parent
                    );

                modEntry.SetupInfo(ufoxlib.uFoxMods[i]);
            }
        }

        private void Update()
        {
            if (!_menuOpen) return;

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace)) InvokeMenuBack();
        }

        public void InvokeMenuEnter()
        {
            if (!_menuOpen) _menuOpen = true;
            MainMenuButtons.Instance.stateCodeAnimation.PlayOut();
            _eventsOnOpen?.Invoke();
        }

        public void InvokeMenuBack()
        {
            if (_menuOpen) _menuOpen = false;
            MainMenuButtons.Instance.stateCodeAnimation.PlayIn();
            _eventsOnClose?.Invoke();
        }

        public void SetMenuBool(bool _menuState)
        {
            if (_menuOpen != _menuState) _menuOpen = _menuState;
        }

        public bool _menuOpen;

        public MainMenuModEntryPrefab prefab_uFoxMod;

        public GameObject infoObject_noMods;

        public CodeAnimation _popUpAnimation;

        public UnityEvent _eventsOnOpen;
        public UnityEvent _eventsOnClose;
    }
}
