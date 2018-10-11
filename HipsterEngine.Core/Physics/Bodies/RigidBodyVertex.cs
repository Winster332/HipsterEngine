using Box2DX.Collision;
using Box2DX.Common;

namespace HipsterEngine.Core.Physics.Bodies
{
    public class RigidBodyVertex : RigidBody
    {
        public RigidBodyVertex(PhysicsController physics) : base(physics)
        {
        }

        public RigidBodyVertex CreateVertex(float restetution, float friction, Vec2[] vert, float density, bool isSensor = false, short group_index = 1)
        {
            PolygonDef pDef = new PolygonDef();
            pDef.Restitution = restetution;
            pDef.Friction = friction;
            pDef.Density = density;
            
            pDef.VertexCount = vert.Length;
            for (int i = 0; i < vert.Length; i++)
            {
                vert[i].X /= PhysicsController.metric;
                vert[i].Y /= PhysicsController.metric;
                
                pDef.Vertices[i].Set(vert[i].X, vert[i].Y);
            }

            pDef.Filter.GroupIndex = group_index;
            pDef.IsSensor = isSensor;

            _shapeDef = pDef;
            
            return this;
        }
        
        public RigidBodyVertex CreateBox(float restetution, float friction, float width, float height, float density, bool isSensor = false, short group_index = 1)
        {
            PolygonDef pDef = new PolygonDef();
            pDef.Restitution = restetution;
            pDef.Friction = friction;
            pDef.Density = density;
            
            pDef.SetAsBox(width / PhysicsController.metric / 2, height / PhysicsController.metric / 2);
            
            pDef.Filter.GroupIndex = group_index;
            pDef.IsSensor = isSensor;

            _shapeDef = pDef;
            
            return this;
        }

        public override void Initialized()
        {
        }
    }
}