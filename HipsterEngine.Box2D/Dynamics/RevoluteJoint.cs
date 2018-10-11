// Decompiled with JetBrains decompiler
// Type: Box2DX.Dynamics.RevoluteJoint
// Assembly: Box2DX, Version=2.0.1.28279, Culture=neutral, PublicKeyToken=null
// MVID: BD116DE6-476A-4E37-B8FC-78B19E349C03
// Assembly location: /Users/kusy/Documents/Новая папка/HipsterEngine/bin/Debug/Box2DX.dll

using Box2DX.Common;

namespace Box2DX.Dynamics
{
  public class RevoluteJoint : Joint
  {
    public Vec2 _localAnchor1;
    public Vec2 _localAnchor2;
    public Vec3 _impulse;
    public float _motorImpulse;
    public Mat33 _mass;
    public float _motorMass;
    public bool _enableMotor;
    public float _maxMotorTorque;
    public float _motorSpeed;
    public bool _enableLimit;
    public float _referenceAngle;
    public float _lowerAngle;
    public float _upperAngle;
    public LimitState _limitState;

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
      Vec2 vec2 = new Vec2(this._impulse.X, this._impulse.Y);
      return inv_dt * vec2;
    }

    public override float GetReactionTorque(float inv_dt)
    {
      return inv_dt * this._impulse.Z;
    }

    public float JointAngle
    {
      get
      {
        return this._body2._sweep.A - this._body1._sweep.A - this._referenceAngle;
      }
    }

    public float JointSpeed
    {
      get
      {
        return this._body2._angularVelocity - this._body1._angularVelocity;
      }
    }

    public bool IsLimitEnabled
    {
      get
      {
        return this._enableLimit;
      }
    }

    public void EnableLimit(bool flag)
    {
      this._body1.WakeUp();
      this._body2.WakeUp();
      this._enableLimit = flag;
    }

    public float LowerLimit
    {
      get
      {
        return this._lowerAngle;
      }
    }

    public float UpperLimit
    {
      get
      {
        return this._upperAngle;
      }
    }

    public void SetLimits(float lower, float upper)
    {
      Box2DXDebug.Assert((double) lower <= (double) upper);
      this._body1.WakeUp();
      this._body2.WakeUp();
      this._lowerAngle = lower;
      this._upperAngle = upper;
    }

    public bool IsMotorEnabled
    {
      get
      {
        return this._enableMotor;
      }
    }

    public void EnableMotor(bool flag)
    {
      this._body1.WakeUp();
      this._body2.WakeUp();
      this._enableMotor = flag;
    }

    public float MotorSpeed
    {
      get
      {
        return this._motorSpeed;
      }
      set
      {
        this._body1.WakeUp();
        this._body2.WakeUp();
        this._motorSpeed = value;
      }
    }

    public void SetMaxMotorTorque(float torque)
    {
      this._body1.WakeUp();
      this._body2.WakeUp();
      this._maxMotorTorque = torque;
    }

    public float MotorTorque
    {
      get
      {
        return this._motorImpulse;
      }
    }

    public RevoluteJoint(RevoluteJointDef def)
      : base((JointDef) def)
    {
      this._localAnchor1 = def.LocalAnchor1;
      this._localAnchor2 = def.LocalAnchor2;
      this._referenceAngle = def.ReferenceAngle;
      this._impulse = new Vec3();
      this._motorImpulse = 0.0f;
      this._lowerAngle = def.LowerAngle;
      this._upperAngle = def.UpperAngle;
      this._maxMotorTorque = def.MaxMotorTorque;
      this._motorSpeed = def.MotorSpeed;
      this._enableLimit = def.EnableLimit;
      this._enableMotor = def.EnableMotor;
    }

