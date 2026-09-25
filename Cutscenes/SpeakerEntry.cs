using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UFoxLib.Cutscenes
{
    [Serializable]
    public class SpeakerEntry
    {
        public SpeakerEntryHolder[] messageEntries;

        [Serializable]
        public class SpeakerEntryHolder
        {
            public SpeakerMessage[] messages;
        }

        [Serializable]
        public class SpeakerMessage
        {
            public SpeakerVoice m_voice;
            public Color m_textColor = Color.white;
            public string m_message = "Placeholder.";
            [Range(0.005f, 1f)] public float m_readSpeed = 0.04f;
            [Range(0f, 1f)] public float m_messageDelay = 0f;
            [Range(0.5f, 2f)] public float m_pitchModifier = 1f;
            public bool m_addSpaceToStart;
            public bool m_clearPreviousMessage;
            public bool m_sentenceEnd = true;
        }
    }
}
