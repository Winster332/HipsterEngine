using Box2DX.Collision;
using Box2DX.Common;
using System;

namespace Box2DX.Dynamics
{
  public class World : IDisposable
  {
    internal bool _lock;
    internal BroadPhase _broadPhase;
    private ContactManager _contactManager;
    private Body _bodyList;
    private Joint _jointList;
    private Vec2 _raycastNormal;
    private object _raycastUserData;
    private Segment _raycastSegment;
    private bool _raycastSolidShape;
    internal Contact _contactList;
    private int _bodyCount;
    internal int _contactCount;
    private int _jointCount;
    private Vec2 _gravity;
    private bool _allowSleep;
    private Body _groundBody;
    private DestructionListener _destructionListener;
    private BoundaryListener _boundaryListener;
    internal ContactFilter _contactFilter;
    internal ContactListener _contactListener;
    private DebugDraw _debugDraw;
    private float _inv_dt0;
    private bool _warmStarting;
    private bool _continuousPhysics;

    public Vec2 Gravity
    {
      get
      {
        return this._gravity;
      }
      set
      {
        this._gravity = value;
      }
    }

    public World(AABB worldAABB, Vec2 gravity, bool doSleep)
    {
      this._destructionListener = (DestructionListener) null;
      this._boundaryListener = (BoundaryListener) null;
      this._contactFilter = WorldCallback.DefaultFilter;
      this._contactListener = (ContactListener) null;
      this._debugDraw = (DebugDraw) null;
      this._bodyList = (Body) null;
      this._contactList = (Contact) null;
      this._jointList = (Joint) null;
      this._bodyCount = 0;
      this._contactCount = 0;
      this._jointCount = 0;
      this._warmStarting = true;
      this._continuousPhysics = true;
      this._allowSleep = doSleep;
      this._gravity = gravity;
      this._lock = false;
      this._inv_dt0 = 0.0f;
      this._contactManager = new ContactManager();
      this._contactManager._world = this;
      this._broadPhase = new BroadPhase(worldAABB, (PairCallback) this._contactManager);
      this._groundBody = this.CreateBody(new BodyDef());
    }

    public void Dispose()
    {
      this.DestroyBody(this._groundBody);
      if (this._broadPhase is IDisposable)
        (this._broadPhase as IDisposable).Dispose();
      this._broadPhase = (BroadPhase) null;
    }

    public void SetDestructionListener(DestructionListener listener)
    {
      this._destructionListener = listener;
    }

    public void SetBoundaryListener(BoundaryListener listener)
    {
      this._boundaryListener = listener;
    }

    public void SetContactFilter(ContactFilter filter)
    {
      this._contactFilter = filter;
    }

    public void SetContactListener(ContactListener listener)
    {
      this._contactListener = listener;
    }

    public void SetDebugDraw(DebugDraw debugDraw)
    {
      this._debugDraw = debugDraw;
    }

    public Body CreateBody(BodyDef def)
    {
      Box2DXDebug.Assert(!this._lock);
      if (this._lock)
        return (Body) null;
      Body body = new Body(def, this);
      body._prev = (Body) null;
      body._next = this._bodyList;
      if (this._bodyList != null)
        this._bodyList._prev = body;
      this._bodyList = body;
      ++this._bodyCount;
      return body;
    }

    public void DestroyBody(Body b)
    {
      Box2DXDebug.Assert(this._bodyCount > 0);
      Box2DXDebug.Assert(!this._lock);
      if (this._lock)
        return;
      JointEdge jointEdge1 = (JointEdge) null;
      if (b._jointList != null)
        jointEdge1 = b._jointList;
      while (jointEdge1 != null)
      {
        JointEdge jointEdge2 = jointEdge1;
        jointEdge1 = jointEdge1.Next;
        if (this._destructionListener != null)
          this._destructionListener.SayGoodbye(jointEdge2.Joint);
        this.DestroyJoint(jointEdge2.Joint);
      }
      Shape shape1 = (Shape) null;
      if (b._shapeList != null)
        shape1 = b._shapeList;
      while (shape1 != null)
      {
        Shape shape2 = shape1;
        shape1 = shape1._next;
        if (this._destructionListener != null)
          this._destructionListener.SayGoodbye(shape2);
        shape2.DestroyProxy(this._broadPhase);
        Shape.Destroy(shape2);
      }
      if (b._prev != null)
        b._prev._next = b._next;
      if (b._next != null)
        b._next._prev = b._prev;
      if (b == this._bodyList)
        this._bodyList = b._next;
      --this._bodyCount;
      b?.Dispose();
      b = (Body) null;
    }

