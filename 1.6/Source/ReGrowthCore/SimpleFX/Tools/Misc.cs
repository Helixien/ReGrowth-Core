using Verse;
using UnityEngine;
using static ReGrowthCore.SplashesUtility;

namespace ReGrowthCore
{
	public static class SplashTools
	{
		public static Vector3 RandomOffset(this Vector3 vector)
		{
			return new Vector3(vector.x + ((fastRandom.Next(100) - 50) / 100f), vector.y, vector.z + ((fastRandom.Next(100) - 50) / 100f));
		}

		public static Vector3 ToVector3Fast(this IntVec3 intVec3)
		{
			return new Vector3((float)intVec3.x, 10.54054f, (float)intVec3.z);
		}
	}
}
