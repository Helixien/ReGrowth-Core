using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ReGrowthCore
{
	public class BiomeWorker_BoilingForest : BiomeWorker
	{
		public override float GetScore(Tile tile, int tileID)
		{
            if (tile.WaterCovered)
            {
                return -100f;
            }
            if (tile.temperature > 6f)
            {
                return 0f;
            }
            if (tile.rainfall < 600f)
            {
                return 0f;
            }
            Vector3 tileCenter = Find.WorldGrid.GetTileCenter(tileID);
			var value = BiomePerlin.GetNoiseFor(BiomeDef.Named("RG_BoilingForest")).GetValue(tileCenter);
			if (value >= 0.3f)
            {
                var initValue = 16f + (tile.temperature - 7f) + (tile.rainfall - 600f) / 180f;
                return initValue + value;
            }
            return -100f;
		}
	}
}