    public Joint CreateJoint(JointDef def)
    {
      Box2DXDebug.Assert(!this._lock);
      Joint joint = Joint.Create(def);
      joint._prev = (Joint) null;
      joint._next = this._jointList;
      if (this._jointList != null)
        this._jointList._prev = joint;
      this._jointList = joint;
      ++this._jointCount;
      joint._node1.Joint = joint;
      joint._node1.Other = joint._body2;
      joint._node1.Prev = (JointEdge) null;
      joint._node1.Next = joint._body1._jointList;
      if (joint._body1._jointList != null)
        joint._body1._jointList.Prev = joint._node1;
      joint._body1._jointList = joint._node1;
      joint._node2.Joint = joint;
      joint._node2.Other = joint._body1;
      joint._node2.Prev = (JointEdge) null;
      joint._node2.Next = joint._body2._jointList;
      if (joint._body2._jointList != null)
        joint._body2._jointList.Prev = joint._node2;
      joint._body2._jointList = joint._node2;
      if (!def.CollideConnected)
      {
        Body body = def.Body1._shapeCount < def.Body2._shapeCount ? def.Body1 : def.Body2;
        for (Shape shape = body._shapeList; shape != null; shape = shape._next)
          shape.RefilterProxy(this._broadPhase, body.GetXForm());
      }
      return joint;
    }

    public void DestroyJoint(Joint j)
    {
      Box2DXDebug.Assert(!this._lock);
      bool collideConnected = j._collideConnected;
      if (j._prev != null)
        j._prev._next = j._next;
      if (j._next != null)
        j._next._prev = j._prev;
      if (j == this._jointList)
        this._jointList = j._next;
      Body body1 = j._body1;
      Body body2 = j._body2;
      body1.WakeUp();
      body2.WakeUp();
      if (j._node1.Prev != null)
        j._node1.Prev.Next = j._node1.Next;
      if (j._node1.Next != null)
        j._node1.Next.Prev = j._node1.Prev;
      if (j._node1 == body1._jointList)
        body1._jointList = j._node1.Next;
      j._node1.Prev = (JointEdge) null;
      j._node1.Next = (JointEdge) null;
      if (j._node2.Prev != null)
        j._node2.Prev.Next = j._node2.Next;
      if (j._node2.Next != null)
        j._node2.Next.Prev = j._node2.Prev;
      if (j._node2 == body2._jointList)
        body2._jointList = j._node2.Next;
      j._node2.Prev = (JointEdge) null;
      j._node2.Next = (JointEdge) null;
      Joint.Destroy(j);
      Box2DXDebug.Assert(this._jointCount > 0);
      --this._jointCount;
      if (collideConnected)
        return;
      Body body = body1._shapeCount < body2._shapeCount ? body1 : body2;
      for (Shape shape = body._shapeList; shape != null; shape = shape._next)
        shape.RefilterProxy(this._broadPhase, body.GetXForm());
    }

    public Body GetGroundBody()
    {
      return this._groundBody;
    }

    public Body GetBodyList()
    {
      return this._bodyList;
    }

    public Joint GetJointList()
    {
      return this._jointList;
    }

    public void Refilter(Shape shape)
    {
      Box2DXDebug.Assert(!this._lock);
      shape.RefilterProxy(this._broadPhase, shape.GetBody().GetXForm());
    }

    public void SetWarmStarting(bool flag)
    {
      this._warmStarting = flag;
    }

    public void SetContinuousPhysics(bool flag)
    {
      this._continuousPhysics = flag;
    }

    public void Validate()
    {
      this._broadPhase.Validate();
    }

    public int GetProxyCount()
    {
      return this._broadPhase._proxyCount;
    }

