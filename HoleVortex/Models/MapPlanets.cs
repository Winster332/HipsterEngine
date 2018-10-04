using System;
using System.Collections.Generic;
using System.Linq;
using Box2DX.Common;
using HipsterEngine.Particles;
using HoleVortex.Asserts;
using SkiaSharp;

namespace HoleVortex.Models
{
    public class MapPlanets : IDisposable
    {
        public Triangle Triangle { get; set; }
        public List<Planet> Planets { get; set; }
        public event EndGameEventHandler EndGame;
        public int Balls { get; set; }
        public bool IsEndGame { get; set; }
        private ConsoleApplication2.HipsterEngine _engine;
        private Random _random;
        public SKPaint PaintFinishedLine { get; set; }
        
        public MapPlanets(ConsoleApplication2.HipsterEngine engine, Triangle triangle)
        {
            _engine = engine;
            _random = new Random();
            Planets = new List<Planet>();
            Triangle = triangle;
            Balls = 1;
            Triangle.IncrementBalls += (sender, args) => { Balls++; };
            IsEndGame = false;
            PaintFinishedLine = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                Color = new SKColor(150, 150, 150)
            };
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
            
            CheckEndGame();
        }
        
        private void CheckEndGame()
        {
            if (Triangle.X <= -Triangle.Radius * 2 || Triangle.X >= _engine.Surface.Width + Triangle.Radius * 2)
            {
                OnGameEnd(new GameResult
                {
                    IsWon = false,
                    Balls = Balls
                });
            }

            if (Planets.Last().Y - 150 >= Triangle.Y)
            {
                for (var i = 0; i < _engine.Surface.Width; i += 10)
                {
                    var ps = (ParticlesControllerFire) _engine.Particles.GetSystem(typeof(ParticlesControllerFire));
                    ps.AddBlood(i, Planets.Last().Y - 150, new Vec2(), 1);
                }
                
                OnGameEnd(new GameResult
                {
                    IsWon = true,
                    Balls = Balls
                });
            }

            if ((-_engine.Surface.Canvas.Camera.Y - Triangle.Y) + _engine.Surface.Height <= 0)
            {
                OnGameEnd(new GameResult
                {
                    IsWon = false,
                    Balls = Balls
                });
            }
        }

        public void OnGameEnd(GameResult result)
        {
            if (!IsEndGame)
            {
                EndGame?.Invoke(result);
                
                IsEndGame = true;
            }
        }

        public void Draw()
        {
            Triangle.Draw();
            Planets.ForEach(p => p.Draw());
            
            _engine.Surface.Canvas.DrawLine(0, Planets.Last().Y - 150,  _engine.Surface.Width, Planets.Last().Y - 150, PaintFinishedLine);
        }

        public void Dispose()
        {
            Triangle?.Dispose();
            PaintFinishedLine.Dispose();
            Planets.Clear();
        }
    }
}