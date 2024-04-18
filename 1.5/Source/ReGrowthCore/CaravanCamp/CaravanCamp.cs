using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ReGrowthCore
{
    public class CaravanCamp : MapParent
    {
        public static readonly IntVec3 MapSize = new IntVec3(120, 1, 120);

        public override bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject)
        {
            if (!base.Map.mapPawns.AnyPawnBlockingMapRemoval)
            {
                alsoRemoveWorldObject = true;
                return true;
            }
            alsoRemoveWorldObject = false;
            return false;
        }

        public override void PostMapGenerate()
        {
            base.PostMapGenerate();
            WorldComponent_Camps.Instance.TryResetCamps();
            if (WorldComponent_Camps.Instance.savedCamps.TryGetValue(Tile, out var savedCamp))
            {
                RestoreCampMap(savedCamp);
            }
        }

        private void RestoreCampMap(SavedCamp savedCamp)
        {
            SyncGrids(Map.terrainGrid.underGrid, savedCamp.terrainGrid.underGrid, delegate (int i, TerrainDef def)
            {
                var cell = Map.cellIndices.IndexToCell(i);
                Map.terrainGrid.SetUnderTerrain(cell, def);
            });
            SyncGrids(Map.terrainGrid.topGrid, savedCamp.terrainGrid.topGrid, delegate (int i, TerrainDef def)
            {
                var cell = Map.cellIndices.IndexToCell(i);
                Map.terrainGrid.SetTerrain(cell, def);
            });
            SyncGrids(Map.terrainGrid.colorGrid, savedCamp.terrainGrid.colorGrid, delegate (int i, ColorDef def)
            {
                var cell = Map.cellIndices.IndexToCell(i);
                Map.terrainGrid.SetTerrainColor(cell, def);
            });
            DespawnThings(Map.listerThings.AllThings.Where(x => x is Building).ToList());
            DespawnThings(Map.listerThings.AllThings.Where(x => x is Plant).ToList());
            DespawnThings(Map.listerThings.AllThings.Where(x => x.def.category == ThingCategory.Item).ToList());
            SpawnThings(savedCamp.buildings);
            SpawnThings(savedCamp.plants);
            SpawnThings(savedCamp.items);
            var itemsRottables = savedCamp.items.ToDictionary(x => x, x => x.TryGetComp<CompRottable>());
            itemsRottables.RemoveAll(x => x.Value is null);
            var ticksPassed = Find.TickManager.TicksGame - savedCamp.ticksSaved;
            for (var i = 0; i < ticksPassed; i++)
            {
                foreach (var item in itemsRottables)
                {
                    if (item.Value.parent.Destroyed is false)
                    {
                        item.Value.Tick(1);
                    }
                }
                var map = Map;
                int num = Mathf.CeilToInt((float)map.Area * 0.0006f);
                int area = map.Area;
                for (int j = 0; j < num; j++)
                {
                    if (Map.steadyEnvironmentEffects.cycleIndex >= area)
                    {
                        Map.steadyEnvironmentEffects.cycleIndex = 0;
                    }
                    IntVec3 c = map.cellsInRandomOrder.Get(Map.steadyEnvironmentEffects.cycleIndex);
                    DoCellSteadyEffects(Map.steadyEnvironmentEffects, c);
                    Map.steadyEnvironmentEffects.cycleIndex++;
                }
            }

            SyncGrids(Map.roofGrid.roofGrid, savedCamp.roofGrid.roofGrid, delegate (int i, RoofDef def)
            {
                var cell = Map.cellIndices.IndexToCell(i);
                Map.roofGrid.SetRoof(cell, def);
            });
        }

        private void DoCellSteadyEffects(SteadyEnvironmentEffects steadyEnvironmentEffects, IntVec3 c)
        {
            var map = steadyEnvironmentEffects.map;
            Room room = c.GetRoom(map);
            bool flag = map.roofGrid.Roofed(c);
            bool flag2 = room?.UsesOutdoorTemperature ?? false;

            if (room != null)
            {
                TerrainDef terrain = c.GetTerrain(map);
                List<Thing> thingList = c.GetThingList(map);
                for (int num = thingList.Count - 1; num >= 0; num--)
                {
                    Thing thing = thingList[num];
                    if (thing is not Filth)
                    {
                        steadyEnvironmentEffects.TryDoDeteriorate(thing, flag, flag2, terrain);
                    }
                }
            }
            map.gameConditionManager.DoSteadyEffects(c, map);
            GasUtility.DoSteadyEffects(c, map);
        }

        private void SpawnThings<T>(List<T> things) where T : Thing
        {
            foreach (var thing in things)
            {
                if (thing.Destroyed)
                {
                    thing.mapIndexOrState = -1;
                }
                GenSpawn.Spawn(thing, thing.Position, Map, thing.Rotation);
            }
        }

        private void DespawnThings(List<Thing> allThings)
        {
            foreach (var thing in allThings)
            {
                thing.DeSpawn();
            }
        }

        private void SyncGrids<T>(T[] mapGrid, T[] savedGrid, Action<int, T> syncAction)
        {
            for (int i = mapGrid.Count() - 1; i >= 0; i--)
            {
                var mapValue = mapGrid[i];
                var savedValue = savedGrid[i];
                if (EqualityComparer<T>.Default.Equals(mapValue, savedValue) is false)
                {
                    syncAction(i, savedValue);
                }
            }
        }

        public override void Notify_MyMapAboutToBeRemoved()
        {
            base.Notify_MyMapAboutToBeRemoved();
            var map = Map;
            WorldComponent_Camps.Instance.savedCamps[this.Tile] = new SavedCamp
            {
                buildings = map.listerThings.AllThings.OfType<Building>().ToList(),
                plants = map.listerThings.AllThings.OfType<Plant>().ToList(),
                items = map.listerThings.AllThings.Where(x => x.def.category == ThingCategory.Item).ToList(),
                terrainGrid = map.terrainGrid,
                roofGrid = map.roofGrid,
                ticksSaved = Find.TickManager.TicksGame
            };
        }
    }
}

