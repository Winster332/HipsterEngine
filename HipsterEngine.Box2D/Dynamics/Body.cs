// Decompiled with JetBrains decompiler
// Type: Box2DX.Dynamics.Body
// Assembly: Box2DX, Version=2.0.1.28279, Culture=neutral, PublicKeyToken=null
// MVID: BD116DE6-476A-4E37-B8FC-78B19E349C03
// Assembly location: /Users/kusy/Documents/Новая папка/HipsterEngine/bin/Debug/Box2DX.dll

using Box2DX.Collision;
using Box2DX.Common;
using System;

namespace Box2DX.Dynamics
{
  public class Body : IDisposable
  {
    internal Body.BodyFlags _flags;
    private Body.BodyType _type;
    internal int _islandIndex;
    private XForm _xf;
    internal Sweep _sweep;
    internal Vec2 _linearVelocity;
    internal float _angularVelocity;
    internal Vec2 _force;
    internal float _torque;
    private World _world;
    internal Body _prev;
    internal Body _next;
    internal Shape _shapeList;
    internal int _shapeCount;
    internal JointEdge _jointList;
    internal ContactEdge _contactList;
    internal float _mass;
    internal float _invMass;
    private float _I;
    internal float _invI;
    internal float _linearDamping;
    internal float _angularDamping;
    internal float _sleepTime;
    private object _userData;

    internal Body(BodyDef bd, World world)
    {
      Box2DXDebug.Assert(!world._lock);
      this._flags = (Body.BodyFlags) 0;
      if (bd.IsBullet)
        this._flags |= Body.BodyFlags.Bullet;
      if (bd.FixedRotation)
        this._flags |= Body.BodyFlags.FixedRotation;
      if (bd.AllowSleep)
        this._flags |= Body.BodyFlags.AllowSleep;
      if (bd.IsSleeping)
        this._flags |= Body.BodyFlags.Sleep;
      this._world = world;
      this._xf.Position = bd.Position;
      this._xf.R.Set(bd.Angle);
      this._sweep.LocalCenter = bd.MassData.Center;
      this._sweep.T0 = 1f;
      this._sweep.A0 = this._sweep.A = bd.Angle;
      this._sweep.C0 = this._sweep.C = Box2DX.Common.Math.Mul(this._xf, this._sweep.LocalCenter);
      this._jointList = (JointEdge) null;
      this._contactList = (ContactEdge) null;
      this._prev = (Body) null;
      this._next = (Body) null;
      this._linearDamping = bd.LinearDamping;
      this._angularDamping = bd.AngularDamping;
      this._force.Set(0.0f, 0.0f);
      this._torque = 0.0f;
      this._linearVelocity.SetZero();
      this._angularVelocity = 0.0f;
      this._sleepTime = 0.0f;
      this._invMass = 0.0f;
      this._I = 0.0f;
      this._invI = 0.0f;
      this._mass = bd.MassData.Mass;
      if ((double) this._mass > 0.0)
        this._invMass = 1f / this._mass;
      if ((this._flags & Body.BodyFlags.FixedRotation) == (Body.BodyFlags) 0)
        this._I = bd.MassData.I;
      if ((double) this._I > 0.0)
        this._invI = 1f / this._I;
      this._type = (double) this._invMass != 0.0 || (double) this._invI != 0.0 ? Body.BodyType.Dynamic : Body.BodyType.Static;
      this._userData = bd.UserData;
      this._shapeList = (Shape) null;
      this._shapeCount = 0;
    }

    public void Dispose()
    {
      Box2DXDebug.Assert(!this._world._lock);
    }

    public Shape CreateShape(ShapeDef shapeDef)
    {
      Box2DXDebug.Assert(!this._world._lock);
      if (this._world._lock)
        return (Shape) null;
      Shape shape = Shape.Create(shapeDef);
      shape._next = this._shapeList;
      this._shapeList = shape;
      ++this._shapeCount;
      shape._body = this;
      shape.CreateProxy(this._world._broadPhase, this._xf);
      shape.UpdateSweepRadius(this._sweep.LocalCenter);
      return shape;
    }

