using Box2DX.Common;

namespace Box2DX.Dynamics
{
  public class PulleyJoint : Joint
  {
    public static readonly float MinPulleyLength = 2f;
    public Body _ground;
    public Vec2 _groundAnchor1;
    public Vec2 _groundAnchor2;
    public Vec2 _localAnchor1;
    public Vec2 _localAnchor2;
    public Vec2 _u1;
    public Vec2 _u2;
    public float _constant;
    public float _ratio;
    public float _maxLength1;
    public float _maxLength2;
    public float _pulleyMass;
    public float _limitMass1;
    public float _limitMass2;
    public float _impulse;
    public float _limitImpulse1;
    public float _limitImpulse2;
    public LimitState _state;
    public LimitState _limitState1;
    public LimitState _limitState2;

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
      Vec2 vec2 = this._impulse * this._u2;
      return inv_dt * vec2;
    }

    public override float GetReactionTorque(float inv_dt)
    {
      return 0.0f;
    }

    public Vec2 GroundAnchor1
    {
      get
      {
        return this._ground.GetXForm().Position + this._groundAnchor1;
      }
    }

    public Vec2 GroundAnchor2
    {
      get
      {
        return this._ground.GetXForm().Position + this._groundAnchor2;
      }
    }

    public float Length1
    {
      get
      {
        return (this._body1.GetWorldPoint(this._localAnchor1) - (this._ground.GetXForm().Position + this._groundAnchor1)).Length();
      }
    }

    public float Length2
    {
      get
      {
        return (this._body2.GetWorldPoint(this._localAnchor2) - (this._ground.GetXForm().Position + this._groundAnchor2)).Length();
      }
    }

    public float Ratio
    {
      get
      {
        return this._ratio;
      }
    }

    public PulleyJoint(PulleyJointDef def)
      : base((JointDef) def)
    {
      this._ground = this._body1.GetWorld().GetGroundBody();
      this._groundAnchor1 = def.GroundAnchor1 - this._ground.GetXForm().Position;
      this._groundAnchor2 = def.GroundAnchor2 - this._ground.GetXForm().Position;
      this._localAnchor1 = def.LocalAnchor1;
      this._localAnchor2 = def.LocalAnchor2;
      Box2DXDebug.Assert((double) def.Ratio != 0.0);
      this._ratio = def.Ratio;
      this._constant = def.Length1 + this._ratio * def.Length2;
      this._maxLength1 = Math.Min(def.MaxLength1, this._constant - this._ratio * PulleyJoint.MinPulleyLength);
      this._maxLength2 = Math.Min(def.MaxLength2, (this._constant - PulleyJoint.MinPulleyLength) / this._ratio);
      this._impulse = 0.0f;
      this._limitImpulse1 = 0.0f;
      this._limitImpulse2 = 0.0f;
    }

