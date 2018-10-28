using HipsterEngine.Core.Graphics;
using SkiaSharp;

namespace HipsterEngine.UI.Components
{
    public class Label : RectangeElement
    {
        public string Text { get; set; }

        public Label(Core.HipsterEngine engine) : base(engine)
        {
            Paint += OnPaint;
            X = 0;
            Y = 0;
            Width = 200;
            Height = 100;
            Text = "Text";
            Style.TextAlign = SKTextAlign.Center;
            Style.TextSize = 20;
        }

        private void OnPaint(Element element, Canvas canvas)
        {
            canvas.Save();
            
            Parent.UseClip(canvas);
            canvas.Translate(X, Y);
            canvas.DrawText(Text, 0, 0, Style);
            
            canvas.Restore();
        }
    }
}