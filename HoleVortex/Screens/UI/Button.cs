using ConsoleApplication2.UI.Collisions;
using ConsoleApplication2.UI.Components.Screens;
using HoleVortex.Asserts;
using SkiaSharp;

namespace HoleVortex.Screens.UI
{
    public abstract class Button : CircleCollision
    {
        public int TextureId { get; set; }
        protected ConsoleApplication2.HipsterEngine _engine;
        private SKRect _rect;

        public Button(ConsoleApplication2.HipsterEngine engine, float x, float y, float radius)
        {
            _engine = engine;

            X = x;
            Y = y;
            Radius = radius;
            
            _rect = SKRect.Create(-Radius, -Radius, Radius*2, Radius*2);
            
            OnLoad();
        }
        
        public abstract void OnLoad();
        
        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);

            canvas.Save();
            canvas.Translate(X, Y);
            canvas.DrawBitmap(Assert.Bitmaps[TextureId], _rect);
            canvas.Restore();
        }
    }
}