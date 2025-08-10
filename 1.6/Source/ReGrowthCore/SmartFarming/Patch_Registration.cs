using HarmonyLib;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace ReGrowthCore
{
    //Zone spawn
    [HarmonyPatch(typeof(Zone), nameof(Zone.PostRegister))]
    static class Patch_PostRegister
    {
        static void Postfix(Zone __instance)
        {
            try
            {
                Zone_Growing growZone = __instance as Zone_Growing;
                if
                (
                    growZone != null && //Is a grow zone?
                    ReGrowthCore_SmartFarming.compCache.TryGetValue(growZone.Map?.uniqueID ?? -1, out MapComponent_SmartFarming comp) && //Can find map component?
                    !comp.growZoneRegistry.ContainsKey(__instance.ID) //Zone data not yet made?
                )
                {
                    comp.growZoneRegistry.Add(growZone.ID, new ZoneData());
                    comp.growZoneRegistry[growZone.ID].Init(comp, growZone);
                    if (Prefs.DevMode && ReGrowthCore_SmartFarming.ModSettings.logging) Log.Message("Zone ID " + growZone.ID + " registered.");
                }
            }
            catch (System.Exception ex)
            {
                Log.Error("[Smart Farming] Error registering new grow zone:\n" + ex);
            }
        }
    }

    //Zone delete
    [HarmonyPatch(typeof(Zone), nameof(Zone.Deregister))]
    static class Patch_Deregister
    {
        static void Prefix(Zone __instance)
        {
            if (__instance is Zone_Growing) ReGrowthCore_SmartFarming.compCache.TryGetValue(__instance.Map?.uniqueID ?? -1)?.growZoneRegistry?.Remove(__instance.ID);
        }
    }

    //Change plant type
    [HarmonyPatch(typeof(Zone_Growing), nameof(Zone_Growing.SetPlantDefToGrow))]
    static class Patch_SetPlantDefToGrow
    {
        static void Postfix(Zone_Growing __instance)
        {
            if (ReGrowthCore_SmartFarming.compCache.TryGetValue(__instance.zoneManager.map.uniqueID, out MapComponent_SmartFarming mapComp))
            {
                mapComp.CalculateAll(__instance);
            }
        }
    }

    //Zone expand
    [HarmonyPatch(typeof(Zone_Growing), nameof(Zone_Growing.AddCell))]
    static class Patch_AddCell
    {
        static void Postfix(Zone_Growing __instance)
        {
            if (ReGrowthCore_SmartFarming.compCache.TryGetValue(__instance.zoneManager.map.uniqueID, out MapComponent_SmartFarming mapComp))
            {
                mapComp.CalculateAll(__instance);
                if (mapComp.growZoneRegistry.TryGetValue(__instance.ID, out ZoneData zoneData)) zoneData.CalculateCornerCell(__instance);
            }
        }
    }

    //Zone shrink
    [HarmonyPatch(typeof(Zone), nameof(Zone.RemoveCell))]
    static class Patch_RemoveCell
    {
        static void Postfix(Zone __instance)
        {
            if (__instance is Zone_Growing zone && zone.cells.Count > 0 && ReGrowthCore_SmartFarming.compCache.TryGetValue(zone.zoneManager.map.uniqueID, out MapComponent_SmartFarming mapComp))
            {
                mapComp.CalculateAll(zone);
                if (mapComp.growZoneRegistry.TryGetValue(zone.ID, out ZoneData zoneData)) zoneData.CalculateCornerCell(zone);
            }
        }
    }

    //Flush the cache on reload
    [HarmonyPatch(typeof(World), nameof(World.FinalizeInit))]
    public class Patch_LoadGame
    {
        static void Prefix()
        {
            ReGrowthCore_SmartFarming.compCache.Clear();
        }
    }
}