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

        private static Vector2 scrollPosition = Vector2.zero;
        private static float scrollHeight;
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            var list = new Listing_Standard();
            Rect rect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            Rect rect2 = new Rect(0f, 0f, inRect.width - 30f, scrollHeight);
            Widgets.BeginScrollView(rect, ref scrollPosition, rect2, true);
            scrollHeight = 0;
            list.Begin(rect2);
            Text.Font = GameFont.Small;
            list.CheckboxLabeled("Enable all leaves spawners", ref settings.enableLeaveSpawners);
            list.Gap(5);
            scrollHeight += 29;
            list.CheckboxLabeled("Enable all autumn leaves spawners", ref settings.enableAutumnLeaveSpawners);
            list.Gap(5);
            scrollHeight += 29;
            foreach (var patch in LoadedModManager.RunningMods.SelectMany(x => x.Patches.OfType<PatchOperationModSettings>()))
            {
                if (patch.mods != null && patch.mods.Any(x => ModLister.HasActiveModWithName(x)) is false)
                {
                    continue;
                }
                if (patch is PatchOperationModOption modOption)
                {
                    var value = settings.PatchOperationEnabled(patch.id, modOption.defaultValue);
                    list.CheckboxLabeled(patch.label, ref value, patch.tooltip);
                    settings.patchOperationStates[patch.id] = value;
                    list.Gap(5);
                    scrollHeight += 29;
                }
                else if (patch is PatchOperationSliderFloat sliderFloat)
                {
                    var value = settings.PatchOperationValue(patch.id, sliderFloat.defaultValue);
                    value = (float)Math.Round(list.SliderLabeled(patch.label + ": " + value, value, sliderFloat.range.TrueMin,
                        sliderFloat.range.TrueMax, tooltip: patch.tooltip), sliderFloat.roundToDecimalPlaces);
                    settings.patchOperationValues[patch.id] = value;
                    list.Gap(5);
                    scrollHeight += 29;
                }
                else if (patch is PatchOperationSliderInt sliderInt)
                {
                    var value = (int)settings.PatchOperationValue(patch.id, sliderInt.defaultValue);
                    value = (int)list.SliderLabeled(patch.label + ": " + value, value, sliderInt.range.TrueMin, 
                        sliderInt.range.TrueMax, tooltip: patch.tooltip);
                    settings.patchOperationValues[patch.id] = value;
                    list.Gap(5);
                    scrollHeight += 29;
                }
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
                    scrollHeight += 29;
                }
            }
            list.End();
            Widgets.EndScrollView();
        }

        public override string SettingsCategory()
        {
            return Content.Name;
        }
    }
}
