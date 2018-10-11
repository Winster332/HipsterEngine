// Decompiled with JetBrains decompiler
// Type: Box2DX.Dynamics.MouseJoint
// Assembly: Box2DX, Version=2.0.1.28279, Culture=neutral, PublicKeyToken=null
// MVID: BD116DE6-476A-4E37-B8FC-78B19E349C03
// Assembly location: /Users/kusy/Documents/Новая папка/HipsterEngine/bin/Debug/Box2DX.dll

using Box2DX.Common;

namespace Box2DX.Dynamics
{
  public class MouseJoint : Joint
  {
    public Vec2 _localAnchor;
    public Vec2 _target;
    public Vec2 _impulse;
    public Mat22 _mass;
    public Vec2 _C;
    public float _maxForce;
    public float _frequencyHz;
    public float _dampingRatio;
    public float _beta;
    public float _gamma;

    public override Vec2 Anchor1
    {
      get
      {
        return this._target;
      }
    }

    public override Vec2 Anchor2
    {
      get
      {
        return this._body2.GetWorldPoint(this._localAnchor);
      }
    }

    public override Vec2 GetReactionForce(float inv_dt)
    {
      return inv_dt * this._impulse;
    }

    public override float GetReactionTorque(float inv_dt)
    {
      return inv_dt * 0.0f;
    }

    public void SetTarget(Vec2 target)
    {
      if (this._body2.IsSleeping())
        this._body2.WakeUp();
      this._target = target;
    }

    public MouseJoint(MouseJointDef def)
      : base((JointDef) def)
    {
      this._target = def.Target;
      this._localAnchor = Math.MulT(this._body2.GetXForm(), this._target);
      this._maxForce = def.MaxForce;
      this._impulse.SetZero();
      this._frequencyHz = def.FrequencyHz;
      this._dampingRatio = def.DampingRatio;
      this._beta = 0.0f;
      this._gamma = 0.0f;
    }

    internal override void InitVelocityConstraints(TimeStep step)
    {
      Body body2 = this._body2;
      float mass = body2.GetMass();
      float num1 = 2f * Settings.Pi * this._frequencyHz;
      float num2 = 2f * mass * this._dampingRatio * num1;
      float num3 = mass * (num1 * num1);
      Box2DXDebug.Assert((double) num2 + (double) step.Dt * (double) num3 > (double) Settings.FLT_EPSILON);
      this._gamma = (float) (1.0 / ((double) step.Dt * ((double) num2 + (double) step.Dt * (double) num3)));
      this._beta = step.Dt * num3 * this._gamma;
      Vec2 a = Math.Mul(body2.GetXForm().R, this._localAnchor - body2.GetLocalCenter());
      float invMass = body2._invMass;
      float invI = body2._invI;
      Mat22 mat22 = new Mat22()
      {
        Col1 = {
          X = invMass
        },
        Col2 = {
          X = 0.0f
        }
      } + new Mat22()
      {
        Col1 = {
          X = invI * a.Y * a.Y
        },
        Col2 = {
          X = -invI * a.X * a.Y
        }
      };
      mat22.Col1.X += this._gamma;
      mat22.Col2.Y += this._gamma;
      this._mass = mat22.Invert();
      this._C = body2._sweep.C + a - this._target;
      body2._angularVelocity *= 0.98f;
      this._impulse *= step.DtRatio;
      body2._linearVelocity += invMass * this._impulse;
      body2._angularVelocity += invI * Vec2.Cross(a, this._impulse);
    }

    internal override void SolveVelocityConstraints(TimeStep step)
    {
      Body body2 = this._body2;
      Vec2 a = Math.Mul(body2.GetXForm().R, this._localAnchor - body2.GetLocalCenter());
      Vec2 vec2 = Math.Mul(this._mass, -(body2._linearVelocity + Vec2.Cross(body2._angularVelocity, a) + this._beta * this._C + this._gamma * this._impulse));
      Vec2 impulse = this._impulse;
      this._impulse += vec2;
      float num = step.Dt * this._maxForce;
      if ((double) this._impulse.LengthSquared() > (double) num * (double) num)
        this._impulse *= num / this._impulse.Length();
      Vec2 b = this._impulse - impulse;
      body2._linearVelocity += body2._invMass * b;
      body2._angularVelocity += body2._invI * Vec2.Cross(a, b);
    }

    internal override bool SolvePositionConstraints(float baumgarte)
    {
      return true;
    }
  }
}
