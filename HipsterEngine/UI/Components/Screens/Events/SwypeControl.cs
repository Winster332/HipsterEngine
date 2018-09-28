using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms.VisualStyles;
using Box2DX.Common;
using Math = System.Math;

namespace ConsoleApplication2.UI.Components.Screens
{
    public class SwypeControl
    {
        public float ValueX { get; set; }
        public float ValueY { get; set; }
        private Screen _screen { get; set; }
        private Vec2 MousePosDown { get; set; }
        private Vec2 MousePosMove { get; set; }
        private Vec2 MousePosUp { get; set; }
        private bool IsMouseDown { get; set; }
        private float MinHeight { get; set; }
        private float MaxHeight { get; set; }
        private float MaxWidth { get; set; }
        private float MinWidth { get; set; }
        public event EventHandler Scroll;

        public int ActiveScreen { get; set; }
        private float CurrentScrollWidth { get; set; }
        private float CurrentScrollHeight { get; set; }
        private HipsterEngine _engine;
        private List<SwypeScreen> listScreens { get; set; }

        public SwypeControl(HipsterEngine engine)
        {
            _engine = engine;
            _screen = _engine.Screens.CurrentScreen;
            listScreens = new List<SwypeScreen>();
            ValueX = 0;
            ValueY = 0;
            MaxHeight = 0;
            MinHeight = 0;
            MaxWidth = 0;
            MinHeight = 0;
            MousePosDown = new Vec2();
            MousePosMove = new Vec2();
            MousePosUp = new Vec2();
            IsMouseDown = false;
            CurrentScrollWidth = 0;
            CurrentScrollHeight = 0;
            ActiveScreen = 0;

            InitMouse();

            _screen.MouseDown += (element, state) => listScreens.ForEach(s =>
            {
                s.OnMouseAction(state);
            });
            _screen.MouseMove += (element, state) => listScreens.ForEach(s => s.OnMouseAction(state));
            _screen.MouseUp += (element, state) => listScreens.ForEach(s => s.OnMouseAction(state));
        }

        public SwypeControl SetBoundVertical(float minHeight, float maxHeight)
        {
            MinHeight = minHeight;
            MaxHeight = maxHeight;

            return this;
        }
        
        public SwypeControl SetBoundHorizontal(float minWidth, float maxWidth)
        {
            MinWidth = minWidth;
            MaxWidth = maxWidth;

            return this;
        }

        public void AddScreen(float x, float y, SwypeScreen screen)
        {
            screen.X = x;
            screen.Y = y;
            screen.Width = _screen.Width;
            screen.Height = _screen.Height;
            screen.HipsterEngine = _engine;
            screen.OnLoad();
            
            listScreens.Add(screen);
        }
        
        private void InitMouse()
        {
            _screen.MouseDown += (element, state) =>
            {
                IsMouseDown = true;
                MousePosDown = new Vec2(state.X, state.Y);
            };
            _screen.MouseUp += (element, state) =>
            {
                IsMouseDown = false;
                MousePosUp = new Vec2(state.X + CurrentScrollWidth, state.Y);
                
                var value = new Vec2(ValueX, ValueY);
                var center = new Vec2(CurrentScrollWidth, CurrentScrollHeight /2);
                var distance = value.X - center.X;


                if (distance == -_screen.Width || distance == _screen.Width)
                {
                }
                else if (distance >= 80)
                {
                    ValueX = 0;
                    ValueY = 0;

                    if (ActiveScreen < 1)
                    {
                        ActiveScreen++;
                        CurrentScrollWidth += _screen.Width;
                    }
                }
                else if (distance <= -80)
                {
                    ValueX = 0;
                    ValueY = 0;

                    if (ActiveScreen > -1)
                    {
                        CurrentScrollWidth -= _screen.Width;
                        ActiveScreen--;
                    }
                }
                else
                {
                }
                
                _engine.Surface.Canvas.Camera.SetTarget(CurrentScrollWidth + _screen.Width / 2, _screen.Height / 2);

                Debug.WriteLine(distance);
            };
            _screen.MouseMove += (element, state) =>
            {
                if (IsMouseDown)
                {
                    var x = state.X - MousePosDown.X;
                    var y = state.Y - MousePosDown.Y;
                    MousePosMove = new Vec2(x, y);

                    if (y < -MinHeight && y > -MaxHeight)
                    {
                        ValueY = state.Y - MousePosDown.Y;

                        Scroll?.Invoke(this, null);
                    }
                    else
                    {
                        MousePosDown = new Vec2(MousePosDown.X, state.Y - ValueY);
                    }

                    if (x < -MinWidth && x > -MaxWidth)
                    {
                        ValueX = state.X - MousePosDown.X + CurrentScrollWidth;
                        
                        Scroll?.Invoke(this, null);
                    }
                    else
                    {
                        MousePosDown = new Vec2(state.X - ValueX, MousePosDown.Y);
                    }
                }
            };
        }

        public void Update(double time, float dt)
        {
            listScreens.ForEach(s => s.OnUpdate(time, dt));
        }

        public void Draw()
        {
            listScreens.ForEach(s =>
            {
                _engine.Surface.Canvas.Save();
                _engine.Surface.Canvas.Translate(-_screen.Width / 2 + s.X, -_screen.Height / 2 + s.Y);
                s.OnDraw(_engine.Surface.Canvas);
                _engine.Surface.Canvas.Restore();
            });
        }
    }
}