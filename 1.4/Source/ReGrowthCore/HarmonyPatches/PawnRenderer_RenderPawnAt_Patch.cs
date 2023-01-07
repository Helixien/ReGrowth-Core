using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace ReGrowthCore
{
    [HotSwappable]
    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnAt")]
    public static class PawnRenderer_RenderPawnAt_Patch
    {
        public static void UseDynamicDrawIfNeeded(Pawn pawn, ref bool flag)
        {
            if (pawn.IsBathingNow())
            {
                pawn.ClearGraphicCache();
                flag = false;
            }
        }

        public static void ClearGraphicCache(this Pawn pawn)
        {
            if (pawn.Drawer.renderer.graphics.apparelGraphics.Any())
            {
                pawn.Drawer.renderer.graphics.ClearCache();
                pawn.Drawer.renderer.graphics.apparelGraphics.Clear();
            }
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            bool found = false;
            var codes = codeInstructions.ToList();
            foreach (var code in codes)
            {
                yield return code;
                if (!found && code.opcode == OpCodes.Stloc_3)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PawnRenderer), "pawn"));
                    yield return new CodeInstruction(OpCodes.Ldloca, 3);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PawnRenderer_RenderPawnAt_Patch), nameof(UseDynamicDrawIfNeeded)));
                    found = true;
                }
            }
            if (!found)
            {
                Log.Error("Regrowth Core failed to transpile PawnRenderer.RenderPawnAt");
            }
        }

    }
}