    public int GetPairCount()
    {
      return this._broadPhase._pairManager._pairCount;
    }

    public int GetBodyCount()
    {
      return this._bodyCount;
    }

    public int GetJointCount()
    {
      return this._jointCount;
    }

    public int GetContactCount()
    {
      return this._contactCount;
    }

    public void Step(float dt, int velocityIterations, int positionIteration)
    {
      this._lock = true;
      TimeStep step = new TimeStep();
      step.Dt = dt;
      step.VelocityIterations = velocityIterations;
      step.PositionIterations = positionIteration;
      step.Inv_Dt = (double) dt <= 0.0 ? 0.0f : 1f / dt;
      step.DtRatio = this._inv_dt0 * dt;
      step.WarmStarting = this._warmStarting;
      this._contactManager.Collide();
      if ((double) step.Dt > 0.0)
        this.Solve(step);
      if (this._continuousPhysics && (double) step.Dt > 0.0)
        this.SolveTOI(step);
      this.DrawDebugData();
      this._inv_dt0 = step.Inv_Dt;
      this._lock = false;
    }

    public int Query(AABB aabb, Shape[] shapes, int maxCount)
    {
      object[] userData = new object[maxCount];
      int num = this._broadPhase.Query(aabb, userData, maxCount);
      for (int index = 0; index < num; ++index)
        shapes[index] = (Shape) userData[index];
      return num;
    }

    public int Raycast(Segment segment, Shape[] shapes, int maxCount, bool solidShapes, object userData)
    {
      this._raycastSegment = segment;
      this._raycastUserData = userData;
      this._raycastSolidShape = solidShapes;
      object[] userData1 = new object[maxCount];
      int num = this._broadPhase.QuerySegment(segment, userData1, maxCount, new SortKeyFunc(World.RaycastSortKey));
      for (int index = 0; index < num; ++index)
        shapes[index] = (Shape) userData1[index];
      return num;
    }

    public Shape RaycastOne(Segment segment, out float lambda, out Vec2 normal, bool solidShapes, object userData)
    {
      lambda = 0.0f;
      normal = new Vec2(0.0f, 0.0f);
      int maxCount = 1;
      Shape[] shapes = new Shape[maxCount];
      int num1 = this.Raycast(segment, shapes, maxCount, solidShapes, userData);
      if (num1 == 0)
        return (Shape) null;
      Box2DXDebug.Assert(num1 == 1);
      XForm xform = shapes[0].GetBody().GetXForm();
      int num2 = (int) shapes[0].TestSegment(xform, out lambda, out normal, segment, 1f);
      return shapes[0];
    }

