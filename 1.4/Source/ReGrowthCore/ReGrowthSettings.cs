using Verse;

namespace ReGrowthCore
{
    public class ReGrowthSettings : ModSettings
    {
        public bool enableLeaveSpawners = true;
        public bool enableAutumnLeaveSpawners = true;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref enableLeaveSpawners, "enableLeaveSpawners", true, true);
            Scribe_Values.Look(ref enableAutumnLeaveSpawners, "enableAutumnLeaveSpawners", true, true);
        }
    }
}
