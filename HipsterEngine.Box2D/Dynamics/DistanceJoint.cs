// Decompiled with JetBrains decompiler
// Type: Box2DX.Dynamics.DistanceJoint
// Assembly: Box2DX, Version=2.0.1.28279, Culture=neutral, PublicKeyToken=null
// MVID: BD116DE6-476A-4E37-B8FC-78B19E349C03
// Assembly location: /Users/kusy/Documents/Новая папка/HipsterEngine/bin/Debug/Box2DX.dll

using Box2DX.Common;

namespace Box2DX.Dynamics
{
  public class DistanceJoint : Joint
  {
    public Vec2 _localAnchor1;
    public Vec2 _localAnchor2;
    public Vec2 _u;
    public float _frequencyHz;
    public float _dampingRatio;
    public float _gamma;
    public float _bias;
    public float _impulse;
    public float _mass;
    public float _length;

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
      return inv_dt * this._impulse * this._u;
    }

    public override float GetReactionTorque(float inv_dt)
    {
      return 0.0f;
    }

    public DistanceJoint(DistanceJointDef def)
      : base((JointDef) def)
    {
      this._localAnchor1 = def.LocalAnchor1;
      this._localAnchor2 = def.LocalAnchor2;
      this._length = def.Length;
      this._frequencyHz = def.FrequencyHz;
      this._dampingRatio = def.DampingRatio;
      this._impulse = 0.0f;
      this._gamma = 0.0f;
      this._bias = 0.0f;
    }

    internal override void InitVelocityConstraints(TimeStep step)
    {
      Body body1 = this._body1;
      Body body2 = this._body2;
      Vec2 a1 = Box2DX.Common.Math.Mul(body1.GetXForm().R, this._localAnchor1 - body1.GetLocalCenter());
      Vec2 a2 = Box2DX.Common.Math.Mul(body2.GetXForm().R, this._localAnchor2 - body2.GetLocalCenter());
      this._u = body2._sweep.C + a2 - body1._sweep.C - a1;
      float num1 = this._u.Length();
      if ((double) num1 > (double) Settings.LinearSlop)
        this._u *= 1f / num1;
      else
        this._u.Set(0.0f, 0.0f);
      float num2 = Vec2.Cross(a1, this._u);
      float num3 = Vec2.Cross(a2, this._u);
      float num4 = (float) ((double) body1._invMass + (double) body1._invI * (double) num2 * (double) num2 + (double) body2._invMass + (double) body2._invI * (double) num3 * (double) num3);
      Box2DXDebug.Assert((double) num4 > (double) Settings.FLT_EPSILON);
      this._mass = 1f / num4;
      if ((double) this._frequencyHz > 0.0)
      {
        float num5 = num1 - this._length;
        float num6 = 2f * Settings.Pi * this._frequencyHz;
        float num7 = 2f * this._mass * this._dampingRatio * num6;
        float num8 = this._mass * num6 * num6;
        this._gamma = (float) (1.0 / ((double) step.Dt * ((double) num7 + (double) step.Dt * (double) num8)));
        this._bias = num5 * step.Dt * num8 * this._gamma;
        this._mass = (float) (1.0 / ((double) num4 + (double) this._gamma));
      }
      if (step.WarmStarting)
      {
        this._impulse *= step.DtRatio;
        Vec2 b = this._impulse * this._u;
        body1._linearVelocity -= body1._invMass * b;
        body1._angularVelocity -= body1._invI * Vec2.Cross(a1, b);
        body2._linearVelocity += body2._invMass * b;
        body2._angularVelocity += body2._invI * Vec2.Cross(a2, b);
      }
      else
        this._impulse = 0.0f;
    }

    internal override bool SolvePositionConstraints(float baumgarte)
    {
      if ((double) this._frequencyHz > 0.0)
        return true;
      Body body1 = this._body1;
      Body body2 = this._body2;
      Vec2 a1 = Box2DX.Common.Math.Mul(body1.GetXForm().R, this._localAnchor1 - body1.GetLocalCenter());
      Vec2 a2 = Box2DX.Common.Math.Mul(body2.GetXForm().R, this._localAnchor2 - body2.GetLocalCenter());
      Vec2 vec2 = body2._sweep.C + a2 - body1._sweep.C - a1;
      float num1 = Box2DX.Common.Math.Clamp(vec2.Normalize() - this._length, -Settings.MaxLinearCorrection, Settings.MaxLinearCorrection);
      float num2 = -this._mass * num1;
      this._u = vec2;
      Vec2 b = num2 * this._u;
      body1._sweep.C -= body1._invMass * b;
      body1._sweep.A -= body1._invI * Vec2.Cross(a1, b);
      body2._sweep.C += body2._invMass * b;
      body2._sweep.A += body2._invI * Vec2.Cross(a2, b);
      body1.SynchronizeTransform();
      body2.SynchronizeTransform();
      return (double) System.Math.Abs(num1) < (double) Settings.LinearSlop;
    }

    internal override void SolveVelocityConstraints(TimeStep step)
    {
      Body body1 = this._body1;
      Body body2 = this._body2;
      Vec2 a1 = Box2DX.Common.Math.Mul(body1.GetXForm().R, this._localAnchor1 - body1.GetLocalCenter());
      Vec2 a2 = Box2DX.Common.Math.Mul(body2.GetXForm().R, this._localAnchor2 - body2.GetLocalCenter());
      Vec2 vec2 = body1._linearVelocity + Vec2.Cross(body1._angularVelocity, a1);
      float num = (float) (-(double) this._mass * ((double) Vec2.Dot(this._u, body2._linearVelocity + Vec2.Cross(body2._angularVelocity, a2) - vec2) + (double) this._bias + (double) this._gamma * (double) this._impulse));
      this._impulse += num;
      Vec2 b = num * this._u;
      body1._linearVelocity -= body1._invMass * b;
      body1._angularVelocity -= body1._invI * Vec2.Cross(a1, b);
      body2._linearVelocity += body2._invMass * b;
      body2._angularVelocity += body2._invI * Vec2.Cross(a2, b);
    }
  }
}
