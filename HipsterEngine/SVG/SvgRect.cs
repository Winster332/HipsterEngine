namespace TestOpenTK
{
    public class SvgRect : SvgElement
    {
        public SvgStyle Style { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public SvgTransform Transform { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        
        public SvgRect()
        {
            X = 0;
            Y = 0;
            Style = null;
            Transform = null;
        }
    }
}