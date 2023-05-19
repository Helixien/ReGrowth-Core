using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ReGrowthCore
{
    public class ReGrowthSettings : ModSettings
    {
        public bool enableLeaveSpawners = true;
        public bool enableAutumnLeaveSpawners = true;
        public float? batheJoyGiver_baseChance;

        public Dictionary<string, bool> patchOperationStates = new Dictionary<string, bool>();
        public Dictionary<string, bool> weatherDefStates = new Dictionary<string, bool>();
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref enableLeaveSpawners, "enableLeaveSpawners", true, true);
            Scribe_Values.Look(ref enableAutumnLeaveSpawners, "enableAutumnLeaveSpawners", true, true);
            Scribe_Values.Look(ref batheJoyGiver_baseChance, "RG_BatheJoyGiver_baseChance");
            Scribe_Collections.Look(ref patchOperationStates, "patchOperationStates");
            Scribe_Collections.Look(ref weatherDefStates, "weatherDefStates");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                patchOperationStates ??= new Dictionary<string, bool>();
                weatherDefStates ??= new Dictionary<string, bool>();
            }
        }

        public static void ApplySettings()
        {
            RG_DefOf.RG_BatheJoyGiver.baseChance = ReGrowthMod.settings.batheJoyGiver_baseChance.Value;
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
