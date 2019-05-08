using System;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace HipsterEngine.Core.Desktop
{
    public class GameWindowGPU : OpenTK.GameWindow
    {
        private GRContext context;
        private GRBackendRenderTargetDesc renderTarget;
        public event EventHandler<SKPaintGLSurfaceEventArgs> PaintSurface;
        public event EventHandler<double> UpdateSurface;
        public InputMouse InputMouse { get; set; }

        public HipsterEngine Engine { get; set; }
        
        public GameWindowGPU(int width, int height) 
            : base(width, height, GraphicsMode.Default, "HipsterEngine 1.0", GameWindowFlags.Default, DisplayDevice.Default)
        {
            VSync = VSyncMode.Off;
            
            Engine = new HipsterEngine(Width, Height);
            
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

            var props = new SKSurfaceProps
            {
                Flags = SKSurfacePropsFlags.None,
                PixelGeometry = SKPixelGeometry.RgbHorizontal
            };
            using (var surface = SKSurface.Create(this.context, this.renderTarget))
            {
                Debug.Assert(surface != null);
                Debug.Assert(surface.Handle != IntPtr.Zero);
                
                var canvas = surface.Canvas;

                canvas.Flush();

                var info = this.renderTarget;

                PaintSurface?.Invoke(this, new SKPaintGLSurfaceEventArgs(surface, renderTarget));

                canvas.Flush();
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