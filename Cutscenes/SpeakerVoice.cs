using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFBGames;
using UnityEngine;

namespace UFoxLib.Cutscenes
{
    [Serializable]
    [CreateAssetMenu(fileName = "New SpeakerVoice", menuName = "UFox/Lib/Speaker Voice", order = 1)]
    public class SpeakerVoice
    {
        public AudioPathData m_messageStartSoundbyte;
        public AudioPathData m_characterSoundbyte;
        //SpeakerVoice/Name
    }
}
