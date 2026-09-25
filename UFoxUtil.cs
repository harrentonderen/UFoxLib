using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DM;
using Landfall.TABS;
using UnityEngine;
using UFoxLib.SubFactions;
using System.Collections;
using static DM.AssetDatabaseFile;
using Landfall.TABS.UnitEditor;

namespace UFoxLib
{
    public static class UFoxUtil
    {
        public static void ExampleFunc()
        {
        }
        public static void LOG(string modPrefix, string message)
        {
            string text = ((modPrefix == "") ? "UFoxLib" : modPrefix);
                Debug.Log("* " + text + ": " + message);
        }

        public static void AddSubFactions(UFoxAssetDatabase db)
        {
            m_activeSubFactions.AddRange(db.m_unitAssets.m_subFactions);
        }

        public static SubFactionData GetSubFaction(DatabaseID unitToFactionID)
        {
            SubFactionData _subFaction = 
                UFoxUtil.m_activeSubFactions.ToList<SubFactionData>().Find((SubFactionData x) => x.m_unitID == unitToFactionID);

            if (_subFaction == null)
            {
                //if (UFoxLib_Init.m_debug) UFoxUtil.LOG("", "Cannot find Sub Faction Button with ID " + unitToFactionID.ToString());
                return null;
            }

            return _subFaction;
        }

        public static Faction GetFactionFromSubFaction(DatabaseID m_subFactionID)
        {
            var _subFactionPresent = UFoxUtil.m_subFactionFiles
                            .ToList<Faction>()
                            .Find((Faction x) => x.Entity.GUID == m_subFactionID);
            return (_subFactionPresent ? _subFactionPresent : null);
        }

        public static bool CheckSubFaction(DatabaseID unitToFactionID)
        {
            return UFoxUtil.m_activeSubFactions.ToList<SubFactionData>().Find((SubFactionData x) => x.m_unitID == unitToFactionID) != null;
        }

        //=============================================================

