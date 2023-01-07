using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ReGrowthCore
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HotSwappableAttribute : Attribute
    {
    }
    [HotSwappable]
    [StaticConstructorOnStartup]
    public static class ReGrowthUtils
    {
        public static bool IsHotSpring(this TerrainDef terrainDef)
        {
            return terrainDef == RG_DefOf.RG_HotSpring || terrainDef == RG_DefOf.RG_HotSpringDeep;
        }

        public static bool IsBathingNow(this Pawn pawn)
        {
            return pawn.jobs.curDriver is JobDriver_Bathe driver_Bathe && driver_Bathe.IsBathingNow();
        }
        private static Dictionary<Material, Material> materials = new();
        public static Material GetBatheMat(Material baseMat)
        {
            if (!materials.TryGetValue(baseMat, out var value))
            {
                materials[baseMat] = value = new Material(baseMat);
                value.color = new Color(value.color.r, value.color.g, value.color.b, 0.1f);
            }
            return value;
        }
    }
}