    internal override void InitVelocityConstraints(TimeStep step)
    {
      Body body1 = this._body1;
      Body body2 = this._body2;
      Vec2 a1 = Math.Mul(body1.GetXForm().R, this._localAnchor1 - body1.GetLocalCenter());
      Vec2 a2 = Math.Mul(body2.GetXForm().R, this._localAnchor2 - body2.GetLocalCenter());
      float invMass1 = body1._invMass;
      float invMass2 = body2._invMass;
      float invI1 = body1._invI;
      float invI2 = body2._invI;
      this._mass.Col1.X = (float) ((double) invMass1 + (double) invMass2 + (double) a1.Y * (double) a1.Y * (double) invI1 + (double) a2.Y * (double) a2.Y * (double) invI2);
      this._mass.Col2.X = (float) (-(double) a1.Y * (double) a1.X * (double) invI1 - (double) a2.Y * (double) a2.X * (double) invI2);
      this._mass.Col3.X = (float) (-(double) a1.Y * (double) invI1 - (double) a2.Y * (double) invI2);
      this._mass.Col1.Y = this._mass.Col2.X;
      this._mass.Col2.Y = (float) ((double) invMass1 + (double) invMass2 + (double) a1.X * (double) a1.X * (double) invI1 + (double) a2.X * (double) a2.X * (double) invI2);
      this._mass.Col3.Y = (float) ((double) a1.X * (double) invI1 + (double) a2.X * (double) invI2);
      this._mass.Col1.Z = this._mass.Col3.X;
      this._mass.Col2.Z = this._mass.Col3.Y;
      this._mass.Col3.Z = invI1 + invI2;
      this._motorMass = (float) (1.0 / ((double) invI1 + (double) invI2));
      if (!this._enableMotor)
        this._motorImpulse = 0.0f;
      if (this._enableLimit)
      {
        float num = body2._sweep.A - body1._sweep.A - this._referenceAngle;
        if ((double) Math.Abs(this._upperAngle - this._lowerAngle) < 2.0 * (double) Settings.AngularSlop)
          this._limitState = LimitState.EqualLimits;
        else if ((double) num <= (double) this._lowerAngle)
        {
          if (this._limitState != LimitState.AtLowerLimit)
            this._impulse.Z = 0.0f;
          this._limitState = LimitState.AtLowerLimit;
        }
        else if ((double) num >= (double) this._upperAngle)
        {
          if (this._limitState != LimitState.AtUpperLimit)
            this._impulse.Z = 0.0f;
          this._limitState = LimitState.AtUpperLimit;
        }
        else
        {
          this._limitState = LimitState.InactiveLimit;
          this._impulse.Z = 0.0f;
        }
      }
      if (step.WarmStarting)
      {
        this._impulse *= step.DtRatio;
        this._motorImpulse *= step.DtRatio;
        Vec2 b = new Vec2(this._impulse.X, this._impulse.Y);
        body1._linearVelocity -= invMass1 * b;
        body1._angularVelocity -= invI1 * (Vec2.Cross(a1, b) + this._motorImpulse + this._impulse.Z);
        body2._linearVelocity += invMass2 * b;
        body2._angularVelocity += invI2 * (Vec2.Cross(a2, b) + this._motorImpulse + this._impulse.Z);
      }
      else
      {
        this._impulse.SetZero();
        this._motorImpulse = 0.0f;
      }
    }

    internal override void SolveVelocityConstraints(TimeStep step)
    {
      Body body1 = this._body1;
      Body body2 = this._body2;
      Vec2 linearVelocity1 = body1._linearVelocity;
      float angularVelocity1 = body1._angularVelocity;
      Vec2 linearVelocity2 = body2._linearVelocity;
      float angularVelocity2 = body2._angularVelocity;
      float invMass1 = body1._invMass;
      float invMass2 = body2._invMass;
      float invI1 = body1._invI;
      float invI2 = body2._invI;
      if (this._enableMotor && this._limitState != LimitState.EqualLimits)
      {
        float num1 = this._motorMass * -(angularVelocity2 - angularVelocity1 - this._motorSpeed);
        float motorImpulse = this._motorImpulse;
        float high = step.Dt * this._maxMotorTorque;
        this._motorImpulse = Math.Clamp(this._motorImpulse + num1, -high, high);
        float num2 = this._motorImpulse - motorImpulse;
        angularVelocity1 -= invI1 * num2;
        angularVelocity2 += invI2 * num2;
      }
      Vec2 vec2_1;
      float num3;
      Vec2 vec2_2;
      float num4;
      if (this._enableLimit && this._limitState != LimitState.InactiveLimit)
      {
        Vec2 a1 = Math.Mul(body1.GetXForm().R, this._localAnchor1 - body1.GetLocalCenter());
        Vec2 a2 = Math.Mul(body2.GetXForm().R, this._localAnchor2 - body2.GetLocalCenter());
        Vec2 vec2_3 = linearVelocity2 + Vec2.Cross(angularVelocity2, a2) - linearVelocity1 - Vec2.Cross(angularVelocity1, a1);
        float z = angularVelocity2 - angularVelocity1;
        Vec3 vec3 = this._mass.Solve33(-new Vec3(vec2_3.X, vec2_3.Y, z));
        if (this._limitState == LimitState.EqualLimits)
          this._impulse += vec3;
        else if (this._limitState == LimitState.AtLowerLimit)
        {
          if ((double) (this._impulse.Z + vec3.Z) < 0.0)
          {
            Vec2 vec2_4 = this._mass.Solve22(-vec2_3);
            vec3.X = vec2_4.X;
            vec3.Y = vec2_4.Y;
            vec3.Z = -this._impulse.Z;
            this._impulse.X += vec2_4.X;
            this._impulse.Y += vec2_4.Y;
            this._impulse.Z = 0.0f;
          }
        }
        else if (this._limitState == LimitState.AtUpperLimit && (double) (this._impulse.Z + vec3.Z) > 0.0)
        {
          Vec2 vec2_4 = this._mass.Solve22(-vec2_3);
          vec3.X = vec2_4.X;
          vec3.Y = vec2_4.Y;
          vec3.Z = -this._impulse.Z;
          this._impulse.X += vec2_4.X;
          this._impulse.Y += vec2_4.Y;
          this._impulse.Z = 0.0f;
        }
        Vec2 b = new Vec2(vec3.X, vec3.Y);
        vec2_1 = linearVelocity1 - invMass1 * b;
        num3 = angularVelocity1 - invI1 * (Vec2.Cross(a1, b) + vec3.Z);
        vec2_2 = linearVelocity2 + invMass2 * b;
        num4 = angularVelocity2 + invI2 * (Vec2.Cross(a2, b) + vec3.Z);
      }
      else
      {
        Vec2 a1 = Math.Mul(body1.GetXForm().R, this._localAnchor1 - body1.GetLocalCenter());
        Vec2 a2 = Math.Mul(body2.GetXForm().R, this._localAnchor2 - body2.GetLocalCenter());
        Vec2 b = this._mass.Solve22(-(linearVelocity2 + Vec2.Cross(angularVelocity2, a2) - linearVelocity1 - Vec2.Cross(angularVelocity1, a1)));
        this._impulse.X += b.X;
        this._impulse.Y += b.Y;
        vec2_1 = linearVelocity1 - invMass1 * b;
        num3 = angularVelocity1 - invI1 * Vec2.Cross(a1, b);
        vec2_2 = linearVelocity2 + invMass2 * b;
        num4 = angularVelocity2 + invI2 * Vec2.Cross(a2, b);
      }
      body1._linearVelocity = vec2_1;
      body1._angularVelocity = num3;
      body2._linearVelocity = vec2_2;
      body2._angularVelocity = num4;
    }

