using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SkiaSharp;
using SkiaSharp.Views.Android;

namespace HipsterEngine.Core.Android
{
    public delegate void SKGLPaintEventHandler(SKSurface surface, GRBackendRenderTargetDesc renderTarget);

    public class SKGLRenderer : SKGLSurfaceView.ISKRenderer
    {
        public event SKGLPaintEventHandler PaintSurface;

        public void OnDrawFrame(SKSurface surface, GRBackendRenderTargetDesc renderTarget)
        {
            PaintSurface?.Invoke(surface, renderTarget);
        }
    }
}