using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace ReGrowthCore
{
    [HarmonyPatch]
    public static class PawnRenderer_MatAt_Patch
    {
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(PawnRenderer), "DrawPawnBody");
            yield return AccessTools.Method(typeof(PawnRenderer), "DrawHeadHair");
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase method)
        {
            var matAtInfo = AccessTools.Method(typeof(Graphic), "MatAt", new Type[] { typeof(Rot4), typeof(Thing) });
            var modifyMatIfNeedInfo = AccessTools.Method(typeof(PawnRenderer_MatAt_Patch), "ModifyMatIfNeed");
            var flagsIndex = method.GetParameters().FirstIndexOf(x => x.ParameterType == typeof(PawnRenderFlags)) + 1;
            foreach (var instruction in instructions)
            {
                yield return instruction;
                if (instruction.Calls(matAtInfo))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_S, flagsIndex);
                    yield return new CodeInstruction(OpCodes.Call, modifyMatIfNeedInfo);
                }
            }
        }

        public static Material ModifyMatIfNeed(Material mat, PawnRenderer instance, PawnRenderFlags flags)
        {
            if (!flags.FlagSet(PawnRenderFlags.Portrait) && instance.pawn.IsBathingNow())
            {
                return ReGrowthUtils.GetBatheMat(mat);
            }
            return mat;
        }
    }
}
