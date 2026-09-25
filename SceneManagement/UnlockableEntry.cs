using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UFoxLib.SceneManagement
{
    public class UnlockableEntry : MonoBehaviour
    {
        [Tooltip("DO NOT LEAVE EMPTY!!!")] public string sceneName = "";
        public GameObject[] objectsToSpawnInScene;
    }
}
