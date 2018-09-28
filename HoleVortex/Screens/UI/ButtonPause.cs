using ConsoleApplication2.UI.Components;
using HoleVortex.Asserts;
using SkiaSharp;

namespace HoleVortex.Screens.UI
{
    public class ButtonPause : Button
    {
        public ButtonPause(ConsoleApplication2.HipsterEngine engine, float x, float y, float radius) : base(engine, x,
            y, radius)
        {
        }

        public override void OnLoad()
        {
            TextureId = 0;
        }
    }
}