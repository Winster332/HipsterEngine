namespace TestOpenTK
{
    public class SvgCircle : SvgElement
    {
        public SvgStyle Style { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Radius { get; set; }
        
        public SvgCircle(float x, float y, float radius, SvgStyle style)
        {
            X = x;
            Y = y;
            Radius = radius;
            Style = style;
        }

        public SvgCircle()
        {
            X = 0;
            Y = 0;
            Radius = 0;
            Style = null;
        }
    }
}