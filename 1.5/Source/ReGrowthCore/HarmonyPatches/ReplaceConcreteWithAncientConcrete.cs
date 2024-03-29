using HarmonyLib;
using RimWorld;
using RimWorld.BaseGen;
using RimWorld.SketchGen;
using System.Collections.Generic;
using System.Reflection;

namespace ReGrowthCore
{
    [HarmonyPatch]
    public static class ReplaceConcreteWithAncientConcrete
    {
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(SymbolResolver_AncientComplex_Base), "ResolveComplex");
            yield return AccessTools.Method(typeof(SymbolResolver_AncientShrine), "Resolve");
            yield return AccessTools.Method(typeof(SketchResolver_AncientLandingPad), "ResolveInt");
            yield return AccessTools.Method(typeof(SketchResolver_AncientUtilityBuilding), "ResolveInt");
        }

        public static void Prefix()
        {
            TerrainDefOf.Concrete = RG_DefOf.RG_AncientConcrete;
        }

        public static void Postfix()
        {
            TerrainDefOf.Concrete = RG_DefOf.Concrete;
        }
    }
}
