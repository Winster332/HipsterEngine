using System;
using System.Collections.Generic;
using System.Linq;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using HipsterEngine.Core.Physics.Bodies;
using HipsterEngine.Core.SVG;

namespace HipsterEngine.Core.Physics
{
    public class PhysicsController
    {
        public static float metric = 30f;
        private World world;
        private SolverContacts _solverContacts { get; set; }
        private bool IsInitialized { get; set; }
        private HipsterEngine _engine { get; set; }
        public BodyFactory FactoryBody { get; set; }
        public HipsterEngine GetEngine() => _engine;

        public PhysicsController(HipsterEngine engine)
        {
            _engine = engine;
            _solverContacts = new SolverContacts();
            IsInitialized = false;
            FactoryBody = new BodyFactory(this);
        }

        public SolverContacts GetSolver() => _solverContacts;

        public void Initialize(float x, float y, float w, float h, float grav_x, float grav_y, bool doSleep)
        {
            AABB ab = new AABB();
            ab.LowerBound.Set(x, y);
            ab.UpperBound.Set(w, h);

            world = new World(ab, new Vec2(grav_x, grav_y), doSleep);
            world.SetContactListener(_solverContacts);
            IsInitialized = true;
        }

        public void LoadSvg(string pathToFile)
        {
            var svg = Svg.Load(pathToFile);
            svg.Elements.ForEach(element =>
            {
                if (typeof(SvgGroup) == element.GetType())
                {
                    var group = (SvgGroup) element;
                    var listBody = new List<Body>();

                    group.Elements.ForEach(e =>
                    {
                        if (typeof(SvgRect) == e.GetType())
                        {
                            var rect = (SvgRect) e;

                            Body b = null;
                            if (rect.Transform == null)
                                b = FactoryBody.CreateRigidVertex()
                                    .CreateBox(0.2f, 0.2f, rect.Width, rect.Height, 0.2f)
                                    .CreateBodyDef(rect.X + rect.Width / 2, rect.Y, 0, true, false).Build(1).GetBody();
                            else
                                b = FactoryBody.CreateRigidVertex()
                                    .CreateBox(0.2f, 0.2f, rect.Width, rect.Height, 0.2f)
                                    .CreateBodyDef(rect.X + rect.Width / 2, rect.Y, rect.Transform.GetAngle(), true, false).Build(1).GetBody();
                            listBody.Add(b);
                        }
                    });

                    for (var i = 1; i < listBody.Count; i++)
                    {
                        var pos = listBody.First().GetPosition();
                        pos.X *= PhysicsController.metric;
                        pos.Y *= PhysicsController.metric;
                        
                        AddJoint(listBody.First(), listBody[i], pos.X, pos.Y, false, 0, 0);
                    }
                }

                if (typeof(SvgRect) == element.GetType())
                {
                    var rect = (SvgRect) element;

                    if (rect.Transform == null)
                        FactoryBody.CreateRigidVertex()
                            .CreateBox(0.2f, 0.2f, rect.Width, rect.Height, 0.2f)
                            .CreateBodyDef(rect.X + rect.Width / 2, rect.Y, 0, true, false).Build(1)
                            .GetBody();
                    else
                        FactoryBody.CreateRigidVertex()
                            .CreateBox(0.2f, 0.2f, rect.Width, rect.Height, 0.2f)
                            .CreateBodyDef(rect.X + rect.Width / 2, rect.Y, rect.Transform.GetAngle(), true, false).Build(1)
                            .GetBody();
                }

                if (typeof(SvgPolygon) == element.GetType())
                {
                    var polygon = (SvgPolygon) element;
                    var size = polygon.ExtractSize();

                    var verts = polygon.Points.Select(x => new Vec2(x.X, x.Y)).ToArray();
                    AddVert(0, 0, verts, 0, 0.2f, 0.2f, 0.2f, 1);
                    FactoryBody.CreateRigidVertex()
                        .CreateVertex(0.2f, 0.2f, verts, 0.2f)
                        .CreateBodyDef(0, 0, 0, true, false).Build(1)
                        .GetBody();
                }

                if (typeof(SvgCircle) == element.GetType())
                {
                    var circle = (SvgCircle) element;

                    FactoryBody
                        .CreateRigidCircle()
                        .CreateCircleDef(0.2f, 0.2f, 0.2f, circle.Radius)
                        .CreateBodyDef(circle.X, circle.Y, 0, true, false)
                        .Build(1);
                }
            });
        }

