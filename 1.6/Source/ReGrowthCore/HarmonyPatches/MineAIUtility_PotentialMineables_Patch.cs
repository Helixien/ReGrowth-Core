using System.Collections.Generic;
using Verse;
using RimWorld;
using HarmonyLib;
using System.Linq;

namespace ReGrowthCore
{
    [HarmonyPatch(typeof(MineAIUtility), "PotentialMineables")]
    public static class MineAIUtility_PotentialMineables_Patch
    {
        private static List<Designation> tmpDesignations = new List<Designation>();
        public static IEnumerable<Thing> Postfix(IEnumerable<Thing> __result, Pawn pawn)
        {
            foreach (Thing t in __result)
            {
                yield return t;
            }
            tmpDesignations.Clear();
            tmpDesignations.AddRange(pawn.Map.designationManager.SpawnedDesignationsOfDef(DesignationDefOf.Mine));
            tmpDesignations.AddRange(pawn.Map.designationManager.SpawnedDesignationsOfDef(DesignationDefOf.MineVein));
            foreach (Designation tmpDesignation in tmpDesignations)
            {
                var thing = tmpDesignation.target.Cell.GetFirstMineable(pawn.Map);
                if (thing != null && (thing.def.size.x > 1 || thing.def.size.z > 1))
                {
                    bool flag = false;
                    var cellRect = thing.OccupiedRect().ExpandedBy(1).EdgeCells;
                    foreach (IntVec3 c in cellRect)
                    {
                        if (c.InBounds(pawn.Map) && c.Walkable(pawn.Map))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        if (__result.Contains(thing) is false)
                        {
                            yield return thing;
                        }
                    }
                }

            }
        }
    }
}