    internal override void InitVelocityConstraints(TimeStep step)
    {
      Body body1 = this._body1;
      Body body2 = this._body2;
      Vec2 a1 = Math.Mul(body1.GetXForm().R, this._localAnchor1 - body1.GetLocalCenter());
      Vec2 a2 = Math.Mul(body2.GetXForm().R, this._localAnchor2 - body2.GetLocalCenter());
      Vec2 vec2_1 = body1._sweep.C + a1;
      Vec2 vec2_2 = body2._sweep.C + a2;
      Vec2 vec2_3 = this._ground.GetXForm().Position + this._groundAnchor1;
      Vec2 vec2_4 = this._ground.GetXForm().Position + this._groundAnchor2;
      this._u1 = vec2_1 - vec2_3;
      this._u2 = vec2_2 - vec2_4;
      float num1 = this._u1.Length();
      float num2 = this._u2.Length();
      if ((double) num1 > (double) Settings.LinearSlop)
        this._u1 *= 1f / num1;
      else
        this._u1.SetZero();
      if ((double) num2 > (double) Settings.LinearSlop)
        this._u2 *= 1f / num2;
      else
        this._u2.SetZero();
      if ((double) this._constant - (double) num1 - (double) this._ratio * (double) num2 > 0.0)
      {
        this._state = LimitState.InactiveLimit;
        this._impulse = 0.0f;
      }
      else
        this._state = LimitState.AtUpperLimit;
      if ((double) num1 < (double) this._maxLength1)
      {
        this._limitState1 = LimitState.InactiveLimit;
        this._limitImpulse1 = 0.0f;
      }
      else
        this._limitState1 = LimitState.AtUpperLimit;
      if ((double) num2 < (double) this._maxLength2)
      {
        this._limitState2 = LimitState.InactiveLimit;
        this._limitImpulse2 = 0.0f;
      }
      else
        this._limitState2 = LimitState.AtUpperLimit;
      float num3 = Vec2.Cross(a1, this._u1);
      float num4 = Vec2.Cross(a2, this._u2);
      this._limitMass1 = body1._invMass + body1._invI * num3 * num3;
      this._limitMass2 = body2._invMass + body2._invI * num4 * num4;
      this._pulleyMass = this._limitMass1 + this._ratio * this._ratio * this._limitMass2;
      Box2DXDebug.Assert((double) this._limitMass1 > (double) Settings.FLT_EPSILON);
      Box2DXDebug.Assert((double) this._limitMass2 > (double) Settings.FLT_EPSILON);
      Box2DXDebug.Assert((double) this._pulleyMass > (double) Settings.FLT_EPSILON);
      this._limitMass1 = 1f / this._limitMass1;
      this._limitMass2 = 1f / this._limitMass2;
      this._pulleyMass = 1f / this._pulleyMass;
      if (step.WarmStarting)
      {
        this._impulse *= step.DtRatio;
        this._limitImpulse1 *= step.DtRatio;
        this._limitImpulse2 *= step.DtRatio;
        Vec2 b1 = (float) -((double) this._impulse + (double) this._limitImpulse1) * this._u1;
        Vec2 b2 = (-this._ratio * this._impulse - this._limitImpulse2) * this._u2;
        body1._linearVelocity += body1._invMass * b1;
        body1._angularVelocity += body1._invI * Vec2.Cross(a1, b1);
        body2._linearVelocity += body2._invMass * b2;
        body2._angularVelocity += body2._invI * Vec2.Cross(a2, b2);
      }
      else
      {
        this._impulse = 0.0f;
        this._limitImpulse1 = 0.0f;
        this._limitImpulse2 = 0.0f;
      }
    }

    internal override void SolveVelocityConstraints(TimeStep step)
    {
      Body body1 = this._body1;
      Body body2 = this._body2;
      Vec2 a1 = Math.Mul(body1.GetXForm().R, this._localAnchor1 - body1.GetLocalCenter());
      Vec2 a2 = Math.Mul(body2.GetXForm().R, this._localAnchor2 - body2.GetLocalCenter());
      if (this._state == LimitState.AtUpperLimit)
      {
        float num1 = this._pulleyMass * (float) -(-(double) Vec2.Dot(this._u1, body1._linearVelocity + Vec2.Cross(body1._angularVelocity, a1)) - (double) this._ratio * (double) Vec2.Dot(this._u2, body2._linearVelocity + Vec2.Cross(body2._angularVelocity, a2)));
        float impulse = this._impulse;
        this._impulse = Math.Max(0.0f, this._impulse + num1);
        float num2 = this._impulse - impulse;
        Vec2 b1 = -num2 * this._u1;
        Vec2 b2 = -this._ratio * num2 * this._u2;
        body1._linearVelocity += body1._invMass * b1;
        body1._angularVelocity += body1._invI * Vec2.Cross(a1, b1);
        body2._linearVelocity += body2._invMass * b2;
        body2._angularVelocity += body2._invI * Vec2.Cross(a2, b2);
      }
      if (this._limitState1 == LimitState.AtUpperLimit)
      {
        float num = -this._limitMass1 * -Vec2.Dot(this._u1, body1._linearVelocity + Vec2.Cross(body1._angularVelocity, a1));
        float limitImpulse1 = this._limitImpulse1;
        this._limitImpulse1 = Math.Max(0.0f, this._limitImpulse1 + num);
        Vec2 b = -(this._limitImpulse1 - limitImpulse1) * this._u1;
        body1._linearVelocity += body1._invMass * b;
        body1._angularVelocity += body1._invI * Vec2.Cross(a1, b);
      }
      if (this._limitState2 != LimitState.AtUpperLimit)
        return;
      float num3 = -this._limitMass2 * -Vec2.Dot(this._u2, body2._linearVelocity + Vec2.Cross(body2._angularVelocity, a2));
      float limitImpulse2 = this._limitImpulse2;
      this._limitImpulse2 = Math.Max(0.0f, this._limitImpulse2 + num3);
      Vec2 b3 = -(this._limitImpulse2 - limitImpulse2) * this._u2;
      body2._linearVelocity += body2._invMass * b3;
      body2._angularVelocity += body2._invI * Vec2.Cross(a2, b3);
    }

