using System;
using HarmonyLib;
using ReGrowthCore;
using RimWorld;
using Verse;

namespace ReGrowthCore
{
	[HarmonyPatch(typeof(JobDriver_CleanFilth), "TryMakePreToilReservations", null)]
	public static class CleanFilthPrefixPatch
	{
		[HarmonyPrefix]
		public static bool CleanFilthPrefix(JobDriver_CleanFilth __instance)
		{
			if (!ReGrowthSettings.RainCleanWaterPuddles)
			{
				foreach (LocalTargetInfo localTargetInfo in __instance.job.targetQueueA)
				{
					Thing thing = localTargetInfo.Thing;
					Filth filth = thing as Filth;
					if (filth != null && (filth.def == RGDefOf.RG_Filth_Water || filth.def == RGDefOf.RG_Filth_WaterSpatter) 
						&& thing.GetRoom(RegionType.Set_Passable).UsesOutdoorTemperature 
						&& !thing.Map.roofGrid.Roofed(thing.Position))
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}
	}
}
