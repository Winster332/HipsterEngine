using HipsterEngine.Core.Graphics;

namespace HipsterEngine.UI.Components
{
    public class CircleElement : Element
    {
        public float Radius { get; set; }
        
        public CircleElement(Core.HipsterEngine engine) : base(engine)
        {
            Radius = 0;
        }

        public override void UseClip(Canvas canvas)
        {
            // error
        }
    }
}