        private static Dictionary<DatabaseID, global::UnityEngine.Object> GetNonStreamableAssets()
        {
            return 
                (Dictionary<DatabaseID, global::UnityEngine.Object>)
                typeof(AssetLoader)
                .GetField("m_nonStreamableAssets", 
                BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(ContentDatabase.Instance().AssetLoader);
        }

        public static void AddTeamColorPalette(UnitEditorColorPalette newData)
        {
            UnitEditorColorPalette unitEditorColorPalette = ContentDatabase.Instance().LandfallContentDatabase.GetUnitEditorColorPalette();
            List<TeamColorPaletteData> list = unitEditorColorPalette.m_ColorPalleteParentCatagories[1].colorPaletteCatagories[0].TeamColors.ToList<TeamColorPaletteData>();
            list.AddRange(newData.m_teamColors.ToList<TeamColorPaletteData>());
            unitEditorColorPalette.m_ColorPalleteParentCatagories[1].colorPaletteCatagories[0].TeamColors = list.ToArray<TeamColorPaletteData>();
            unitEditorColorPalette.Initialize();
        }

        public static void AddUnitBlueprints(ContentDatabase databaseVanilla, UFoxAssetDatabase databaseMod)
        {
            Dictionary<DatabaseID, global::UnityEngine.Object> _nonStreamable = UFoxUtil.GetNonStreamableAssets();

            LandfallContentDatabase _landfallContentDatabase = databaseVanilla.LandfallContentDatabase;

            Dictionary<DatabaseID, UnitBlueprint> _vanillaUnits = 
                (Dictionary<DatabaseID, UnitBlueprint>)typeof(LandfallContentDatabase)
                .GetField("m_unitBlueprints", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(_landfallContentDatabase);

            foreach (UnitBlueprint _modBlueprint in databaseMod.m_unitAssets.m_units)
            {
                string name = _modBlueprint.UnitBase.name;
                switch (name)
                {
                    case "HOLDER_HALFLING":
                        _modBlueprint.UnitBase = _landfallContentDatabase.GetUnitBases()
                            .ToList<GameObject>()
                            .Find((GameObject x) => x.name == "Halfling_1 Prefabs_VB");
                        break;
                    case "HOLDER_STIFFY":
                        _modBlueprint.UnitBase = _landfallContentDatabase.GetUnitBases()
                            .ToList<GameObject>()
                            .Find((GameObject x) => x.name == "Stiffy_1 Prefabs_VB");
                        break;
                    case "HOLDER_HORSE":
                        _modBlueprint.UnitBase = _landfallContentDatabase.GetUnitBases()
                            .ToList<GameObject>()
                            .Find((GameObject x) => x.name == "Horse_1 Prefabs_VB");
                        break;
                    case "HOLDER_MINOTAUR":
                        _modBlueprint.UnitBase = _landfallContentDatabase.GetUnitBases()
                            .ToList<GameObject>()
                            .Find((GameObject x) => x.name == "Minotaur_1 Prefabs_VB");
                        break;
                    //default is HOLDER_HUMANOID
                    default:
                        _modBlueprint.UnitBase = _landfallContentDatabase.GetUnitBases()
                            .ToList<GameObject>()
                            .Find((GameObject x) => x.name == "Humanoid_1 Prefabs_VB");
                        break;
                }

                _vanillaUnits.Add(_modBlueprint.Entity.GUID, _modBlueprint);
                _nonStreamable.Add(_modBlueprint.Entity.GUID, _modBlueprint);
            }
            typeof(LandfallContentDatabase)
                .GetField("m_unitBlueprints", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(_landfallContentDatabase, _vanillaUnits);
        }

        public static void AddFactions(ContentDatabase databaseVanilla, UFoxAssetDatabase databaseMod)
        {
            Dictionary<DatabaseID, global::UnityEngine.Object> _nonStreamableAssets = UFoxUtil.GetNonStreamableAssets();

            LandfallContentDatabase _landfallContentDatabase = databaseVanilla.LandfallContentDatabase;

            Dictionary<DatabaseID, Faction> _vanillaFactions = 
                (Dictionary<DatabaseID, Faction>)typeof(LandfallContentDatabase)
                .GetField("m_factions", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(_landfallContentDatabase);

            List<DatabaseID> _vanillaFactionsHotbar = 
                (List<DatabaseID>)typeof(LandfallContentDatabase)
                .GetField("m_defaultHotbarFactionIds", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(_landfallContentDatabase);

            foreach (Faction _modFaction in databaseMod.m_unitAssets.m_factions)
            {
                _vanillaFactions.Add(_modFaction.Entity.GUID, _modFaction);
                _nonStreamableAssets.Add(_modFaction.Entity.GUID, _modFaction);
                bool displayFaction = _modFaction.m_displayFaction;
                if (displayFaction)
                {
                    _vanillaFactionsHotbar.Add(_modFaction.Entity.GUID);
                }

                for (int sf = 0; sf < m_activeSubFactions.Count; sf++)
                {
                    if (m_activeSubFactions[sf].m_factionRef == _modFaction.Entity.GUID)
                        m_subFactionFiles.Add(_modFaction);
                }
            }   

            typeof(LandfallContentDatabase)
                .GetField("m_factions", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(_landfallContentDatabase, _vanillaFactions);

            typeof(LandfallContentDatabase)
                .GetField("m_defaultHotbarFactionIds", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(_landfallContentDatabase, _vanillaFactionsHotbar
                .OrderBy<DatabaseID, int>((DatabaseID x) => _vanillaFactions[x].index)
                .ToList<DatabaseID>());
        }

        public static void AddUnitCreatorPass(ContentDatabase databaseVanilla, UFoxAssetDatabase databaseMod)
        {
            //Horrible, terrible code, but whatever, if it works - right?

            Dictionary<DatabaseID, global::UnityEngine.Object> _nonStreamable = UFoxUtil.GetNonStreamableAssets();

            LandfallContentDatabase _landfallContentDatabase = databaseVanilla.LandfallContentDatabase;

            //--------------------------------------------------------------------

            Dictionary<DatabaseID, GameObject> _unitBases = 
                (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase)
                .GetField("m_unitBases", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_landfallContentDatabase);
            foreach (GameObject m_unitBase in databaseMod.m_UCAssets.m_unitBases)
            {
                _unitBases.Add(m_unitBase.GetComponent<Unit>().Entity.GUID, m_unitBase);
                _nonStreamable.Add(m_unitBase.GetComponent<Unit>().Entity.GUID, m_unitBase);
            }
            typeof(LandfallContentDatabase).GetField("m_unitBases", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(_landfallContentDatabase, _unitBases);

            //--------------------------------------------------------------------

            Dictionary<DatabaseID, GameObject> _props = 
                (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase)
                .GetField("m_characterProps", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_landfallContentDatabase);
            foreach (GameObject m_props in databaseMod.m_UCAssets.m_props)
            {
                _props.Add(m_props.GetComponent<PropItem>().Entity.GUID, m_props);
                _nonStreamable.Add(m_props.GetComponent<PropItem>().Entity.GUID, m_props);
            }
            typeof(LandfallContentDatabase).GetField("m_characterProps", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(_landfallContentDatabase, _props);

            //--------------------------------------------------------------------

            Dictionary<DatabaseID, GameObject> _moves = 
                (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase)
                .GetField("m_combatMoves", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_landfallContentDatabase);
            foreach (GameObject m_moves in databaseMod.m_UCAssets.m_combatMoves)
            {
                _moves.Add(m_moves.GetComponent<SpecialAbility>().Entity.GUID, m_moves);
                _nonStreamable.Add(m_moves.GetComponent<SpecialAbility>().Entity.GUID, m_moves);
            }
            typeof(LandfallContentDatabase).GetField("m_combatMoves", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(_landfallContentDatabase, _moves);

            //--------------------------------------------------------------------

            Dictionary<DatabaseID, GameObject> _weapons = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase)
                .GetField("m_weapons", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_landfallContentDatabase);
            foreach (GameObject m_weapons in databaseMod.m_UCAssets.m_weapons)
            {
                _weapons.Add(m_weapons.GetComponent<WeaponItem>().Entity.GUID, m_weapons);
                _nonStreamable.Add(m_weapons.GetComponent<WeaponItem>().Entity.GUID, m_weapons);
            }
            typeof(LandfallContentDatabase).GetField("m_weapons", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(_landfallContentDatabase, _weapons);

            //--------------------------------------------------------------------

            Dictionary<DatabaseID, GameObject> _projectiles = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase)
                .GetField("m_projectiles", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_landfallContentDatabase);
            foreach (GameObject m_projectiles in databaseMod.m_UCAssets.m_projectiles)
            {
                _projectiles.Add(m_projectiles.GetComponent<ProjectileEntity>().Entity.GUID, m_projectiles);
                _nonStreamable.Add(m_projectiles.GetComponent<ProjectileEntity>().Entity.GUID, m_projectiles);
            }
            typeof(LandfallContentDatabase).GetField("m_projectiles", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(_landfallContentDatabase, _projectiles);

            //--------------------------------------------------------------------

            Dictionary<DatabaseID, VoiceBundle> _voices = (Dictionary<DatabaseID, VoiceBundle>)typeof(LandfallContentDatabase)
                .GetField("m_voiceBundles", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_landfallContentDatabase);
            foreach (VoiceBundle m_voiceBundle in databaseMod.m_UCAssets.m_voiceBundles)
            {
                _voices.Add(m_voiceBundle.Entity.GUID, m_voiceBundle);
                _nonStreamable.Add(m_voiceBundle.Entity.GUID, m_voiceBundle);
            }
            typeof(LandfallContentDatabase).GetField("m_voiceBundles", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(_landfallContentDatabase, _voices);

            //--------------------------------------------------------------------

            List<DatabaseID> _factionIcons = (List<DatabaseID>)typeof(LandfallContentDatabase)
                .GetField("m_factionIconIds", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_landfallContentDatabase);
            foreach (FactionIcon factionIcon in databaseMod.m_UCAssets.m_factionIcons)
            {
                _factionIcons.Add(factionIcon.Entity.GUID);
                _nonStreamable.Add(factionIcon.Entity.GUID, factionIcon);
            }
            typeof(LandfallContentDatabase).GetField("m_factionIconIds", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(_landfallContentDatabase, _factionIcons);

            //--------------------------------------------------------------------
        }

        public static void RemoveUnitContent(AssetBundle bundle, UFoxAssetDatabase databaseMod, string unitToRemove, string fromFaction)
        {
            UnitBlueprint _unitToRemove = databaseMod.m_unitAssets.m_units
                .ToList<UnitBlueprint>().Find((UnitBlueprint x) => x.Entity.Name == unitToRemove);
            bundle.LoadAsset<Faction>(fromFaction).RemoveUnit(_unitToRemove);

            GameObject[] props = _unitToRemove.m_props;
            for (int i = 0; i < props.Length; i++)
            {
                CharacterItem component = props[i].GetComponent<CharacterItem>();
                bool flag = component && component.ShowInEditor;
                if (flag) component.SetVariable("m_showInEditor", false);

            }

            List<WeaponItem> list = new List<WeaponItem>();
            list.Add(_unitToRemove.RightWeapon.GetComponent<WeaponItem>());
            list.Add(_unitToRemove.LeftWeapon.GetComponent<WeaponItem>());

            bool flag2 = list != null;
            if (flag2)
            {
                foreach (WeaponItem weaponItem in list)
                {
                    bool showInEditor = weaponItem.ShowInEditor;
                    if (showInEditor) weaponItem.SetVariable("m_showInEditor", false);
                }
            }

            List<SpecialAbility> list2 = new List<SpecialAbility>();
            GameObject[] objectsToSpawnAsChildren = _unitToRemove.objectsToSpawnAsChildren;
            for (int j = 0; j < objectsToSpawnAsChildren.Length; j++)
            {
                SpecialAbility component2 = objectsToSpawnAsChildren[j].GetComponent<SpecialAbility>();
                bool flag3 = component2;
                if (flag3)
                {
                    list2.Add(component2);
                }
            }

            foreach (SpecialAbility specialAbility in list2)
            {
                bool showInEditor2 = specialAbility.ShowInEditor;
                if (showInEditor2) specialAbility.SetVariable("m_showInEditor", false);
            }
        }

        public static T SetVariable<T>(this object me, string name, T value)
        {
            Type type = me.GetType();
            FieldInfo field;
            while ((field = type.GetField(name, (BindingFlags)(-1))) == null && (type = type.BaseType) != null)
            {
            }
            bool flag = field == null;
            T t;
            if (flag)
            {
                string _log = string.Concat(new string[]
                {
            "Couldn't find the variable '",
            name,
            "' in '",
            me.GetType().Name,
            "'"
                });
                UFoxUtil.LOG("", _log);
                t = value;
            }
            else
            {
                try
                {
                    field.SetValue(me, value);
                }
                catch (Exception ex)
                {
                    string _err = string.Format("Couldn't set the value of '{0}' in '{1}: {2}'", name, me.GetType().Name, ex);
                    UFoxUtil.LOG("", _err);
                }
                t = value;
            }
            return t;
        }

        //=============================================================

        public static List<SubFactionData> m_activeSubFactions = new List<SubFactionData>();
        public static List<Faction> m_subFactionFiles = new List<Faction>();
    }
}
