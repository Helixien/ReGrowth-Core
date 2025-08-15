using HarmonyLib;
using Verse;
using RimWorld;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using static ReGrowthCore.ZoneData;
using Verse.AI;

namespace ReGrowthCore
{
	//This handles the zone gizmos
	[HarmonyPatch(typeof(Zone_Growing), nameof(Zone_Growing.GetGizmos))]
	static class Patch_GetGizmos
	{
		static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> values, Zone_Growing __instance)
		{
			if (!ReGrowthCore_SmartFarming.ModSettings.enabled)
			{
				foreach (var value in values)
				{
					yield return value;
				}
				yield break;
			}
			//Pass along all other gizmos except vanilla sow, which we only identify via its hotkey...
			foreach (var value in values)
			{
				if (((Command)value)?.hotKey == KeyBindingDefOf.Command_ItemForbid) continue;
				yield return value;
			}

			Map map = __instance.Map;
			if (ReGrowthCore_SmartFarming.compCache.TryGetValue(map.uniqueID, out MapComponent_SmartFarming comp) && comp.growZoneRegistry.TryGetValue(__instance.ID, out ZoneData zoneData))
			{
				//Return the sow mode gizmo and priority gizmo
				if (Find.Selector.selected.Count == 1)
				{
					yield return zoneData.sowGizmo;
					yield return zoneData.priorityGizmo;
				}
				else
				{
					foreach (var gizmo in GetMultiZoneGizmos(comp, zoneData, __instance))
					{
						yield return gizmo;
					}
				}

				//Petty jobs gizmo
				yield return zoneData.pettyJobsGizmo;
				//Allow harvest gizmo
				if (AllowHarvest.allowHarvestGizmoPatched) yield return zoneData.allowHarvestGizmo;

				//Harvest now gizmo
				ThingDef crop = __instance.plantDefToGrow;
				if (crop == null) yield break;

				if (HarvestNowGizmo(__instance, map, crop)) yield return zoneData.harvestGizmo;

				//Orchard align?
				if (ReGrowthCore_SmartFarming.ModSettings.orchardAlignment && crop.plant.blockAdjacentSow) yield return zoneData.orchardGizmo;
			}
		}

		static bool HarvestNowGizmo(Zone_Growing zone, Map map, ThingDef plantDefToGrow)
		{
			var cells = zone.cells;
			var thingGrid = map.thingGrid;
			for (int i = cells.Count; i-- > 0;)
			{
				var cell = cells[i];
				if (thingGrid.ThingAt(cell, ThingCategory.Plant) is Plant plant && plant.def == plantDefToGrow && plant.HarvestableNow)
				{
					return true;
				}
			}
			return false;
		}

