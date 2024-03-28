using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using VREAndroids;

namespace VREAndroidsRegrowthCore
{
    [DefOf]
    public static class VREA_RG_DefOf
    {
        public static WeatherDef RG_SandStorm, RG_SandStormHard, RG_FoggySandstorm;
    }
    public class Gene_SandstormVulnerability : Gene
    {
        public static HashSet<WeatherDef> sandstormWeathers = new HashSet<WeatherDef>
        {
            VREA_RG_DefOf.RG_SandStorm, 
            VREA_RG_DefOf.RG_SandStormHard,
            VREA_RG_DefOf.RG_FoggySandstorm, 
        };
        public static readonly SimpleCurve sandstormVulnerabilityPerPowerChance = new SimpleCurve
        {
            new CurvePoint(0.5f, 0f),
            new CurvePoint(0.75f, 0.035f),
            new CurvePoint(1f, 0.05f)
        };
        public override void Tick()
        {
            base.Tick();
            if (pawn.Spawned && pawn.IsHashIntervalTick(600) && sandstormWeathers.Contains(pawn.Map.weatherManager.curWeather)
                && pawn.Position.Roofed(pawn.Map) is false)
            {
                var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(VREA_DefOf.VREA_Reactor) as Hediff_AndroidReactor;
                if (hediff != null)
                {
                    if (Rand.Chance(sandstormVulnerabilityPerPowerChance.Evaluate(hediff.Energy)))
                    {
                        TaggedString taggedString = "VREA.AndroidShortCircuitSandstorm".Translate(pawn.Named("PAWN"), pawn);
                        TargetInfo targetInfo = new TargetInfo(pawn.Position, pawn.Map);
                        if (pawn.Faction == Faction.OfPlayer)
                        {
                            Find.LetterStack.ReceiveLetter("LetterLabelShortCircuit".Translate(), taggedString, LetterDefOf.NegativeEvent, targetInfo);
                        }
                        else
                        {
                            Messages.Message(taggedString, targetInfo, MessageTypeDefOf.NeutralEvent);
                        }
                        GenExplosion.DoExplosion(pawn.OccupiedRect().RandomCell, pawn.Map, 1.9f, DamageDefOf.Flame, null);
                        hediff.Energy -= 0.05f;
                    }
                }
            }
        }
    }
}
