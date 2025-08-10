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
        public float minDieOfHeatTemperature = 45f;
        public float maxDieOfHeatTemperature = 50f;
        public bool plantRestEnabled = true;
        public float restBeginsInDayPct = 0.8f;
        public float restEndsInDayPct = 0.25f;
        public bool diesFromColdTemperature = true;
        public bool diesFromHeatTemperature = false;
        public bool ignoresLightForGrow = false;
    }

    public class PlantExpandable : Plant
    {
        private PlantExtension _extension;
        public PlantExtension Extension => _extension ??= this.def.GetModExtension<PlantExtension>();
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
            else if (Extension.diesFromHeatTemperature && base.AmbientTemperature > this.LeaflessHeatTemperatureThresh)
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
                if (IsCrop && MessagesRepeatAvoider.MessageShowAllowed("RG.MessagePlantDiedOfHeat-" + def.defName, 240f))
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

                return Rand.RangeSeeded(extension.minLeaflessTemperature, extension.maxLeaflessTemperature, thingIDNumber ^ 838051265);
            }
        }

        public float LeaflessHeatTemperatureThresh
        {
            get
            {
                var extension = Extension;
                return Rand.RangeSeeded(extension.minDieOfHeatTemperature, extension.maxDieOfHeatTemperature, thingIDNumber ^ 838051265);
            }
        }
    }
}

