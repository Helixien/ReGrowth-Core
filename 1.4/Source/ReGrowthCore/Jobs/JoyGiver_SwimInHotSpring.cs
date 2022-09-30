using System;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using Verse.Noise;

namespace ReGrowthCore
{
	public class JoyGiver_SwimInHotSpring : JoyGiver
	{
        public override Job TryGiveJob(Pawn pawn)
        {
            if (!JoyUtility.EnjoyableOutsideNow(pawn))
            {
                return null;
            }
            if (PawnUtility.WillSoonHaveBasicNeed(pawn))
            {
                return null;
            }
            var result = FindHotSpot(pawn);
            if (result.IsValid)
            {
                return JobMaker.MakeJob(RGDefOf.RG_SwimInHotSpring, result);
            }
            return null;
        }

        public IntVec3 FindHotSpot(Pawn pawn)
        {
            Predicate<IntVec3> cellValidator = (IntVec3 x) => x.GetTerrain(pawn.Map) == RGDefOf.RG_HotSpring && !PawnUtility.KnownDangerAt(x, pawn.Map, pawn)
                && x.Standable(pawn.Map) && !x.Roofed(pawn.Map) && pawn.CanReserve(x);
            var terrains = pawn.Map.AllCells.Where(x => cellValidator(x)).OrderBy(x => pawn.Position.DistanceTo(x)).Take(20);
            if (terrains?.Count() > 0)
            {
                return terrains.RandomElement();
            }
            return IntVec3.Invalid;
        }
	}
}
