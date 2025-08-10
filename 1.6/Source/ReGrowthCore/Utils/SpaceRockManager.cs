using System.Collections.Generic;
using Verse;

namespace RimWorld
{
    public static class SpaceRockManager
    {
        public const string SpaceRockTexPath = "Things/Building/Linked/RG_AsteroidWallFlecked_Atlas";
        public static Dictionary<Mineable, bool> spaceRocks = new Dictionary<Mineable, bool>();
        public static bool isSpawningMeteoriteContents = false;
        public static bool isGeneratingOresForAsteroidStep = false;
        public static void MarkAsSpaceRock(this Mineable m)
        {
            if (m == null) return;
            if (!spaceRocks.ContainsKey(m))
            {
                spaceRocks.Add(m, true);
                if (m.Spawned)
                {
                    m.UpdateGraphic();
                }
            }
            else
            {
                spaceRocks[m] = true;
            }
        }

        public static void UnmarkSpaceRock(this Mineable m)
        {
            if (m == null) return;
            spaceRocks.Remove(m);
        }

        public static bool IsSpaceRock(this Mineable m)
        {
            if (m == null) return false;
            return spaceRocks.TryGetValue(m, out var isSpaceRock) && isSpaceRock;
        }
        
        public static void UpdateGraphic(this Mineable mineable)
        {
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                Graphic graphic = mineable.Graphic;
                if (graphic.data.texPath != SpaceRockTexPath)
                {
                    var copy = new GraphicData();
                    copy.CopyFrom(graphic.data);
                    copy.texPath = SpaceRockTexPath;
                    mineable.graphicInt = copy.GraphicColoredFor(mineable);
                }
                
                var map = mineable.Map;
                mineable.DirtyMapMesh(map);
            });
        }
    }
}
