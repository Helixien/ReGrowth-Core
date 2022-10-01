using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ReGrowthCore
{
    public static class ReGrowthUtils
    {
        public static bool IsBathing(this Pawn pawn)
        {
            return pawn.CurJobDef == RGDefOf.RG_Bathe && pawn.pather.moving is false;
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
