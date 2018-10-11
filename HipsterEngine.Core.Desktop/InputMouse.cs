using System.Drawing;
using HipsterEngine.Core.Graphics;
using HipsterEngine.Core.UI.Events.Mouse;
using OpenTK.Input;
using SkiaSharp;
using MouseButton = HipsterEngine.Core.UI.Events.Mouse.MouseButton;
using MouseState = HipsterEngine.Core.UI.Events.Mouse.MouseState;

namespace HipsterEngine.Core.Desktop
{
    public delegate void MouseEventHandler(MouseState state);
    public enum MouseType { Down, Up, Move, Stay }
    
    public class InputMouse
    {
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MousePressedMove;
        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseUp;
        
        private GameWindowGPU _window;
        private SKPaint PaintStay { get; set; }
        public float Radius { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public MouseType State { get; set; }
        public OpenTK.Input.MouseButton Button { get; set; }
        private float Angle = 0;
        private float KX;
        private float KY;
        private Rectangle WindowRect { get; set; }
        private bool _mouseDown = false;
        private bool _mouseUp = true;
        private float _prevX;
        private float _prevY;
        public bool Enabled { get; set; }
        
        public InputMouse(GameWindowGPU window)
        {
            _window = window;

            PaintStay = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = new SKColor(200, 200, 200)
            };

            X = 0;
            Y = 0;
            Radius = 10;
            KX = Mouse.GetState().X;
            KY = Mouse.GetState().Y;
            WindowRect = _window.ClientRectangle;
            State = MouseType.Stay;
            Enabled = true;
        }

        public void Update()
        {
            if (Enabled)
            {
                RulesCursor();
                CheckEvents();
            }
        }

        private void RulesCursor()
        {
            X = -(KX - Mouse.GetState().X);
            Y = -(KY - Mouse.GetState().Y);
            Angle += 0.05f;
            
            if (X < 0)
            {
                X = 0;
                KX = Mouse.GetState().X;
            } 
            
            if (X > _window.Width)
            {
                X = _window.Width;
                KX = Mouse.GetState().X - _window.Width;
            }

            if (Y < 0)
            {
                Y = 0;
                KY = Mouse.GetState().Y;
            }

            if (Y > _window.Height)
            {
                Y = _window.Height;
                KY = Mouse.GetState().Y - _window.Height;
            }
        }

        private void CheckEvents()
        {
            var state = Mouse.GetCursorState();

            if (state.IsAnyButtonDown && !_mouseDown)
            {
                _mouseUp = false;
                _mouseDown = true;
                _prevX = X;
                _prevY = Y;
                State = MouseType.Down;

                IsMouseDown(state);
            }
            
            var isUp = !state.IsButtonUp(Button);
            if (!isUp && !_mouseUp)
            {

                _mouseUp = true;
                _mouseDown = false;
                State = MouseType.Up;

                IsMouseUp(state);
            }

            if (_prevX != X && _prevY != Y)
            {
                IsMouseMove();
            }
        }

        private void IsMouseMove()
        {
            MouseMove?.Invoke(new MouseState
            {
                Action = MouseAction.Move,
                Button = MouseButton.Left & MouseButton.Right,
                X = X,
                Y = Y
            });

            if (_mouseDown && !_mouseUp)
            {
                MousePressedMove?.Invoke(new MouseState
                {
                    Action = MouseAction.PressedMove,
                    Button = MouseButton.Left & MouseButton.Right,
                    X = X,
                    Y = Y
                });
            }
        }

        private void IsMouseDown(OpenTK.Input.MouseState state)
        {
            if (state.LeftButton == ButtonState.Pressed)
            {
                MouseDown?.Invoke(new MouseState
                {
                    Action = MouseAction.Down,
                    Button = MouseButton.Left,
                    X = X,
                    Y = Y
                });
                Button = OpenTK.Input.MouseButton.Left;
            }
            else if (state.RightButton == ButtonState.Pressed)
            {
                MouseDown?.Invoke(new MouseState
                {
                    Action = MouseAction.Down,
                    Button = MouseButton.Right,
                    X = X,
                    Y = Y
                });
                Button = OpenTK.Input.MouseButton.Right;
            }
        }

        private void IsMouseUp(OpenTK.Input.MouseState state)
        {
            MouseUp?.Invoke(new MouseState
            {
                Action = MouseAction.Up,
                Button = Button == OpenTK.Input.MouseButton.Left ? MouseButton.Left : MouseButton.Right,
                X = X,
                Y = Y
            });
        }

        public void Draw(Canvas canvas)
        {
        //    canvas.DrawText($"[{X}, {Y}] state: {State}, button: {Button}", 20, 50, new SKPaint
        //    {
        //        Style = SKPaintStyle.Fill,
        //        IsAntialias = true,
        //        TextSize = 40,
        //        Color = new SKColor(200, 200, 200)
        //    });
            
            canvas.Save();
            canvas.Translate(X, Y);
            canvas.RotateRadians(Angle);
            canvas.DrawCircle(0, 0, Radius * 3, new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                StrokeWidth = 2,
                Color = new SKColor(200, 200, 200)
            });
            canvas.DrawLine(-Radius * 3, 0, Radius * 3, 0, new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                StrokeWidth = 1,
                Color = new SKColor(200, 200, 200)
            });
            canvas.DrawLine(0, -Radius * 3, 0, Radius * 3, new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                StrokeWidth = 1,
                Color = new SKColor(200, 200, 200)
            });
            canvas.DrawCircle(0, 0, Radius / 2, PaintStay);
            canvas.Restore();
        }
    }
}