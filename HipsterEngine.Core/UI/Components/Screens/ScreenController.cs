using System;
using HipsterEngine.Core.UI.Events.Keyboard;
using HipsterEngine.Core.UI.Events.Mouse;
using SkiaSharp;

namespace HipsterEngine.Core.UI.Components.Screens
{
    public class ScreenController : IDisposable
    {
        public Screen CurrentScreen { get; set; }
        public UIController UI { get; set; }
        private HipsterEngine _hipsterEngine { get; set; }
        private float Width => _hipsterEngine.Surface.Width;
        private float Height => _hipsterEngine.Surface.Height;

        public ScreenController(HipsterEngine engine)
        {
            _hipsterEngine = engine;
            CurrentScreen = null;
            UI = new UIController();
        }

        public void SetScreen(Screen screen)
        {
            CurrentScreen?.OnPaused();
            CurrentScreen?.Dispose();

            screen.Width = Width;
            screen.Height = Height;
            screen.HipsterEngine = _hipsterEngine;
            screen.SetUIController(UI);
            CurrentScreen = screen;
            CurrentScreen.OnLoad();
        }

        public void OnMouse(MouseState mouseState)
        {
            CurrentScreen?.OnMouseAction(mouseState);
        }

        public void OnKeyDown(Keys key)
        {
            CurrentScreen?.OnKeyDown(key);
        }
        
        public void OnKeyUp(Keys key)
        {
            CurrentScreen?.OnKeyUp(key);
        }
        
        public void Step(double time, float dt)
        {
            CurrentScreen?.OnUpdate(time, dt);
        }

        public void Draw(SKCanvas canvas)
        {
            CurrentScreen?.OnDraw(canvas);
        }

        public void Dispose()
        {
            CurrentScreen.Dispose();
        }
    }
}