using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Noise;

namespace ReGrowthCore
{
    public class JobDriver_SwimInHotSpring : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(TargetA, job);
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => !JoyUtility.EnjoyableOutsideNow(pawn));
            Toil goToil = Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            yield return goToil;
            Toil toil = new Toil();
            toil.initAction = delegate
            {
                startTick = Find.TickManager.TicksGame;
                this.pawn.jobs.posture = Rand.Chance(0.5f) ? PawnPosture.LayingOnGroundFaceUp : PawnPosture.LayingOnGroundNormal;
                this.pawn.Drawer.renderer.graphics.ClearCache();
                this.pawn.Drawer.renderer.graphics.apparelGraphics.Clear();
            };
            toil.tickAction = delegate
            {
                if (Find.TickManager.TicksGame > startTick + job.def.joyDuration)
                {
                    pawn.needs?.mood?.thoughts.memories.TryGainMemory(RGDefOf.RG_HotSpringThought);
                    OnComplection();
                    EndJobWith(JobCondition.Succeeded);
                }
                else
                {
                    JoyUtility.JoyTickCheckEnd(pawn);
                }
                this.pawn.GainComfortFromCellIfPossible(false);
            };
            toil.AddFinishAction(delegate
            {
                OnComplection();
            });
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            yield return new Toil { initAction = delegate () {
                OnComplection();
            } };
            yield return toil;
        }

        private void OnComplection()
        {
            this.pawn.Drawer.renderer.graphics.ResolveApparelGraphics();
        }
    }
}