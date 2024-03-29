using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ReGrowthCore
{
    public class PlantExtension : DefModExtension
    {
        public List<TerrainDef> terrainListToGrowOnly;
        public float minLeaflessTemperature = -18f;
        public float maxLeaflessTemperature = -10f;
        public float minGrowthTemperature = 0f;
        public float minOptimalGrowthTemperature = 6f;
        public float maxOptimalGrowthTemperature = 42f;
        public float maxGrowthTemperature = 58f;
        public bool plantRestEnabled = true;
        public float restBeginsInDayPct = 0.8f;
        public float restEndsInDayPct = 0.25f;
        public bool diesFromColdTemperature = true;
        public bool diesFromHeatTemperature = false;
    }

    public class PlantExpandable : Plant
    {
        private PlantExtension _extension;
        public PlantExtension Extension => _extension ??= this.def.GetModExtension<PlantExtension>();

        public static Dictionary<Map, Dictionary<IntVec3, PlantExpandable>> allPlants = new();

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!allPlants.TryGetValue(map, out var dict))
            {
                allPlants[map] = dict = new Dictionary<IntVec3, PlantExpandable>();
            }
            dict[Position] = this;
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            var map = Map;
            var pos = Position;
            base.DeSpawn(mode);
            if (allPlants.TryGetValue(map, out var dict))
            {
                dict.Remove(pos);
            }
        }
        public override bool Resting
        {
            get
            {
                var extension = Extension;
                if (extension.plantRestEnabled)
                {
                    return GenLocalDate.DayPercent(this) < extension.restEndsInDayPct || GenLocalDate.DayPercent(this) > extension.restBeginsInDayPct;
                }
                return false;
            }
        }

        public override void CheckMakeLeafless()
        {
            if (DyingFromPollution)
            {
                MakeLeafless(LeaflessCause.Pollution);
            }
            else if (DyingFromNoPollution)
            {
                MakeLeafless(LeaflessCause.NoPollution);
            }
            else if (Extension.diesFromColdTemperature && base.AmbientTemperature < LeaflessTemperatureThresh)
            {
                MakeLeafless(LeaflessCause.Cold);
            }
            else if (Extension.diesFromHeatTemperature && base.AmbientTemperature > LeaflessTemperatureThresh)
            {
                MakeLeaflessOfHeat();
            }
        }

        public void MakeLeaflessOfHeat()
        {
            bool num = !LeaflessNow;
            Map map = base.Map;
            if (def.plant.dieIfLeafless)
            {
                if (IsCrop)
                {
                    Messages.Message("RG.MessagePlantDiedOfHeat".Translate(GetCustomLabelNoCount(includeHp: false)), new TargetInfo(base.Position, map), MessageTypeDefOf.NegativeEvent);
                }
                TakeDamage(new DamageInfo(DamageDefOf.Rotting, 99999f));
            }
            else
            {
                madeLeaflessTick = Find.TickManager.TicksGame;
            }
            if (num)
            {
                map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlagDefOf.Things);
            }
        }

        public override float LeaflessTemperatureThresh
        {
            get
            {
                var extension = Extension;
                var value = Rand.RangeSeeded(extension.minLeaflessTemperature, extension.maxLeaflessTemperature, thingIDNumber ^ 838051265);
                return value;
            }
        }

        [HarmonyPatch(typeof(Plant), "GrowthRateFactor_Temperature", MethodType.Getter)]
        public static class Plant_GrowthRateFactor_Temperature_Plant
        {
            public static bool Prefix(Plant __instance, ref float __result)
            {
                if (__instance is PlantExpandable plant)
                {
                    __result = plant.GrowthRateFactor_Temperature;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(WorkGiver_GrowerSow), "JobOnCell")]
        public static class WorkGiver_GrowerSow_JobOnCell_Patch
        {
            public static bool isJobSearchingNow;
            public static void Prefix()
            {
                isJobSearchingNow = true;
            }

            public static void Postfix()
            {
                isJobSearchingNow = false;
            }
        }

        [HarmonyPatch(typeof(PlantUtility), "GrowthSeasonNow")]
        public static class PlantUtility_GrowthSeasonNow_Patch
        {
            public static bool Prefix(IntVec3 c, Map map, ref bool __result, bool forSowing = false)
            {
                if (WorkGiver_GrowerSow_JobOnCell_Patch.isJobSearchingNow)
                {
                    if (WorkGiver_Grower.wantedPlantDef == null)
                    {
                        WorkGiver_Grower.wantedPlantDef = WorkGiver_Grower.CalculateWantedPlantDef(c, map);
                    }
                    if (WorkGiver_Grower.wantedPlantDef != null)
                    {
                        var extension = WorkGiver_Grower.wantedPlantDef.GetModExtension<PlantExtension>();
                        if (extension != null)
                        {
                            __result = PlantExpandable.GrowthSeasonNow(extension, c, map, forSowing);
                            return false;
                        }
                    }
                }
                else
                {
                    if (PlantExpandable.allPlants.TryGetValue(map, out var dict))
                    {
                        if (dict.TryGetValue(c, out var plant))
                        {
                            __result = PlantExpandable.GrowthSeasonNow(plant.Extension, c, map, forSowing);
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public static bool GrowthSeasonNow(PlantExtension extension, IntVec3 c, Map map, bool forSowing = false)
        {
            float temperature = c.GetTemperature(map);
            if (temperature > extension.minGrowthTemperature)
            {
                return temperature < extension.maxGrowthTemperature;
            }
            return false;
        }
        public override float GrowthRate
        {
            get
            {
                if (Blighted)
                {
                    return 0f;
                }
                return GrowthRateFactor_Fertility * GrowthRateFactor_Temperature * GrowthRateFactor_Light * GrowthRateFactor_NoxiousHaze;
            }
        }
        public new float GrowthRateFactor_Temperature
        {
            get
            {
                float cellTemp;
                if (!GenTemperature.TryGetTemperatureForCell(Position, Map, out cellTemp))
                    return 1;

                return GrowthRateFactorFor_Temperature(cellTemp, Extension);
            }
        }

        public static float GrowthRateFactorFor_Temperature(float cellTemp, PlantExtension extension)
        {
            if (cellTemp < extension.minOptimalGrowthTemperature)
            {
                return Mathf.InverseLerp(extension.minGrowthTemperature, extension.minOptimalGrowthTemperature, cellTemp);
            }
            if (cellTemp > extension.maxOptimalGrowthTemperature)
            {
                return Mathf.InverseLerp(extension.maxGrowthTemperature, extension.maxOptimalGrowthTemperature, cellTemp);
            }
            return 1f;
        }
    }
}

