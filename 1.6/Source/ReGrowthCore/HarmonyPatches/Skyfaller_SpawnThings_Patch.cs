using HarmonyLib;

namespace RimWorld
{
    [HarmonyPatch(typeof(Skyfaller), nameof(Skyfaller.SpawnThings))]
    public static class Skyfaller_SpawnThings_Patch
    {
        public static void Prefix(Skyfaller __instance)
        {
            if (__instance.def == ThingDefOf.MeteoriteIncoming)
            {
                SpaceRockManager.isSpawningMeteoriteContents = true;
            }
        }

        public static void Finalizer()
        {
            SpaceRockManager.isSpawningMeteoriteContents = false;
        }
    }
}
