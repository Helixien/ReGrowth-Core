﻿using System;
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
            var patchOpSwapExtension = new PatchOperationAttributeSet
            {
                xpath = "Defs/BiomeDef/modExtensions/li[@Class=\"BiomesKit.BiomesKitControls\"]",
                attribute = "Class",
                value = "RegrowthCore.BiomesKitControl",
                success = PatchOperation.Success.Always,
            };
            patchOpSwapExtension.Apply(xmlDoc);
            string xmlText = "<workerClass>ReGrowthCore.UniversalBiomeWorker</workerClass>";
            XmlDocument xmlDoc2 = new XmlDocument();
            xmlDoc2.LoadXml(xmlText);
            var patchOpSwapBiomeWorker = new PatchOperationReplace
            {
                xpath = "Defs/BiomeDef[workerClass=\"BiomesKit.UniversalBiomeWorker\"]/workerClass",
                value = new XmlContainer
                {
                    node = xmlDoc2
                },
                success = PatchOperation.Success.Always,
            };
            patchOpSwapBiomeWorker.Apply(xmlDoc);
        }
    }

    [HarmonyPatch(typeof(WorldLayer_Hills), "Regenerate")]
    internal static class WorldLayer
    {
        internal static void Prefix()
        {
            if (ReGrowthUtils.WorldBeautificationIsActive)
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
