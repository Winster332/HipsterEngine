using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using HipsterEngine.Core.Android;
using HipsterEngine.Core.Configurations;
using FlatPlant.Screens;

namespace FlatPlanet.Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var metrics = Resources.DisplayMetrics;
            var height = metrics.HeightPixels;
            var width = metrics.WidthPixels;
                
            Builder.Create(new HipsterStartup(this), width, height)
                .SetTargetFPS(60, 60)
                .Run(new GameScreen());
        }
    }
}