    public void DestroyShape(Shape shape)
    {
      Box2DXDebug.Assert(!this._world._lock);
      if (this._world._lock)
        return;
      Box2DXDebug.Assert(shape.GetBody() == this);
      shape.DestroyProxy(this._world._broadPhase);
      Box2DXDebug.Assert(this._shapeCount > 0);
      Shape shape1 = this._shapeList;
      bool condition = false;
      for (; shape1 != null; shape1 = shape1._next)
      {
        if (shape1 == shape)
        {
          this._shapeList = shape._next;
          condition = true;
          break;
        }
      }
      Box2DXDebug.Assert(condition);
      shape._body = (Body) null;
      shape._next = (Shape) null;
      --this._shapeCount;
      Shape.Destroy(shape);
    }

    public void SetMass(MassData massData)
    {
      Box2DXDebug.Assert(!this._world._lock);
      if (this._world._lock)
        return;
      this._invMass = 0.0f;
      this._I = 0.0f;
      this._invI = 0.0f;
      this._mass = massData.Mass;
      if ((double) this._mass > 0.0)
        this._invMass = 1f / this._mass;
      if ((this._flags & Body.BodyFlags.FixedRotation) == (Body.BodyFlags) 0)
        this._I = massData.I;
      if ((double) this._I > 0.0)
        this._invI = 1f / this._I;
      this._sweep.LocalCenter = massData.Center;
      this._sweep.C0 = this._sweep.C = Box2DX.Common.Math.Mul(this._xf, this._sweep.LocalCenter);
      for (Shape shape = this._shapeList; shape != null; shape = shape._next)
        shape.UpdateSweepRadius(this._sweep.LocalCenter);
      Body.BodyType type = this._type;
      this._type = (double) this._invMass != 0.0 || (double) this._invI != 0.0 ? Body.BodyType.Dynamic : Body.BodyType.Static;
      if (type == this._type)
        return;
      for (Shape shape = this._shapeList; shape != null; shape = shape._next)
        shape.RefilterProxy(this._world._broadPhase, this._xf);
    }

    public void SetMassFromShapes()
    {
      Box2DXDebug.Assert(!this._world._lock);
      if (this._world._lock)
        return;
      this._mass = 0.0f;
      this._invMass = 0.0f;
      this._I = 0.0f;
      this._invI = 0.0f;
      Vec2 zero = Vec2.Zero;
      for (Shape shape = this._shapeList; shape != null; shape = shape._next)
      {
        MassData massData;
        shape.ComputeMass(out massData);
        this._mass += massData.Mass;
        zero += massData.Mass * massData.Center;
        this._I += massData.I;
      }
      if ((double) this._mass > 0.0)
      {
        this._invMass = 1f / this._mass;
        zero *= this._invMass;
      }
      if ((double) this._I > 0.0 && (this._flags & Body.BodyFlags.FixedRotation) == (Body.BodyFlags) 0)
      {
        this._I -= this._mass * Vec2.Dot(zero, zero);
        Box2DXDebug.Assert((double) this._I > 0.0);
        this._invI = 1f / this._I;
      }
      else
      {
        this._I = 0.0f;
        this._invI = 0.0f;
      }
      this._sweep.LocalCenter = zero;
      this._sweep.C0 = this._sweep.C = Box2DX.Common.Math.Mul(this._xf, this._sweep.LocalCenter);
      for (Shape shape = this._shapeList; shape != null; shape = shape._next)
        shape.UpdateSweepRadius(this._sweep.LocalCenter);
      Body.BodyType type = this._type;
      this._type = (double) this._invMass != 0.0 || (double) this._invI != 0.0 ? Body.BodyType.Dynamic : Body.BodyType.Static;
      if (type == this._type)
        return;
      for (Shape shape = this._shapeList; shape != null; shape = shape._next)
        shape.RefilterProxy(this._world._broadPhase, this._xf);
    }

