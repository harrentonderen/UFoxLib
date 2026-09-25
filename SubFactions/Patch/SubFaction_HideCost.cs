using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using DM;
using Landfall.TABS;

namespace UFoxLib.SubFactions.Patch
{
    [HarmonyPatch(typeof(UnitButton), "ShowCost")]
    internal class SubFaction_HideCost
    {
        public static bool Prefix(UnitButton __instance, bool show)
        {
            if (__instance.disabled || UFoxUtil.CheckSubFaction(__instance.m_unitID))
            {
                return false;
            }

            if (SandboxUIManager.UIMode == SandboxUIManager.PlacementUIMode.PlacementMode)
            {
                __instance.m_customUnitCost.enabled = show;
                __instance.m_cost.enabled = show;
                __instance.m_icon.enabled = !show;
            }

            return false;
        }
    }
}
