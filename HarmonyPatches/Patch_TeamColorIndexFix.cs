using System;
using DM;
using HarmonyLib;
using Landfall.TABS;

namespace UFoxLib.HarmonyPatches
{
    [HarmonyPatch(typeof(TeamColor), "Awake")]
    internal class Patch_TeamColorIndexFixer
    {
        [HarmonyPrefix]
        public static bool Prefix(TeamColor __instance)
        {
            TeamColorPaletteData[] teamColors = ContentDatabase.Instance().GetUnitEditorColorPalette().TeamColors;
            for (int i = 0; i < teamColors.Length; i++)
            {
                bool flag = __instance.redMaterial != null && teamColors[i].m_materialRed.name == __instance.redMaterial.name;
                if (flag)
                {
                    __instance.m_teamColorIndex = i;
                    break;
                }
            }
            return true;
        }
    }
}
