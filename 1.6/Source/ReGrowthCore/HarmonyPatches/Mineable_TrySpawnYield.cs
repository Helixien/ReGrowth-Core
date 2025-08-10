using Verse;
using RimWorld;
using HarmonyLib;
using System;

namespace ReGrowthCore
{
    [HarmonyPatch(typeof(Mineable), "TrySpawnYield", new Type[] { typeof(Map), typeof(bool), typeof(Pawn) })]
    public static class Mineable_TrySpawnYield
    {
        public static void Postfix(Mineable __instance, Map map, Pawn pawn)
        {
            if (__instance.TryGetComp<CompContainsOre>() is CompContainsOre comp && comp.chosenOreDef != null)
            {
                Thing thing = ThingMaker.MakeThing(comp.chosenOreDef);
                thing.stackCount = comp.chosenAmount;
                GenSpawn.Spawn(thing, __instance.PositionHeld, map, WipeMode.Vanish);
                ForbidIfNecessary(thing, comp.chosenAmount);
            }

            void ForbidIfNecessary(Thing thing, int count)
            {
                if ((pawn == null || pawn.Faction != Faction.OfPlayer) && thing.def.EverHaulable && !thing.def.designateHaulable)
                {
                    thing.SetForbidden(value: true, warnOnFail: false);
                }
            }
        }
    }
}
