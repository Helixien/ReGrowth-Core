using HarmonyLib;
using Verse;

namespace ReGrowthCore
{
    public class ReGrowthMod : Mod
    {
        public ReGrowthMod(ModContentPack pack) : base(pack)
        {
            new Harmony("Helixien.ReGrowthCore").PatchAll();
        }
    }
}
