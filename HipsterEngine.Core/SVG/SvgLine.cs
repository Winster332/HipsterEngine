namespace HipsterEngine.Core.SVG
{
    public class SvgLine : SvgElement
    {
        public SvgStyle Style { get; set; }
        public float X1 { get; set; }
        public float Y1 { get; set; }
        public float X2 { get; set; }
        public float Y2 { get; set; }

        public SvgLine()
        {
            Style = null;
            X1 = 0;
            Y1 = 0;
            X2 = 0;
            Y2 = 0;
        }
    }
}