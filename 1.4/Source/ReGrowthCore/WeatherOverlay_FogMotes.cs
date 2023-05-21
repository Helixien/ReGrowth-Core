using RimWorld;
using System.Linq;
using Verse;

namespace ReGrowthCore
{
    public class WeatherOverlay_FogMotes : SkyOverlay
    {
        public override void TickOverlay(Map map)
        {
            base.TickOverlay(map);
            var extension = map.weatherManager.curWeather?.GetModExtension<WeatherExtension_FogMotes>();
            if (extension != null)
            {
                if (Rand.Chance(extension.fogSpawnRate))
                {
                    var cell = map.AllCells.Where(x => x.Roofed(map) is false).RandomElement();
                    FleckMaker.ThrowSmoke(cell.ToVector3Shifted(), map, extension.fogSize);
                }
            }
        }
    }

    public class WeatherExtension_FogMotes : DefModExtension
    {
        public float fogSize;

        public float fogSpawnRate;
    }
}