    private void Solve(TimeStep step)
    {
      Island island = new Island(this._bodyCount, this._contactCount, this._jointCount, this._contactListener);
      for (Body body = this._bodyList; body != null; body = body._next)
        body._flags &= ~Body.BodyFlags.Island;
      for (Contact contact = this._contactList; contact != null; contact = contact._next)
        contact._flags &= ~Contact.CollisionFlags.Island;
      for (Joint joint = this._jointList; joint != null; joint = joint._next)
        joint._islandFlag = false;
      int bodyCount = this._bodyCount;
      Body[] bodyArray1 = new Body[bodyCount];
      for (Body body1 = this._bodyList; body1 != null; body1 = body1._next)
      {
        if ((body1._flags & (Body.BodyFlags.Frozen | Body.BodyFlags.Island | Body.BodyFlags.Sleep)) == (Body.BodyFlags) 0 && !body1.IsStatic())
        {
          island.Clear();
          int num1 = 0;
          Body[] bodyArray2 = bodyArray1;
          int index1 = num1;
          int num2 = index1 + 1;
          Body body2 = body1;
          bodyArray2[index1] = body2;
          body1._flags |= Body.BodyFlags.Island;
          while (num2 > 0)
          {
            Body body3 = bodyArray1[--num2];
            island.Add(body3);
            body3._flags &= ~Body.BodyFlags.Sleep;
            if (!body3.IsStatic())
            {
              for (ContactEdge contactEdge = body3._contactList; contactEdge != null; contactEdge = contactEdge.Next)
              {
                if ((contactEdge.Contact._flags & (Contact.CollisionFlags.NonSolid | Contact.CollisionFlags.Island)) == (Contact.CollisionFlags) 0 && contactEdge.Contact.GetManifoldCount() != 0)
                {
                  island.Add(contactEdge.Contact);
                  contactEdge.Contact._flags |= Contact.CollisionFlags.Island;
                  Body other = contactEdge.Other;
                  if ((other._flags & Body.BodyFlags.Island) == (Body.BodyFlags) 0)
                  {
                    Box2DXDebug.Assert(num2 < bodyCount);
                    bodyArray1[num2++] = other;
                    other._flags |= Body.BodyFlags.Island;
                  }
                }
              }
              for (JointEdge jointEdge = body3._jointList; jointEdge != null; jointEdge = jointEdge.Next)
              {
                if (!jointEdge.Joint._islandFlag)
                {
                  island.Add(jointEdge.Joint);
                  jointEdge.Joint._islandFlag = true;
                  Body other = jointEdge.Other;
                  if ((other._flags & Body.BodyFlags.Island) == (Body.BodyFlags) 0)
                  {
                    Box2DXDebug.Assert(num2 < bodyCount);
                    bodyArray1[num2++] = other;
                    other._flags |= Body.BodyFlags.Island;
                  }
                }
              }
            }
          }
          island.Solve(step, this._gravity, this._allowSleep);
          for (int index2 = 0; index2 < island._bodyCount; ++index2)
          {
            Body body3 = island._bodies[index2];
            if (body3.IsStatic())
              body3._flags &= ~Body.BodyFlags.Island;
          }
        }
      }
      for (Body body = this._bodyList; body != null; body = body.GetNext())
      {
        if ((body._flags & (Body.BodyFlags.Frozen | Body.BodyFlags.Sleep)) == (Body.BodyFlags) 0 && !body.IsStatic() && (!body.SynchronizeShapes() && this._boundaryListener != null))
          this._boundaryListener.Violation(body);
      }
      this._broadPhase.Commit();
    }

