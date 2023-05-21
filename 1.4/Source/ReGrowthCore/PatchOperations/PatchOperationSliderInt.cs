using System.Xml;
using Verse;

namespace ReGrowthCore
{
    public class PatchOperationSliderInt : PatchOperationModSettings
    {
        protected string xpath;
        public IntRange range;
        public int defaultValue;
        public override bool ApplyWorker(XmlDocument xml)
        {
            if (CanRun())
            {
                var node = xml.SelectSingleNode(xpath);
                var value = ReGrowthMod.settings.PatchOperationValue(id, defaultValue);
                node.InnerText = value.ToString();
            }
            return true;
        }
    }
}
