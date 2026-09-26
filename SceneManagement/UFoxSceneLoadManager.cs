using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.SceneManagement;
using UFoxLib.MainMenu;
using Landfall.TABS.UnitEditor;
using System.ComponentModel;
using GamepadUI.StateManager.Core;
using TFBGames;

namespace UFoxLib.SceneManagement
{
    public static class UFoxSceneLoadManager
    {
        public static void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            switch (scene.name)
            {
                case "MainMenu":
                    OnMainMenu();
                    break;
                case "UnitCreator_GamepadUI":
                    OnUnitCreator(scene);
                    break;
                case "GameScene":
                    OnGameScene(scene);
                    break;
                default:
                    OnSecretUnits(scene);
                    break;
            }
        }
        //============================================================================================================================
        static void OnGameScene(Scene _currentScene)
        {
        }
        //============================================================================================================================
        static void OnSecretUnits(Scene _currentScene)
        {
            for (int j = 0; j < UFoxSceneLoadManager.m_sceneUnlockables.Count; j++)
            {
                for (int k = 0; k < UFoxSceneLoadManager.m_sceneUnlockables[j].unlockableEntries.Length; k++)
                {
                    bool _sceneMatch = _currentScene.name == UFoxSceneLoadManager.m_sceneUnlockables[j].unlockableEntries[k].sceneName;
                    if (_sceneMatch)
                    {
                        string _secretParentName =
                                    ((UFoxSceneLoadManager.m_sceneUnlockables[j].unlockablesParentName != "") //name is not null
                                    ? UFoxSceneLoadManager.m_sceneUnlockables[j].unlockablesParentName
                                    : "UFoxSecretProps");

                        GameObject[] _objectsToSpawnInScene
                                   = UFoxSceneLoadManager.m_sceneUnlockables[j].unlockableEntries[k].objectsToSpawnInScene;

                        for (int l = 0; l < _objectsToSpawnInScene.Length; l++)
                        {
                            GameObject _findExistingParent = _currentScene.GetRootGameObjects().ToList<GameObject>()
                                        .Find((GameObject x) => x.name == _secretParentName);

                            GameObject[] objectsToSpawnInScene = UFoxSceneLoadManager.m_sceneUnlockables[j].unlockableEntries[k].objectsToSpawnInScene;

                            bool _parentAlreadyExist = _findExistingParent != null;
                            if (!_parentAlreadyExist)
                            {
                                GameObject _newParent = new GameObject
                                {
                                    name = _secretParentName.ToString()
                                };

                                for (int g = 0; g < objectsToSpawnInScene.Length; g++)
                                {
                                    global::UnityEngine.Object.Instantiate<GameObject>(objectsToSpawnInScene[g], _newParent.transform, true);
                                }
                                return;
                            }

                            for (int g2 = 0; g2 < objectsToSpawnInScene.Length; g2++)
                            {
                                global::UnityEngine.Object.Instantiate<GameObject>(objectsToSpawnInScene[g2], _findExistingParent.transform, true);
                            }
                        }
                    }
                }
            }
        }
        //============================================================================================================================
        static void OnUnitCreator(Scene _currentScene)
        {
            if (UFoxSceneLoadManager.m_ucUnitBases == null) return;

            UnitEditorManager _ucManager =
                       _currentScene.GetRootGameObjects().ToList<GameObject>()
                       .Find((GameObject x) => x.GetComponent<UnitEditorManager>()).GetComponent<UnitEditorManager>();

            List<UnitEditorManager.UnitBaseWrapper> _baseList = new List<UnitEditorManager.UnitBaseWrapper>(_ucManager.UnitBases);

            foreach (UnitBlueprint m_baseBlueprint in UFoxSceneLoadManager.m_ucUnitBases)
            {
                UnitEditorManager.UnitBaseWrapper wrapper = new UnitEditorManager.UnitBaseWrapper
                {
                    BaseDisplayName = m_baseBlueprint.Entity.Name,
                    UnitBaseBlueprint = m_baseBlueprint,
                    UnitBaseRestriction = CharacterItem.UnitBaseRestrictions.None
                };
                m_baseBlueprint.Entity.GetSpriteIconAsync(delegate (Sprite sprite)
                {
                    wrapper.BaseIcon = sprite;
                });
                _baseList.Add(wrapper);
            }
            _ucManager.UnitBases = _baseList.ToArray();
        }
        //============================================================================================================================
        static void OnMainMenu()
        {
        }
        //============================================================================================================================

        public static List<UFoxSceneLoadManager.MainMenuUnlockEntry> m_mainMenuUnlocks
            = new List<UFoxSceneLoadManager.MainMenuUnlockEntry>();

        public static List<Unlockables> m_sceneUnlockables = new List<Unlockables>();

        public static List<UnitBlueprint> m_ucUnitBases = new List<UnitBlueprint>();

        [Serializable] public class MainMenuUnlockEntry
        {
            public string m_unlockKey;

            public string m_unlockTitle;

            public Sprite m_icon;
        }
    }
}
