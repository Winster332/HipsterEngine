using ConsoleApplication2.Graphics;

namespace ConsoleApplication2.UI.Components.Screens
{
    public interface IScreenAnimation
    {
        void Update();
        void Draw(Canvas canvas);
    }
}