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
        static ReGrowthUtils()
        {
            foreach (var def in DefDatabase<TerrainDef>.AllDefs)
            {
                var extension = def.GetModExtension<TerrainExtension>();
                if (extension != null)
                {
                    if (extension.biomeSpecificTerrains != null)
                    {
                        foreach (var biomeTerrain in extension.biomeSpecificTerrains)
                        {
                            biomeTerrain.ResolveData(def);
                        }
                    }
                }
            }
        }

        public static bool IsHotSpring(this TerrainDef terrainDef)
        {
            return terrainDef == RG_DefOf.RG_HotSpring || terrainDef == RG_DefOf.RG_HotSpringDeep;
        }

        public static bool IsBathingNow(this Pawn pawn)
        {
            return pawn?.jobs?.curDriver is JobDriver_Bathe driver_Bathe && driver_Bathe.IsBathingNow();
        }
        private static Dictionary<Material, Material> materials = new();
        public static Material GetBatheMat(Material baseMat, float transparency)
        {
            if (baseMat is null) return baseMat;
            if (!materials.TryGetValue(baseMat, out var value))
            {
                materials[baseMat] = value = new Material(baseMat);
            }
            value.color = new Color(value.color.r, value.color.g, value.color.b, transparency);
            return value;
        }

        public static bool TryGetBiomeSpecificTerrain(this TerrainDef def, Map map, out TerrainByBiome terrain)
        {
            terrain = null;
            var extension = def.GetModExtension<TerrainExtension>();
            if (extension?.biomeSpecificTerrains != null)
            {
                var biome = map.Biome;
                foreach (var biomeTerrain in extension.biomeSpecificTerrains)
                {
                    if (biomeTerrain.biomes.Contains(biome))
                    {
                        terrain = biomeTerrain;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
