using System;
using RimWorld;
using HarmonyLib;
using Verse;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse.Noise;
using UnityEngine;
using System.Linq;
using System.Xml;
using ModSettingsFramework;

namespace ReGrowthCore
{
    [HarmonyPatch(typeof(LoadedModManager), "ApplyPatches")]
    public static class LoadedModManager_ApplyPatches
    {
        public static void Postfix(XmlDocument xmlDoc, Dictionary<XmlNode, LoadableXmlAsset> assetlookup)
        {
            var patchOp = new PatchOperationAttributeSet
            {
                xpath = "Defs/BiomeDef/modExtensions/li[@Class=\"BiomesKit.BiomesKitControls\"]",
                attribute = "Class",
                value = "RegrowthCore.BiomesKitControl"
            };
            patchOp.Apply(xmlDoc);
        }
    }

    [HarmonyPatch(typeof(WorldLayer_Hills), "Regenerate")]
    internal static class WorldLayer
    {
        internal static void Prefix()
        {
            var container = ModSettingsFrameworkSettings.GetModSettingsContainer(ReGrowthMod.modPack.PackageIdPlayerFacing);
            if (container.patchOperationStates.TryGetValue("RG_WorldMapBeautificationProject", out var state) && state)
            {
                Material noMaterial = MaterialPool.MatFrom("Transparent", ShaderDatabase.WorldOverlayTransparentLit, 3510);
                AccessTools.Field(typeof(WorldMaterials), nameof(WorldMaterials.SmallHills)).SetValue(null, noMaterial);
                AccessTools.Field(typeof(WorldMaterials), nameof(WorldMaterials.LargeHills)).SetValue(null, noMaterial);
                AccessTools.Field(typeof(WorldMaterials), nameof(WorldMaterials.Mountains)).SetValue(null, noMaterial);
                AccessTools.Field(typeof(WorldMaterials), nameof(WorldMaterials.ImpassableMountains)).SetValue(null, noMaterial);
            }
        }
    }
}