    private void SolveTOI(TimeStep step)
    {
      Island island = new Island(this._bodyCount, Settings.MaxTOIContactsPerIsland, Settings.MaxTOIJointsPerIsland, this._contactListener);
      int bodyCount = this._bodyCount;
      Body[] bodyArray1 = new Body[bodyCount];
      for (Body body = this._bodyList; body != null; body = body._next)
      {
        body._flags &= ~Body.BodyFlags.Island;
        body._sweep.T0 = 0.0f;
      }
      for (Contact contact = this._contactList; contact != null; contact = contact._next)
        contact._flags &= ~(Contact.CollisionFlags.Island | Contact.CollisionFlags.Toi);
      while (true)
      {
        Contact contact1;
        float t;
        Body body1;
        Body body2;
        do
        {
          contact1 = (Contact) null;
          t = 1f;
          for (Contact contact2 = this._contactList; contact2 != null; contact2 = contact2._next)
          {
            if ((contact2._flags & (Contact.CollisionFlags.NonSolid | Contact.CollisionFlags.Slow)) == (Contact.CollisionFlags) 0)
            {
              float num;
              if ((contact2._flags & Contact.CollisionFlags.Toi) != (Contact.CollisionFlags) 0)
              {
                num = contact2._toi;
              }
              else
              {
                Shape shape1 = contact2.GetShape1();
                Shape shape2 = contact2.GetShape2();
                Body body3 = shape1.GetBody();
                Body body4 = shape2.GetBody();
                if (!body3.IsStatic() && !body3.IsSleeping() || !body4.IsStatic() && !body4.IsSleeping())
                {
                  float t0 = body3._sweep.T0;
                  if ((double) body3._sweep.T0 < (double) body4._sweep.T0)
                  {
                    t0 = body4._sweep.T0;
                    body3._sweep.Advance(t0);
                  }
                  else if ((double) body4._sweep.T0 < (double) body3._sweep.T0)
                  {
                    t0 = body3._sweep.T0;
                    body4._sweep.Advance(t0);
                  }
                  Box2DXDebug.Assert((double) t0 < 1.0);
                  num = Box2DX.Collision.Collision.TimeOfImpact(contact2._shape1, body3._sweep, contact2._shape2, body4._sweep);
                  Box2DXDebug.Assert(0.0 <= (double) num && (double) num <= 1.0);
                  if ((double) num > 0.0 && (double) num < 1.0)
                    num = Box2DX.Common.Math.Min((1f - num) * t0 + num, 1f);
                  contact2._toi = num;
                  contact2._flags |= Contact.CollisionFlags.Toi;
                }
                else
                  continue;
              }
              if ((double) Settings.FLT_EPSILON < (double) num && (double) num < (double) t)
              {
                contact1 = contact2;
                t = num;
              }
            }
          }
          if (contact1 != null && 1.0 - 100.0 * (double) Settings.FLT_EPSILON >= (double) t)
          {
            Shape shape1 = contact1.GetShape1();
            Shape shape2 = contact1.GetShape2();
            body1 = shape1.GetBody();
            body2 = shape2.GetBody();
            body1.Advance(t);
            body2.Advance(t);
            contact1.Update(this._contactListener);
            contact1._flags &= ~Contact.CollisionFlags.Toi;
          }
          else
            goto label_54;
        }
        while (contact1.GetManifoldCount() == 0);
        Body body5 = body1;
        if (body5.IsStatic())
          body5 = body2;
        island.Clear();
        int num1 = 0;
        int num2 = 0;
        Body[] bodyArray2 = bodyArray1;
        int num3 = num1;
        int num4 = num2;
        int num5 = num4 + 1;
        int index1 = num3 + num4;
        Body body6 = body5;
        bodyArray2[index1] = body6;
        body5._flags |= Body.BodyFlags.Island;
        while (num5 > 0)
        {
          Body body3 = bodyArray1[num1++];
          --num5;
          island.Add(body3);
          body3._flags &= ~Body.BodyFlags.Sleep;
          if (!body3.IsStatic())
          {
            for (ContactEdge contactEdge = body3._contactList; contactEdge != null; contactEdge = contactEdge.Next)
            {
              if (island._contactCount != island._contactCapacity && (contactEdge.Contact._flags & (Contact.CollisionFlags.NonSolid | Contact.CollisionFlags.Slow | Contact.CollisionFlags.Island)) == (Contact.CollisionFlags) 0 && contactEdge.Contact.GetManifoldCount() != 0)
              {
                island.Add(contactEdge.Contact);
                contactEdge.Contact._flags |= Contact.CollisionFlags.Island;
                Body other = contactEdge.Other;
                if ((other._flags & Body.BodyFlags.Island) == (Body.BodyFlags) 0)
                {
                  if (!other.IsStatic())
                  {
                    other.Advance(t);
                    other.WakeUp();
                  }
                  Box2DXDebug.Assert(num1 + num5 < bodyCount);
                  bodyArray1[num1 + num5++] = other;
                  other._flags |= Body.BodyFlags.Island;
                }
              }
            }
          }
        }
        TimeStep subStep = new TimeStep();
        subStep.WarmStarting = false;
        subStep.Dt = (1f - t) * step.Dt;
        Box2DXDebug.Assert((double) subStep.Dt > (double) Settings.FLT_EPSILON);
        subStep.Inv_Dt = 1f / subStep.Dt;
        subStep.VelocityIterations = step.VelocityIterations;
        subStep.PositionIterations = step.PositionIterations;
        island.SolveTOI(ref subStep);
        for (int index2 = 0; index2 < island._bodyCount; ++index2)
        {
          Body body3 = island._bodies[index2];
          body3._flags &= ~Body.BodyFlags.Island;
          if ((body3._flags & (Body.BodyFlags.Frozen | Body.BodyFlags.Sleep)) == (Body.BodyFlags) 0 && !body3.IsStatic())
          {
            if (!body3.SynchronizeShapes() && this._boundaryListener != null)
              this._boundaryListener.Violation(body3);
            for (ContactEdge contactEdge = body3._contactList; contactEdge != null; contactEdge = contactEdge.Next)
              contactEdge.Contact._flags &= ~Contact.CollisionFlags.Toi;
          }
        }
        for (int index2 = 0; index2 < island._contactCount; ++index2)
          island._contacts[index2]._flags &= ~(Contact.CollisionFlags.Island | Contact.CollisionFlags.Toi);
        for (int index2 = 0; index2 < island._jointCount; ++index2)
          island._joints[index2]._islandFlag = false;
        this._broadPhase.Commit();
      }
label_54:;
    }

