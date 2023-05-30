using RimWorld;
using Verse;

namespace ReGrowthCore
{
    public class DeathActionWorker_Hemolope : DeathActionWorker
    {
        public override void PawnDied(Corpse corpse)
        {
            FilthMaker.TryMakeFilth(corpse.PositionHeld, corpse.MapHeld, ThingDefOf.Filth_Blood, count: Rand.RangeInclusive(7, 10), shouldPropagate: true);
        }
    }
}
