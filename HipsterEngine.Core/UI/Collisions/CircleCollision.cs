using System;
using HipsterEngine.Core.UI.Components;

namespace HipsterEngine.Core.UI.Collisions
{
    public class CircleCollision : UIElement
    {
        public float Radius { get; set; }
        public float Angle { get; set; }

        public CircleCollision()
        {
            X = 0;
            Y = 0;
            Radius = 0;
        }

        public override bool IsIntersection(float x, float y)
        {
            var distance = Math.Sqrt(Math.Pow(X - x, 2) + Math.Pow(Y - y, 2));

            return distance <= Radius;
        }
    }
}