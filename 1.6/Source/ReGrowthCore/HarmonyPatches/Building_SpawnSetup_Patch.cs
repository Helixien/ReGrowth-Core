using HarmonyLib;
using UnityEngine;
using Verse;

namespace RimWorld
{
    [HarmonyPatch(typeof(Building), nameof(Building.SpawnSetup))]
    public static class Building_SpawnSetup_Patch
    {
        public static void Postfix(Building __instance)
        {
            if (__instance is Mineable mineable && mineable.IsSpaceRock())
            {
                mineable.UpdateGraphic();
            }
        }
    }
}
