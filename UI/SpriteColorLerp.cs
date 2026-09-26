using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UFoxLib.UI
{
    public class SpriteColorLerp : MonoBehaviour
    {
        public void FadeIn() => StartCoroutine(AnimateImage(true));

        public void FadeOut() => StartCoroutine(AnimateImage(false));
        public void FadePing() => StartCoroutine(AnimateImagePing());

        public void StopFade(bool _inOut)
        {
            StopCoroutine(AnimateImage(_inOut));
        }

        private IEnumerator AnimateImage(bool inOut)
        {
            currentAnimTime = 0f;
            while (currentAnimTime < m_lerpTime)
            {
                m_bg.color = Color.Lerp
                (
                    (inOut ? m_color1 : m_color2), 
                    (!inOut ? m_color2 : m_color1), 
                    currentAnimTime / m_lerpTime
                );

                currentAnimTime += Time.unscaledDeltaTime;
                yield return null;
            }

            yield break;
        }

        private IEnumerator AnimateImagePing()
        {
            float timer = 0f;

            while (timer < 1f)
            {
                timer += Time.unscaledDeltaTime * 5f;
                float a = Mathf.Lerp(1f, 0f, timer);
                Color color = m_bg.color;
                color.a = a;
                m_bg.color = color;
                yield return null;
            }

            yield break;
        }

        public Image m_bg;

        public Color m_color1;
        public Color m_color2;

        public float m_lerpTime;

        private float currentAnimTime;
    }
}
