using HipsterEngine.Core.Graphics;
using HipsterEngine.Core.UI.Components.Screens.Events;
using HipsterEngine.Core.UI.Events.Mouse;

namespace HipsterEngine.Core.UI.Components.Screens
{
    public abstract class SwypeScreen
    {
        public event PaintEventHandler Paint;
        public event UpdateEventHandler Update;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseUp;
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public HipsterEngine HipsterEngine { get; set; }
        protected bool Enabled { get; set; }

        public SwypeScreen()
        {
            Enabled = true;
        }
        
        public void OnUpdate(double time, float dt)
        {
            if (Enabled)
            {
                Update?.Invoke(time, dt);
            }
        }

        public void OnDraw(Canvas canvas)
        {
            if (Enabled)
            {
                Paint?.Invoke(null, canvas.GetSkiaCanvas());
            }
        }

        public void OnMouseAction(MouseState mouseState)
        {
            switch (mouseState.Action)
            {
                case MouseAction.Down: MouseDown?.Invoke(null, mouseState); break;
                case MouseAction.Move: MouseMove?.Invoke(null, mouseState); break;
                case MouseAction.Up: MouseUp?.Invoke(null, mouseState); break;
            }
        }

        public abstract void OnLoad();
    }
}