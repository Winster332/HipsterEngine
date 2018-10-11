using HipsterEngine.Core.UI.Collisions;

namespace HipsterEngine.Core.UI.Components.Layout
{
    public class LinearLayout : AbsoluteLayout
    {
        public LinearLayoutOrientation Orientation { get; set; }
        public float StepPosition { get; set; }

        public LinearLayout(LinearLayoutOrientation orientation)
        {
            Orientation = orientation;
        }

        public void AddElement(RectCollision element)
        {
            var next = 0.0f;

            if (Orientation == LinearLayoutOrientation.Horizontal)
            {
                Childs.ForEach(x => next += ((RectCollision) x).Width + StepPosition);
                element.X = next;
            }
            if (Orientation == LinearLayoutOrientation.Vertical)
            {
                Childs.ForEach(x => next += ((RectCollision) x).Height + StepPosition);
                element.Y = next;
            }

            base.AddElement(element);
        }
    }
}