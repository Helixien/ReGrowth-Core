﻿using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ReGrowthCore
{
    public class JobDriver_Bathe : JobDriver
    {
        private int bathStartTick = -1;
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(TargetA, job);
        }

        private bool IsBathingNow()
        {
            return CurToilIndex == 2 && pawn.pather.moving is false;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref bathStartTick, "bathStartTick", -1);
            if (Scribe.mode == LoadSaveMode.PostLoadInit && IsBathingNow())
            {
                ClearCache();
            }
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(delegate
            {
                StringBuilder failReason = new();
                bool isGoodSpot = JoyGiver_Bathe.IsGoodSpotForBathing(pawn.Map, TargetA.Cell, JoyGiver_Bathe.GetComfortTempRange(pawn), failReason);
                //if (IsBathingNow() && isGoodSpot is false && failReason.Length > 0)
                if (isGoodSpot is false && failReason.Length > 0)
                {
                    Messages.Message("RG.StoppedBathingMessage".Translate(pawn.Named("PAWN"), failReason.ToString()), pawn, MessageTypeDefOf.NeutralEvent);
                }
                return isGoodSpot is false;
            });
            Toil goToil = Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            yield return goToil;
            Toil batheToil = new()
            {
                initAction = delegate
                {
                    bathStartTick = Find.TickManager.TicksGame;
                    pawn.jobs.posture = Rand.Bool ? PawnPosture.LayingOnGroundNormal : PawnPosture.LayingOnGroundFaceUp;
                    ClearCache();
                },
                tickAction = delegate
                {
                    if (pawn.Drawer.renderer.graphics.cachedMatsBodyBaseHash != -1)
                    {
                        ClearCache();
                    }
                    if (ModCompatibility.DubsBadHygieneActive)
                    {
                        ModCompatibility.CleanHygiene(pawn);
                    }
                    if (pawn.IsHashIntervalTick(60))
                    {
                        TerrainDef terrain = pawn.Position.GetTerrain(Map);
                        if (terrain.IsHotSpring())
                        {
                            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Hypothermia);
                            if (hediff != null)
                            {
                                float value = hediff.Severity * 0.027f;
                                value = Mathf.Clamp(value, 0.0015f, 0.015f);
                                hediff.Severity -= value / 2f;
                            }
                        }
                        else if (terrain.IsWater)
                        {
                            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Heatstroke);
                            if (hediff != null)
                            {
                                float ambientTemperature = pawn.AmbientTemperature;
                                if (hediff != null && ambientTemperature < 60)
                                {
                                    float value = hediff.Severity * 0.027f;
                                    value = Mathf.Clamp(value, 0.0015f, 0.015f);
                                    hediff.Severity -= value / 2f;
                                }
                            }
                        }
                    }
                    pawn.GainComfortFromCellIfPossible(false);
                    if (Find.TickManager.TicksGame > bathStartTick + job.def.joyDuration)
                    {
                        DoBatheEffects();
                        OnComplection();
                        EndJobWith(JobCondition.Succeeded);
                    }
                    else if (JoyUtility.JoyTickCheckEnd(pawn))
                    {
                        DoBatheEffects();
                        OnComplection();
                    }
                }
            };
            batheToil.AddFinishAction(delegate
            {
                OnComplection();
            });
            batheToil.socialMode = RandomSocialMode.SuperActive;
            batheToil.defaultCompleteMode = ToilCompleteMode.Never;
            yield return new Toil
            {
                initAction = delegate ()
                {
                    OnComplection();
                }
            };
            yield return batheToil;
        }

        private void DoBatheEffects()
        {
            TerrainDef terrain = pawn.Position.GetTerrain(Map);
            BatheExtension batheExtension = terrain.GetModExtension<BatheExtension>();
            if (batheExtension != null)
            {
                if (batheExtension.thoughtAfterBathing != null)
                {
                    pawn.needs?.mood?.thoughts.memories.TryGainMemory(batheExtension.thoughtAfterBathing);
                }
                if (batheExtension.hediffAfterBathing != null)
                {
                    pawn.health.AddHediff(batheExtension.hediffAfterBathing);
                }
            }
        }

        private void ClearCache()
        {
            pawn.Drawer.renderer.graphics.ClearCache();
            pawn.Drawer.renderer.graphics.apparelGraphics.Clear();
        }
        private void OnComplection()
        {
            pawn.Drawer.renderer.graphics.ResolveApparelGraphics();
        }
    }
}