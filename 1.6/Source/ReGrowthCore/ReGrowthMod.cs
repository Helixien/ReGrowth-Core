using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace ReGrowthCore
{
    public class ReGrowthMod : Mod
    {
        public static ModContentPack modPack;
        public ReGrowthMod(ModContentPack pack) : base(pack)
        {
            modPack = pack;
        }
    }
    
    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
            new Harmony("Helixien.ReGrowthCore").PatchAll();
        }
    }
}
