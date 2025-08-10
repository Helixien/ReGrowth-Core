using HarmonyLib;
using Verse;

namespace RimWorld
{
    [HarmonyPatch(typeof(GenStep_Asteroid), nameof(GenStep_Asteroid.SpawnOres))]
    public static class GenStep_Asteroid_SpawnOres_Patch
    {
        public static void Prefix()
        {
            SpaceRockManager.isGeneratingOresForAsteroidStep = true;
        }

        public static void Finalizer()
        {
            SpaceRockManager.isGeneratingOresForAsteroidStep = false;
        }
    }
}
