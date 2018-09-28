using System.Collections.Generic;

namespace TestOpenTK
{
    public class SvgGroup : SvgElement
    {
        public List<SvgElement> Elements { get; set; }

        public SvgGroup()
        {
            Elements = new List<SvgElement>();
        }

        public void AddElement(SvgElement element)
        {
            Elements.Add(element);
        }
    }
}