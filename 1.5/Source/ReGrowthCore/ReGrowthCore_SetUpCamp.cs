using ModSettingsFramework;
using Verse;

namespace ReGrowthCore
{
    public class ReGrowthCore_SetUpCamp : PatchOperationWorker
    {
        public bool enableSetUpCamp = true;

        public override void CopyFrom(PatchOperationWorker savedWorker)
        {
            var copy = savedWorker as ReGrowthCore_SetUpCamp;
            enableSetUpCamp = copy.enableSetUpCamp;
        }

        public override void DoSettings(ModSettingsContainer container, Listing_Standard list)
        {
            DoCheckbox(list, "RG.EnableSetUpCamp".Translate(), ref enableSetUpCamp, "RG.EnableSetUpCampDesc".Translate());
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref enableSetUpCamp, "enableSetUpCamp");
        }

        public override void Reset()
        {
            enableSetUpCamp = true;
        }
    }
}
