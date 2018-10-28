using SkiaSharp;

namespace HipsterEngine.UI.Components
{
    public class Style : SKPaint
    {
        public Border Border { get; set; }
        public Element Element { get; set; }
        
        public Style(Element parent)
        {
            Element = parent;
            Border = null;
            IsAntialias = true;
            Color = new SKColor(255, 255, 255);
        }
    }
}