using ModSettingsFramework;
using Verse;
using System;
using System.Linq;
using System.Xml;
using System.Collections.Generic;

namespace ReGrowthCore
{
    public class ReGrowthCore_SimpleFX : PatchOperationWorker
    {
        private static ReGrowthCore_SimpleFX _handle;
        public static ReGrowthCore_SimpleFX ModSettings => _handle ??= LoadedModManager.GetMod<ReGrowthMod>().Content
            .Patches.OfType<ReGrowthCore_SimpleFX>().FirstOrDefault();
        public bool considerOutdoors = false;
        public float sizeMultiplier = 1f;
        public float splashRarity = 1f;
        public float natureFilter = 0.1f;
#pragma warning disable IDE0044
        private List<PatchOperation> operations = new List<PatchOperation>();
#pragma warning restore IDE0044 
        public override void ExposeData()
        {
            Scribe_Values.Look(ref considerOutdoors, "considerOutdoors", false);
            Scribe_Values.Look(ref sizeMultiplier, "sizeMultiplier", 1f);
            Scribe_Values.Look(ref splashRarity, "splashRarity", 1f);
            Scribe_Values.Look(ref natureFilter, "natureFilter", 0.1f);
        }

        public override void CopyFrom(PatchOperationWorker other)
        {
            if (other is ReGrowthCore_SimpleFX s)
            {
                considerOutdoors = s.considerOutdoors;
                sizeMultiplier = s.sizeMultiplier;
                splashRarity = s.splashRarity;
                natureFilter = s.natureFilter;
            }
        }

        public override void DoSettings(ModSettingsContainer container, Listing_Standard list)
        {
            DoCheckbox(list, "SimpleFxVapor.Settings.ConsiderOutdoors".Translate(), ref considerOutdoors, "SimpleFxVapor.Settings.ConsiderOutdoors.Desc".Translate());

            DoSlider(list, "SimpleFxSplashes.Settings.SizeMultiplier".Translate("1", "0.5", "2", Math.Round(sizeMultiplier, 1)), ref sizeMultiplier, sizeMultiplier.ToString(), 0.5f, 2f, "SimpleFxSplashes.Settings.SizeMultiplier.Desc".Translate());

            DoSlider(list, "SimpleFxSplashes.Settings.SplashRarity".Translate("1", "0.5", "2", Math.Round(splashRarity, 1)), ref splashRarity, splashRarity.ToString(), 0.5f, 2f, "SimpleFxSplashes.Settings.SplashRarity.Desc".Translate());

            DoSlider(list, "SimpleFxSplashes.Settings.FilterNature".Translate("0.1", "0", "1", Math.Round(natureFilter, 2)), ref natureFilter, natureFilter.ToString(), 0f, 1f, "SimpleFxSplashes.Settings.FilterNature.Desc".Translate((Math.Round(natureFilter, 2) * 100).ToString()));
        }

        public override void Reset()
        {
            considerOutdoors = false;
            sizeMultiplier = 1f;
            splashRarity = 1f;
            natureFilter = 0.1f;
        }

        public override void ApplySettings()
        {
            base.ApplySettings();
            if (Current.ProgramState == ProgramState.Playing && Find.CurrentMap != null)
            {
                foreach (var map in Find.Maps) SplashesUtility.RebuildCache(map);
            }
        }

        public override bool ApplyWorker(XmlDocument xml)
        {
            if (CanRun())
            {
                var container = SettingsContainer;
                if (container != null)
                {
                    if (operations != null)
                    {
                        foreach (PatchOperation operation in operations)
                        {
                            if (!operation.Apply(xml))
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
            }
            return true;
        }

    }
}
