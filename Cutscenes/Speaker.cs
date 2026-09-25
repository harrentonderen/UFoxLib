using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using TFBGames;
using TMPro;
using UnityEngine;

namespace UFoxLib.Cutscenes
{
    public class Speaker : MonoBehaviour
    {
        void Start() {
            _soundPlayer = ServiceLocator.GetService<SoundPlayer>(); }

        public void SetMessage(SpeakerEntry readable)
        {
            if (m_currentMessage != readable) m_currentMessage = readable;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!m_keyToBeginNextSentence) m_keyToBeginNextSentence = true;
            }
        }

        public void ReadMessage()
        {
            if (!m_isReading) StartCoroutine(ReadMessageCoro());
        }

        IEnumerator ReadMessageCoro()
        {
            m_amountOfMessages = 0;
            m_isReading = true;
            m_text.text = "";
            m_entryHolder = m_currentMessage.messageEntries[UnityEngine.Random.Range(0, m_currentMessage.messageEntries.Length)];
            m_amountOfMessages = m_entryHolder.messages.Length;

            int u = 0;
            while (u < m_amountOfMessages)
            {
                if (m_sentenceEndicator.activeInHierarchy) m_sentenceEndicator.SetActive(false);
                if (m_keyToBeginNextSentence) m_keyToBeginNextSentence = false;

                var b = u;

                if (m_entryHolder.messages[b].m_messageDelay != 0f) yield return new WaitForSeconds(m_entryHolder.messages[b].m_messageDelay);

                if (m_entryHolder.messages[b].m_clearPreviousMessage) m_text.text = "";

                char[] charArray = GetMessageCharacter(m_entryHolder.messages[b]);
                int num;

                if (m_entryHolder.messages[b].m_addSpaceToStart) m_text.text += " ";

                if (m_text.color != m_entryHolder.messages[b].m_textColor) m_text.color = m_entryHolder.messages[b].m_textColor;

                var _startSound = m_entryHolder.messages[b].m_voice.m_messageStartSoundbyte;
                if (_startSound != null) _soundPlayer.PlaySoundEffectNonAlloc(_startSound, 1f, base.transform.position);

                m_currentVoice = m_entryHolder.messages[b].m_voice.m_messageStartSoundbyte;
                for (int i = 0; i < charArray.Length; i = num + 1)
                {
                    if (!char.IsWhiteSpace(charArray[i])) FeedbackVoice();
                    m_text.text += charArray[i].ToString();
                    yield return new WaitForSeconds(m_entryHolder.messages[b].m_readSpeed);
                    num = i;
                }

                if (m_entryHolder.messages[b].m_sentenceEnd)
                {
                    if (!m_sentenceEndicator.activeInHierarchy) m_sentenceEndicator.SetActive(true);
                    yield return new WaitUntil(() => m_keyToBeginNextSentence == true);
                }
                u++;
            }
            if (m_isReading) m_isReading = false;
            if (m_sentenceEndicator.activeInHierarchy) m_sentenceEndicator.SetActive(false);
            yield break;
        }

        public char[] GetMessageCharacter(SpeakerEntry.SpeakerMessage e)
        {
            return e.m_message.ToCharArray();
        }

        public void FeedbackVoice()
        {
            _soundPlayer.PlaySoundEffectNonAlloc(m_currentVoice, 1f, base.transform.position);
        }

        private SoundPlayer _soundPlayer;

        public SpeakerEntry m_currentMessage;
        SpeakerEntry.SpeakerEntryHolder m_entryHolder;
        public AudioPathData m_currentVoice;

        [Header("---REFERENCES---")] 
            public TextMeshProUGUI m_text; 
                public GameObject m_sentenceEndicator;

        [Header("---DEBUG---")] 
            public int m_amountOfMessages; 
                public bool m_isReading; 
                  public bool m_nextMessageWillBeFlushed; 
                     public bool m_keyToBeginNextSentence = true;
    }
}
