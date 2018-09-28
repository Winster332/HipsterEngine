using System.Security.Cryptography.X509Certificates;
using ConsoleApplication2.UI.Collisions;
using SkiaSharp;

namespace ConsoleApplication2.UI.Components.Layout
{
    public class AbsoluteLayout : RectCollision
    {
        public AbsoluteLayout()
        {
            Paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.DarkCyan
            };
        }
        
        public override void Draw(SKCanvas canvas)
        {
            canvas.DrawRect(X, Y, Width, Height, Paint);
            
            base.Draw(canvas);
        }
    }
}