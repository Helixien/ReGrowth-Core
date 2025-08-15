using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ReGrowthCore
{
    [StaticConstructorOnStartup]
    public class Command_Hide_ZoneBathing : Command_Hide
    {
        private static Texture2D cachedIcon;

        public Command_Hide_ZoneBathing(IHideable hideable)
            : base(hideable)
        {
        }

        public override IEnumerable<FloatMenuOption> GetOptions()
        {
            return GetHideOptions();
        }

        public static IEnumerable<FloatMenuOption> GetHideOptions()
        {
            yield return new FloatMenuOption("ShowAllZones".Translate(), delegate
            {
                Command_Hide.ToggleAll(hidden: false);
            });
            yield return new FloatMenuOption("HideAllZones".Translate(), delegate
            {
                Command_Hide.ToggleAll(hidden: true);
            });
            if ((object)cachedIcon == null)
            {
                cachedIcon = ContentFinder<Texture2D>.Get("UI/Zones/ZoneCreate_Bathe");
            }
            foreach (FloatMenuOption item in Command_Hide.ZoneTypeOptions<Zone_Bathe>("RG.BathingGroup".Translate(), cachedIcon))
            {
                yield return item;
            }
        }
    }
}
