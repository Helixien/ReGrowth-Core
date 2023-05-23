using System.Linq;
using Verse;

namespace ModSettingsFramework
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
            foreach (var mod in LoadedModManager.RunningMods)
            {
                if (ModSettingsFrameworkSettings.modSettingsPerModId.TryGetValue(mod.PackageIdPlayerFacing.ToLower(), out var modSettings))
                {
                    foreach (var patch in mod.Patches.OfType<PatchOperationWorker>())
                    {
                        if (modSettings.patchWorkers.TryGetValue(patch.GetType(), out var worker))
                        {
                            patch.CopyFrom(worker);
                            patch.ApplySettings();
                        }
                        else
                        {
                            patch.ApplySettings();
                        }
                    }
                }
            }
        }
    }
}
