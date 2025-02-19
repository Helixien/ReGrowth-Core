using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ReGrowthCore
{
	[HarmonyPatch(typeof(ThingDef), "SpecialDisplayStats")]
	public static class ThingDef_SpecialDisplayStats_Patch
	{
		public static readonly string minGrowthTempCheck = "MinGrowthTemperature".Translate().CapitalizeFirst();
		public static readonly string maxGrowthTempCheck = "MaxGrowthTemperature".Translate().CapitalizeFirst();
		public static readonly string minGrowthTempString = "MinGrowthTemperature".Translate();
		public static readonly string maxGrowthTempString = "MaxGrowthTemperature".Translate();
		public static void Postfix(ThingDef __instance, ref IEnumerable<StatDrawEntry> __result)
		{
			if (__instance.plant == null) return;
			float minGrowthTemperature;
			float maxGrowthTemperature;
			var plantextension = __instance.GetModExtension<PlantExtension>();
			if (plantextension == null) return;
			minGrowthTemperature = plantextension.minGrowthTemperature;
			maxGrowthTemperature = plantextension.maxGrowthTemperature;
			__result = __result.Select(entry =>
			{
				if (entry.LabelCap == minGrowthTempCheck) return new StatDrawEntry(StatCategoryDefOf.Basics,
				 minGrowthTempString, minGrowthTemperature.ToStringTemperature(),
				"Stat_Thing_Plant_MinGrowthTemperature_Desc".Translate(), 4152);

				if (entry.LabelCap == maxGrowthTempCheck) return new StatDrawEntry(StatCategoryDefOf.Basics, maxGrowthTempString,
				maxGrowthTemperature.ToStringTemperature(),
				"Stat_Thing_Plant_MaxGrowthTemperature_Desc".Translate(), 4153);

				return entry;
			});
		}
	}
}