using System.Windows.Forms;
using Box2DX.Common;
using ConsoleApplication2.UI.Events;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace HipsterEngine.Desktop
{
    public class GameWindow : Form
    {
        public SKControl SkiaControl;
        public int FPS { get; set; } = 10;
        public bool IsLife = false;
        public ConsoleApplication2.HipsterEngine Engine { get; set; }

        public GameWindow(int width = 720, int height = 640)
        {
            Width = width;
            Height = height;
            IsLife = true;
            
            SkiaControl = new SKControl
            {
                Width = Width,
                Height = Height
            };
            Controls.Add(SkiaControl);
            
            Engine = new ConsoleApplication2.HipsterEngine(Width, Height);

            SkiaControl.PaintSurface += SkiaControlOnPaintSurface;
            
            SizeChanged += (sender, args) =>
            {
                SkiaControl.Width = Width;
                SkiaControl.Height = Height;
                
                Engine.OnResize(SkiaControl.Width, SkiaControl.Height);
            };
            
            SkiaControl.MouseDown += (o, e) =>
            {
                var button = MouseButton.Left;

                if (e.Button == MouseButtons.Left)
                    button = MouseButton.Left;
                if (e.Button == MouseButtons.Right)
                    button = MouseButton.Right;

                Engine.OnMouse(new MouseState(e.X, e.Y, MouseAction.Down, button));
            };
            
            SkiaControl.MouseUp   += (o, e) =>
            {
                var button = MouseButton.Left;

                if (e.Button == MouseButtons.Left)
                    button = MouseButton.Left;
                if (e.Button == MouseButtons.Right)
                    button = MouseButton.Right;
                
                Engine.OnMouse(new MouseState(e.X, e.Y, MouseAction.Up, button));
            };
            SkiaControl.MouseMove += (o, e) =>
            {
                var button = MouseButton.Left;

                if (e.Button == MouseButtons.Left)
                    button = MouseButton.Left;
                if (e.Button == MouseButtons.Right)
                    button = MouseButton.Right;
                
                Engine.OnMouse(new MouseState(e.X, e.Y, MouseAction.Move, button));
            };

            KeyDown += (o, e) =>
            {
            };
        }

        public void InitCPURenderer()
        {
            
        }

        private void SkiaControlOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            if (FPS >= 5)
            {
                canvas.Clear(new SKColor(50, 50, 50));

                Engine.Step(1, 1);
                Engine.Draw(canvas);
                FPS = 0;
            }

            if (IsLife)
            {
                FPS++;

                SkiaControl.Invalidate();
            }
        }
    }
}