using System.Collections.Generic;
using Verse;

namespace ReGrowthCore
{
    public abstract class PatchOperationModSettings : PatchOperation
    {
        public List<string> mods;
        public string id;
        public string label;
        public string tooltip;
        public bool CanRun()
        {
            if (mods != null)
            {
                for (int i = 0; i < mods.Count; i++)
                {
                    if (ModLister.HasActiveModWithName(mods[i]))
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
    }
}
