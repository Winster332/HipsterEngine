using System;
using SimplePhysics.Physics.Common;

namespace SimplePhysics.Physics.Collision
{
    public class CircleShape : Shape
    {
        public float Radius { get; set; }
        
        public bool CirclevsCircleUnoptimized(CircleShape a, CircleShape b)
        {
            var r = a.Radius + b.Radius;
            return r < Vec2.Distance(a.Position, b.Position);
        }
 
        public override bool Intersect(Shape a)
        {
            var shape = (CircleShape) a;
            
            var r = Radius + shape.Radius;
            var distance = Vec2.Distance(Position, shape.Position);
            var collide = distance < r;

            return collide;
        }
    }
}