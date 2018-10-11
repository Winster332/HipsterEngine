using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using HipsterEngine.Core.UI.Events.Mouse;
using HoleVortex.Android.Asserts;
using HoleVortex.Android.Screens;
using SkiaSharp;
using SkiaSharp.Views.Android;

namespace HoleVortex.Android
{
    [Activity(Label = "HoleVortex.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public HipsterEngine.Core.HipsterEngine Engine { get; set; }
        public SKCanvasView _view;
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Assert.Init(FilesDir.Path);
            var xxx = Assets.GetLocales();
            Assert.Load(Assets, new[]
            {
                @"button_pause.png",
                @"button_resume.png",
                @"circle_0.png",
                @"circle_1.png",
                @"circle_2.png",
                @"circle_3.png",
                @"circle_4.png",
                @"circle_5.png",
                @"circle_6.png",
                @"circle_7.png"
            });
            
            _view = new SKCanvasView(this);
            _view.Touch += ViewOnTouch;
            _view.PaintSurface += ViewOnPaintSurface;
            SetContentView(_view);
        }

        private void ViewOnTouch(object sender, View.TouchEventArgs e)
        {
        }

        public void InitEngine(int width, int height)
        {
        }

        private void ViewOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
        }
    }
}