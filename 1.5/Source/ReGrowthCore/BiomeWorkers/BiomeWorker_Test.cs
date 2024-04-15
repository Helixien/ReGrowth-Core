using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace ReGrowthCore
{
    public class BiomeWorker_Test : BiomeWorker
    {
        public override float GetScore(Tile tile, int tileID)
        {
            Vector3 tileCenter = Find.WorldGrid.GetTileCenter(tileID);
            var value = BiomePerlin.GetNoiseFor(BiomeDef.Named("TemperateForest")).GetValue(tileCenter);
            if (value >= 0.2)
            {
                return 50;
            }
            return -999f;
        }
    }
}
