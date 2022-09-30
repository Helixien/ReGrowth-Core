using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace ReGrowthCore
{
    public class IncidentWorker_DevilDusts : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            var devilDustCount = Rand.RangeInclusive(3, 5);
            var devilDusts = new List<Thing>();
            var spawnCenter = CellFinderLoose.RandomCellWith((IntVec3 c) => c.Standable(map) && !c.Roofed(map) && !c.Fogged(map) && !map.areaManager.Home.ActiveCells.Contains(c), map);
            var takenCells = new List<IntVec3>();
            for (var i = 0; i < devilDustCount; i++)
            {
                var devilDust = ThingMaker.MakeThing(RGDefOf.RG_DustDevil);
                if (CellFinder.TryFindRandomCellNear(spawnCenter, map, 50, (IntVec3 c) => c.Standable(map) && !c.Roofed(map) && !c.Fogged(map) 
                && !map.areaManager.Home.ActiveCells.Contains(c) && !takenCells.Contains(c), out IntVec3 result))
                {
                    GenSpawn.Spawn(devilDust, result, map);
                    devilDusts.Add(devilDust);
                    takenCells.Add(result);
                }
            }
            SendStandardLetter(parms, devilDusts);
            return true;
        }
    }
}

