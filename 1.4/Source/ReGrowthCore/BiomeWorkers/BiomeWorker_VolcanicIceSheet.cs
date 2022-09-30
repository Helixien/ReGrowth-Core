using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Noise;

// Copyright Helixien 2020

namespace RGW_VolcanicIceSheet
{
    public class RGW_VolcanicIceSheet_BiomeWorker : BiomeWorker
    {
		public override float GetScore(Tile tile, int tileID)
		{
			if (tile.WaterCovered)
			{
				return -100f;
			}

			Perlin perlin = new Perlin(0.079999998211860657, 2.0, 1, 6, Rand.Int, QualityMode.High);
			Vector3 tileCenter = Find.WorldGrid.GetTileCenter(tileID);
			var value = perlin.GetValue(tileCenter);
			if (value > 1.5f && !AtCoats(tileID))
			{
				return PermaIceScore(tile) + 0.01f;
			}
			return -100f;
		}

		public static float PermaIceScore(Tile tile)
		{
			return -20f + (0f - tile.temperature) * 2f;
		}

		public static bool AtCoats(int tileID)
        {
			List<int> neighbours = new List<int>();
			Find.WorldGrid.GetTileNeighbors(tileID, neighbours);
			foreach (var neighbourTile in neighbours)
            {
				try
                {
					if (Find.WorldGrid[neighbourTile].WaterCovered || Find.WorldGrid[neighbourTile].biome == BiomeDefOf.Ocean || Find.World.CoastDirectionAt(tileID) != Rot4.Invalid)
					{
						return true;
					}
				}
				catch { };
            }
			return false;
        }
	}
}
