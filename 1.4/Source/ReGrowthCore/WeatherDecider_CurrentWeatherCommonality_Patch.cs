using HarmonyLib;
using RimWorld;
using Verse;

namespace ReGrowthCore
{
    [HarmonyPatch(typeof(WeatherDecider), "CurrentWeatherCommonality")]
    public static class WeatherDecider_CurrentWeatherCommonality_Patch
    {
        public static void Postfix(ref float __result, WeatherDef weather)
        {
            if (ReGrowthMod.settings.weatherDefStates.TryGetValue(weather.defName, out var state))
            {
                if (state is false)
                {
                    __result = 0f;
                }
            }
        }
    }
}
