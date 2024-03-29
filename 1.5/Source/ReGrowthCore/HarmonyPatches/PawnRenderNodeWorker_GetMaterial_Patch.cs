using HarmonyLib;
using UnityEngine;
using Verse;

namespace ReGrowthCore
{
    [HarmonyPatch(typeof(PawnRenderNodeWorker), "GetMaterial")]
    public static class PawnRenderNodeWorker_GetMaterial_Patch
    {
        public static void Postfix(ref Material __result, PawnRenderNodeWorker __instance, PawnRenderNode node, PawnDrawParms parms)
        {
            if (NodeIsBodyOrHasParentBody(node))
            {
                if (parms.Portrait is false && parms.pawn.IsBathingNow())
                {
                    __result = ReGrowthUtils.GetBatheMat(__result);
                }
            }
        }
    
        private static bool NodeIsBodyOrHasParentBody(PawnRenderNode node)
        {
            if (node is PawnRenderNode_Body)
            {
                return true;
            }
            if (node.parent is not null)
            {
                return NodeIsBodyOrHasParentBody(node.parent);
            }
            return false;
        }
    }
}
