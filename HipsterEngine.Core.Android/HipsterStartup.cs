using Android.App;
using Android.Views;
using HipsterEngine.Core.Configurations;
using HipsterEngine.Core.UI.Events.Mouse;
using SkiaSharp;
using SkiaSharp.Views.Android;

namespace HipsterEngine.Core.Android
{
    public class HipsterStartup : IStartup
    {
        public HipsterEngine Engine { get; set; }
        private SKCanvasView _view;
        private Activity _activity;

        public HipsterStartup(Activity activity)
        {
            _activity = activity;
        }

        public HipsterEngine CreateEngine(int width, int height)
        {
            _view = new SKCanvasView(_activity);
            _view.Touch += ViewOnTouch;
            _view.PaintSurface += ViewOnPaintSurface;
            
            Engine = new HipsterEngine(width, height);
            Engine.OnResize(width, height);

            return Engine;
        }

        private void ViewOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            
            canvas.Clear(new SKColor(50, 50, 50));
            
            Engine?.Step(1, 1);
            Engine?.Draw(canvas);
            
            _view.Invalidate();
        }

        private void ViewOnTouch(object sender, View.TouchEventArgs e)
        {
            var state = new MouseState();
            state.X = e.Event.GetX();
            state.Y = e.Event.GetY();

            if (e.Event.Action == MotionEventActions.Down)
                state.Action = MouseAction.Down;
            if (e.Event.Action == MotionEventActions.Move)
                state.Action = MouseAction.Move;
            if (e.Event.Action == MotionEventActions.Up)
                state.Action = MouseAction.Up;

            state.Button = MouseButton.Left | MouseButton.Right;
            
            Engine.OnMouse(state);
        }

        public void Run(double updatePerSecond, double framesPerSecond)
        {
            _activity.SetContentView(_view);
        }
        
        public void Dispose()
        {
        }
    }
}