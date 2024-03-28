using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ReGrowthCore
{
	// Token: 0x02000005 RID: 5
	[HarmonyPatch(typeof(Pawn_FilthTracker))]
	[HarmonyPatch("TryPickupFilth")]
	[HarmonyPatch(new Type[]
	{

	})]
	internal class TryPickupFilth
	{
		// Token: 0x06000006 RID: 6 RVA: 0x00002624 File Offset: 0x00000824
		private static void Postfix(Pawn_FilthTracker __instance, Pawn ___pawn)
		{
			if (___pawn == null)
			{
				return;
			}
			List<Thing> thingList = ___pawn.Position.GetThingList(___pawn.Map);
			for (int i = thingList.Count - 1; i >= 0; i--)
			{
				Filth filth = thingList[i] as Filth;
				if (filth != null && !(filth.def == RGDefOf.RG_Filth_Water))
				{
					__instance.GainFilth(RGDefOf.RG_Filth_WaterSpatter, filth.sources);
					filth.ThinFilth();
				}
			}
		}
	}
}
