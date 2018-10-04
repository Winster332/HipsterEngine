using ConsoleApplication2.UI.Components;

namespace ConsoleApplication2.UI.Collisions
{
    public class RectCollision : UIElement
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public RectCollision()
        {
            LocalX = 0;
            LocalY = 0;
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
        }
        
        public override bool IsIntersection(float x, float y)
        {
            return x >= X && x <= X + Width && y >= Y && y <= Y + Height;
        }
    }
}