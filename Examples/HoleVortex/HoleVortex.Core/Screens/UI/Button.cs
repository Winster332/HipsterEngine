using HipsterEngine.Core.UI.Collisions;
using HoleVortex.Core.IO;
using SkiaSharp;

namespace HoleVortex.Core.Screens.UI
{
    public abstract class Button : CircleCollision
    {
        public int TextureId { get; set; }
        protected HipsterEngine.Core.HipsterEngine _engine;
        private SKRect _rect;

        public Button(HipsterEngine.Core.HipsterEngine engine, float x, float y, float radius)
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
            canvas.DrawBitmap(Assets.Bitmaps[TextureId], _rect);
            canvas.Restore();
        }
    }
}