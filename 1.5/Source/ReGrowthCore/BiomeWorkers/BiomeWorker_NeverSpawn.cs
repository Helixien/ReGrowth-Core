using RimWorld;
using RimWorld.Planet;

namespace ReGrowthCore
{
	public class BiomeWorker_NeverSpawn : BiomeWorker
	{
		public override float GetScore(Tile tile, int tileID)
		{
			return -100f;
		}
	}
}

