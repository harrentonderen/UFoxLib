using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM;
using HarmonyLib;
using Landfall.TABS;

namespace UFoxLib.SubFactions.Patch
{
    [HarmonyPatch(typeof(UnitButton), "SelectUnit")]
    internal class SubFaction_Behavior
    {
        public static bool Prefix(UnitButton __instance, bool makeSound = false)
        {
            //return true;

            if (!__instance.m_unlocked) return false;
            bool _isSubFaction = false;

            SubFactionData subFaction = UFoxUtil.GetSubFaction(__instance.m_unitID);
            if (subFaction != null)
            {
                PlacementUI _placementUI = PlacementUI.Instance;
                _placementUI.m_selectedFaction = _placementUI.m_unitDatabase.GetFaction(subFaction.m_factionRef);
                _placementUI.RedrawFactionUnits(_placementUI.m_selectedFaction);
                _placementUI.SetFirstUnit();
                _placementUI.SelectFaction();
                //var _subFactionUnitButton = _placementUI.unitLayoutGroup.GetUnitButton(_placementUI.);
                //_placementUI.m_selectedButton = _placementUI.SelectUnit
                //(UFoxUtil.GetFactionFromSubFaction(subFaction.m_factionRef).Units[0].Entity.GUID, __instance);
                var _subFactionSecondUnit = UFoxUtil.GetFactionFromSubFaction(subFaction.m_factionRef).Units[1].Entity.GUID;
                if ( __instance.m_placementUI is PlacementUI && __instance.m_placementUI.SelectUnit(__instance.UnitID, __instance))
                {
                    UnitButton._lastSelectedUnit = _subFactionSecondUnit;
                }

                _isSubFaction = true;
                //return false;
            }

            if (makeSound)
            {
                UnitBlueprint unitBlueprint = ContentDatabase.Instance().GetUnitBlueprint(__instance.m_unitID);
                __instance.soundPlayer.PlaySoundEffectNonAlloc(
                    unitBlueprint.VocalPathData, 
                    1f, 
                    __instance.m_mainCamTransform.position + __instance.m_mainCamTransform.forward * 25f, 
                    SoundEffectVariations.MaterialType.Default, 
                    null, 
                    unitBlueprint.VoicePitch);
            }

            if (!_isSubFaction && __instance.m_placementUI is PlacementUI && __instance.m_placementUI.SelectUnit(__instance.m_unitID, __instance))
            {
                UnitButton._lastSelectedUnit = __instance.m_unitID;
            }

            return false;
        }
    }
}
