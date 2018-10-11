using HipsterEngine.Core.UI.Collisions;
using HipsterEngine.Core.UI.Components.Texts;
using SkiaSharp;

namespace HipsterEngine.Core.UI.Components.Buttons
{
    public abstract class CircleButton : CircleCollision
    {
        public TextBox Caption { get; set; }

        public CircleButton(float x, float y, float radius, string text)
        {
            Caption = new TextBox();
            X = x;
            Y = y;
            Radius = radius;
            Caption.Text = text;
            Angle = 0;
            Paint = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                StrokeWidth = 0,
                IsAntialias = true,
                Color = new SKColor(0, 140, 220)
            };

            var color = Paint.Color;
            var colorText = Caption.Paint.Color;
            Caption.Y += 5;
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

        public abstract void Draw(SKCanvas canvas);
    }
}