using RimWorld;
using System.Linq;
using Verse;
using ModSettingsFramework;
using System.Collections.Generic;

namespace ReGrowthCore
{
	public class ReGrowthCore_General : PatchOperationWorker
	{
		public bool plantPollutionNoDeath = true;
		private Dictionary<ThingDef, Pollution> originalPollutionValues = new Dictionary<ThingDef, Pollution>();
		public override void ExposeData()
		{
			Scribe_Values.Look(ref plantPollutionNoDeath, "plantPollutionNoDeath", true);
		}

		public override void CopyFrom(PatchOperationWorker other)
		{
			if (other is ReGrowthCore_General otherSettings)
			{
				plantPollutionNoDeath = otherSettings.plantPollutionNoDeath;
			}
		}

		public override void DoSettings(ModSettingsContainer container, Listing_Standard listing)
		{
			DoCheckbox(listing, "RG.PlantPollutionNoDeath".Translate(), ref plantPollutionNoDeath,
				"RG.PlantPollutionNoDeathDesc".Translate());
		}

		public override void Reset()
		{
			plantPollutionNoDeath = true;
			ApplySettings();
		}

		public override void ApplySettings()
		{
			var plants = DefDatabase<ThingDef>.AllDefs.Where(x => x.plant != null).ToList();
			if (plantPollutionNoDeath)
			{
				foreach (var plant in plants)
				{
					if (plant.plant.pollutedGraphicPath.NullOrEmpty() is false
						&& plant.plant.pollution == Pollution.CleanOnly)
					{
						if (originalPollutionValues.ContainsKey(plant) is false)
						{
							originalPollutionValues[plant] = plant.plant.pollution;
						}
						plant.plant.pollution = Pollution.Any;
					}
				}
			}
			else
			{
				foreach (var plant in plants)
				{
					if (originalPollutionValues.TryGetValue(plant, out var pollution))
					{
						plant.plant.pollution = pollution;
					}
				}
			}
		}
	}
}