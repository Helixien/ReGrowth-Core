using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace ReGrowthCore
{
    [HarmonyPatch(typeof(MemoryThoughtHandler), "TryGainMemory", new Type[]
    {
        typeof(Thought_Memory),
        typeof(Pawn)
    })]
    public static class MemoryThoughtHandler_TryGainMemory_Patch
    {
        private static bool Prefix(MemoryThoughtHandler __instance, ref Thought_Memory newThought, Pawn otherPawn)
        {
            if (__instance.pawn.CurJobDef == RG_DefOf.RG_Bathe && newThought.def == ThoughtDefOf.SoakingWet)
            {
                return false;
            }
            foreach (var hediff in __instance.pawn.health.hediffSet.hediffs)
            {
                var comp = hediff.TryGetComp<HediffCompPreventThought>();
                if (comp != null && comp.PreventsThought(newThought.def))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