    private void DrawJoint(Joint joint)
    {
      Body body1 = joint.GetBody1();
      Body body2 = joint.GetBody2();
      XForm xform1 = body1.GetXForm();
      XForm xform2 = body2.GetXForm();
      Vec2 position1 = xform1.Position;
      Vec2 position2 = xform2.Position;
      Vec2 anchor1 = joint.Anchor1;
      Vec2 anchor2 = joint.Anchor2;
      Color color = new Color(0.5f, 0.8f, 0.8f);
      switch (joint.GetType())
      {
        case JointType.DistanceJoint:
          this._debugDraw.DrawSegment(anchor1, anchor2, color);
          break;
        case JointType.PulleyJoint:
          PulleyJoint pulleyJoint = (PulleyJoint) joint;
          Vec2 groundAnchor1 = pulleyJoint.GroundAnchor1;
          Vec2 groundAnchor2 = pulleyJoint.GroundAnchor2;
          this._debugDraw.DrawSegment(groundAnchor1, anchor1, color);
          this._debugDraw.DrawSegment(groundAnchor2, anchor2, color);
          this._debugDraw.DrawSegment(groundAnchor1, groundAnchor2, color);
          break;
        case JointType.MouseJoint:
          break;
        default:
          this._debugDraw.DrawSegment(position1, anchor1, color);
          this._debugDraw.DrawSegment(anchor1, anchor2, color);
          this._debugDraw.DrawSegment(position2, anchor2, color);
          break;
      }
    }

    private void DrawShape(Shape shape, XForm xf, Color color, bool core)
    {
      Color color1 = new Color(0.9f, 0.6f, 0.6f);
      switch (shape.GetType())
      {
        case ShapeType.CircleShape:
          CircleShape circleShape = (CircleShape) shape;
          Vec2 center = Box2DX.Common.Math.Mul(xf, circleShape.GetLocalPosition());
          float radius = circleShape.GetRadius();
          Vec2 col1 = xf.R.Col1;
          this._debugDraw.DrawSolidCircle(center, radius, col1, color);
          if (!core)
            break;
          this._debugDraw.DrawCircle(center, radius - Settings.ToiSlop, color1);
          break;
        case ShapeType.PolygonShape:
          PolygonShape polygonShape = (PolygonShape) shape;
          int vertexCount = polygonShape.VertexCount;
          Vec2[] vertices1 = polygonShape.GetVertices();
          Box2DXDebug.Assert(vertexCount <= Settings.MaxPolygonVertices);
          Vec2[] vertices2 = new Vec2[Settings.MaxPolygonVertices];
          for (int index = 0; index < vertexCount; ++index)
            vertices2[index] = Box2DX.Common.Math.Mul(xf, vertices1[index]);
          this._debugDraw.DrawSolidPolygon(vertices2, vertexCount, color);
          if (!core)
            break;
          Vec2[] coreVertices = polygonShape.GetCoreVertices();
          for (int index = 0; index < vertexCount; ++index)
            vertices2[index] = Box2DX.Common.Math.Mul(xf, coreVertices[index]);
          this._debugDraw.DrawPolygon(vertices2, vertexCount, color1);
          break;
      }
    }