        public Body GetRayBody(float x, float y)
        {
            Body body = null;
            var point = new Vec2(x / metric, y / metric);

            for (var list = world.GetBodyList(); list != null; list = list.GetNext())
            {
                var shape = list.GetShapeList();

                if (shape != null)
                {
                    var insible = shape.TestPoint(list.GetXForm(), point);

                    if (insible)
                    {
                        body = list;
                    }
                }
            }

            return body;
        }

        public void Dispose()
        {
            for (Body list = world.GetBodyList(); list != null; list = list.GetNext())
            {
                world.DestroyBody(list);
            }
            //	world.Dispose();
        }

        public void Step(float dt, int iterat)
        {
            if (IsInitialized)
            {
                world.Step(dt / 20.0f, iterat, iterat);
            }
        }

        public Body AddRect(float x, float y, float w, float h, float angle, float density,
            float friction, float restetution, float mass, bool IsBullet = true,
            bool IsSensor = false, bool AllowSleep = false, short group_index = 1, Object userDate = null)
        {
            BodyDef bDef = new BodyDef();
            bDef.Position.Set(x / metric, y / metric);
            bDef.Angle = angle;
            bDef.AllowSleep = AllowSleep;
            bDef.IsBullet = IsBullet;

            PolygonDef pDef = new PolygonDef();
            pDef.Restitution = restetution;
            pDef.Friction = friction;
            pDef.Density = density;
            pDef.SetAsBox(w / metric / 2, h / metric / 2);
            pDef.Filter.GroupIndex = group_index;
            pDef.IsSensor = IsSensor;

            Body body = world.CreateBody(bDef);
            body.CreateShape(pDef);
            body.SetMassFromShapes();

            float Inertia = body.GetInertia();
            MassData md = new MassData();
            md.I = Inertia;
            md.Mass = mass;
            body.SetMass(md);

            return body;
        }
        
        public void AddCircle(float x, float y, float radius, float angle, float density,
            float friction, float restetution, float mass, bool IsBullet = true,
            bool IsSensor = false, bool AllowSleep = false, short group_index = 1, Object userDate = null)
        {
            BodyDef bDef = new BodyDef();
            bDef.Position.Set(x / metric, y / metric);
            bDef.Angle = angle;
            bDef.AllowSleep = AllowSleep;
            bDef.IsBullet = IsBullet;

            CircleDef pDef = new CircleDef();
            pDef.Restitution = restetution;
            pDef.Friction = friction;
            pDef.Density = density;
            pDef.Radius = radius / metric;
            pDef.Filter.GroupIndex = group_index;
            pDef.IsSensor = IsSensor;

            Body body = world.CreateBody(bDef);
            body.CreateShape(pDef);
            body.SetMassFromShapes();

            float Inertia = body.GetInertia();
            MassData md = new MassData();
            md.I = Inertia;
            md.Mass = mass;
            body.SetMass(md);
        }
        

        public void AddVert(float x, float y, Vec2[] vert, float angle, float density,
            float friction, float restetution, float mass, Vec2 centerMass = new Vec2(), bool IsBullet = true,
            bool IsSensor = false, bool AllowSleep = false, short group_index = 1, Object userDate = null)
        {

            BodyDef bDef = new BodyDef();
            bDef.Position.Set(x / metric, y / metric);
            bDef.Angle = angle;
            bDef.AllowSleep = AllowSleep;
            bDef.IsBullet = IsBullet;

            PolygonDef pDef = new PolygonDef();
            pDef.Restitution = restetution;
            pDef.Friction = friction;
            pDef.Density = density;
            
            pDef.VertexCount = vert.Length;
            for (int i = 0; i < vert.Length; i++)
            {
                vert[i].X /= metric;
                vert[i].Y /= metric;

                pDef.Vertices[i].Set(vert[i].X, vert[i].Y);
            }
            
            pDef.Filter.GroupIndex = group_index;
            pDef.IsSensor = IsSensor;

            Body body = world.CreateBody(bDef);
            body.CreateShape(pDef);
            body.SetMassFromShapes();

            float Inertia = body.GetInertia();
            MassData md = new MassData();
            md.I = Inertia;
            md.Center = body.GetLocalCenter();
            md.Mass = mass;
            body.SetMass(md);
        }
        

        public void RemoveBody(Body body)
        {
            world.DestroyBody(body);
        }
        
        public Joint AddJoint(Body b1, Body b2, float x, float y, bool enableLimit = false)
        {
            RevoluteJointDef jd = new RevoluteJointDef();
            jd.Initialize(b1, b2, new Vec2(x / metric, y / metric));
            jd.EnableLimit = enableLimit;

            Joint joint = world.CreateJoint(jd);

            return joint;
        }

