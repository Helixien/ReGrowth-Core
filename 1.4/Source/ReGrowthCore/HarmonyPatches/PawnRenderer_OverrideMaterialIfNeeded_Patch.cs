using HarmonyLib;
using UnityEngine;
using Verse;

namespace ReGrowthCore
{
    [HarmonyPatch(typeof(PawnRenderer), "OverrideMaterialIfNeeded")]
    public static class PawnRenderer_OverrideMaterialIfNeeded_Patch
    {
        private static void Postfix(PawnRenderer __instance, Pawn ___pawn, ref Material __result, Material original, Pawn pawn, bool portrait = false)
        {
            if (!portrait && ___pawn.IsBathingNow())
            {
                __result = ReGrowthUtils.GetBatheMat(original);
            }
        }
    }
}
