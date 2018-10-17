using Android.App;
using Android.OS;
using Android.Support.V7.App;
using System.Collections.Generic;
using System.IO;
using HipsterEngine.Core.Android;
using HipsterEngine.Core.Configurations;
using HoleVortex.Core.Screens;

namespace HoleVortex.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var assetsFiles = new List<string>()
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
            };
            var metrics = Resources.DisplayMetrics;

            var height = metrics.HeightPixels;
            var width = metrics.WidthPixels;

            Core.IO.Assets.Init(FilesDir.Path);
            Core.IO.Assets.Load(GetStreams(assetsFiles));
                
            Builder.Create(new HipsterStartup(this), width, height)
                .SetTargetFPS(60, 60)
                .Run(new MenuScreen());
        }

        public Stream[] GetStreams(List<string> files)
        {
            var listStreams = new List<Stream>();
            
            files.ForEach(file =>
            {
                listStreams.Add(Assets.Open(file));
            });

            return listStreams.ToArray();
        }
    }
}