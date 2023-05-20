using System.Collections.Generic;
using System.Xml;
using Verse;

namespace ReGrowthCore
{
    public class PatchOperationModOption : PatchOperation
    {
        public string id;
        public string modOptionLabel;
        public bool defaultValue;
        private List<PatchOperation> operations;
        public List<string> mods;
        private PatchOperation lastFailedOperation;
        public override bool ApplyWorker(XmlDocument xml)
        {
            bool flag = mods is null;
            if (!flag)
            {
                for (int i = 0; i < mods.Count; i++)
                {
                    if (ModLister.HasActiveModWithName(mods[i]))
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (flag)
            {
                if (!ReGrowthMod.settings.patchOperationStates.TryGetValue(id, out var enabled))
                {
                    ReGrowthMod.settings.patchOperationStates[id] = enabled = defaultValue;
                }
                if (enabled)
                {
                    foreach (PatchOperation operation in operations)
                    {
                        if (!operation.Apply(xml))
                        {
                            lastFailedOperation = operation;
                            return false;
                        }
                    }
                    return true;
                }
            }
            return true;
        }

        public override void Complete(string modIdentifier)
        {
            base.Complete(modIdentifier);
            lastFailedOperation = null;
        }

        public override string ToString()
        {
            int num = ((operations != null) ? operations.Count : 0);
            string text = $"{base.ToString()}(count={num}";
            if (lastFailedOperation != null)
            {
                text = text + ", lastFailedOperation=" + lastFailedOperation;
            }
            return text + ")";
        }
    }
}
