using System;
using HipsterEngine.Core.Particles;
using HipsterEngine.Core.UI.Animations;
using HipsterEngine.Core.UI.Components;
using HipsterEngine.Core.UI.Components.Screens;
using HipsterEngine.Core.UI.Events.Mouse;
using HoleVortex.Core.Background;
using HoleVortex.Core.IO;
using HoleVortex.Core.Models;
using HoleVortex.Core.Models.Meshes;
using HoleVortex.Core.Models.Planets;
using HoleVortex.Core.Models.Planets.Meshes;
using HoleVortex.Core.Particles;
using HoleVortex.Core.Screens.UI;
using SkiaSharp;

namespace HoleVortex.Core.Screens
{
    public class GameScreen : Screen
    {
        public Triangle Triangle { get; set; }
        public LayoutTop LayoutTop { get; set; }
        public LayoutRecord LayoutRecords { get; set; }
        public PlanetStart PlanetStart { get; set; }
        public MapPlanets Map { get; set; }
        public AnimationFloat AnimationEndGame { get; set; }
        public ParallaxBackground Background { get; set; }
        
        public override void OnLoad()
        {
            Update += OnUpdate;
            Paint += OnPaint;
            MouseDown += OnMouseDown;
            Unloaded += OnUnloaded;

            LayoutRecords = new LayoutRecord(HipsterEngine);
            LayoutTop = new LayoutTop(HipsterEngine);
            PlanetStart = new PlanetStart(HipsterEngine, Width / 2, Height / 4, Height / 8);
            PlanetStart.AngularVelocity = 0.04f;
            
            HipsterEngine.Particles.AddParticleSystem(new ParticlesControllerFire(HipsterEngine));
            
            var triangleRadius = PlanetStart.Radius / 4;
            var y = PlanetStart.Transform.Y - PlanetStart.Radius - triangleRadius + 2;
            Triangle = new Triangle(HipsterEngine, Width / 2, y, triangleRadius);
            Triangle.SetPlanet(PlanetStart);
            
            HipsterEngine.Particles.AddParticleSystem(new TriangleParticlesController(HipsterEngine, Triangle));
            
            HipsterEngine.Surface.Canvas.Camera.X = -Width / 2;
            HipsterEngine.Surface.Canvas.Camera.Y = Height / 2;
            HipsterEngine.Surface.Canvas.Camera.SetTarget(PlanetStart.Transform.X - Width / 2, PlanetStart.Transform.Y + Height / 4);
            
            var profile = LoadProfile();
            
            Map = new MapPlanets(HipsterEngine, Triangle);
            Map.Generate(profile.Level);
            Map.EndGame += TriangleOnEndGame;
            
            AnimationEndGame = new AnimationFloat();
            AnimationEndGame.Timer.Complated += tick =>
            {
                HipsterEngine.Screens.SetScreen(new MenuScreen());
            };

            LayoutRecords.TextRecord = profile.Balls.ToString();
            PlanetStart.Text = profile.Level.ToString();
            
            Background = new ParallaxBackground(HipsterEngine, Triangle);
        }
        
        public Profile LoadProfile()
        {
            Profile profile = null;
            
            if (!Assets.ExistProfile())
            {
                profile = new Profile
                {
                    Level = 1,
                    Balls = 0
                };
                Assets.SaveProfile(HipsterEngine, profile);
            }
            else
            {
                profile = Assets.GetProfile(HipsterEngine);
            }

            return profile;
        }

        private void OnUnloaded(Screen screen)
        {
            Triangle.Dispose();
            LayoutRecords.Dispose();
            LayoutTop.Dispose();
            PlanetStart.Dispose();
            AnimationEndGame.Dispose();
            
            Update -= OnUpdate;
            Paint -= OnPaint;
            MouseDown -= OnMouseDown;
            Unloaded -= OnUnloaded;
        }

        private void TriangleOnEndGame(GameResult result)
        {
            var profile = Assets.GetProfile(HipsterEngine);
            profile.Balls += result.Balls;

            if (result.IsWon)
            {
                profile.Level++;
            }

            Assets.SaveProfile(HipsterEngine, profile);

            AnimationEndGame.Start(0, 255, 5);
        }

        private void OnMouseDown(UIElement element, MouseState mousestate)
        {
            if (!LayoutTop.BtnPause.IsIntersection(mousestate.X, mousestate.Y) && HipsterEngine.Screens.CurrentScreen.Enabled)
            {
                Triangle.Jump();
            }
        }

        private void OnUpdate(double time, float dt)
        {
            LayoutTop.Update();
            Map.Update();
            PlanetStart.Step();
            AnimationEndGame.Update();
            Background.Step();
        }

        private void OnPaint(UIElement element, SKCanvas canvas)
        {
            Background.Draw(HipsterEngine.Surface.Canvas);
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
            
            HipsterEngine.Particles.Draw(HipsterEngine.Surface.Canvas.GetSkiaCanvas());
            
            HipsterEngine.Surface.Canvas.DrawRect(0, 0, Width, Height, new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(0, 0, 0, Convert.ToByte(AnimationEndGame.CurrentValue))
            });
        }
    }
}