    internal override bool SolvePositionConstraints(float baumgarte)
    {
      Body body1 = this._body1;
      Body body2 = this._body2;
      float num1 = 0.0f;
      if (this._enableLimit && this._limitState != LimitState.InactiveLimit)
      {
        float a1 = body2._sweep.A - body1._sweep.A - this._referenceAngle;
        float num2 = 0.0f;
        if (this._limitState == LimitState.EqualLimits)
        {
          float a2 = Math.Clamp(a1, -Settings.MaxAngularCorrection, Settings.MaxAngularCorrection);
          num2 = -this._motorMass * a2;
          num1 = Math.Abs(a2);
        }
        else if (this._limitState == LimitState.AtLowerLimit)
        {
          float num3 = a1 - this._lowerAngle;
          num1 = -num3;
          num2 = -this._motorMass * Math.Clamp(num3 + Settings.AngularSlop, -Settings.MaxAngularCorrection, 0.0f);
        }
        else if (this._limitState == LimitState.AtUpperLimit)
        {
          float num3 = a1 - this._upperAngle;
          num1 = num3;
          num2 = -this._motorMass * Math.Clamp(num3 - Settings.AngularSlop, 0.0f, Settings.MaxAngularCorrection);
        }
        body1._sweep.A -= body1._invI * num2;
        body2._sweep.A += body2._invI * num2;
        body1.SynchronizeTransform();
        body2.SynchronizeTransform();
      }
      Vec2 a3 = Math.Mul(body1.GetXForm().R, this._localAnchor1 - body1.GetLocalCenter());
      Vec2 a4 = Math.Mul(body2.GetXForm().R, this._localAnchor2 - body2.GetLocalCenter());
      Vec2 vec2_1 = body2._sweep.C + a4 - body1._sweep.C - a3;
      float num4 = vec2_1.Length();
      float invMass1 = body1._invMass;
      float invMass2 = body2._invMass;
      float invI1 = body1._invI;
      float invI2 = body2._invI;
      float num5 = 10f * Settings.LinearSlop;
      if ((double) vec2_1.LengthSquared() > (double) num5 * (double) num5)
      {
        double num2 = (double) vec2_1.Normalize();
        float num3 = invMass1 + invMass2;
        Box2DXDebug.Assert((double) num3 > (double) Settings.FLT_EPSILON);
        Vec2 vec2_2 = 1f / num3 * -vec2_1;
        float num6 = 0.5f;
        body1._sweep.C -= num6 * invMass1 * vec2_2;
        body2._sweep.C += num6 * invMass2 * vec2_2;
        vec2_1 = body2._sweep.C + a4 - body1._sweep.C - a3;
      }
      Vec2 b = (new Mat22()
      {
        Col1 = {
          X = invMass1 + invMass2
        },
        Col2 = {
          X = 0.0f
        }
      } + new Mat22()
      {
        Col1 = {
          X = invI1 * a3.Y * a3.Y
        },
        Col2 = {
          X = -invI1 * a3.X * a3.Y
        }
      } + new Mat22()
      {
        Col1 = {
          X = invI2 * a4.Y * a4.Y
        },
        Col2 = {
          X = -invI2 * a4.X * a4.Y
        }
      }).Solve(-vec2_1);
      body1._sweep.C -= body1._invMass * b;
      body1._sweep.A -= body1._invI * Vec2.Cross(a3, b);
      body2._sweep.C += body2._invMass * b;
      body2._sweep.A += body2._invI * Vec2.Cross(a4, b);
      body1.SynchronizeTransform();
      body2.SynchronizeTransform();
      return (double) num4 <= (double) Settings.LinearSlop && (double) num1 <= (double) Settings.AngularSlop;
    }
  }
}
