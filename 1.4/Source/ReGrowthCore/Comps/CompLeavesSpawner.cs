using RimWorld;
using VFECore;

namespace ReGrowthCore
{
    public class CompLeavesSpawner : CompLeavesSpawnerBase
    {
        public override bool ShouldSpawn()
        {
            if (!parent.Spawned || !ReGrowthMod.settings.enableLeaveSpawners)
            {
                return false;
            }
            return true;
        }

        public override void CheckShouldSpawn()
        {
            if (parent is Plant tree && !tree.LeaflessNow)
            {
                if (ticksUntilSpawn <= 0)
                {
                    TryDoSpawn();
                    ResetCountdown();
                }
            }
        }
    }
}
