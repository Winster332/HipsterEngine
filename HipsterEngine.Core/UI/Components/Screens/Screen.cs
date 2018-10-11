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

        protected Screen()
        {
            Enabled = true;
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
            if (Enabled)
            {
                Update?.Invoke(time, dt);
            }
        }

        public void OnDraw(SKCanvas canvas)
        {
            Paint?.Invoke(null, canvas);
            _uiController.Step(canvas);
        }

        public void OnMouseAction(MouseState mouseState)
        {
            switch (mouseState.Action)
            {
                case MouseAction.Down: MouseDown?.Invoke(null, mouseState); break;
                case MouseAction.Move: MouseMove?.Invoke(null, mouseState); break;
                case MouseAction.Up: MouseUp?.Invoke(null, mouseState); break;
            }
            
            _uiController.SendMouse(mouseState);
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
            _uiController.Dispose();
            Unloaded?.Invoke(this);
        }

        public void OnPaused()
        {
            Enabled = false;
        }

        public abstract void OnLoad();

        public void SetUIController(UIController controller)
        {
            _uiController = controller;
        }

        public void OnResume()
        {
            Enabled = true;
        }
    }
}