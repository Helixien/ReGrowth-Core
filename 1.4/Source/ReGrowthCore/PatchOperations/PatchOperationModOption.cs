using System.Collections.Generic;
using System.Xml;
using Verse;

namespace ReGrowthCore
{
    public class PatchOperationModOption : PatchOperationModSettings
    {
        public bool defaultValue;
        private List<PatchOperation> operations;
        private PatchOperation lastFailedOperation;
        public override bool ApplyWorker(XmlDocument xml)
        {
            if (CanRun())
            {
                if (ReGrowthMod.settings.PatchOperationEnabled(id, defaultValue))
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
