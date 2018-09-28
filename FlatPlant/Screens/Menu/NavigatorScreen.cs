using Box2DX.Common;
using ConsoleApplication2.UI.Components;
using ConsoleApplication2.UI.Components.Screens;
using SkiaSharp;

namespace FlatPlant.Screens
{
    public class NavigatorScreen : Screen
    {
        public SwypeControl SwypeControl { get; set; }
        public Vec2 MousePosition { get; set; }
        
        public override void OnLoad()
        {
            Paint += OnPaint;
            
            HipsterEngine.Surface.Canvas.Camera.SetTarget(Width / 2, Height / 2);
            
            SwypeControl = new SwypeControl(HipsterEngine);
            SwypeControl.SetBoundVertical(0, 0)
                .SetBoundHorizontal(-Width * 2, Width * 2);
            SwypeControl.Scroll += (sender, args) =>
            {
                HipsterEngine.Surface.Canvas.Camera.SetTarget(SwypeControl.ValueX + Width / 2, SwypeControl.ValueY + Height / 2);
            };
            MousePosition = new Vec2();
            MouseMove += (element, state) => MousePosition = new Vec2(state.X, state.Y);
            
            SwypeControl.AddScreen(0, 0, new PlayScreen());
            SwypeControl.AddScreen(Width, 0, new RecordsScreen());
            SwypeControl.AddScreen(-Width, 0, new StoreScreen());
        }

        private void OnPaint(UIElement element, SKCanvas e)
        {
            HipsterEngine.Surface.Canvas.Camera.Update();
            
            var canvas = HipsterEngine.Surface.Canvas;
            canvas.Save();
            
            canvas.Translate(canvas.Camera.X, canvas.Camera.Y);
        //    canvas.Scale(HipsterEngine.Surface.KSize, HipsterEngine.Surface.KSize);
            SwypeControl.Update(1, 1);
            SwypeControl.Draw();

            canvas.Restore();
        }
    }
}