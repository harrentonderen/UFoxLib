using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//using BepInEx;
//using HarmonyLib;
using UFoxLib.MainMenu;
using UFoxLib.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UFoxLib
{
    //[BepInPlugin("terren.ufoxlib", "UFoxLib", "2.0.0")]
   // public class UFoxLib_Init : BaseUnityPlugin
    public class UFoxLib_Init
    {
        public UFoxLib_Init()
        {
            //new Harmony("UFoxPatcher_Core").PatchAll();
            //SceneManager.sceneLoaded += UFoxSceneLoadManager.SceneLoaded;
           // CreatePersistentObject();
            //Initialize();
           // if (UFoxLib_Init.m_debug) CreateTestModEntry();
        }

        void Initialize()
        {
            UFoxUtil.LOG("", "Library is initialized! Say hi!!!");
            //UFoxUtil.AddTeamColorPalette(UFoxLib_Init.bundle.LoadAsset<UnitEditorColorPalette>("UFoxColorPalette"));
        }

        void CreatePersistentObject()
        {
           // GameObject UFoxPersistent = new GameObject("UFoxPersistent");
           // UFoxPersistent.hideFlags = HideFlags.HideAndDontSave;
           // global::UnityEngine.Object.DontDestroyOnLoad(UFoxPersistent);
            //UFoxPersistent.AddComponent<UFoxLibPersistent>();
        }

        void CreateTestModEntry()
        {
            UFoxModInfoEntry _newEntry = new UFoxModInfoEntry
            {
                modName = "Test Mod",
                modVersion = "1.0.2"
            };
            UFoxLibPersistent.Instance.InitializeUFoxMod(_newEntry);
        }

        //public static AssetBundle bundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("ufoxlib"));
        //public static AssetBundle bundle = AssetBundle.LoadFromMemory(Properties.Resources.ufoxlib);

        public static bool m_debug = true;
    }
}
