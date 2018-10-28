using SkiaSharp;

namespace HipsterEngine.UI.Components
{
    public class Border
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public float BorderSize { get; set; }
        public SKColor BorderColor { get; set; }
        public Element Parent { get; set; }

        public Border(Element parent)
        {
            Parent = parent;
            Width = 0;
            Height = 0;
            BorderSize = 0;
            BorderColor = SKColors.Black;
        }

        public Border(Element parent, float borderSize, SKColor borderColor)
        {
            Parent = parent;
            BorderSize = borderSize;
            BorderColor = borderColor;
        }
    }
}