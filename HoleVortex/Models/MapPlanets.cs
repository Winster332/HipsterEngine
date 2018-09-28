using System;
using System.Collections.Generic;
using System.Linq;
using HoleVortex.Asserts;
using SkiaSharp;

namespace HoleVortex.Models
{
    public class MapPlanets
    {
        public Triangle Triangle { get; set; }
        public List<Planet> Planets { get; set; }
        private ConsoleApplication2.HipsterEngine _engine;
        private Random _random;
        
        public MapPlanets(ConsoleApplication2.HipsterEngine engine)
        {
            _engine = engine;
            _random = new Random();
            Planets = new List<Planet>();
        }

        public void Generate()
        {
            var y = _engine.Surface.Height / 2 - (_engine.Surface.Height / 3);
            var angularVelacity = 0.04f;
            
            for (var i = 0; i < 20; i++)
            {
                var radius = _random.Next(50, 100);
                var x = _random.Next((int)(radius), (int) (_engine.Surface.Width - radius));
                y -= radius*3;
                angularVelacity += 0.001f;
                var vel = _random.Next(-1, 1);
                if (vel == 0)
                    vel = 1;
                else vel = -1;
                angularVelacity *= vel;
                var textureId = _random.Next(2, Assert.Bitmaps.Count);
                AddPlanet(x, y, radius, textureId, angularVelacity);
            }
        }

        public void AddPlanet(float x, float y, float radius, int textureId, float angularVelacity)
        {
            var planet = new Planet(_engine, x, y, radius, textureId);
            planet.AngularVelocity = angularVelacity;
            planet.GenerateView();
            Planets.Add(planet);
        }

        public void Update()
        {
            Triangle.Update();
            
            Planets.ForEach(planet =>
            {
                Triangle.Intersect(planet);
                planet.Update();
            });
        }

        public void Draw()
        {
            Triangle.Draw();
            Planets.ForEach(p => p.Draw());
            
            _engine.Surface.Canvas.DrawLine(0, Planets.Last().Y - 150,  _engine.Surface.Width, Planets.Last().Y - 150, new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                Color = new SKColor(150, 150, 150)
            });
        }
    }
}