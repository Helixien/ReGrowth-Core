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
            if (__instance.pawn.IsBathing() && newThought.def == ThoughtDefOf.SoakingWet)
            {
                return false;
            }
            return true;
        }
    }
}
