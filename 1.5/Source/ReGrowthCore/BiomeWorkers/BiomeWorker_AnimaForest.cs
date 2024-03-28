using ReGrowthCore;
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
	public class BiomeWorker_AnimaForest : BiomeWorker
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

			Vector3 tileCenter = Find.WorldGrid.GetTileCenter(tileID);
			var value = BiomePerlin.GetNoiseFor(BiomeDef.Named("TemperateForest")).GetValue(tileCenter);
			if (value >= 0.2)
            {
				return (15f + (tile.temperature - 7f) + (tile.rainfall - 600f) / 180f) + value;
			}
			return -999f;
		}
	}
}
