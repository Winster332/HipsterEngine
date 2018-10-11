using System;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using HipsterEngine.Core.Physics.Bodies.Events;
using Math = System.Math;

namespace HipsterEngine.Core.Physics.Bodies
{
    public abstract class RigidBody
    {
        public event RigidBodyContactEventHandler ContactAdd;
        public Guid Id { get; set; }
        protected Body _body;
        protected BodyDef _bodyDef;
        protected PhysicsController Physics;
        protected ShapeDef _shapeDef;
        
        public RigidBody(PhysicsController physics)
        {
            Physics = physics;
            Id = Guid.NewGuid();

            Physics.GetSolver().onAdd += point =>
            {
                ContactAdd?.Invoke(point);
            };
        }

        public Joint JointDistance(RigidBody body, float x1, float y1, float x2, float y2, bool isCollide, float hz, float length, float dampingRatio = 0)
        {
            var p1 = GetBody().GetPosition() + new Vec2(x1 / PhysicsController.metric, y1 / PhysicsController.metric);
            var p2 = body.GetBody().GetPosition() +
                     new Vec2(x2 / PhysicsController.metric, y2 / PhysicsController.metric);
                     
            DistanceJointDef jd = new DistanceJointDef();
            jd.Initialize(GetBody(), body.GetBody(), p1, p2);
            jd.FrequencyHz = hz;
            jd.CollideConnected = isCollide;
            jd.Length = length;
            jd.DampingRatio = dampingRatio;

            Joint joint = Physics.GetWorld().CreateJoint(jd);

            return joint;
        }
        
        public Joint JointRevolute(RigidBody body, float x, float y, bool isCollide, bool enableMotor, bool enableLimitMotor)
        {
            var p = GetBody().GetPosition() + new Vec2(x / PhysicsController.metric, y / PhysicsController.metric);
            
            RevoluteJointDef jd = new RevoluteJointDef();
            jd.Initialize(GetBody(), body.GetBody(), p);
            jd.CollideConnected = isCollide;
            jd.EnableMotor = enableMotor;
            jd.EnableLimit = enableLimitMotor;

            Joint joint = Physics.GetWorld().CreateJoint(jd);

            return joint;
        }

        public Joint JointRevolute(RigidBody body, float x, float y, bool isCollide, float lowerAngle, float upperAngle,
            float referenceAngle)
        {
            var p = GetBody().GetPosition() + new Vec2(x / PhysicsController.metric, y / PhysicsController.metric);
            
            RevoluteJointDef jd = new RevoluteJointDef();
            jd.Initialize(GetBody(), body.GetBody(), p);
            jd.CollideConnected = isCollide;
            jd.EnableMotor = true;
            jd.EnableLimit = true;

            jd.LowerAngle = lowerAngle / 180 * (float)Math.PI;
            jd.UpperAngle = upperAngle / 180 * (float)Math.PI;
            
            jd.ReferenceAngle = referenceAngle;

            Joint joint = Physics.GetWorld().CreateJoint(jd);

            return joint;
        }
        
        public Joint JointRevolute(RigidBody body, float x, float y, bool isCollide, float lowerAngle, float upperAngle,
            float referenceAngle, float x1, float y1, float x2, float y2)
        {
            var p = GetBody().GetPosition() + new Vec2(x / PhysicsController.metric, y / PhysicsController.metric);
            
            var p1 = new Vec2(x1 / PhysicsController.metric, y1 / PhysicsController.metric);
            var p2 = new Vec2(x2 / PhysicsController.metric, y2 / PhysicsController.metric);
            
            RevoluteJointDef jd = new RevoluteJointDef();
            jd.Initialize(GetBody(), body.GetBody(), p);
            jd.CollideConnected = isCollide;
            jd.EnableMotor = true;
            jd.EnableLimit = true;

            jd.LowerAngle = lowerAngle / 180 * (float)Math.PI;
            jd.UpperAngle = upperAngle / 180 * (float)Math.PI;
            
            jd.LocalAnchor1 = p1;
            jd.LocalAnchor2 = p2;
		
            jd.ReferenceAngle = referenceAngle;

            Joint joint = Physics.GetWorld().CreateJoint(jd);

            return joint;
        }

        public float GetX() => _body.GetPosition().X * PhysicsController.metric;
        public float GetY() => _body.GetPosition().Y * PhysicsController.metric;

        public RigidBody Build(float mass)
        {
            _body = Physics.GetWorld().CreateBody(_bodyDef);
            _body.CreateShape(_shapeDef);
            _body.SetMassFromShapes();

            float Inertia = _body.GetInertia();
            MassData md = new MassData();
            md.I = Inertia;
            md.Mass = mass;
            _body.SetMass(md);
            _body.SetUserData(this);

            Initialized();
            return this;
        }
        
        public RigidBody CreateVertex(float restetution, float friction, Vec2[] vert, float density, bool isSensor = false, short group_index = 1)
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
        public RigidBody CreateBodyDef(float x, float y, float angle, bool allowSleep, bool isBullet)
        {
            _bodyDef = new BodyDef();
            _bodyDef.Position.Set(x / PhysicsController.metric, y / PhysicsController.metric);
            _bodyDef.Angle = angle;
            _bodyDef.AllowSleep = allowSleep;
            _bodyDef.IsBullet = isBullet;

            return this;
        }
        
        public RigidBody CreateBox(float restetution, float friction, float width, float height, float density, bool isSensor = false, short group_index = 1)
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

        public void SetMass(float mass)
        {
        }

        public abstract void Initialized();

        public Body GetBody()
        {
            return _body;
        }
    }
}

