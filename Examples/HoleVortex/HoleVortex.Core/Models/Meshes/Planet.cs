using HoleVortex.Core.IO;
using HoleVortex.Core.Models.Behaviors.Common;
using SkiaSharp;

namespace HoleVortex.Core.Models.Planets
{
    public class Planet : Behavior
    {
        public float VelacityX { get; set; }
        public float VelacityY { get; set; }
        public float Radius { get; set; }
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
            Transform.X = x;
            Transform.Y = y;
            Radius = radius;
            Transform.Angle = angle;
            VelacityX = 0;
            VelacityY = 0;
            AngularVelocity = angularVelocity;
            CorrectAngle = 0;
            TextureId = textureId;
            
            _rect = SKRect.Create(-Radius, -Radius, Radius*2, Radius*2);
            
            Update += OnUpdate;
            Paint += OnPaint;
        }

        private void OnUpdate()
        {
            Transform.Angle += AngularVelocity;
            Transform.X += VelacityX;
            Transform.Y += VelacityY;
        }

        private void OnPaint()
        {
            _engine.Surface.Canvas.DrawBitmap(Assets.Bitmaps[TextureId], _rect, null);
        }

        public void GenerateView()
        {
        }
    }
}