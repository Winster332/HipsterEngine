using System.Collections.Generic;
using SimplePhysics.Physics.Common;

namespace SimplePhysics.Physics.Collision
{
    public class VertexShape
    {
        public List<Vec2> Vertices { get; set; }

        public VertexShape()
        {
            Vertices = new List<Vec2>();
        }
    }
}