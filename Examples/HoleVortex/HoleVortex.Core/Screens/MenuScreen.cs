using System;
using HipsterEngine.Core.UI.Animations;
using HipsterEngine.Core.UI.Components;
using HipsterEngine.Core.UI.Components.Screens;
using HipsterEngine.Core.UI.Events.Mouse;
using HoleVortex.Core.IO;
using HoleVortex.Core.Models;
using HoleVortex.Core.Models.Planets;
using HoleVortex.Core.Screens.UI;
using SkiaSharp;

namespace HoleVortex.Core.Screens
{
    public class MenuScreen : Screen
    {
        public LayoutRecord LayoutRecords { get; set; }
        public Label LabelTouchMe { get; set; }
        public PlanetStart Planet { get; set; }
        public Triangle Triangle { get; set; }
        public SKColor BackgroundColor { get; set; }
        public AnimationFloat AnimationEndGame { get; set; }
        public SKPaint PaintAnimation { get; set; }
        private float _time;
        
        public override void OnLoad()
        {
            Update += OnUpdate;
            Paint += OnPaint;
            MouseDown += OnMouseDown;
            Unloaded += OnUnloaded;
            
            LayoutRecords = new LayoutRecord(HipsterEngine);
            Planet = new PlanetStart(HipsterEngine, Width / 2, Height / 4, Height / 8);
            var triangleRadius = Planet.Radius / 4;
            var y = Planet.Y - Planet.Radius - triangleRadius + 2;
            Triangle = new Triangle(HipsterEngine, Width / 2, y, triangleRadius);
            LabelTouchMe = new Label(HipsterEngine, Width / 2, Height / 2+10, "TOUCH SCREEN", new SKPaint
            {
                Color = new SKColor(150, 150, 150, 100),
                IsAntialias = true,
                TextAlign = SKTextAlign.Center,
                TextSize = 20
            }, 1);
            BackgroundColor = new SKColor(50, 50, 50);
            _time = 0;

            paint = new SKPaint
            {
                IsAntialias = true,
                Color = new SKColor(0, 0, 0)
            };
            
            AnimationEndGame = new AnimationFloat();
            AnimationEndGame.Start(255, 0, 5);

            LoadProfile();
        }

        public void LoadProfile()
        {
            Profile profile = null;
            
            if (!Assets.ExistProfile())
            {
                profile = new Profile
                {
                    Level = 1,
                    Balls = 0
                };
            //    HipsterEngine.Files.Serialize(profile, Assert.PathToProfile);
                Assets.SaveProfile(HipsterEngine, profile);
            }
            else
            {
            //    profile = HipsterEngine.Files.Deserialize<Profile>(Assert.PathToProfile);
                profile = Assets.GetProfile(HipsterEngine);
            }

            LayoutRecords.TextRecord = profile.Balls.ToString();
            Planet.Text = profile.Level.ToString();
        }

        private void OnUnloaded(Screen screen)
        {
            LayoutRecords.Dispose();
            Planet.Dispose();
            Triangle.Dispose();
            LabelTouchMe.Dispose();
            paint.Dispose();
            AnimationEndGame.Dispose();
            PaintAnimation.Dispose();

            Update -= OnUpdate;
            Paint -= OnPaint;
            MouseDown -= OnMouseDown;
            Unloaded -= OnUnloaded;
        }
        public SKPaint paint;

        private void OnMouseDown(UIElement element, MouseState mousestate)
        {
            HipsterEngine.Screens.SetScreen(new GameScreen());
        }

        private void OnUpdate(double time, float dt)
        {
            _time += 0.03f;
            LabelTouchMe.Scale = (float) Math.Cos(Math.Sin(_time) / 2);
            AnimationEndGame.Update();
        }

        private void OnPaint(UIElement element, SKCanvas canvas)
        {
            canvas.Clear(BackgroundColor);
            
            LayoutRecords.Draw();
            Planet.Draw();
            Triangle.Draw();
            
            HipsterEngine.Surface.Canvas.Save();
            HipsterEngine.Surface.Canvas.Translate(Width / 2, Height / 2);
          //  HipsterEngine.Surface.Canvas.RotateRadians(_time);
        //    HipsterEngine.Surface.Canvas.Scale(0.5f, 0.5f);
          //  if (bitmap != null)
         //       HipsterEngine.Surface.Canvas.DrawBitmap(bitmap, -bitmap.Width / 2,-bitmap.Height / 2, paint);
                //HipsterEngine.Surface.Canvas.DrawBitmap(bitmap, SKRect.Create(0, 0, 100, 100), paint);
            HipsterEngine.Surface.Canvas.Restore();
            
            LabelTouchMe.Draw();

            PaintAnimation = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(0, 0, 0, Convert.ToByte(AnimationEndGame.CurrentValue))
            };
            HipsterEngine.Surface.Canvas.DrawRect(0, 0, Width, Height, PaintAnimation);
        }
    }
}