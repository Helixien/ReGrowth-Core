using Verse;

namespace ReGrowthCore
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
            if (ReGrowthMod.settings.batheJoyGiver_baseChance == null)
            {
                ReGrowthMod.settings.batheJoyGiver_baseChance = RG_DefOf.RG_BatheJoyGiver.baseChance;
            }
            ReGrowthSettings.ApplySettings();
        }
    }
}
