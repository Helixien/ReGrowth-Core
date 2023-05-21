using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ReGrowthCore
{
    public class ReGrowthSettings : ModSettings
    {
        public bool enableLeaveSpawners = true;
        public bool enableAutumnLeaveSpawners = true;

        public Dictionary<string, bool> patchOperationStates = new Dictionary<string, bool>();
        public Dictionary<string, float> patchOperationValues = new Dictionary<string, float>();
        public Dictionary<string, bool> weatherDefStates = new Dictionary<string, bool>();
        public bool PatchOperationEnabled(string id, bool defaultValue)
        {
            if (!patchOperationStates.TryGetValue(id, out var enabled))
            {
                patchOperationStates[id] = enabled = defaultValue;
            }
            return enabled;
        }

        public float PatchOperationValue(string id, float defaultValue)
        {
            if (!patchOperationValues.TryGetValue(id, out var value))
            {
                patchOperationValues[id] = value = defaultValue;
            }
            return value;
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref enableLeaveSpawners, "enableLeaveSpawners", true, true);
            Scribe_Values.Look(ref enableAutumnLeaveSpawners, "enableAutumnLeaveSpawners", true, true);
            Scribe_Collections.Look(ref patchOperationStates, "patchOperationStates");
            Scribe_Collections.Look(ref patchOperationValues, "patchOperationValues");
            Scribe_Collections.Look(ref weatherDefStates, "weatherDefStates");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                patchOperationStates ??= new Dictionary<string, bool>();
                patchOperationValues ??= new Dictionary<string, float>();
                weatherDefStates ??= new Dictionary<string, bool>();
            }
        }

        public static void ApplySettings()
        {
            foreach (var weatherDef in DefDatabase<WeatherDef>.AllDefs.ToList())
            {
                if (weatherDef.modContentPack == ReGrowthMod.modPack)
                {
                    if (!ReGrowthMod.settings.weatherDefStates.TryGetValue(weatherDef.defName, out var state))
                    {
                        ReGrowthMod.settings.weatherDefStates[weatherDef.defName] = state = true;
                    }
                }
            }
        }
    }
}
