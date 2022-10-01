using RimWorld;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace ReGrowthCore
{
    public class JoyGiver_Bathe : JoyGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            if (PawnUtility.WillSoonHaveBasicNeed(pawn))
            {
                return null;
            }
            var result = FindSpotToBathe(pawn);
            if (result.IsValid)
            {
                return JobMaker.MakeJob(RGDefOf.RG_Bathe, result);
            }
            return null;
        }

        public static bool IsGoodSpotForBathing(Pawn pawn, IntVec3 cell, StringBuilder failReason = null)
        {
            var mapHeld = pawn.MapHeld;
            if (cell.UsesOutdoorTemperature(mapHeld) && !JoyUtility.EnjoyableOutsideNow(mapHeld, failReason))
            {
                return false;
            }
            var comfortRange = GenTemperature.ComfortableTemperatureRange(pawn.def, null);
            comfortRange.min -= 10f;
            comfortRange.max += 10f;
            float temp = cell.GetTemperature(mapHeld);
            if (!comfortRange.Includes(temp))
            {
                var terrain = pawn.Position.GetTerrain(mapHeld);
                if (comfortRange.min > temp && terrain != RGDefOf.RG_HotSpring)
                {
                    failReason?.Append("RG.TooCold".Translate());
                    return false;
                }
                else if (temp > comfortRange.max && terrain == RGDefOf.RG_HotSpring)
                {
                    failReason?.Append("RG.TooHot".Translate());
                    return false;
                }
            }
            return true;
        }

        public int NearbyWaterCount(IntVec3 cell, Map map)
        {
            int nearbyWaterCount = 0;
            foreach (var adj in GenAdj.AdjacentCells)
            {
                var nearbyCell = adj + cell;
                if (nearbyCell.GetTerrain(map).IsWater)
                {
                    nearbyWaterCount++;
                }
            }
            return nearbyWaterCount;
        }
        public IntVec3 FindSpotToBathe(Pawn pawn)
        {
            bool CellValidator(IntVec3 x)
            {
                if (x.GetZone(pawn.Map) is Zone_Bathe && !PawnUtility.KnownDangerAt(x, pawn.Map, pawn)
                    && IsGoodSpotForBathing(pawn, x) && pawn.CanReserve(x))
                {
                    foreach (var adj in GenAdj.AdjacentCells)
                    {
                        var nearbyCell = adj + x;
                        if (nearbyCell.GetZone(pawn.Map) is Zone_Bathe && pawn.CanReserve(nearbyCell) is false)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }
            var cells = pawn.Map.AllCells.Where(x => CellValidator(x)).OrderByDescending(x => NearbyWaterCount(x, pawn.Map))
                .ThenBy(x => pawn.Position.DistanceTo(x)).Take(10);
            if (cells.TryRandomElement(out var cell))
            {
                return cell;
            }
            return IntVec3.Invalid;
        }
    }
}
