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

namespace ReGrowthAspenForest
{
	public class BiomeWorker_AspenForest : BiomeWorker
	{
		public override float GetScore(Tile tile, int tileID)
		{
			if (tile.WaterCovered)
			{
				return -100f;
			}
			if (tile.temperature < -5f)
			{
				return 0f;
			}
			if (tile.rainfall < 300f)
			{
				return 0f;
			}
			Vector3 tileCenter = Find.WorldGrid.GetTileCenter(tileID);
			var value = BiomePerlin.GetNoiseFor(BiomeDef.Named("RG-AF_AspenForest")).GetValue(tileCenter);
			var tileTemperature = (0f - tile.temperature);
			if (value >= 0.2 && tileTemperature >= 0)
            {
				return tileTemperature + value;
            }
			return 0f;
		}
	}
}
