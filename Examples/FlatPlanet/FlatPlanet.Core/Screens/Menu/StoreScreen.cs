using System.Collections.Generic;
using FlatPlant.Models.Planets;
using FlatPlant.Models.Robots;
using FlatPlant.Models.Robots.Arms;
using FlatPlant.Models.Robots.Bodies;
using FlatPlant.Models.Robots.Transmissions;
using FlatPlant.Screens.UI;
using HipsterEngine.Core.UI.Components;
using HipsterEngine.Core.UI.Components.Screens;
using SkiaSharp;

namespace FlatPlant.Screens
{
    public class StoreScreen : SwypeScreen
    {
        private List<HorizontalListBox> _listBoxs { get; set; }
        public LevelPlatform Earth { get; set; }
        public List<IRobot> Robots { get; set; }
        
        public override void OnLoad()
        {
            Paint += OnPaint;
            
            _listBoxs = new List<HorizontalListBox>();
            HipsterEngine.Physics.Initialize(-1000, -1000, 1000, 1000, 0, 0f, true);
            
            var h = new HorizontalListBox(HipsterEngine, Width / 2, Height - 100, Width - 100, 100, 8);
            _listBoxs.Add(h);
            
            HipsterEngine.Physics.Initialize(-1000, -1000, 1000, 1000, 0, 0f, true);
            
            var ph = Height / 2;
            var py = Height / 2 + ph / 2 - 20;
            Earth = new LevelPlatform(HipsterEngine, Width / 2, py, Width / 2, 40);
            
            Robots = new List<IRobot>();
            var robot = new Robot(HipsterEngine, Earth);
            robot.Initialize(new TwoWheels(), new Box2(), new Gun1())
                .Build(Width / 2, 120, 90, 20);
            
            Robots.Add(robot);
            
            float x = HipsterEngine.Surface.Width / 2;
            float y = HipsterEngine.Surface.Height / 2;

        }
        
        private void OnPaint(UIElement element, SKCanvas canvas)
        {
            var p = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateDash(new[] { 10.0f, 10.0f }, 10),
                Color = new SKColor(210, 170, 0, 100)
            };
            var pp = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = new SKColor(50, 50, 50)
            };

            float borderSize = 40;
            
            canvas.DrawRect(borderSize, borderSize, Width - borderSize * 2, Height - borderSize * 2, p);

            var w = Width / 2;
            var h = Height / 2;
            var x = Width / 2 - w / 2;
            var y = Height / 2 - h / 2;
            
            
            canvas.DrawCircle(Width - 120, Y + 40, 36, p);
            canvas.DrawCircle(Width - 120, Y + 40, 36, pp);

            canvas.DrawText($"120", Width - 121, Y + 48,
                new SKPaint
                {
                    TextSize = 25,
                    IsAntialias = true,
                    TextAlign = SKTextAlign.Center,
                    Color = new SKColor(100, 100, 100)
                });
            
            
            canvas.DrawRect(x, y, w, h, new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = new SKColor(40, 40, 40)
            });
            canvas.DrawRect(x, y, w, h, p);
            canvas.DrawLine(x + w / 2, borderSize, x + w / 2, y, p);
            canvas.DrawLine(x + w / 2, Height - borderSize, x + w / 2, Height - y, p);
            
            Earth.Step();
            Earth.Draw();
            
            HipsterEngine.Physics.Step(1, 20);
            HipsterEngine.Particles.Draw(HipsterEngine.Surface.Canvas.GetSkiaCanvas());
            
            _listBoxs.ForEach(l =>
            {
                l.Update();
                l.Draw();
            });
            Robots.ForEach(r =>
            {
                r.Update(1, 1);
                r.Draw(HipsterEngine.Surface.Canvas);
            });
        }
    }
}