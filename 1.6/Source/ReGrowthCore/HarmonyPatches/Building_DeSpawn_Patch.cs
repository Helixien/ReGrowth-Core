using HarmonyLib;
using Verse;

namespace RimWorld
{
    [HarmonyPatch(typeof(Building), nameof(Building.DeSpawn))]
    public static class Building_DeSpawn_Patch
    {
        public static void Postfix(Building __instance, DestroyMode mode)
        {
            if (__instance is Mineable mineable)
            {
                mineable.UnmarkSpaceRock();
            }
        }
    }
}
