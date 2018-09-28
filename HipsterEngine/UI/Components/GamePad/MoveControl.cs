using System;
using Box2DX.Common;
using ConsoleApplication2.Graphics;
using SkiaSharp;

namespace ConsoleApplication2.UI.Components.GamePad
{
    public class MoveControl
    {
        private HipsterEngine _engine;
        private Canvas _canvas;
        public float Radius { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public TrackerBall Tracker { get; set; }
        
        public MoveControl(HipsterEngine engine)
        {
            _engine = engine;
            _canvas = _engine.Surface.Canvas;
            Radius = 100;
            X = _engine.Surface.Width - Radius - Radius / 2;
            Y = _engine.Surface.Height - Radius - Radius / 2;
            Tracker = new TrackerBall(_engine, X, Y, Radius / 3, this);
        }
        
        public void Update()
        {
        }

        public void Draw()
        {
            _canvas.DrawCircle(X, Y, Radius, new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = new SKColor(100, 100, 100, 100)
            });
            Tracker.Draw();
        }

        public bool IsIntersect(float x, float y)
        {
            var distance = Vec2.Distance(new Vec2(X, Y), new Vec2(x, y));

            if (distance <= Radius - Tracker.Radius)
                return true;
            else return false;
        }

        public class TrackerBall
        {
            public float X { get; set; }
            public float Y { get; set; }
            private float baseX { get; set; }
            private float baseY { get; set; }
            public float Radius { get; set; }
            public SKPaint PaintFill { get; set; }
            private HipsterEngine _engine;
            private MoveControl _moveControl;
            public event EventHandler<Vec2> Move;  

            public TrackerBall(HipsterEngine engine, float x, float y, float radius, MoveControl moveControl)
            {
                _engine = engine;
                _moveControl = moveControl;

                X = x;
                Y = y;
                baseX = x;
                baseY = y;
                Radius = radius;

                PaintFill = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    Color = new SKColor(40, 40, 40)
                };
                
                InitMouse();
            }

            public void InitMouse()
            {
                var isDown = false;
                
                _engine.Screens.CurrentScreen.MouseDown += (element, state) => { isDown = true; };
                
                _engine.Screens.CurrentScreen.MouseUp += (element, state) =>
                {
                    X = baseX;
                    Y = baseY;
                    isDown = false;
                    Move?.Invoke(null, new Vec2(_moveControl.X - X, 0));
                };
                
                _engine.Screens.CurrentScreen.MouseMove += (element, state) =>
                {
                    if (_moveControl.IsIntersect(state.X, state.Y))
                    {
                        if (isDown)
                        {
                            X = state.X;
                            Y = state.Y;
                            Move?.Invoke(null, new Vec2(_moveControl.X - X, 0));
                        }
                    }
                };
            }
            
            public void Draw()
            {
                _engine.Surface.Canvas.DrawCircle(X, Y, Radius, PaintFill);
            }
        }
    }
}