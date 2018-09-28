using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace FlatPlant.Screens.UI
{
    public class HorizontalListBox
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        private ConsoleApplication2.HipsterEngine _engine;
        private List<LayoutHorizontal> layouts { get; set; }
        
        public HorizontalListBox(ConsoleApplication2.HipsterEngine engine, float x, float y, float width, float height, int countMax)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            
            layouts = new List<LayoutHorizontal>();
            
            _engine = engine;
            _engine.Surface.Canvas.Camera.SetTarget(_engine.Surface.Width + _engine.Surface.Width / 2, _engine.Surface.Height / 2);
            
            layouts.Add(new LayoutHorizontal(_engine, 0, 2));
        }

        public void Update()
        {
        }

        public void Draw()
        {
            layouts.ForEach(l => l.Draw());
        }

        public class LayoutHorizontal
        {
            public float X { get; set; }
            public float Y { get; set; }
            public ButtonScroll ButtonRight { get; set; }
            public ButtonScroll ButtonLeft { get; set; }
            private ConsoleApplication2.HipsterEngine _engine;

            public LayoutHorizontal(ConsoleApplication2.HipsterEngine engine, float x, int section)
            {
                _engine = engine;

                X = x;

                var radius = 50.0f;
                var bX = 150;
                var y = _engine.Surface.Height / section;
                ButtonRight = new ButtonScroll(_engine, _engine.Surface.Width - bX + X, y, radius);
                ButtonLeft = new ButtonScroll(_engine, bX + X, y, radius, 180);
            }

            public void Draw()
            {
                ButtonRight.Draw();
                ButtonLeft.Draw();
            }
        }

        public class ButtonScroll
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Radius { get; set; }
            public float Angle { get; set; }
            private ConsoleApplication2.HipsterEngine _engine;
            
            public ButtonScroll(ConsoleApplication2.HipsterEngine engine, float x, float y, float radius, float angle = 0)
            {
                _engine = engine;
                X = x;
                Y = y;
                Radius = radius;
                Angle = angle;
            }
            public void Update()
            {
            }

            public void Draw()
            {
                _engine.Surface.Canvas.Save();
                _engine.Surface.Canvas.Translate(X, Y);
                _engine.Surface.Canvas.RotateDegrees(Angle);
                
                _engine.Surface.Canvas.DrawCircle(0, 0, Radius, new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    IsAntialias = true,
                    StrokeWidth = 2,
                    Color = new SKColor(100, 100, 100)
                });

                var path = new SKPath();
                path.MoveTo(-(Radius / 4), -Radius + (Radius / 3));
                path.LineTo(Radius - (Radius / 2), 0);
                path.LineTo(-(Radius / 4), Radius - (Radius / 3));

                _engine.Surface.Canvas.DrawPath(path, new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    IsAntialias = true,
                    StrokeWidth = 2,
                    Color = new SKColor(50, 150, 200)
                });
                _engine.Surface.Canvas.Restore();
            }
        }
    }
}