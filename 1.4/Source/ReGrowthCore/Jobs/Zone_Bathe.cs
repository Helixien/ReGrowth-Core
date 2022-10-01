using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ReGrowthCore
{

    public class Zone_Bathe : Zone
    {
        public override bool IsMultiselectable => true;
        public Color BaseColor = Color.blue;
        public override Color NextZoneColor => new(BaseColor.r, BaseColor.g, BaseColor.b, 0.09f);
        public Zone_Bathe()
        {
        }

        public Zone_Bathe(ZoneManager zoneManager)
            : base("RG.BatheZone".Translate(), zoneManager)
        {
        }
        public override IEnumerable<Gizmo> GetZoneAddGizmos()
        {
            yield return DesignatorUtility.FindAllowedDesignator<Designator_ZoneAdd_Bathe_Expand>();
        }
    }
}
