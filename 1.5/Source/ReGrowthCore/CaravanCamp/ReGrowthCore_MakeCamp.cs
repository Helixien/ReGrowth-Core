using ModSettingsFramework;
using Verse;

namespace ReGrowthCore
{
    public class ReGrowthCore_MakeCamp : PatchOperationWorker
    {
        public bool enableMakeCamp = true;
        public int preserveSavedCampsForDays = 3;
        public override void CopyFrom(PatchOperationWorker savedWorker)
        {
            var copy = savedWorker as ReGrowthCore_MakeCamp;
            enableMakeCamp = copy.enableMakeCamp;
            preserveSavedCampsForDays = copy.preserveSavedCampsForDays;
        }

        public override void DoSettings(ModSettingsContainer container, Listing_Standard list)
        {
            DoCheckbox(list, "RG.EnableMakeCamp".Translate(), ref enableMakeCamp, "RG.EnableMakeCampDesc".Translate());
            if (enableMakeCamp)
            {
                DoSlider(list, "RG.PreserveSavedCamps".Translate(), ref preserveSavedCampsForDays, 
                    preserveSavedCampsForDays == 1 ? "Period1Day".Translate() : "PeriodDays".Translate(preserveSavedCampsForDays).ToString(), 1, 60, "RG.PreserveSavedCampsDesc".Translate());
            }
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref enableMakeCamp, "enableMakeCamp", true);
            Scribe_Values.Look(ref preserveSavedCampsForDays, "preserveSavedCampsForDays", 3);
        }

        public override void Reset()
        {
            enableMakeCamp = true;
            preserveSavedCampsForDays = 3;
        }
    }
}
