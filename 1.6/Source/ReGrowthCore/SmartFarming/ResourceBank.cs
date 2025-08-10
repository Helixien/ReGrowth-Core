using UnityEngine;
using Verse;
using RimWorld;

namespace ReGrowthCore
{
	[StaticConstructorOnStartup]
	public static class ResourceBank
	{
		public static readonly Texture2D sowIconOn = ContentFinder<Texture2D>.Get("UI/Gizmos/Sow", true),
			sowIconOff = ContentFinder<Texture2D>.Get("UI/Gizmos/NoSow", true),
			sowIconSmart = ContentFinder<Texture2D>.Get("UI/Gizmos/SmartSow", true),
			sowIconForce = ContentFinder<Texture2D>.Get("UI/Gizmos/ForceSow", true),
			iconPriority = ContentFinder<Texture2D>.Get("UI/Gizmos/Priority", true),
			allowHarvest = ContentFinder<Texture2D>.Get("UI/Gizmos/AllowHarvest", true),
			iconHarvest = ContentFinder<Texture2D>.Get("UI/Designators/Harvest", true),
			orchardAlignment = ContentFinder<Texture2D>.Get("UI/Gizmos/AlignPlants", true),
			mergeZones = ContentFinder<Texture2D>.Get("UI/Gizmos/MergeZones", true);

		public static readonly string minHarvestDay = "SmartFarming.Inspector.MinHarvestDay".Translate(),
			minHarvestDayFail = "SmartFarming.Inspector.MinHarvestDayFail".Translate(),
			yield = "SmartFarming.Inspector.Yield".Translate();

		public static readonly Color white = Color.white, grey = Color.grey, green = Color.green, yellow = Color.yellow, red = Color.red;
	}
}
