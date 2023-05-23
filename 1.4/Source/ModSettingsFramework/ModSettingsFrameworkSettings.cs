using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ModSettingsFramework
{
    public class ModSettingsFrameworkSettings : ModSettings
    {
        public static Dictionary<string, ModSettingsContainer> modSettingsPerModId = new Dictionary<string, ModSettingsContainer>();
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref modSettingsPerModId, "modSettingsPerModId", LookMode.Value, LookMode.Deep);
        }
        public static ModSettingsContainer GetModSettingsContainer(ModContentPack modHandle)
        {
            if (!modSettingsPerModId.TryGetValue(modHandle.PackageIdPlayerFacing.ToLower(), out var container))
            {
                modSettingsPerModId[modHandle.PackageIdPlayerFacing.ToLower()] = container = new ModSettingsContainer
                { 
                    packageID = modHandle.PackageIdPlayerFacing.ToLower(),
                }; 
            }

            if (container.modHandle is null)
            {
                ModSettingsFrameworkMod.fakeInit = true;
                container.modHandle = new ModSettingsFrameworkMod(modHandle)
                {
                    modPackOverride = modHandle,
                    fakeMod = true,
                };
                ModSettingsFrameworkMod.fakeInit = false;
            }
            return container;
        }
	}
}
