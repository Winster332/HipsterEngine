using System;
using SkiaSharp;

namespace HoleVortex.Android.Screens.UI
{
    public class Label : IDisposable 
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Scale { get; set; }
        public string Text { get; set; }
        public SKPaint Style { get; set; }
        private HipsterEngine.Core.HipsterEngine _engine;

        public Label(HipsterEngine.Core.HipsterEngine engine, float x, float y, string text, SKPaint style, float scale = 1)
        {
            X = x;
            Y = y;
            Scale = scale;
            Text = text;
            Style = style;
            _engine = engine;
        }

        public void Draw()
        {
            _engine.Surface.Canvas.Save();
            _engine.Surface.Canvas.Translate(X, Y);
            _engine.Surface.Canvas.Scale(Scale, Scale);
            _engine.Surface.Canvas.DrawText(Text, 0, 0, Style);
            _engine.Surface.Canvas.Restore();
        }

        public void Dispose()
        {
            Style?.Dispose();
        }
    }
}