    private void DrawDebugData()
    {
      if (this._debugDraw == null)
        return;
      DebugDraw.DrawFlags flags = this._debugDraw.Flags;
      if ((flags & DebugDraw.DrawFlags.Shape) != (DebugDraw.DrawFlags) 0)
      {
        bool core = (flags & DebugDraw.DrawFlags.CoreShape) == DebugDraw.DrawFlags.CoreShape;
        for (Body body = this._bodyList; body != null; body = body.GetNext())
        {
          XForm xform = body.GetXForm();
          for (Shape shape = body.GetShapeList(); shape != null; shape = shape.GetNext())
          {
            if (body.IsStatic())
              this.DrawShape(shape, xform, new Color(0.5f, 0.9f, 0.5f), core);
            else if (body.IsSleeping())
              this.DrawShape(shape, xform, new Color(0.5f, 0.5f, 0.9f), core);
            else
              this.DrawShape(shape, xform, new Color(0.9f, 0.9f, 0.9f), core);
          }
        }
      }
      if ((flags & DebugDraw.DrawFlags.Joint) != (DebugDraw.DrawFlags) 0)
      {
        for (Joint joint = this._jointList; joint != null; joint = joint.GetNext())
        {
          if (joint.GetType() != JointType.MouseJoint)
            this.DrawJoint(joint);
        }
      }
      Vec2 vec2;
      Color color;
      if ((flags & DebugDraw.DrawFlags.Pair) != (DebugDraw.DrawFlags) 0)
      {
        BroadPhase broadPhase = this._broadPhase;
        vec2 = new Vec2();
        vec2.Set(1f / broadPhase._quantizationFactor.X, 1f / broadPhase._quantizationFactor.Y);
        color = new Color(0.9f, 0.9f, 0.3f);
        Pair pair;
        for (int index = 0; index < PairManager.TableCapacity; ++index)
        {
          for (ushort next = broadPhase._pairManager._hashTable[index]; (int) next != (int) PairManager.NullPair; next = pair.Next)
          {
            pair = broadPhase._pairManager._pairs[(int) next];
            Proxy proxy1 = broadPhase._proxyPool[(int) pair.ProxyId1];
            Proxy proxy2 = broadPhase._proxyPool[(int) pair.ProxyId2];
            AABB aabb1 = new AABB();
            AABB aabb2 = new AABB();
            aabb1.LowerBound.X = broadPhase._worldAABB.LowerBound.X + vec2.X * (float) broadPhase._bounds[0][(int) proxy1.LowerBounds[0]].Value;
            aabb1.LowerBound.Y = broadPhase._worldAABB.LowerBound.Y + vec2.Y * (float) broadPhase._bounds[1][(int) proxy1.LowerBounds[1]].Value;
            aabb1.UpperBound.X = broadPhase._worldAABB.LowerBound.X + vec2.X * (float) broadPhase._bounds[0][(int) proxy1.UpperBounds[0]].Value;
            aabb1.UpperBound.Y = broadPhase._worldAABB.LowerBound.Y + vec2.Y * (float) broadPhase._bounds[1][(int) proxy1.UpperBounds[1]].Value;
            aabb2.LowerBound.X = broadPhase._worldAABB.LowerBound.X + vec2.X * (float) broadPhase._bounds[0][(int) proxy2.LowerBounds[0]].Value;
            aabb2.LowerBound.Y = broadPhase._worldAABB.LowerBound.Y + vec2.Y * (float) broadPhase._bounds[1][(int) proxy2.LowerBounds[1]].Value;
            aabb2.UpperBound.X = broadPhase._worldAABB.LowerBound.X + vec2.X * (float) broadPhase._bounds[0][(int) proxy2.UpperBounds[0]].Value;
            aabb2.UpperBound.Y = broadPhase._worldAABB.LowerBound.Y + vec2.Y * (float) broadPhase._bounds[1][(int) proxy2.UpperBounds[1]].Value;
            this._debugDraw.DrawSegment(0.5f * (aabb1.LowerBound + aabb1.UpperBound), 0.5f * (aabb2.LowerBound + aabb2.UpperBound), color);
          }
        }
      }
      if ((flags & DebugDraw.DrawFlags.Aabb) != (DebugDraw.DrawFlags) 0)
      {
        BroadPhase broadPhase = this._broadPhase;
        Vec2 lowerBound = broadPhase._worldAABB.LowerBound;
        Vec2 upperBound = broadPhase._worldAABB.UpperBound;
        vec2 = new Vec2();
        vec2.Set(1f / broadPhase._quantizationFactor.X, 1f / broadPhase._quantizationFactor.Y);
        color = new Color(0.9f, 0.3f, 0.9f);
        for (int index = 0; index < Settings.MaxProxies; ++index)
        {
          Proxy proxy = broadPhase._proxyPool[index];
          if (proxy.IsValid)
          {
            AABB aabb = new AABB();
            aabb.LowerBound.X = lowerBound.X + vec2.X * (float) broadPhase._bounds[0][(int) proxy.LowerBounds[0]].Value;
            aabb.LowerBound.Y = lowerBound.Y + vec2.Y * (float) broadPhase._bounds[1][(int) proxy.LowerBounds[1]].Value;
            aabb.UpperBound.X = lowerBound.X + vec2.X * (float) broadPhase._bounds[0][(int) proxy.UpperBounds[0]].Value;
            aabb.UpperBound.Y = lowerBound.Y + vec2.Y * (float) broadPhase._bounds[1][(int) proxy.UpperBounds[1]].Value;
            Vec2[] vertices = new Vec2[4];
            vertices[0].Set(aabb.LowerBound.X, aabb.LowerBound.Y);
            vertices[1].Set(aabb.UpperBound.X, aabb.LowerBound.Y);
            vertices[2].Set(aabb.UpperBound.X, aabb.UpperBound.Y);
            vertices[3].Set(aabb.LowerBound.X, aabb.UpperBound.Y);
            this._debugDraw.DrawPolygon(vertices, 4, color);
          }
        }
        Vec2[] vertices1 = new Vec2[4];
        vertices1[0].Set(lowerBound.X, lowerBound.Y);
        vertices1[1].Set(upperBound.X, lowerBound.Y);
        vertices1[2].Set(upperBound.X, upperBound.Y);
        vertices1[3].Set(lowerBound.X, upperBound.Y);
        this._debugDraw.DrawPolygon(vertices1, 4, new Color(0.3f, 0.9f, 0.9f));
      }
      if ((flags & DebugDraw.DrawFlags.Obb) != (DebugDraw.DrawFlags) 0)
      {
        color = new Color(0.5f, 0.3f, 0.5f);
        for (Body body = this._bodyList; body != null; body = body.GetNext())
        {
          XForm xform = body.GetXForm();
          for (Shape shape = body.GetShapeList(); shape != null; shape = shape.GetNext())
          {
            if (shape.GetType() == ShapeType.PolygonShape)
            {
              OBB obb = ((PolygonShape) shape).GetOBB();
              Vec2 extents = obb.Extents;
              Vec2[] vertices = new Vec2[4];
              vertices[0].Set(-extents.X, -extents.Y);
              vertices[1].Set(extents.X, -extents.Y);
              vertices[2].Set(extents.X, extents.Y);
              vertices[3].Set(-extents.X, extents.Y);
              for (int index = 0; index < 4; ++index)
              {
                vertices[index] = obb.Center + Box2DX.Common.Math.Mul(obb.R, vertices[index]);
                vertices[index] = Box2DX.Common.Math.Mul(xform, vertices[index]);
              }
              this._debugDraw.DrawPolygon(vertices, 4, color);
            }
          }
        }
      }
      if ((flags & DebugDraw.DrawFlags.CenterOfMass) == (DebugDraw.DrawFlags) 0)
        return;
      for (Body body = this._bodyList; body != null; body = body.GetNext())
      {
        XForm xform = body.GetXForm();
        xform.Position = body.GetWorldCenter();
        this._debugDraw.DrawXForm(xform);
      }
    }

    private static float RaycastSortKey(object data)
    {
      Shape shape = data as Shape;
      Box2DXDebug.Assert(shape != null);
      Body body = shape.GetBody();
      World world = body.GetWorld();
      XForm xform = body.GetXForm();
      if (world._contactFilter != null && !world._contactFilter.RayCollide(world._raycastUserData, shape))
        return -1f;
      float lambda;
      SegmentCollide segmentCollide = shape.TestSegment(xform, out lambda, out world._raycastNormal, world._raycastSegment, 1f);
      if (world._raycastSolidShape && segmentCollide == SegmentCollide.MissCollide || !world._raycastSolidShape && segmentCollide != SegmentCollide.HitCollide)
        return -1f;
      return lambda;
    }

    public bool InRange(AABB aabb)
    {
      return this._broadPhase.InRange(aabb);
    }
  }
}