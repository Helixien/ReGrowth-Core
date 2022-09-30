using HarmonyLib;
using Multiplayer.API;
using Verse;

namespace ReGrowthCore
{
    [StaticConstructorOnStartup]
    internal static class MultiplayerSupport
    {
        static MultiplayerSupport()
        {
            if (!MP.enabled)
            {
                return;
            }
            MP.RegisterSyncMethod(typeof(DevilDust_Tornado), "Tick", null);
            var devilDust_Tornado = AccessTools.Method(typeof(DevilDust_Tornado), "Tick", null, null);
            harmony.Patch(devilDust_Tornado, new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPre", null), new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPos", null), null, null);
        }

        private static void FixRNGPre()
        {
            Rand.PushState(Find.TickManager.TicksAbs);
        }

        private static void FixRNGPos()
        {
            Rand.PopState();
        }

        private static readonly Harmony harmony = new Harmony("rimworld.regrowthcore.multiplayersupport");
    }
}

