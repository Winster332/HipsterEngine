namespace HoleVortex.Desktop.Screens.UI
{
    public class ButtonPause : Button
    {
        public ButtonPause(HipsterEngine.Core.HipsterEngine engine, float x, float y, float radius) : base(engine, x,
            y, radius)
        {
        }

        public override void OnLoad()
        {
            TextureId = 0;
        }
    }
}