    internal override bool SolvePositionConstraints(float baumgarte)
    {
      Body body1 = this._body1;
      Body body2 = this._body2;
      Vec2 vec2_1 = this._ground.GetXForm().Position + this._groundAnchor1;
      Vec2 vec2_2 = this._ground.GetXForm().Position + this._groundAnchor2;
      float a1 = 0.0f;
      if (this._state == LimitState.AtUpperLimit)
      {
        Vec2 a2 = Math.Mul(body1.GetXForm().R, this._localAnchor1 - body1.GetLocalCenter());
        Vec2 a3 = Math.Mul(body2.GetXForm().R, this._localAnchor2 - body2.GetLocalCenter());
        Vec2 vec2_3 = body1._sweep.C + a2;
        Vec2 vec2_4 = body2._sweep.C + a3;
        this._u1 = vec2_3 - vec2_1;
        this._u2 = vec2_4 - vec2_2;
        float num1 = this._u1.Length();
        float num2 = this._u2.Length();
        if ((double) num1 > (double) Settings.LinearSlop)
          this._u1 *= 1f / num1;
        else
          this._u1.SetZero();
        if ((double) num2 > (double) Settings.LinearSlop)
          this._u2 *= 1f / num2;
        else
          this._u2.SetZero();
        float num3 = (float) ((double) this._constant - (double) num1 - (double) this._ratio * (double) num2);
        a1 = Math.Max(a1, -num3);
        float num4 = -this._pulleyMass * Math.Clamp(num3 + Settings.LinearSlop, -Settings.MaxLinearCorrection, 0.0f);
        Vec2 b1 = -num4 * this._u1;
        Vec2 b2 = -this._ratio * num4 * this._u2;
        body1._sweep.C += body1._invMass * b1;
        body1._sweep.A += body1._invI * Vec2.Cross(a2, b1);
        body2._sweep.C += body2._invMass * b2;
        body2._sweep.A += body2._invI * Vec2.Cross(a3, b2);
        body1.SynchronizeTransform();
        body2.SynchronizeTransform();
      }
      if (this._limitState1 == LimitState.AtUpperLimit)
      {
        Vec2 a2 = Math.Mul(body1.GetXForm().R, this._localAnchor1 - body1.GetLocalCenter());
        this._u1 = body1._sweep.C + a2 - vec2_1;
        float num1 = this._u1.Length();
        if ((double) num1 > (double) Settings.LinearSlop)
          this._u1 *= 1f / num1;
        else
          this._u1.SetZero();
        float num2 = this._maxLength1 - num1;
        a1 = Math.Max(a1, -num2);
        Vec2 b = -(-this._limitMass1 * Math.Clamp(num2 + Settings.LinearSlop, -Settings.MaxLinearCorrection, 0.0f)) * this._u1;
        body1._sweep.C += body1._invMass * b;
        body1._sweep.A += body1._invI * Vec2.Cross(a2, b);
        body1.SynchronizeTransform();
      }
      if (this._limitState2 == LimitState.AtUpperLimit)
      {
        Vec2 a2 = Math.Mul(body2.GetXForm().R, this._localAnchor2 - body2.GetLocalCenter());
        this._u2 = body2._sweep.C + a2 - vec2_2;
        float num1 = this._u2.Length();
        if ((double) num1 > (double) Settings.LinearSlop)
          this._u2 *= 1f / num1;
        else
          this._u2.SetZero();
        float num2 = this._maxLength2 - num1;
        a1 = Math.Max(a1, -num2);
        Vec2 b = -(-this._limitMass2 * Math.Clamp(num2 + Settings.LinearSlop, -Settings.MaxLinearCorrection, 0.0f)) * this._u2;
        body2._sweep.C += body2._invMass * b;
        body2._sweep.A += body2._invI * Vec2.Cross(a2, b);
        body2.SynchronizeTransform();
      }
      return (double) a1 < (double) Settings.LinearSlop;
    }
  }
}
