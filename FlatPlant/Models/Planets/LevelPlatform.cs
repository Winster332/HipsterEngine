using ConsoleApplication2.Physics.Bodies;
using SkiaSharp;

namespace FlatPlant.Models.Planets
{
    public class LevelPlatform : Model, IPlanet
    {
        public float X { get; set; }
        public float Y { get; set; }
        public RigidBody RigidBody { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        
        public LevelPlatform(ConsoleApplication2.HipsterEngine engine, float x, float y, float width, float height) : base(engine)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            
            RigidBody = (RigidBodyVertex) _engine.Physics.FactoryBody
                .CreateRigidVertex()
                .CreateBox(0.3f, 0.8f, Width, Height, 0.0f)
                .CreateBodyDef(X, Y, 0, true, false)
                .Build(0);
            
            Paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateDash(new[] { 10.0f, 10.0f }, 10),
                
                Color = new SKColor(210, 80, 0, 150)
             //   Shader = SKShader.CreateRadialGradient(new SKPoint(X, Y), Radius, new SKColor[]
             //   {
             //       new SKColor(50, 50, 50), 
             //       new SKColor(0, 140, 210, 100), 
             //   }, new float[] { 0.5f, Radius }, SKShaderTileMode.Repeat),
             //   Color = new SKColor(210, 170, 0, 100)
            };
        }

        public override void Draw()
        {
            var x = RigidBody.GetX();
            var y = RigidBody.GetY();
            
            _canvas.Save();
            _canvas.Translate(x, y);
            _canvas.RotateRadians(RigidBody.GetBody().GetAngle());
            //_canvas.DrawRect(-Width / 2, -Height / 2, Width, Height, Paint);
            _canvas.DrawLine(-Width / 2, -Height / 2, Width / 2, -Height / 2, Paint);
            _canvas.Restore();
        }
    }
}