		static IEnumerable<Gizmo> GetMultiZoneGizmos(MapComponent_SmartFarming comp, ZoneData zoneData, Zone_Growing thisZone)
		{
			ZoneData basisZoneData = zoneData;
			Zone_Growing basisZone = null;
			var selected = Find.Selector.selected;
			for (int i = selected.Count; i-- > 0;)
			{
				if (selected[i] is Zone_Growing growZone && comp.growZoneRegistry.TryGetValue(growZone.ID, out basisZoneData))
				{
					basisZone = growZone;
					break;
				}
			}

			yield return new Command_Action()
			{
				defaultLabel = ("SmartFarming.Icon.SetAll".Translate() + basisZoneData.sowGizmo.defaultLabel.ToLower()),
				defaultDesc = basisZoneData.sowGizmo.defaultDesc,
				hotKey = KeyBindingDefOf.Command_ItemForbid,
				icon = basisZoneData.iconCache[basisZoneData.sowMode],
				action = () =>
				{
					var newMode = basisZoneData.sowMode;
					switch (newMode)
					{
						case SowMode.Force: newMode = SowMode.Off; break;
						case SowMode.On: newMode = SowMode.Smart; break;
						case SowMode.Smart: newMode = SowMode.Force; break;
						default: newMode = SowMode.On; break;
					}

					var selectedZones = Find.Selector.SelectedObjects;
					foreach (var obj in selectedZones)
					{
						if (obj is Zone_Growing growZone && comp.growZoneRegistry.TryGetValue(growZone.ID, out ZoneData data))
						{
							data.SwitchSowMode(comp, growZone, newMode);
						}
					}
				}
			};
			var priorityGizmo = new Command_Action()
			{
				defaultLabel = ("SmartFarming.Icon.SetAll".Translate() + basisZoneData.priorityGizmo.defaultLabel.ToLower()),
				defaultDesc = basisZoneData.priorityGizmo.defaultDesc,
				icon = ResourceBank.iconPriority,
				action = () =>
				{
					var newPriority = basisZoneData.priority;
					newPriority = newPriority != ZoneData.Priority.Critical ? ++newPriority : ZoneData.Priority.Low;

					var selectedZones = Find.Selector.SelectedObjects;
					foreach (var obj in selectedZones)
					{
						if (obj is Zone_Growing growZone && comp.growZoneRegistry.TryGetValue(growZone.ID, out ZoneData data))
						{
							data.SwitchPriority(newPriority);
						}
					}
				}
			};

			switch (basisZoneData.priority)
			{
				case ZoneData.Priority.Low:
					priorityGizmo.SetColorOverride(ResourceBank.grey);
					break;
				case ZoneData.Priority.Preferred:
					priorityGizmo.SetColorOverride(ResourceBank.green);
					break;
				case ZoneData.Priority.Important:
					priorityGizmo.SetColorOverride(ResourceBank.yellow);
					break;
				case ZoneData.Priority.Critical:
					priorityGizmo.SetColorOverride(ResourceBank.red);
					break;
				default:
					priorityGizmo.SetColorOverride(Color.white);
					break;
			}
			yield return priorityGizmo;
			if (basisZone != null)
			{
				yield return new Command_Action()
				{
					defaultLabel = "SmartFarming.Icon.MergeZones".Translate(),
					defaultDesc = "SmartFarming.Icon.MergeZones.Desc".Translate(),
					icon = ResourceBank.mergeZones,
					action = () => zoneData.MergeZones(thisZone, basisZone)
				};
			}
			yield break;
		}
	}

	//This controls whether or not pawns will skip sow jobs based on the seasonable allowance
	[HarmonyPatch(typeof(WorkGiver_GrowerSow), nameof(WorkGiver_GrowerSow.JobOnCell))]
	[HarmonyPriority(HarmonyLib.Priority.Last)]
	static class Patch_JobOnCell
	{
		static bool Prefix(Pawn pawn, IntVec3 c)
		{
			var map = pawn.Map;
			Zone_Growing zone = map.zoneManager.zoneGrid[c.z * map.info.sizeInt.x + c.x] as Zone_Growing;
			if (zone != null && ReGrowthCore_SmartFarming.compCache.TryGetValue(map.uniqueID, out MapComponent_SmartFarming comp) && comp.growZoneRegistry.TryGetValue(zone.ID, out ZoneData zoneData))
			{
				switch (zoneData.sowMode)
				{
					case SowMode.Smart:
						{
							return zoneData.alwaysSow ? true : zoneData.minHarvestDayForNewlySown > -1;
						}
					case SowMode.Force:
						{
							return true;
						}
					case SowMode.On:
						{
							return true; //Vanilla handling
						}
					case SowMode.Off:
						{
							return false;
						}
				}
			}
			return true;
		}
	}

	//This adds information to the inspector window
	[HarmonyPatch(typeof(Zone_Growing), nameof(Zone_Growing.GetInspectString))]
	static class Patch_GetInspectString
	{
		static float totalHungerRate = 0f;
		static string Postfix(string __result, Zone_Growing __instance)
		{
			Map map = __instance.Map;
			if (ReGrowthCore_SmartFarming.compCache.TryGetValue(map.uniqueID, out MapComponent_SmartFarming mapComp) && mapComp.growZoneRegistry.TryGetValue(__instance.ID, out ZoneData zoneData))
			{
				//Update the hunger cache only when it's being viewed
				if (totalHungerRate == 0f || Find.TickManager.TicksGame % 480 == 0)
				{
					try
					{
						totalHungerRate = mapComp.CalculateTotalHungerRate();
					}
					catch (Exception ex)
					{
						Log.Warning("[Smart Farming] Error calculating hunger rate" + ex);
						totalHungerRate = 1f;
					}
				}

				StringBuilder builder = new StringBuilder(__result, 10);
				if (zoneData.averageGrowth < __instance.plantDefToGrow?.plant.harvestMinGrowth)
				{
					if (zoneData.minHarvestDay > 0)
					{
						builder.Append(ResourceBank.minHarvestDay);
						builder.Append(GenDate.DateFullStringAt(zoneData.minHarvestDay, Find.WorldGrid.LongLatOf(map.Tile)));
					}
					else
						builder.Append(ResourceBank.minHarvestDayFail);
				}
				if (zoneData.fertilityAverage != 0)
					builder.Append("SmartFarming.Inspector.Fertility".Translate(zoneData.fertilityAverage.ToStringPercent(), zoneData.fertilityLow.ToStringPercent()));
				if (zoneData.nutritionYield != 0)
				{
					builder.Append(ResourceBank.yield);
					builder.Append(Math.Round(zoneData.nutritionYield, 2));
				}
				if (__instance.plantDefToGrow?.plant.harvestedThingDef?.ingestible?.HumanEdible ?? false)
					builder.Append("SmartFarming.Inspector.DaysWorth".Translate(Math.Round(zoneData.nutritionYield * ReGrowthCore_SmartFarming.ModSettings.processedFoodFactor / totalHungerRate, 2)));

				return builder.ToString();
			}
			else return __result;
		}
	}

	//This is for the "auto cut blighted" mod option
	[HarmonyPatch(typeof(Plant), nameof(Plant.CropBlighted))]
	static class AutoCutIfBlighted
	{
		static void Postfix(Plant __instance)
		{
			Map map = __instance.Map;
			if (ReGrowthCore_SmartFarming.ModSettings.autoCutBlighted && map.designationManager.DesignationOn(__instance, DesignationDefOf.CutPlant) == null)
			{
				map.designationManager.AddDesignation(new Designation(__instance, DesignationDefOf.CutPlant));
			}
		}
	}

	//This is for the "auto cut if dying" mod option
	[HarmonyPatch(typeof(Plant), nameof(Plant.MakeLeafless))]
	static class AutoCutIfDying
	{
		static bool Prepare()
		{
			return ReGrowthCore_SmartFarming.ModSettings.autoCutDying;
		}

		static void Prefix(Plant __instance)
		{
			Map map = __instance.Map;
			if (__instance.def.plant.dieIfLeafless && ReGrowthCore_SmartFarming.compCache.TryGetValue(map.uniqueID, out MapComponent_SmartFarming mapComp) &&
				map.zoneManager.ZoneAt(__instance.Position) is Zone_Growing zone)
			{
				mapComp.HarvestNow(zone);
			}
		}
	}

	//This is for the "allow harvest" gizmo
	[HarmonyPatch(typeof(WorkGiver_GrowerHarvest), nameof(WorkGiver_GrowerHarvest.HasJobOnCell))]
	static class AllowHarvest
	{
		public static bool allowHarvestGizmoPatched = false;
		static bool Prepare()
		{
			allowHarvestGizmoPatched = ReGrowthCore_SmartFarming.ModSettings.allowHarvestOption;
			return ReGrowthCore_SmartFarming.ModSettings.allowHarvestOption;
		}
		static bool Prefix(Pawn pawn, IntVec3 c)
		{
			Map map = pawn?.Map;

			//We don't check the zone type because it's faster for the collection lookup to return with nothing than it is to cast the zone
			int zoneID = map?.zoneManager.zoneGrid[c.z * map.info.sizeInt.x + c.x]?.ID ?? -1;
			if (zoneID == -1) return true;

			if (ReGrowthCore_SmartFarming.compCache.TryGetValue(map.uniqueID, out MapComponent_SmartFarming mapComp) && mapComp.growZoneRegistry.TryGetValue(zoneID, out ZoneData zoneData))
			{
				return zoneData.allowHarvest;
			}

			return true;
		}
	}

	//Skip the contigious check for merged zones
	[HarmonyPatch(typeof(Zone), nameof(Zone.CheckContiguous))]
	static class Patch_CheckContiguous
	{
		static bool Prefix(Zone __instance)
		{
			return !(__instance is Zone_Growing zone &&
			ReGrowthCore_SmartFarming.compCache.TryGetValue(zone.zoneManager.map.uniqueID, out MapComponent_SmartFarming mapComp) &&
			mapComp.growZoneRegistry.TryGetValue(zone.ID, out ZoneData zoneData) && zoneData.isMerged);
		}
	}
}