    public bool SetXForm(Vec2 position, float angle)
    {
      Box2DXDebug.Assert(!this._world._lock);
      if (this._world._lock)
        return true;
      if (this.IsFrozen())
        return false;
      this._xf.R.Set(angle);
      this._xf.Position = position;
      this._sweep.C0 = this._sweep.C = Box2DX.Common.Math.Mul(this._xf, this._sweep.LocalCenter);
      this._sweep.A0 = this._sweep.A = angle;
      bool flag = false;
      for (Shape shape = this._shapeList; shape != null; shape = shape._next)
      {
        if (!shape.Synchronize(this._world._broadPhase, this._xf, this._xf))
        {
          flag = true;
          break;
        }
      }
      if (flag)
      {
        this._flags |= Body.BodyFlags.Frozen;
        this._linearVelocity.SetZero();
        this._angularVelocity = 0.0f;
        for (Shape shape = this._shapeList; shape != null; shape = shape._next)
          shape.DestroyProxy(this._world._broadPhase);
        return false;
      }
      this._world._broadPhase.Commit();
      return true;
    }

    public XForm GetXForm()
    {
      return this._xf;
    }

    public Vec2 GetPosition()
    {
      return this._xf.Position;
    }

    public float GetAngle()
    {
      return this._sweep.A;
    }

    public Vec2 GetWorldCenter()
    {
      return this._sweep.C;
    }

    public Vec2 GetLocalCenter()
    {
      return this._sweep.LocalCenter;
    }

    public void SetLinearVelocity(Vec2 v)
    {
      this._linearVelocity = v;
    }

    public Vec2 GetLinearVelocity()
    {
      return this._linearVelocity;
    }

    public void SetAngularVelocity(float omega)
    {
      this._angularVelocity = omega;
    }

    public float GetAngularVelocity()
    {
      return this._angularVelocity;
    }

    public void ApplyForce(Vec2 force, Vec2 point)
    {
      if (this.IsSleeping())
        this.WakeUp();
      this._force += force;
      this._torque += Vec2.Cross(point - this._sweep.C, force);
    }

    public void ApplyTorque(float torque)
    {
      if (this.IsSleeping())
        this.WakeUp();
      this._torque += torque;
    }

    public void ApplyImpulse(Vec2 impulse, Vec2 point)
    {
      if (this.IsSleeping())
        this.WakeUp();
      this._linearVelocity += this._invMass * impulse;
      this._angularVelocity += this._invI * Vec2.Cross(point - this._sweep.C, impulse);
    }

    public float GetMass()
    {
      return this._mass;
    }

    public float GetInertia()
    {
      return this._I;
    }

    public Vec2 GetWorldPoint(Vec2 localPoint)
    {
      return Box2DX.Common.Math.Mul(this._xf, localPoint);
    }

    public Vec2 GetWorldVector(Vec2 localVector)
    {
      return Box2DX.Common.Math.Mul(this._xf.R, localVector);
    }

    public Vec2 GetLocalPoint(Vec2 worldPoint)
    {
      return Box2DX.Common.Math.MulT(this._xf, worldPoint);
    }

    public Vec2 GetLocalVector(Vec2 worldVector)
    {
      return Box2DX.Common.Math.MulT(this._xf.R, worldVector);
    }

    public Vec2 GetLinearVelocityFromWorldPoint(Vec2 worldPoint)
    {
      return this._linearVelocity + Vec2.Cross(this._angularVelocity, worldPoint - this._sweep.C);
    }

    public Vec2 GetLinearVelocityFromLocalPoint(Vec2 localPoint)
    {
      return this.GetLinearVelocityFromWorldPoint(this.GetWorldPoint(localPoint));
    }

