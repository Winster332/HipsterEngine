using HipsterEngine.Core.Graphics;
using SkiaSharp;

namespace HipsterEngine.UI.Components.Layouts
{
    public class Layout : RectangeElement
    {
        protected Layout(Core.HipsterEngine engine) : base(engine)
        {
            Paint += OnPaint;
            
            Style.Color = new SKColor(100, 100, 100);
        }

        private void OnPaint(Element element, Canvas canvas)
        {
            canvas.Save();
            
            canvas.ClipRect(new SKRect(X, Y, X + Width, Y + Height));
            canvas.Translate(X, Y);
            canvas.DrawRect(0, 0, Width, Height, Style);
            
            canvas.Restore();
        }
    }
}