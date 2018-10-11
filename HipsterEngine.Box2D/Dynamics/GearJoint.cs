// Decompiled with JetBrains decompiler
// Type: Box2DX.Dynamics.GearJoint
// Assembly: Box2DX, Version=2.0.1.28279, Culture=neutral, PublicKeyToken=null
// MVID: BD116DE6-476A-4E37-B8FC-78B19E349C03
// Assembly location: /Users/kusy/Documents/Новая папка/HipsterEngine/bin/Debug/Box2DX.dll

using Box2DX.Common;

namespace Box2DX.Dynamics
{
  public class GearJoint : Joint
  {
    public Body _ground1;
    public Body _ground2;
    public RevoluteJoint _revolute1;
    public PrismaticJoint _prismatic1;
    public RevoluteJoint _revolute2;
    public PrismaticJoint _prismatic2;
    public Vec2 _groundAnchor1;
    public Vec2 _groundAnchor2;
    public Vec2 _localAnchor1;
    public Vec2 _localAnchor2;
    public Jacobian _J;
    public float _constant;
    public float _ratio;
    public float _mass;
    public float _impulse;

    public override Vec2 Anchor1
    {
      get
      {
        return this._body1.GetWorldPoint(this._localAnchor1);
      }
    }

    public override Vec2 Anchor2
    {
      get
      {
        return this._body2.GetWorldPoint(this._localAnchor2);
      }
    }

    public override Vec2 GetReactionForce(float inv_dt)
    {
      Vec2 vec2 = this._impulse * this._J.Linear2;
      return inv_dt * vec2;
    }

    public override float GetReactionTorque(float inv_dt)
    {
      float num = this._impulse * this._J.Angular2 - Vec2.Cross(Math.Mul(this._body2.GetXForm().R, this._localAnchor2 - this._body2.GetLocalCenter()), this._impulse * this._J.Linear2);
      return inv_dt * num;
    }

    public float Ratio
    {
      get
      {
        return this._ratio;
      }
    }

    public GearJoint(GearJointDef def)
      : base((JointDef) def)
    {
      JointType type1 = def.Joint1.GetType();
      JointType type2 = def.Joint2.GetType();
      Box2DXDebug.Assert(type1 == JointType.RevoluteJoint || type1 == JointType.PrismaticJoint);
      Box2DXDebug.Assert(type2 == JointType.RevoluteJoint || type2 == JointType.PrismaticJoint);
      Box2DXDebug.Assert(def.Joint1.GetBody1().IsStatic());
      Box2DXDebug.Assert(def.Joint2.GetBody1().IsStatic());
      this._revolute1 = (RevoluteJoint) null;
      this._prismatic1 = (PrismaticJoint) null;
      this._revolute2 = (RevoluteJoint) null;
      this._prismatic2 = (PrismaticJoint) null;
      this._ground1 = def.Joint1.GetBody1();
      this._body1 = def.Joint1.GetBody2();
      float num1;
      if (type1 == JointType.RevoluteJoint)
      {
        this._revolute1 = (RevoluteJoint) def.Joint1;
        this._groundAnchor1 = this._revolute1._localAnchor1;
        this._localAnchor1 = this._revolute1._localAnchor2;
        num1 = this._revolute1.JointAngle;
      }
      else
      {
        this._prismatic1 = (PrismaticJoint) def.Joint1;
        this._groundAnchor1 = this._prismatic1._localAnchor1;
        this._localAnchor1 = this._prismatic1._localAnchor2;
        num1 = this._prismatic1.JointTranslation;
      }
      this._ground2 = def.Joint2.GetBody1();
      this._body2 = def.Joint2.GetBody2();
      float num2;
      if (type2 == JointType.RevoluteJoint)
      {
        this._revolute2 = (RevoluteJoint) def.Joint2;
        this._groundAnchor2 = this._revolute2._localAnchor1;
        this._localAnchor2 = this._revolute2._localAnchor2;
        num2 = this._revolute2.JointAngle;
      }
      else
      {
        this._prismatic2 = (PrismaticJoint) def.Joint2;
        this._groundAnchor2 = this._prismatic2._localAnchor1;
        this._localAnchor2 = this._prismatic2._localAnchor2;
        num2 = this._prismatic2.JointTranslation;
      }
      this._ratio = def.Ratio;
      this._constant = num1 + this._ratio * num2;
      this._impulse = 0.0f;
    }

