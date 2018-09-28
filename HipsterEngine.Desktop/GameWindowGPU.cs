using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ConsoleApplication2.UI.Events;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using ButtonState = OpenTK.Input.ButtonState;
using MouseButton = OpenTK.Input.MouseButton;
using MouseState = OpenTK.Input.MouseState;

namespace HipsterEngine.Desktop
{
    public class GameWindowGPU : OpenTK.GameWindow
    {
        private GRContext context;
        private GRBackendRenderTargetDesc renderTarget;
        public event EventHandler<SKPaintGLSurfaceEventArgs> PaintSurface;
        public event EventHandler<double> UpdateSurface;
        public InputMouse InputMouse { get; set; }

        public ConsoleApplication2.HipsterEngine Engine { get; set; }
        
        public GameWindowGPU(int width, int height) : base(width, height, GraphicsMode.Default, "OpenTK Quick Start Sample")
        {
            VSync = VSyncMode.Off;
            
            Engine = new ConsoleApplication2.HipsterEngine(Width, Height);
            
            PaintSurface += OnPaintSurface;
        }

        private void OnPaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            canvas.Clear(new SKColor(50, 50, 50));

            Engine.Draw(canvas);
            
            InputMouse.Draw(Engine.Surface.Canvas);
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            Engine.OnResize(ClientRectangle.Width, ClientRectangle.Height);
        }
        

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            
            Engine.Step(e.Time, 1);
            
            UpdateSurface?.Invoke(this, e.Time);
            
            InputMouse.Update();
        }

        protected override void OnLoad(EventArgs ee)
        {
            base.OnLoad(ee);
            var glInterface = GRGlInterface.CreateNativeGlInterface();
            Debug.Assert(glInterface.Validate());

            this.context = GRContext.Create(GRBackend.OpenGL, glInterface);
            Debug.Assert(this.context.Handle != IntPtr.Zero);
            this.renderTarget = CreateRenderTarget();

            this.KeyDown += (o, e) =>
            {
                if (e.Key == Key.Escape)
                    this.Close();
            };
            InputMouse = new InputMouse(this);
            InputMouse.MouseDown += state => Engine.OnMouse(state);
            InputMouse.MouseUp += state => Engine.OnMouse(state);
            InputMouse.MouseMove += state => Engine.OnMouse(state);
            InputMouse.MousePressedMove += state => Engine.OnMouse(state);
            FocusedChanged += OnFocusedChanged;
            
        //    WindowState = WindowState.Fullscreen;
            CursorVisible = false;
        }

        private void OnFocusedChanged(object sender, EventArgs e)
        {
            var isWindowFocus = Focused;

            InputMouse.Enabled = isWindowFocus;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}";

            this.renderTarget.Width = this.Width;
            this.renderTarget.Height = this.Height;

            using (var surface = SKSurface.Create(this.context, this.renderTarget))
            {
                Debug.Assert(surface != null);
                Debug.Assert(surface.Handle != IntPtr.Zero);

                var canvas = surface.Canvas;

                canvas.Flush();

                // <this was a event delegate before
                var info = this.renderTarget;

                PaintSurface?.Invoke(this, new SKPaintGLSurfaceEventArgs(surface, renderTarget));

             //   canvas.Clear(SKColors.ForestGreen);

                // removing this block stop the exception and paints the windows green
           //     using (SKPaint paint = new SKPaint
           //     {
           //         Style = SKPaintStyle.Stroke,
           //         Color = SKColors.Red,
           //         IsAntialias = true,
           //         StrokeWidth = 25
           //     })
           //     {
           //         canvas.DrawCircle(info.Width / 2, info.Height / 2, 100, paint);
           //         paint.Style = SKPaintStyle.Fill;
            //        paint.Color = SKColors.Blue;
            //        canvas.DrawCircle(info.Width / 2, info.Height / 2, 100, paint);
           //     }
                // />

                canvas.Flush();
                // throws System.AccessViolationException:
                //  Attempted to read or write protected memory.
                //  This is often an indication that other memory is corrupt.
                //
                // at SkiaSharp.SkiaApi.sk_canvas_flush(IntPtr canvas)
                //  at SkiaSharp.SKCanvas.Flush()
            }

            this.context.Flush();
            SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            this.context?.Dispose();
            this.context = null;
        }

        public static GRBackendRenderTargetDesc CreateRenderTarget()
        {
            GL.Rotate(10, 10, 10, 10);
            GL.GetInteger(GetPName.FramebufferBinding, out int framebuffer);
            // debug: framebuffer = 0
            GL.GetInteger(GetPName.StencilBits, out int stencil);
            // debug: stencil = 0
            GL.GetInteger(GetPName.Samples, out int samples);
            // debug: samples = 0

            int bufferWidth = 0;
            int bufferHeight = 0;

            //#if __IOS__ || __TVOS__
            GL.GetRenderbufferParameter(RenderbufferTarget.Renderbuffer, RenderbufferParameterName.RenderbufferWidth,
                out bufferWidth);
            // debug: bufferWidth = 0
            GL.GetRenderbufferParameter(RenderbufferTarget.Renderbuffer, RenderbufferParameterName.RenderbufferHeight,
                out bufferHeight);
            // debug: bufferHeight = 0
            //#endif

            return new GRBackendRenderTargetDesc
            {
                Width = bufferWidth,
                Height = bufferHeight,
                Config = GRPixelConfig.Bgra8888, // Question: Is this the right format and how to do it platform independent?
                Origin = GRSurfaceOrigin.BottomLeft,
                SampleCount = samples,
                StencilBits = stencil,
                RenderTargetHandle = (IntPtr) framebuffer,
            };
        }
    }
}
