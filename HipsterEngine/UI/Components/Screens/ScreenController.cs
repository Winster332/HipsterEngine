using System;
using ConsoleApplication2.UI.Events;
using SkiaSharp;

namespace ConsoleApplication2.UI.Components.Screens
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

        public void SetScreen(Screen screen, object data = null, IScreenAnimation animation = null)
        {
            CurrentScreen?.OnPaused();
            CurrentScreen?.Dispose();

            screen.Width = Width;
            screen.Height = Height;
            screen.HipsterEngine = _hipsterEngine;
            screen.SetUIController(UI);
            screen.Intent = new Intent
            {
                From = CurrentScreen,
                To = screen,
                Animation = animation,
                Data = data
            };
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
            CurrentScreen?.Intent?.Animation?.Update();
        }

        public void Draw(SKCanvas canvas)
        {
            CurrentScreen?.OnDraw(canvas);
            CurrentScreen?.Intent?.Animation?.Draw(_hipsterEngine.Surface.Canvas);
        }

        public void Dispose()
        {
            CurrentScreen.Dispose();
        }
    }
}