        public World GetWorld()
        {
            return world;
        }

        public Joint AddJoint(Body b1, Body b2, float x, float y, bool collideConnected, bool enableMotor = false,
            float motor_speed = 0, float maxMotorTorque = float.MaxValue)
        {
            RevoluteJointDef jd = new RevoluteJointDef();
            jd.Initialize(b1, b2, new Vec2(x / metric, y / metric));
            jd.CollideConnected = collideConnected;
            jd.EnableMotor = enableMotor;
            jd.MotorSpeed = motor_speed;
            jd.MaxMotorTorque = maxMotorTorque;

            Joint joint = world.CreateJoint(jd);

            return joint;
        }

        public Joint AddJoint(Body b1, Body b2, float x, float y, bool collideConnected, float upperAngle, float lowerAngle, float referenceAngle = 0)
        {
            RevoluteJointDef jd = new RevoluteJointDef();
            jd.Initialize(b1, b2, new Vec2(x / metric, y / metric));
            jd.CollideConnected = collideConnected;
            jd.EnableMotor = true;
            jd.EnableLimit = true;

            jd.LowerAngle = lowerAngle;
            jd.UpperAngle = upperAngle;
		
            jd.ReferenceAngle = referenceAngle;

            Joint joint = world.CreateJoint(jd);

            return joint;
        }
        
        public Joint AddDistanceJoint(Body b1, Body b2, float x1, float y1, float x2, float y2, 
            bool collideConnected = true, float hz = 1f)
        {
            DistanceJointDef jd = new DistanceJointDef();
            jd.Initialize(b1, b2, new Vec2(x1 / metric, y1 / metric), new Vec2(x2 / metric, y2 / metric));
            jd.FrequencyHz = 0.3f;
            jd.CollideConnected = collideConnected;
            jd.FrequencyHz = hz;

            Joint joint = world.CreateJoint(jd);

            return joint;
        }

        public Joint AddDistanceJoint(Body b1, Body b2, float x1, float y1, float x2, float y2, float length,
            bool collideConnected = true, float hz = 1f)
        {
            DistanceJointDef jd = new DistanceJointDef();
            jd.Initialize(b1, b2, new Vec2(x1 / metric, y1 / metric), new Vec2(x2 / metric, y2 / metric));
            jd.FrequencyHz = 0.3f;
            jd.CollideConnected = collideConnected;
            jd.FrequencyHz = hz;
            jd.Length = length;

            Joint joint = world.CreateJoint(jd);

            return joint;
        }
        public Joint AddDistanceJoint(Body b1, Body b2, float x1, float y1, float x2, float y2, float length,
            bool collideConnected = true, float hz = 1f, float dampingRatio = 0)
        {
            DistanceJointDef jd = new DistanceJointDef();
            jd.Initialize(b1, b2, new Vec2(x1 / metric, y1 / metric), new Vec2(x2 / metric, y2 / metric));
            jd.FrequencyHz = 0.3f;
            jd.CollideConnected = collideConnected;
            jd.FrequencyHz = hz;
            jd.Length = length;
            jd.DampingRatio = dampingRatio;

            Joint joint = world.CreateJoint(jd);

            return joint;
        }
        
        public Joint AddJoint(Body b1, Body b2, float x, float y,float ddd)
        {
            RevoluteJointDef jd = new RevoluteJointDef();
            jd.Initialize(b1, b2, new Vec2(x / metric, y / metric));
            Joint joint = world.CreateJoint(jd);
			
            return joint;
        }

        public MouseJoint AddJointMouse(Body b1, Body b2, Vec2 target)
        {
            var mouseJointDef = new MouseJointDef();
            mouseJointDef.Body1 = b1;
            mouseJointDef.Body2 = b2;

            mouseJointDef.MaxForce = 1000 * b2.GetMass();
            mouseJointDef.Target = target;
            mouseJointDef.CollideConnected = true;
            mouseJointDef.DampingRatio = 0;

            var joint = (MouseJoint)world.CreateJoint(mouseJointDef);

            return joint;
        }
        
        public MouseJoint AddJointMouse(Body body, Vec2 target)
        {
            var mouseJointDef = new MouseJointDef();
            mouseJointDef.Body1 = world.GetGroundBody();
            mouseJointDef.Body2 = body;

            mouseJointDef.MaxForce = 1000 * body.GetMass();
            mouseJointDef.Target = target;
            mouseJointDef.CollideConnected = true;
            mouseJointDef.DampingRatio = 0;

            var joint = (MouseJoint)world.CreateJoint(mouseJointDef);

            return joint;
        }
    }
}