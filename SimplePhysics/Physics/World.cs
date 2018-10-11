using System;
using System.Collections.Generic;
using SimplePhysics.Physics.Collision;
using SimplePhysics.Physics.Common;
using SimplePhysics.Physics.Dynamic;

namespace SimplePhysics.Physics
{
    public class World
    {
        public Vec2 Gravity { get; set; }
        public List<Body> Bodies { get; set; }

        public World(Vec2 gravity)
        {
            Gravity = gravity;
            
            Bodies = new List<Body>();
        }

        public void Step(Action<Body> actionBody)
        {
            for (var i = 0; i < Bodies.Count; i++)
            {
                var body1 = Bodies[i];

                body1.Step();
                body1.Shape.Velacity -= Gravity;
                actionBody(body1);
                
                for (var j = 0; j < Bodies.Count; j++)
                {
                    if (i != j)
                    {
                        var body2 = Bodies[j];

                        if (IsCollide(body1, body2))
                        {
                            SolveCollision(body1, body2);
                        }
                    }
                }
            }
        }

        public void SolveCollision(Body b1, Body b2)
        {
            if (b1.Shape.GetType() == typeof(CircleShape) && b2.Shape.GetType() == typeof(CircleShape))
            {
                var shape1 = (CircleShape) b1.Shape;
                var shape2 = (CircleShape) b2.Shape;
                
                var r = shape1.Radius + shape2.Radius;
                var distance = Vec2.Distance(b1.Shape.Position, b2.Shape.Position);

                var kdepth = distance - r;
                var normal = Vec2.Normalize(shape1.Velacity - shape2.Velacity);
                var impulse = normal * (kdepth / 2);

                shape1.Position += (normal * kdepth);

                shape1.Velacity += impulse;
                shape2.Velacity += impulse;
            }
        }

        public bool IsCollide(Body b1, Body b2)
        {
            return b1.Shape.Intersect(b2.Shape);
        }

        public Body AddCircle(float x, float y, float radius)
        {
            var body = new Body
            {
                Shape = new CircleShape
                {
                    Position = new Vec2(x, y),
                    Radius = radius
                }
            };
            
            Bodies.Add(body);

            return body;
        }
    }
}