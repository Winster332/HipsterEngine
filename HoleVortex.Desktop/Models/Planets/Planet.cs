using HoleVortex.Desktop.Asserts;
using SkiaSharp;

namespace HoleVortex.Desktop.Models.Planets
{
    public class Planet
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float VelacityX { get; set; }
        public float VelacityY { get; set; }
        public float Radius { get; set; }
        public float Angle { get; set; }
        public float CorrectAngle { get; set; }
        public float AngularVelocity { get; set; }
        public bool IsCheck = false;
        public int TextureId { get; set; }
        private SKRect _rect;
        protected HipsterEngine.Core.HipsterEngine _engine;

        public Planet(HipsterEngine.Core.HipsterEngine engine, float x, float y, float radius, int textureId, float angle = 0,
            float angularVelocity = 0)
        {
            _engine = engine;
            X = x;
            Y = y;
            Radius = radius;
            Angle = angle;
            VelacityX = 0;
            VelacityY = 0;
            AngularVelocity = angularVelocity;
            CorrectAngle = 0;
            TextureId = textureId;
            
            _rect = SKRect.Create(-Radius, -Radius, Radius*2, Radius*2);
        }

        public void GenerateView()
        {
        }

        public void Update()
        {
            Angle += AngularVelocity;
            X += VelacityX;
            Y += VelacityY;
        }

        public void Draw()
        {
            _engine.Surface.Canvas.Save();
            _engine.Surface.Canvas.Translate(X, Y);
            _engine.Surface.Canvas.RotateRadians(Angle);
            _engine.Surface.Canvas.DrawBitmap(Assert.Bitmaps[TextureId], _rect, null);
            _engine.Surface.Canvas.Restore();
        }
    }
}