using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace ReGrowthCore
{
    [HarmonyPatch(typeof(Caravan), nameof(Caravan.GetGizmos))]
    public static class Caravan_GetGizmos_Patch
    {
        public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Caravan __instance)
        {
            foreach (Gizmo item in __result)
            {
                yield return item;
            }

            if (__instance.IsPlayerControlled && ReGrowthUtils.SetUpCampPatchWorker.enableSetUpCamp)
            {
                var setUpCamp = new Command_Action
                {
                    defaultLabel = "RG.SetUpCamp".Translate(),
                    defaultDesc = "RG.SetUpCampDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Gizmos/CreateCamp"),
                    action = () =>
                    {
                        SetupCamp(__instance);
                    }
                };
                var invalidReason = new StringBuilder();
                if (!TileFinder.IsValidTileForNewSettlement(__instance.Tile, invalidReason))
                {
                    setUpCamp.Disable(invalidReason.ToString());
                }
                yield return setUpCamp;
            }
        }

        public static void SetupCamp(Caravan caravan)
        {
            LongEventHandler.QueueLongEvent(delegate
            {
                CaravanCamp caravanCamp = (CaravanCamp)WorldObjectMaker.MakeWorldObject(RG_DefOf.RG_CaravanCamp);
                caravanCamp.Tile = caravan.Tile;
                caravanCamp.SetFaction(caravan.Faction);
                Find.WorldObjects.Add(caravanCamp);
                var map = MapGenerator.GenerateMap(new IntVec3(120, 1, 120), caravanCamp, caravanCamp.MapGeneratorDef, caravanCamp.ExtraGenStepDefs);
                CaravanEnterMapUtility.Enter(caravan, map, CaravanEnterMode.Edge);
                CameraJumper.TryJump(MapGenerator.PlayerStartSpot, map);
            }, "GeneratingMap", doAsynchronously: false, null);
        }
    }
}
