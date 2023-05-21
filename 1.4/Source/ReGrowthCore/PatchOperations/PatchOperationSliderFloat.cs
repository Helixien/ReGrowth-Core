using System.Linq;
using System.Xml;
using Verse;

namespace ReGrowthCore
{
    public class PatchOperationSliderFloat : PatchOperationModSettings
    {
        protected string xpath;
        public FloatRange range;
        public float defaultValue;
        public int roundToDecimalPlaces = 2;
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
