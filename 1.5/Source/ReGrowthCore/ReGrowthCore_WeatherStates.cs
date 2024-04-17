using ModSettingsFramework;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ReGrowthCore
{

    public class ReGrowthCore_WeatherStates : PatchOperationWorker
    {
        public Dictionary<string, bool> weatherDefStates = new Dictionary<string, bool>();
        public override void ExposeData()
        {
            Scribe_Collections.Look(ref weatherDefStates, "weatherDefStates", LookMode.Value, LookMode.Value);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                weatherDefStates ??= new Dictionary<string, bool>();
            }
        }

        public override void CopyFrom(PatchOperationWorker savedWorker)
        {
            var copy = savedWorker as ReGrowthCore_WeatherStates;
            weatherDefStates = copy.weatherDefStates;
        }

        public override void ApplySettings()
        {
            base.ApplySettings();
            foreach (var weatherDef in DefDatabase<WeatherDef>.AllDefs.ToList())
            {
                if (weatherDef.modContentPack == ReGrowthMod.modPack)
                {
                    if (!weatherDefStates.TryGetValue(weatherDef.defName, out var state))
                    {
                        weatherDefStates[weatherDef.defName] = state = true;
                    }
                }
            }
        }

        public override void DoSettings(ModSettingsContainer container, Listing_Standard list)
        {
            foreach (var weatherState in weatherDefStates.ToList())
            {
                var weatherDef = DefDatabase<WeatherDef>.GetNamedSilentFail(weatherState.Key);
                if (weatherDef != null)
                {
                    var value = weatherState.Value;
                    CheckboxLabeled(list, "Enable " + weatherDef.label, ref value);
                    weatherDefStates[weatherState.Key] = value;
                    list.Gap(5);
                }
            }
        }

        public override int SettingsHeight()
        {
            var scrollHeight = 0;
            foreach (var weatherState in weatherDefStates.ToList())
            {
                var weatherDef = DefDatabase<WeatherDef>.GetNamedSilentFail(weatherState.Key);
                if (weatherDef != null)
                {
                    scrollHeight += 29;
                }
            }
            return scrollHeight;
        }

        public override void Reset()
        {
            foreach (var weatherDef in DefDatabase<WeatherDef>.AllDefs.ToList())
            {
                if (weatherDef.modContentPack == ReGrowthMod.modPack)
                {
                    weatherDefStates[weatherDef.defName] = true;
                }
            }
        }
    }
}
