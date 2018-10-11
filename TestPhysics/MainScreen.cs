using System;
using HipsterEngine.Core.Physics;
using HipsterEngine.Core.UI.Components;
using HipsterEngine.Core.UI.Components.Screens;
using SkiaSharp;

namespace TestPhysics
{
    public class MainScreen : Screen
    {
        public SKDrawDebug Renderer;

        public override void OnLoad()
        {
            Paint += OnPaint;

            Renderer = new SKDrawDebug(HipsterEngine);

            HipsterEngine.Physics.Initialize(-1000, -1000, 1000, 1000, 0, 1f, true);
            HipsterEngine.Physics.GetWorld().SetDebugDraw(Renderer);

            HipsterEngine.Physics.FactoryBody
                .CreateRigidVertex()
                .CreateBox(0.2f, 0.2f, 50, 50, 0.2f)
                .CreateBodyDef(0, 100, 0, true, false)
                .Build(1f);
            
            HipsterEngine.Physics.FactoryBody
                .CreateRigidVertex()
                .CreateBox(0.2f, 0.2f, 50, 50, 0.2f)
                .CreateBodyDef(30, 0, 0, true, false)
                .Build(1f);
            
            HipsterEngine.Physics.FactoryBody
                .CreateRigidVertex()
                .CreateBox(0.2f, 0.2f, 350, 50, 0f)
                .CreateBodyDef(0, 400, 0, true, false)
                .Build(0);
        }

        private void OnPaint(UIElement element, SKCanvas canvas)
        {
            canvas.DrawText($"Current screen: {GetType().Name} [FPS: {HipsterEngine.DeltaTime.GetFPS()}]", 10, 75,
                new SKPaint
                {
                    TextSize = 20,
                    Color = new SKColor(100, 100, 100)
                });

            HipsterEngine.Surface.Canvas.Save();
    //        HipsterEngine.Surface.Canvas.Translate(HipsterEngine.Surface.Canvas.Camera.X,
    //            HipsterEngine.Surface.Canvas.Camera.Y);
    //        HipsterEngine.Surface.Canvas.Scale(HipsterEngine.Surface.Canvas.Camera.ScaleX, HipsterEngine.Surface.Canvas.Camera.ScaleY);

            HipsterEngine.Physics.Step(1.0f, 20);

            HipsterEngine.Particles.Draw(HipsterEngine.Surface.Canvas.GetSkiaCanvas());
            HipsterEngine.Surface.Canvas.Restore();
        }
    }
}