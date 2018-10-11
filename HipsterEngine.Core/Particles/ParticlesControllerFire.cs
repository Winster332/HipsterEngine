using System;
using Box2DX.Common;
using Math = System.Math;

namespace HipsterEngine.Core.Particles
{
    public class ParticlesControllerFire : ParticlesSystem
    {
        private Random Random { get; set; }

        public ParticlesControllerFire(HipsterEngine engine) : base(engine)
        {
            Random = new Random();
        }
        public void AddBlood(float x, float y, Vec2 vector, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var velocity = GetV2((float)(Math.PI * 5d * Random.NextDouble()), (float) Random.NextDouble());
                var part = new CircleParticle(15, 0.2f, x, y, velocity.X, velocity.Y, 0, 0.01f, 200);
                
                Particles.Add(part);
            }
        }

        public Vec2 GetV2(float angle, float length)
        {
            var _x = (float)Math.Cos(angle) * length;
            var _y = -(float)Math.Sin(angle) * length;
            
            return new Vec2(_x, _y);
        }
    }
}