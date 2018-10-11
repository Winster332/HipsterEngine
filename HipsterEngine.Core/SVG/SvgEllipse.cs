namespace HipsterEngine.Core.SVG
{
    public class SvgEllipse : SvgElement
    {
        public SvgStyle Style { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float RadiusX { get; set; }
        public float RadiusY { get; set; }
        public SvgTransform Transform { get; set; }
        
        public SvgEllipse(float x, float y, float radiusX, float radiusY, SvgStyle style)
        {
            X = x;
            Y = y;
            RadiusX = radiusX;
            RadiusY = radiusY;
            Style = style;
            Transform = null;
        }

        public SvgEllipse()
        {
            X = 0;
            Y = 0;
            RadiusX = 0;
            RadiusY = 0;
            Style = null;
            Transform = null;
        }
    }
}