    internal override void InitVelocityConstraints(TimeStep step)
    {
      Body ground1 = this._ground1;
      Body ground2 = this._ground2;
      Body body1 = this._body1;
      Body body2 = this._body2;
      float num1 = 0.0f;
      this._J.SetZero();
      float num2;
      if (this._revolute1 != null)
      {
        this._J.Angular1 = -1f;
        num2 = num1 + body1._invI;
      }
      else
      {
        Vec2 b = Math.Mul(ground1.GetXForm().R, this._prismatic1._localXAxis1);
        float num3 = Vec2.Cross(Math.Mul(body1.GetXForm().R, this._localAnchor1 - body1.GetLocalCenter()), b);
        this._J.Linear1 = -b;
        this._J.Angular1 = -num3;
        num2 = num1 + (body1._invMass + body1._invI * num3 * num3);
      }
      float num4;
      if (this._revolute2 != null)
      {
        this._J.Angular2 = -this._ratio;
        num4 = num2 + this._ratio * this._ratio * body2._invI;
      }
      else
      {
        Vec2 b = Math.Mul(ground2.GetXForm().R, this._prismatic2._localXAxis1);
        float num3 = Vec2.Cross(Math.Mul(body2.GetXForm().R, this._localAnchor2 - body2.GetLocalCenter()), b);
        this._J.Linear2 = -this._ratio * b;
        this._J.Angular2 = -this._ratio * num3;
        num4 = num2 + (float) ((double) this._ratio * (double) this._ratio * ((double) body2._invMass + (double) body2._invI * (double) num3 * (double) num3));
      }
      Box2DXDebug.Assert((double) num4 > 0.0);
      this._mass = 1f / num4;
      if (step.WarmStarting)
      {
        body1._linearVelocity += body1._invMass * this._impulse * this._J.Linear1;
        body1._angularVelocity += body1._invI * this._impulse * this._J.Angular1;
        body2._linearVelocity += body2._invMass * this._impulse * this._J.Linear2;
        body2._angularVelocity += body2._invI * this._impulse * this._J.Angular2;
      }
      else
        this._impulse = 0.0f;
    }

    internal override void SolveVelocityConstraints(TimeStep step)
    {
      Body body1 = this._body1;
      Body body2 = this._body2;
      float num = this._mass * -this._J.Compute(body1._linearVelocity, body1._angularVelocity, body2._linearVelocity, body2._angularVelocity);
      this._impulse += num;
      body1._linearVelocity += body1._invMass * num * this._J.Linear1;
      body1._angularVelocity += body1._invI * num * this._J.Angular1;
      body2._linearVelocity += body2._invMass * num * this._J.Linear2;
      body2._angularVelocity += body2._invI * num * this._J.Angular2;
    }

    internal override bool SolvePositionConstraints(float baumgarte)
    {
      float num1 = 0.0f;
      Body body1 = this._body1;
      Body body2 = this._body2;
      float num2 = this._mass * -(this._constant - ((this._revolute1 == null ? this._prismatic1.JointTranslation : this._revolute1.JointAngle) + this._ratio * (this._revolute2 == null ? this._prismatic2.JointTranslation : this._revolute2.JointAngle)));
      body1._sweep.C += body1._invMass * num2 * this._J.Linear1;
      body1._sweep.A += body1._invI * num2 * this._J.Angular1;
      body2._sweep.C += body2._invMass * num2 * this._J.Linear2;
      body2._sweep.A += body2._invI * num2 * this._J.Angular2;
      body1.SynchronizeTransform();
      body2.SynchronizeTransform();
      return (double) num1 < (double) Settings.LinearSlop;
    }
  }
}
