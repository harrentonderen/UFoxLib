using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFoxLib.MainMenu;
using UnityEngine;

namespace UFoxLib
{
    public class UFoxLibPersistent : MonoBehaviour
    {
        private void Awake() =>
            UFoxLibPersistent.Instance = this;

        public void InitializeUFoxMod(UFoxModInfoEntry _entry) => 
            this.uFoxMods.Add(_entry);

        public static UFoxLibPersistent Instance;

        public List<UFoxModInfoEntry> uFoxMods = new List<UFoxModInfoEntry>();
    }
}
