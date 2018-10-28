using HipsterEngine.Core.Graphics;
using SkiaSharp;

namespace HipsterEngine.UI.Components
{
    public class RectangeElement : Element
    {
        public float Width { get; set; }
        public float Height { get; set; }
        
        public RectangeElement(Core.HipsterEngine engine) : base(engine)
        {
            Width = 0;
            Height = 0;
        }

        public override void UseClip(Canvas canvas)
        {
            if (Parent != null)
            {
                var w = X + Width;
                var h = Y + Height;

                canvas.ClipRect(new SKRect(X, Y, w, h));
            }
        }
    }
}