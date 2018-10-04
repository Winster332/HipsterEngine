using System;
using System.Collections.Generic;
using SkiaSharp;

namespace HipsterEngine.Particles
{
    public class ParticlesController : IDisposable
    {
        private Dictionary<Type, ParticlesSystem> _particlesSystems;
        
        public ParticlesController()
        {
            _particlesSystems = new Dictionary<Type, ParticlesSystem>();
        }

        public void AddParticleSystem<T>(T particlesSystem) where T : ParticlesSystem
        {
            if (!_particlesSystems.ContainsKey(particlesSystem.GetType()))
            {
                _particlesSystems.Add(particlesSystem.GetType(), particlesSystem);
            }
        }
        
        public ParticlesSystem GetSystem(Type type)
        {
            return _particlesSystems[type];
        }

        public void Draw(SKCanvas canvas)
        {
            foreach (var keyValuePair in _particlesSystems)
            {
                var particles = keyValuePair.Value;
                
                particles.Step(1);
                particles.Draw(canvas);
            }
        }
        
        public void Dispose()
        {
            _particlesSystems.Clear();
        }
    }
}