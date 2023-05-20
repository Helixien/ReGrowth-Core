using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace ReGrowthCore
{

    public class ReGrowthMod : Mod
    {
        public static ReGrowthSettings settings;
        public static ModContentPack modPack;
        public ReGrowthMod(ModContentPack pack) : base(pack)
        {
            new Harmony("Helixien.ReGrowthCore").PatchAll();
            settings = GetSettings<ReGrowthSettings>();
            modPack = pack;
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            ReGrowthSettings.ApplySettings();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            var list = new Listing_Standard();
            list.Begin(inRect);
            Text.Font = GameFont.Small;
            list.CheckboxLabeled("Enable all leaves spawners", ref settings.enableLeaveSpawners);
            list.Gap(5);
            list.CheckboxLabeled("Enable all autumn leaves spawners", ref settings.enableAutumnLeaveSpawners);
            list.Gap(5);
            settings.batheJoyGiver_baseChance = (float)Math.Round(list.SliderLabeled("Base chance of bathing activity: " +
                settings.batheJoyGiver_baseChance, settings.batheJoyGiver_baseChance.Value, 0, 10), 2);
            list.Gap(5);
            foreach (var patch in Content.Patches.OfType<PatchOperationModOption>())
            {
                if (patch.mods != null && patch.mods.Any(x => ModLister.HasActiveModWithName(x)) is false)
                {
                    continue;
                }
                if (!settings.patchOperationStates.TryGetValue(patch.id, out var state))
                {
                    settings.patchOperationStates[patch.id] = state = patch.defaultValue;
                }
                var value = settings.patchOperationStates[patch.id];
                list.CheckboxLabeled(patch.modOptionLabel, ref value);
                settings.patchOperationStates[patch.id] = value;
                list.Gap(5);
            }

            foreach (var weatherState in settings.weatherDefStates.ToList())
            {
                var weatherDef = DefDatabase<WeatherDef>.GetNamedSilentFail(weatherState.Key);
                if (weatherDef != null)
                {
                    var value = weatherState.Value;
                    list.CheckboxLabeled("Enable " + weatherDef.label, ref value);
                    settings.weatherDefStates[weatherState.Key] = value;
                    list.Gap(5);
                }
            }
            list.End();
        }

        public override string SettingsCategory()
        {
            return Content.Name;
        }
    }
}
