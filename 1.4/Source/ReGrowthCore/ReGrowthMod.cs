using HarmonyLib;
using UnityEngine;
using Verse;

namespace ReGrowthCore
{
    public class ReGrowthMod : Mod
    {
        public static ReGrowthSettings settings;
        public ReGrowthMod(ModContentPack pack) : base(pack)
        {
            new Harmony("Helixien.ReGrowthCore").PatchAll();
            settings = GetSettings<ReGrowthSettings>();
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
            list.End();
        }

        public override string SettingsCategory()
        {
            return Content.Name;
        }
    }
}
