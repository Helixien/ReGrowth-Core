using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ReGrowthCore
{
    [HarmonyPatch(typeof(MapComponentUtility), "MapComponentUpdate")]
    public static class MapComponentUtility_MapComponentUpdate_Patch
    {
        public static void Postfix(Map map)
        {
            HashSet<TerrainDef> checkedTerrains = new HashSet<TerrainDef>();
            var allTerrains = map.terrainGrid.waterCells.Select(x => map.terrainGrid.TopTerrainAt(x));
            foreach (var terrain in allTerrains)
            {
                if (checkedTerrains.Contains(terrain))
                {
                    continue;
                }
                checkedTerrains.Add(terrain);
                var extension = terrain.GetModExtension<DefExtension_ShaderSpeedMult>();
                if (extension != null)
                {
                    extension.DoWork(terrain);
                }
            }
        }
    }
}
