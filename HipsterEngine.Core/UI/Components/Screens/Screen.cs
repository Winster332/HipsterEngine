using System;
using HipsterEngine.Core.UI.Components.Screens.Events;
using HipsterEngine.Core.UI.Events.Keyboard;
using HipsterEngine.Core.UI.Events.Mouse;
using SkiaSharp;

namespace HipsterEngine.Core.UI.Components.Screens
{
    public abstract class Screen : IDisposable
    {
        public event PaintEventHandler Paint;
        public event UpdateEventHandler Update;
        public event UnloadedScreenEventHandler Unloaded;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseUp;
        public event KeyboardEventHandler KeyDown;
        public event KeyboardEventHandler KeyUp;
        public float Width { get; set; }
        public float Height { get; set; }
        public HipsterEngine HipsterEngine { get; set; }
        private UIController _uiController { get; set; }
        public bool Enabled { get; set; }
        public ScreenState State { get; set; }
        private SKPaint _debugPaint;

        protected Screen()
        {
            Enabled = true;
            State = ScreenState.Initialize;

            _debugPaint = new SKPaint
            {
                TextSize = 20,
                Color = new SKColor(100, 100, 100),
                Typeface = SKTypeface.FromFamilyName(
                    "Arial", 
                    SKFontStyleWeight.Normal, 
                    SKFontStyleWidth.Normal, 
                    SKFontStyleSlant.Upright),
            };
        }

        public void AddView(UIElement element)
        {
            _uiController.AddElement(element);
        }
        
        public void RemoveElement(UIElement element)
        {
            _uiController.RemoveElement(element);
        }

        public void OnUpdate(double time, float dt)
        {
            if (State == ScreenState.Running)
            {
                if (Enabled)
                {
                    Update?.Invoke(time, dt);
                }
            }
        }

        public void OnDraw(SKCanvas canvas)
        {
            if (State == ScreenState.Running)
            {
                Paint?.Invoke(null, canvas);
                _uiController.Step(canvas);

                if (System.Diagnostics.Debugger.IsAttached)
                {
                    canvas.DrawText($"FPS: {HipsterEngine.DeltaTime.GetFPS()}", 10, 25, _debugPaint);
                    canvas.DrawText($"Screen: {GetType().Name}", 10, 50, _debugPaint);
                    canvas.DrawText($"State: {State}", 10, 75, _debugPaint);
                }
            }
        }

        public void OnMouseAction(MouseState mouseState)
        {
            if (State != ScreenState.Finished)
            {
                switch (mouseState.Action)
                {
                    case MouseAction.Down: MouseDown?.Invoke(null, mouseState); break;
                    case MouseAction.Move: MouseMove?.Invoke(null, mouseState); break;
                    case MouseAction.Up: MouseUp?.Invoke(null, mouseState); break;
                }

                _uiController.SendMouse(mouseState);
            }
        }

        public void OnKeyDown(Keys key)
        {
            KeyDown?.Invoke(null, key);
        }

        public void OnKeyUp(Keys key)
        {
            KeyUp?.Invoke(null, key);
        }

        public void Dispose()
        {
            State = ScreenState.Finished;

            _uiController?.Dispose();
            Unloaded?.Invoke(this);
        }

        public void OnPaused()
        {
            State = ScreenState.Paused;
            Enabled = false;
        }

        public abstract void OnLoad();

        public void SetUIController(UIController controller)
        {
            _uiController = controller;
        }

        public void OnResume()
        {
            State = ScreenState.Running;

            Enabled = true;
        }
    }
}