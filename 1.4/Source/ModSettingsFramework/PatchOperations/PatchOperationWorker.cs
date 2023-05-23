using System.Xml;
using Verse;

namespace ModSettingsFramework
{
    public abstract class PatchOperationWorker : PatchOperationModSettings, IExposable
    {
        public abstract void ApplySettings();
        public abstract void ExposeData();
        public abstract void CopyFrom(PatchOperationWorker savedWorker);

        public abstract void Reset();
        public override bool ApplyWorker(XmlDocument xml)
        {
            return true;
        }
    }
}
