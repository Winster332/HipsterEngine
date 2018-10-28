using System.Collections.Generic;
using HipsterEngine.Core.Graphics;
using HipsterEngine.Core.Particles;
using HipsterEngine.Core.Physics;
using HipsterEngine.Core.UI.Components.Screens;
using HipsterEngine.Core.UI.Events.Mouse;
using SkiaSharp;

namespace HipsterEngine.Core
{
    public class HipsterEngine
    {
        public Surface Surface { get; set; }
        public ParticlesController Particles { get; set; }
        public ScreenController Screens { get; set; }
        public PhysicsController Physics { get; set; }
        public DeltaTime DeltaTime { get; set; }
        public Files.Files Files { get; set; }

        public HipsterEngine(float width, float height)
        {
            Physics = new PhysicsController(this);
            Surface = new Surface();
            Surface.Width = width;
            Surface.Height = height;
            Particles = new ParticlesController();
            Screens = new ScreenController(this);
            DeltaTime = new DeltaTime();
            Files = new Files.Files();
        }

        public void SetStartScreen(Screen screen)
        {
            Screens.SetScreen(screen);
        }


        public void OnMouse(MouseState mouseState)
        {
            //   mouseState.X -= Surface.Canvas.Camera.X;
            //   mouseState.Y -= Surface.Canvas.Camera.Y;

            //    mouseState.X *= Surface.KSize;
            Screens.OnMouse(mouseState);
        }

        public void OnTap(List<MouseState> states)
        {
            states.ForEach(s =>
            {
                Screens.OnMouse(s);
            });
        }
        
        public void Step(double time, float dt)
        {
            DeltaTime.Update();
            
            Screens.Step(time, dt);
        }

        public void Draw(SKCanvas canvas)
        {
            Surface.Canvas.SetSkiaCanvas(canvas);
            
            Screens.Draw(Surface.Canvas.GetSkiaCanvas());
        }

        public void OnResize(float width, float height)
        {
            Surface.OnResize(width, height);
        }
    }
}