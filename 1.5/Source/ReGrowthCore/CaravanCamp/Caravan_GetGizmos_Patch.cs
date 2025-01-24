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
            bool yieldedMakeCamp = false;
            foreach (Gizmo item in __result)
            {
                yield return item;
                if (yieldedMakeCamp is false && item is Command_Action command && command.icon == SettleUtility.SettleCommandTex 
                    && command.defaultLabel == "CommandSettle".Translate())
                {
                    if (__instance.IsPlayerControlled && ReGrowthUtils.MakeCampPatchWorker.enableMakeCamp)
                    {
                        var makeCamp = new Command_Action
                        {
                            defaultLabel = "RG.MakeCamp".Translate(),
                            defaultDesc = "RG.MakeCampDesc".Translate(),
                            icon = ContentFinder<Texture2D>.Get("UI/Gizmos/CreateCamp"),
                            action = () =>
                            {
                                MakeCamp(__instance);
                            }
                        };
                        var invalidReason = new StringBuilder();
                        if (!TileFinder.IsValidTileForNewSettlement(__instance.Tile, invalidReason))
                        {
                            makeCamp.Disable(invalidReason.ToString());
                        }
                        yield return makeCamp;
                        yieldedMakeCamp = true;
                    }
                }
            }


        }

        public static void MakeCamp(Caravan caravan)
        {
            LongEventHandler.QueueLongEvent(delegate
            {
                CaravanCamp caravanCamp = (CaravanCamp)WorldObjectMaker.MakeWorldObject(RG_DefOf.RG_CaravanCamp);
                caravanCamp.Tile = caravan.Tile;
                caravanCamp.SetFaction(caravan.Faction);
                caravanCamp.AddSitePart();
                Find.WorldObjects.Add(caravanCamp);
                var map = MapGenerator.GenerateMap(CaravanCamp.MapSize, caravanCamp, caravanCamp.MapGeneratorDef, caravanCamp.ExtraGenStepDefs);
                CaravanEnterMapUtility.Enter(caravan, map, CaravanEnterMode.Edge);
                CameraJumper.TryJump(MapGenerator.PlayerStartSpot, map);
            }, "GeneratingMap", doAsynchronously: false, null);
        }
    }
}
