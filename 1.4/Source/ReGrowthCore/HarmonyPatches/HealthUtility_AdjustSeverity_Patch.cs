using HarmonyLib;
using RimWorld;
using Verse;

namespace ReGrowthCore
{
    [HarmonyPatch(typeof(HealthUtility), "AdjustSeverity")]
    public static class HealthUtility_AdjustSeverity_Patch
    {
        private static bool Prefix(Pawn pawn, HediffDef hdDef, float sevOffset)
        {
            if (sevOffset > 0 && pawn.IsBathing() &&
                ((pawn.Position.GetTerrain(pawn.Map) == RGDefOf.RG_HotSpring && hdDef == HediffDefOf.Hypothermia)
                || (pawn.Position.GetTerrain(pawn.Map).IsWater && hdDef == HediffDefOf.Heatstroke)))
            {
                return false;
            }
            return true;
        }
    }
}
