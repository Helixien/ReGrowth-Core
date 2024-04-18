using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace ReGrowthCore
{
    public class WorldComponent_Camps : WorldComponent
    {
        public Dictionary<int, SavedCamp> savedCamps = new Dictionary<int, SavedCamp>();
        public static WorldComponent_Camps Instance;
        public WorldComponent_Camps(World world) : base(world)
        {
            Instance = this;
        }

        public override void WorldComponentTick()
        {
            base.WorldComponentTick();
            if (Find.TickManager.TicksGame % 60 == 0)
            {
                TryResetCamps();
            }
        }

        public void TryResetCamps()
        {
            savedCamps.RemoveAll(x => Find.TickManager.TicksGame - x.Value.ticksSaved
            >= ReGrowthUtils.MakeCampPatchWorker.preserveSavedCampsForDays * GenDate.TicksPerDay);
        }

        public override void ExposeData()
        {
            Instance = this;
            base.ExposeData();
            Scribe_Collections.Look(ref savedCamps, "savedCamps", LookMode.Value, LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                savedCamps ??= new Dictionary<int, SavedCamp>();
            }
        }
    }
}

