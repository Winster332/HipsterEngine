using ConsoleApplication2.UI.Collisions;
using ConsoleApplication2.UI.Components.Texts;
using SkiaSharp;

namespace ConsoleApplication2.UI.Components.Buttons
{
    public class RectButton : RectCollision
    {
        public TextBox Caption { get; set; }

        public RectButton(float x, float y, float width, float height, string text)
        {
            Caption = new TextBox();
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Caption.Text = text;
            Paint = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                StrokeWidth = 0,
                Color = new SKColor(0, 140, 220)
            };

            var color = Paint.Color;
            var colorText = Caption.Paint.Color;
            MouseDown += (o, e) =>
            {
                Paint.StrokeWidth = 2;
                Paint.Color = new SKColor(color.Red, color.Green, color.Blue, 150);
                Caption.Paint.Color = new SKColor(colorText.Red, colorText.Green, colorText.Blue, 150);
            };
            MouseUp += (o, e) =>
            {
                Paint.StrokeWidth = 0;
                Paint.Color = color;
                Caption.Paint.Color = colorText;
            };
        }
        
        public override void Draw(SKCanvas canvas)
        {
            canvas.Save();
            canvas.Translate(X, Y);
            base.Draw(canvas);

            Caption.X = Width / 2;
           Caption.Y = Height / 2 + 5;
            
            canvas.DrawRect(0, 0, Width, Height, Paint);
            Caption.Draw(canvas);
            canvas.Restore();
        }
    }
}