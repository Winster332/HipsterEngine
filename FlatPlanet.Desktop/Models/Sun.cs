using HipsterEngine.Core.Graphics;
using SkiaSharp;

namespace FlatPlant.Models
{
    public class Sun
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Force { get; set; }
        public SKPaint Paint { get; set; }
        public float Radius { get; set; }

        public Sun(float x, float y, float radius)
        {
            X = x;
            Y = y;
            Radius = radius;
            
            Paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = new SKColor(255, 180, 0)
            };
        }

        public void Draw(Canvas canvas)
        {
            canvas.DrawCircle(X, Y, Radius, Paint);
        }
    }
}