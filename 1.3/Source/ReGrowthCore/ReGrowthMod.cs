using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ReGrowthCore
{
    class ReGrowthMod : Mod
    {
        public ReGrowthMod(ModContentPack pack) : base(pack)
        {
            new Harmony("Helixien.ReGrowthCore").PatchAll();
        }
    }
}
