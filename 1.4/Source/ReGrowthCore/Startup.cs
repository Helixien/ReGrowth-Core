using Verse;

namespace ReGrowthCore
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
            ReGrowthSettings.ApplySettings();
        }
    }
}