    public bool IsBullet()
    {
      return (this._flags & Body.BodyFlags.Bullet) == Body.BodyFlags.Bullet;
    }

    public void SetBullet(bool flag)
    {
      if (flag)
        this._flags |= Body.BodyFlags.Bullet;
      else
        this._flags &= ~Body.BodyFlags.Bullet;
    }

    public bool IsStatic()
    {
      return this._type == Body.BodyType.Static;
    }

    public bool IsDynamic()
    {
      return this._type == Body.BodyType.Dynamic;
    }

    public bool IsFrozen()
    {
      return (this._flags & Body.BodyFlags.Frozen) == Body.BodyFlags.Frozen;
    }

    public bool IsSleeping()
    {
      return (this._flags & Body.BodyFlags.Sleep) == Body.BodyFlags.Sleep;
    }

    public void AllowSleeping(bool flag)
    {
      if (flag)
      {
        this._flags |= Body.BodyFlags.AllowSleep;
      }
      else
      {
        this._flags &= ~Body.BodyFlags.AllowSleep;
        this.WakeUp();
      }
    }

    public void WakeUp()
    {
      this._flags &= ~Body.BodyFlags.Sleep;
      this._sleepTime = 0.0f;
    }

    public void PutToSleep()
    {
      this._flags |= Body.BodyFlags.Sleep;
      this._sleepTime = 0.0f;
      this._linearVelocity.SetZero();
      this._angularVelocity = 0.0f;
      this._force.SetZero();
      this._torque = 0.0f;
    }

    public Shape GetShapeList()
    {
      return this._shapeList;
    }

    public JointEdge GetJointList()
    {
      return this._jointList;
    }

    public Body GetNext()
    {
      return this._next;
    }

    public object GetUserData()
    {
      return this._userData;
    }

    public void SetUserData(object data)
    {
      this._userData = data;
    }

    public World GetWorld()
    {
      return this._world;
    }

    internal bool SynchronizeShapes()
    {
      XForm transform1 = new XForm();
      transform1.R.Set(this._sweep.A0);
      transform1.Position = this._sweep.C0 - Box2DX.Common.Math.Mul(transform1.R, this._sweep.LocalCenter);
      bool flag = true;
      for (Shape shape = this._shapeList; shape != null; shape = shape._next)
      {
        flag = shape.Synchronize(this._world._broadPhase, transform1, this._xf);
        if (!flag)
          break;
      }
      if (flag)
        return true;
      this._flags |= Body.BodyFlags.Frozen;
      this._linearVelocity.SetZero();
      this._angularVelocity = 0.0f;
      for (Shape shape = this._shapeList; shape != null; shape = shape._next)
        shape.DestroyProxy(this._world._broadPhase);
      return false;
    }

    internal void SynchronizeTransform()
    {
      this._xf.R.Set(this._sweep.A);
      this._xf.Position = this._sweep.C - Box2DX.Common.Math.Mul(this._xf.R, this._sweep.LocalCenter);
    }

    internal bool IsConnected(Body other)
    {
      for (JointEdge jointEdge = this._jointList; jointEdge != null; jointEdge = jointEdge.Next)
      {
        if (jointEdge.Other == other)
          return !jointEdge.Joint._collideConnected;
      }
      return false;
    }

    internal void Advance(float t)
    {
      this._sweep.Advance(t);
      this._sweep.C = this._sweep.C0;
      this._sweep.A = this._sweep.A0;
      this.SynchronizeTransform();
    }

    [Flags]
    public enum BodyFlags
    {
      Frozen = 2,
      Island = 4,
      Sleep = 8,
      AllowSleep = 16, // 0x00000010
      Bullet = 32, // 0x00000020
      FixedRotation = 64, // 0x00000040
    }

    public enum BodyType
    {
      Static,
      Dynamic,
      MaxTypes,
    }
  }
}
