using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ReGrowthCore
{
	// Token: 0x02000003 RID: 3
	[HarmonyPatch(typeof(SteadyEnvironmentEffects))]
	[HarmonyPatch("DoCellSteadyEffects")]
	[HarmonyPatch(new Type[]
	{
		typeof(IntVec3)
	})]
	internal class DoCellSteadyEffects
	{
		private static void Postfix(IntVec3 c, SteadyEnvironmentEffects __instance, Map ___map)
		{
			if (___map == null)
			{
				return;
			}
			Room room = c.GetRoom(___map, RegionType.Set_Passable);
			if (ReGrowthSettings.ColdFog || ReGrowthSettings.IceLayer)
			{
				Thing thing = (from t in c.GetThingList(___map)
				where t.def == RGDefOf.RG_IceOverlay
				select t).FirstOrDefault<Thing>();
				if (room == null && thing != null && ReGrowthSettings.IceLayer)
				{
					thing.Destroy(DestroyMode.Vanish);
					if (Rand.Range(1, 100) <= 20)
					{
						FilthMaker.TryMakeFilth(c, ___map, RGDefOf.RG_Filth_Water, 1);
					}
				}
				if (room != null && !room.UsesOutdoorTemperature && !room.Fogged && !room.IsDoorway)
				{
					float num = 0.8f;
					if (room.Temperature < (float)ReGrowthSettings.FogTemp)
					{
						if (thing == null && ReGrowthSettings.IceLayer)
						{
							GenSpawn.Spawn(ThingMaker.MakeThing(RGDefOf.RG_IceOverlay), c, ___map);
						}
						if (ReGrowthSettings.ColdFog)
						{
							Vector3 vector = c.ToVector3Shifted();
							bool flag = true;
							if (!GenView.ShouldSpawnMotesAt(vector, ___map) || ___map.moteCounter.SaturatedLowPriority)
							{
								flag = false;
							}
							vector += num * new Vector3(Rand.Value - 0.5f, 0f, Rand.Value - 0.5f);
							if (!GenGrid.InBounds(vector, ___map))
							{
								flag = false;
							}
							if (flag)
							{
								MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(RGDefOf.RG_Mote_FrostGlow, null);
								moteThrown.Scale = Rand.Range(4f, 6f) * num;
								moteThrown.rotationRate = Rand.Range(-3f, 3f);
								moteThrown.exactPosition = vector;
								moteThrown.SetVelocity((float)(Rand.Bool ? -90 : 90), (float)((double)ReGrowthSettings.FogVelocity * 0.01));
								GenSpawn.Spawn(moteThrown, IntVec3Utility.ToIntVec3(vector), ___map);
							}
						}
					}
					else if (thing != null)
					{
						thing.Destroy(DestroyMode.Vanish);
						if (Rand.Range(1, 100) <= 20)
						{
							FilthMaker.TryMakeFilth(c, ___map, RGDefOf.RG_Filth_Water, 1);
						}
					}
				}
			}
			if (!ReGrowthSettings.IceLayer)
			{
				Thing thing2 = (from t in c.GetThingList(___map)
				where t.def == RGDefOf.RG_IceOverlay
				select t).FirstOrDefault<Thing>();
				if (thing2 != null)
				{
					thing2.Destroy(DestroyMode.Vanish);
				}
			}
			if (___map.roofGrid != null && !___map.roofGrid.Roofed(c) && (float)___map.weatherManager.curWeatherAge >= 7500f && (___map.weatherManager.curWeather.rainRate <= 0f 
				|| ___map.weatherManager.curWeather.snowRate > 0f))
			{
				Thing thing3 = (from t in c.GetThingList(___map)
				where t.def == RGDefOf.RG_Filth_Water || t.def == RGDefOf.RG_Filth_WaterSpatter
				select t).FirstOrDefault<Thing>();
				if (thing3 != null && Rand.Value <= 0.2f)
				{
					((Filth)thing3).ThinFilth();
				}
			}
		}
	}
}
