using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Landfall.TABS;
using Landfall.TABS.Workshop;
using UFoxLib.MainMenu;
using UFoxLib.SubFactions;
using UnityEngine;

namespace UFoxLib
{
    [Serializable]
    [CreateAssetMenu(fileName = "New UFoxDatabase", menuName = "UFox/Lib/UFoxDatabase", order = 1)]
    public class UFoxAssetDatabase : ScriptableObject
    {
        public UFoxModInfoEntry m_modInfo;

        public UFoxAssetDatabase.UnitAssets m_unitAssets;
        public UFoxAssetDatabase.UnitCreatorAssets m_UCAssets;
        public UnitEditorColorPalette m_ucPalette;

        public List<UFoxAssetDatabase.SpecialKeys> m_specialKeys = new List<UFoxAssetDatabase.SpecialKeys>();

        public List<MapAsset> m_maps = new List<MapAsset>();
        public List<TABSCampaignLevelAsset> m_campaignLevels = new List<TABSCampaignLevelAsset>();
        public List<TABSCampaignAsset> m_campaign = new List<TABSCampaignAsset>();

        //[Tooltip("Those keys will trigger ")] public List<string> m_specialSecretKeys = new List<string>();

        [Serializable]
        public class UnitAssets
        {
            public List<UnitBlueprint> m_units = new List<UnitBlueprint>();
            public List<Faction> m_factions = new List<Faction>();
            public List<SubFactionData> m_subFactions = new List<SubFactionData>();
        }

        [Serializable]
        public class UnitCreatorAssets
        {
            public List<string> m_UnitCreatorBlueprints = new List<string>();
            public List<GameObject> m_unitBases = new List<GameObject>();
            public List<GameObject> m_weapons = new List<GameObject>();
            public List<GameObject> m_projectiles = new List<GameObject>();
            public List<GameObject> m_props = new List<GameObject>();
            public List<GameObject> m_combatMoves = new List<GameObject>();
            public List<VoiceBundle> m_voiceBundles = new List<VoiceBundle>();
            public List<FactionIcon> m_factionIcons = new List<FactionIcon>();
        }

        [Serializable] public class SpecialKeys
        {
            public string m_secretKey;
            public AudioClip m_unlockClip;
        }
    }
}
