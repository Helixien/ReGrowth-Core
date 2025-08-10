using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using Verse;

namespace ReGrowthCore
{
    public class AbandonedCamp : WorldObject
    {
        public SavedCamp savedCamp;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref savedCamp, "savedCamp");
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
        {
            foreach (var o in base.GetFloatMenuOptions(caravan))
            {
                yield return o;
            }

            foreach (var o in CaravanArrivalAction_VisitAbandonedCamp.GetFloatMenuOptions(caravan, this))
            {
                yield return o;
            }
        }

        public override IEnumerable<Gizmo> GetCaravanGizmos(Caravan caravan)
        {
            foreach (var o in base.GetCaravanGizmos(caravan))
            {
                yield return o;
            }
            if (caravan.Faction == Faction.OfPlayer)
            {
                yield return SetupCampGizmo(caravan);
            }
        }

        public Command SetupCampGizmo(Caravan caravan)
        {
            Command_Action command_Action = new Command_Action();
            command_Action.defaultLabel = "CommandCamp".Translate();
            command_Action.defaultDesc = "CommandCampDesc".Translate();
            command_Action.icon = SettleUtility.CreateCampCommandTex;
            command_Action.action = delegate
            {
                Visit(caravan);
            };
            if (!SettleInEmptyTileUtility.CanCreateMapAt(caravan.Tile))
            {
                command_Action.Disable("CommandCampFailSiteAlreadyThere".Translate());
            }
            else if (Find.WorldObjects.AnyMapParentAt(caravan.Tile))
            {
                command_Action.Disable("CommandCampFailExistingWorldObject".Translate());
            }
            return command_Action;
        }

        public void Visit(Caravan caravan)
        {
            var camp = (CaravanCamp)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Camp);
            camp.Tile = Tile;
            camp.savedCamp = savedCamp;
            Find.WorldObjects.Add(camp);
            Find.WorldObjects.Remove(this);
            LongEventHandler.QueueLongEvent(delegate
            {
                GetOrGenerateMapUtility.GetOrGenerateMap(camp.Tile, WorldObjectDefOf.Camp.overrideMapSize ?? Find.World.info.initialMapSize, WorldObjectDefOf.Camp);
                var map = camp.Map;
                Pawn pawn = caravan.PawnsListForReading[0];
                map.Parent.SetFaction(caravan.Faction);
                CaravanEnterMapUtility.Enter(caravan, map, CaravanEnterMode.Edge, CaravanDropInventoryMode.DoNotDrop, draftColonists: false, (IntVec3 x) => x.GetRoom(map).CellCount >= 600);
                map.Parent.GetComponent<TimedDetectionRaids>()?.StartDetectionCountdown(240000, 60000);
                CameraJumper.TryJump(pawn);
                FloodFillerFog.DebugRefogMap(map);
            }, "GeneratingMap", doAsynchronously: true, GameAndMapInitExceptionHandlers.ErrorWhileGeneratingMap);
        }
    }
}
