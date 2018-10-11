using Box2DX.Collision;

namespace HipsterEngine.Core.Physics.Bodies
{
    public class RigidBodyCircle : RigidBody
    {
        public RigidBodyCircle(PhysicsController physics) : base(physics)
        {
        }
        
        public RigidBody CreateCircleDef(float restetution, float friction, float density, 
            float radius, short group_index = 1, bool isSensor = false)
        {
            var pDef = new CircleDef();
            pDef.Restitution = restetution;
            pDef.Friction = friction;
            pDef.Density = density;
            pDef.Radius = radius / PhysicsController.metric;
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