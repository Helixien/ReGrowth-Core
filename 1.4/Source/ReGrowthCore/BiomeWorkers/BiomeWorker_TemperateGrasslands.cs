using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReGrowthCore
{
    public class BiomeWorker_TemperateGrasslands : BiomeWorker
    {
        public override float GetScore(Tile tile, int tileID)
        {
            if (tile.WaterCovered)
            {
                return -100f;
            }
            if (tile.temperature < -10f)
            {
                return 0f;
            }
            if (tile.rainfall < 600f)
            {
                return 0f;
            }
            var tileCenter = Find.WorldGrid.GetTileCenter(tileID);
            float value = BiomePerlin.GetNoiseFor(BiomeDef.Named("RG_TemperateGrasslands")).GetValue(tileCenter);
            float score = 15f + (tile.temperature - 7f) + ((tile.rainfall - 600f) / 180f);
            if (value >= 0.4f && score >= 0)
            {
                return score + value;
            }
            return 0f;
        }
    }
}
