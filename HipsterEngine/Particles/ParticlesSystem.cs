using System;
using System.Collections.Generic;
using SkiaSharp;

namespace HipsterEngine.Particles
{
    public class ParticlesSystem : IDisposable
    {
        public List<Particle> Particles { get; set; }
        private ConsoleApplication2.HipsterEngine _engine;

        public ParticlesSystem(ConsoleApplication2.HipsterEngine engine)
        {
            _engine = engine;
            Particles = new List<Particle>();
        }

        public void Step(float dt)
        {
            for (var i = 0; i < Particles.Count; i++)
            {
                var particle = Particles[i];
                
                if (!particle.IsDead)
                    particle.Step(dt);
                else Particles.Remove(particle);
            }
        }
        
        public void Draw(SKCanvas canvas)
        {
            for (var i = 0; i < Particles.Count; i++)
            {
                var particle = Particles[i];
                
                if (!particle.IsDead)
                    DrawParticle(particle, canvas);
                else Particles.Remove(particle);
            }
        }

        private void DrawParticle(Particle particle, SKCanvas canvas)
        {
            if (particle.GetType() == typeof(CircleParticle))
            {
                var p = (CircleParticle) particle;

                using (var paint = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.Coral,
                    StrokeWidth = 3
                })
                {
                    _engine.Surface.Canvas.Save();
                    _engine.Surface.Canvas.Translate(p.X + _engine.Surface.Canvas.Camera.X, p.Y + _engine.Surface.Canvas.Camera.Y);
                    _engine.Surface.Canvas.RotateRadians(p.Angle);
                    _engine.Surface.Canvas.DrawCircle(0, 0, p.Radius, paint);
                    _engine.Surface.Canvas.Restore();
                }
            }
        }

        public void Dispose()
        {
            Particles.Clear();
        }
        
    }
}