using ModSettingsFramework;
using System.Linq;
using Verse;

namespace ReGrowthCore
{
    public class ReGrowthCore_MakeCamp : PatchOperationWorker
    {
        private static ReGrowthCore_MakeCamp _handle;
        public static ReGrowthCore_MakeCamp ModSettings => _handle ??= LoadedModManager.GetMod<ReGrowthMod>().Content
            .Patches.OfType<ReGrowthCore_MakeCamp>().FirstOrDefault();
        public int preserveSavedCampsForDays = 3;
        public override void CopyFrom(PatchOperationWorker savedWorker)
        {
            var copy = savedWorker as ReGrowthCore_MakeCamp;
            preserveSavedCampsForDays = copy.preserveSavedCampsForDays;
        }

        public override void DoSettings(ModSettingsContainer container, Listing_Standard list)
        {
            DoSlider(list, "RG.PreserveSavedCamps".Translate(), ref preserveSavedCampsForDays,
                preserveSavedCampsForDays == 1 ? "Period1Day".Translate() : "PeriodDays".Translate(preserveSavedCampsForDays).ToString(), 1, 60, "RG.PreserveSavedCampsDesc".Translate());
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref preserveSavedCampsForDays, "preserveSavedCampsForDays", 3);
        }

        public override void Reset()
        {
            preserveSavedCampsForDays = 3;
        }
    }
}
