using System;
using ConsoleApplication2.UI.Components;
using ConsoleApplication2.UI.Components.Screens;
using ConsoleApplication2.UI.Events;
using HoleVortex.Asserts;
using HoleVortex.Models;
using HoleVortex.Screens.UI;
using SkiaSharp;

namespace HoleVortex.Screens
{
    public class GameScreen : Screen
    {
        public Triangle Triangle { get; set; }
        public LayoutTop LayoutTop { get; set; }
        public LayoutRecord LayoutRecords { get; set; }
        public PlanetStart PlanetStart { get; set; }
        public MapPlanets Map { get; set; }
        
        public override void OnLoad()
        {
            Update += OnUpdate;
            Paint += OnPaint;
            MouseDown += OnMouseDown;

            LayoutRecords = new LayoutRecord(HipsterEngine);
            LayoutTop = new LayoutTop(HipsterEngine);
            PlanetStart = new PlanetStart(HipsterEngine, Width / 2, Height / 4, Height / 8);
            PlanetStart.AngularVelocity = 0.04f;
            var triangleRadius = PlanetStart.Radius / 4;
            var y = PlanetStart.Y - PlanetStart.Radius - triangleRadius + 2;
            Triangle = new Triangle(HipsterEngine, Width / 2, y, triangleRadius);
            Triangle.SetPlanet(PlanetStart);
            Triangle.EndGame += TriangleOnEndGame;
            
            HipsterEngine.Surface.Canvas.Camera.X = -Width / 2;
            HipsterEngine.Surface.Canvas.Camera.Y = Height / 2;
            HipsterEngine.Surface.Canvas.Camera.SetTarget(PlanetStart.X - Width / 2, PlanetStart.Y + Height / 4);
            
            Map = new MapPlanets(HipsterEngine);
            Map.Generate();
            Map.Triangle = Triangle;
            
        }

        private void TriangleOnEndGame(GameResult result)
        {
            Console.WriteLine("123");
        }

        private void OnMouseDown(UIElement element, MouseState mousestate)
        {
            Triangle.Jump();
        }

        private void OnUpdate(double time, float dt)
        {
            LayoutTop.Update();
            Map.Update();
            PlanetStart.Update();
        }

        private void OnPaint(UIElement element, SKCanvas canvas)
        {
            LayoutTop.Draw();
            
            HipsterEngine.Surface.Canvas.Camera.Update();
            
            HipsterEngine.Surface.Canvas.Save();
            HipsterEngine.Surface.Canvas.Translate(HipsterEngine.Surface.Canvas.Camera.X,
                HipsterEngine.Surface.Canvas.Camera.Y);
            HipsterEngine.Surface.Canvas.RotateRadians(HipsterEngine.Surface.Canvas.Camera.Angle, 
                HipsterEngine.Surface.Canvas.Camera.CenterRotation.X, HipsterEngine.Surface.Canvas.Camera.CenterRotation.Y);
            
            LayoutRecords.Draw();
            PlanetStart.Draw();
            Map.Draw();
            
            HipsterEngine.Surface.Canvas.Restore();
        }
    }
}