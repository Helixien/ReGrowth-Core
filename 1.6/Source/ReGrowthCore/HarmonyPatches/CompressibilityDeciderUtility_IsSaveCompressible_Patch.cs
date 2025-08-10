using HarmonyLib;
using Verse;

namespace RimWorld
{
    [HarmonyPatch(typeof(CompressibilityDeciderUtility), "IsSaveCompressible")]
    public static class CompressibilityDeciderUtility_IsSaveCompressible_Patch
    {
        public static void Postfix(ref bool __result, Thing t)
        {
            if (__result && t is Mineable mineable && mineable.IsSpaceRock())
            {
                Log.Message("Space rock is not compressible");
                __result = false;
            }
        }
    }
}
