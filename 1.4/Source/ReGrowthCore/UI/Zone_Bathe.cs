using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace ReGrowthCore
{

    public class Zone_Bathe : Zone
    {
        public override bool IsMultiselectable => false;
        public Color BaseColor = Color.blue;
        public override Color NextZoneColor => new(BaseColor.r, BaseColor.g, BaseColor.b, 0.09f);
        public Zone_Bathe()
        {
        }

        public Zone_Bathe(ZoneManager zoneManager)
            : base("RG.BatheZone".Translate(), zoneManager)
        {
        }

        public override string GetInspectString()
        {
            var sb = new StringBuilder(base.GetInspectString() + "\n");
            var comfortRange = JoyGiver_Bathe.GetComfortTempRange(null);
            var cell = Cells.FirstOrDefault();
            if (cell.UsesOutdoorTemperature(Map))
            {
                GetBathingPeriod(comfortRange, out int bathingPeriodStart, out int bathingPeriodEnd);
                if (bathingPeriodStart != -1 && bathingPeriodEnd != -1)
                {
                    int endDay = (bathingPeriodEnd - bathingPeriodStart) / GenDate.TicksPerDay;
                    int startDay = Mathf.Max(0, Mathf.Min(endDay, (Find.TickManager.TicksAbs - bathingPeriodStart) / GenDate.TicksPerDay));
                    sb.AppendLine("RG.BathingPeriod".Translate(startDay, endDay, GenDate.QuadrumDateStringAt(bathingPeriodStart, Find.WorldGrid.LongLatOf(Map.Tile).x),
                        GenDate.QuadrumDateStringAt(bathingPeriodEnd, Find.WorldGrid.LongLatOf(Map.Tile).x)));
                }
            }

            var failReason = new StringBuilder();
            if (JoyGiver_Bathe.IsGoodSpotForBathing(Map, cell, comfortRange, failReason))
            {
                sb.AppendLine("RG.BathingSeasonIsHere".Translate());
            }
            else
            {
                sb.AppendLine("RG.BathingIsNotPossible".Translate(failReason.ToString()));
            }
            return sb.ToString().TrimEndNewlines();
        }

        private void GetBathingPeriod(FloatRange comfortRange, out int bathingPeriodStart, out int bathingPeriodEnd)
        {
            bathingPeriodStart = -1;
            bathingPeriodEnd = -1;
            int yearCount = 0;
            while (bathingPeriodEnd == -1 && yearCount <= 3)
            {
                int start = Mathf.Min(Mathf.Max(0, GenDate.DaysPassed - 30), (GenDate.YearsPassed + yearCount) * 60);
                int end = start + 120;
                bathingPeriodStart = -1;
                bathingPeriodEnd = -1;
                yearCount++;
                for (int i = start; i < end; i++)
                {
                    int ticksAbs = Find.TickManager.gameStartAbsTick + (i * GenDate.TicksPerDay);
                    float temperatureFromSeasonAtTile = GenTemperature.GetTemperatureFromSeasonAtTile(ticksAbs, Map.Tile);
                    if (bathingPeriodStart == -1 && comfortRange.Includes(temperatureFromSeasonAtTile))
                    {
                        bathingPeriodStart = ticksAbs;
                    }
                    else if (bathingPeriodStart != -1 && bathingPeriodEnd == -1 && !comfortRange.Includes(temperatureFromSeasonAtTile))
                    {
                        if (Find.TickManager.TicksAbs < ticksAbs)
                        {
                            bathingPeriodEnd = ticksAbs;
                            break;
                        }
                        else
                        {
                            bathingPeriodStart = -1;
                        }
                    }
                }
            }
        }

        public override IEnumerable<Gizmo> GetZoneAddGizmos()
        {
            yield return DesignatorUtility.FindAllowedDesignator<Designator_ZoneAdd_Bathe_Expand>();
        }
    }
}
