using SkiaSharp;

namespace HipsterEngine.Core.UI.Components.Texts
{
    public class TextBox : UIElement
    {
        public float Angle { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public string Text { get; set; }

        public TextBox()
        {
            X = 0;
            Y = 0;
            Angle = 0;
            Text = "";
            Paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                TextAlign = SKTextAlign.Center,
                IsAntialias = true,
                Color = new SKColor(255, 255, 255),
                Typeface = SKTypeface.FromFamilyName(
                    "Arial", 
                    SKFontStyleWeight.Normal, 
                    SKFontStyleWidth.Normal, 
                    SKFontStyleSlant.Upright)
            };
        }
        
        public override bool IsIntersection(float x, float y)
        {
            return false;
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);
            
            canvas.DrawText(Text, X, Y, Paint);
        }

        public override void Dispose()
        {
            base.Dispose();

            Text = null;
        }
    }
}