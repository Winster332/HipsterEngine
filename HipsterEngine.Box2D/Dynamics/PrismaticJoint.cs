// Decompiled with JetBrains decompiler
// Type: Box2DX.Dynamics.PrismaticJoint
// Assembly: Box2DX, Version=2.0.1.28279, Culture=neutral, PublicKeyToken=null
// MVID: BD116DE6-476A-4E37-B8FC-78B19E349C03
// Assembly location: /Users/kusy/Documents/Новая папка/HipsterEngine/bin/Debug/Box2DX.dll

using Box2DX.Common;

namespace Box2DX.Dynamics
{
  public class PrismaticJoint : Joint
  {
    public Vec2 _localAnchor1;
    public Vec2 _localAnchor2;
    public Vec2 _localXAxis1;
    public Vec2 _localYAxis1;
    public float _refAngle;
    public Vec2 _axis;
    public Vec2 _perp;
    public float _s1;
    public float _s2;
    public float _a1;
    public float _a2;
    public Mat33 _K;
    public Vec3 _impulse;
    public float _motorMass;
    public float _motorImpulse;
    public float _lowerTranslation;
    public float _upperTranslation;
    public float _maxMotorForce;
    public float _motorSpeed;
    public bool _enableLimit;
    public bool _enableMotor;
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
      return inv_dt * (this._impulse.X * this._perp + (this._motorImpulse + this._impulse.Z) * this._axis);
    }

    public override float GetReactionTorque(float inv_dt)
    {
      return inv_dt * this._impulse.Y;
    }

    public float JointTranslation
    {
      get
      {
        Body body1 = this._body1;
        return Vec2.Dot(this._body2.GetWorldPoint(this._localAnchor2) - body1.GetWorldPoint(this._localAnchor1), body1.GetWorldVector(this._localXAxis1));
      }
    }

    public float JointSpeed
    {
      get
      {
        Body body1 = this._body1;
        Body body2 = this._body2;
        Vec2 a1 = Math.Mul(body1.GetXForm().R, this._localAnchor1 - body1.GetLocalCenter());
        Vec2 a2 = Math.Mul(body2.GetXForm().R, this._localAnchor2 - body2.GetLocalCenter());
        Vec2 vec2 = body1._sweep.C + a1;
        Vec2 a3 = body2._sweep.C + a2 - vec2;
        Vec2 worldVector = body1.GetWorldVector(this._localXAxis1);
        Vec2 linearVelocity1 = body1._linearVelocity;
        Vec2 linearVelocity2 = body2._linearVelocity;
        float angularVelocity1 = body1._angularVelocity;
        float angularVelocity2 = body2._angularVelocity;
        return Vec2.Dot(a3, Vec2.Cross(angularVelocity1, worldVector)) + Vec2.Dot(worldVector, linearVelocity2 + Vec2.Cross(angularVelocity2, a2) - linearVelocity1 - Vec2.Cross(angularVelocity1, a1));
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
        return this._lowerTranslation;
      }
    }

    public float UpperLimit
    {
      get
      {
        return this._upperTranslation;
      }
    }

    public void SetLimits(float lower, float upper)
    {
      Box2DXDebug.Assert((double) lower <= (double) upper);
      this._body1.WakeUp();
      this._body2.WakeUp();
      this._lowerTranslation = lower;
      this._upperTranslation = upper;
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

    public void SetMaxMotorForce(float force)
    {
      this._body1.WakeUp();
      this._body2.WakeUp();
      this._maxMotorForce = Settings.FORCE_SCALE(1f) * force;
    }

    public float MotorForce
    {
      get
      {
        return this._motorImpulse;
      }
    }

    public PrismaticJoint(PrismaticJointDef def)
      : base((JointDef) def)
    {
      this._localAnchor1 = def.LocalAnchor1;
      this._localAnchor2 = def.LocalAnchor2;
      this._localXAxis1 = def.LocalAxis1;
      this._localYAxis1 = Vec2.Cross(1f, this._localXAxis1);
      this._refAngle = def.ReferenceAngle;
      this._impulse.SetZero();
      this._motorMass = 0.0f;
      this._motorImpulse = 0.0f;
      this._lowerTranslation = def.LowerTranslation;
      this._upperTranslation = def.UpperTranslation;
      this._maxMotorForce = Settings.FORCE_INV_SCALE(def.MaxMotorForce);
      this._motorSpeed = def.MotorSpeed;
      this._enableLimit = def.EnableLimit;
      this._enableMotor = def.EnableMotor;
      this._axis.SetZero();
      this._perp.SetZero();
    }

    internal override void InitVelocityConstraints(TimeStep step)
    {
      Body body1 = this._body1;
      Body body2 = this._body2;
      this._localCenter1 = body1.GetLocalCenter();
      this._localCenter2 = body2.GetLocalCenter();
      XForm xform1 = body1.GetXForm();
      XForm xform2 = body2.GetXForm();
      Vec2 vec2_1 = Math.Mul(xform1.R, this._localAnchor1 - this._localCenter1);
      Vec2 a = Math.Mul(xform2.R, this._localAnchor2 - this._localCenter2);
      Vec2 b = body2._sweep.C + a - body1._sweep.C - vec2_1;
      this._invMass1 = body1._invMass;
      this._invI1 = body1._invI;
      this._invMass2 = body2._invMass;
      this._invI2 = body2._invI;
      this._axis = Math.Mul(xform1.R, this._localXAxis1);
      this._a1 = Vec2.Cross(b + vec2_1, this._axis);
      this._a2 = Vec2.Cross(a, this._axis);
      this._motorMass = (float) ((double) this._invMass1 + (double) this._invMass2 + (double) this._invI1 * (double) this._a1 * (double) this._a1 + (double) this._invI2 * (double) this._a2 * (double) this._a2);
      Box2DXDebug.Assert((double) this._motorMass > (double) Settings.FLT_EPSILON);
      this._motorMass = 1f / this._motorMass;
      this._perp = Math.Mul(xform1.R, this._localYAxis1);
      this._s1 = Vec2.Cross(b + vec2_1, this._perp);
      this._s2 = Vec2.Cross(a, this._perp);
      float invMass1 = this._invMass1;
      float invMass2 = this._invMass2;
      float invI1 = this._invI1;
      float invI2 = this._invI2;
      float x = (float) ((double) invMass1 + (double) invMass2 + (double) invI1 * (double) this._s1 * (double) this._s1 + (double) invI2 * (double) this._s2 * (double) this._s2);
      float num1 = (float) ((double) invI1 * (double) this._s1 + (double) invI2 * (double) this._s2);
      float num2 = (float) ((double) invI1 * (double) this._s1 * (double) this._a1 + (double) invI2 * (double) this._s2 * (double) this._a2);
      float y = invI1 + invI2;
      float num3 = (float) ((double) invI1 * (double) this._a1 + (double) invI2 * (double) this._a2);
      float z = (float) ((double) invMass1 + (double) invMass2 + (double) invI1 * (double) this._a1 * (double) this._a1 + (double) invI2 * (double) this._a2 * (double) this._a2);
      this._K.Col1.Set(x, num1, num2);
      this._K.Col2.Set(num1, y, num3);
      this._K.Col3.Set(num2, num3, z);
      if (this._enableLimit)
      {
        float num4 = Vec2.Dot(this._axis, b);
        if ((double) Math.Abs(this._upperTranslation - this._lowerTranslation) < 2.0 * (double) Settings.LinearSlop)
          this._limitState = LimitState.EqualLimits;
        else if ((double) num4 <= (double) this._lowerTranslation)
        {
          if (this._limitState != LimitState.AtLowerLimit)
          {
            this._limitState = LimitState.AtLowerLimit;
            this._impulse.Z = 0.0f;
          }
        }
        else if ((double) num4 >= (double) this._upperTranslation)
        {
          if (this._limitState != LimitState.AtUpperLimit)
          {
            this._limitState = LimitState.AtUpperLimit;
            this._impulse.Z = 0.0f;
          }
        }
        else
        {
          this._limitState = LimitState.InactiveLimit;
          this._impulse.Z = 0.0f;
        }
      }
      if (!this._enableMotor)
        this._motorImpulse = 0.0f;
      if (step.WarmStarting)
      {
        this._impulse *= step.DtRatio;
        this._motorImpulse *= step.DtRatio;
        Vec2 vec2_2 = this._impulse.X * this._perp + (this._motorImpulse + this._impulse.Z) * this._axis;
        float num4 = (float) ((double) this._impulse.X * (double) this._s1 + (double) this._impulse.Y + ((double) this._motorImpulse + (double) this._impulse.Z) * (double) this._a1);
        float num5 = (float) ((double) this._impulse.X * (double) this._s2 + (double) this._impulse.Y + ((double) this._motorImpulse + (double) this._impulse.Z) * (double) this._a2);
        body1._linearVelocity -= this._invMass1 * vec2_2;
        body1._angularVelocity -= this._invI1 * num4;
        body2._linearVelocity += this._invMass2 * vec2_2;
        body2._angularVelocity += this._invI2 * num5;
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
      if (this._enableMotor && this._limitState != LimitState.EqualLimits)
      {
        float num1 = this._motorMass * (this._motorSpeed - (float) ((double) Vec2.Dot(this._axis, linearVelocity2 - linearVelocity1) + (double) this._a2 * (double) angularVelocity2 - (double) this._a1 * (double) angularVelocity1));
        float motorImpulse = this._motorImpulse;
        float high = step.Dt * this._maxMotorForce;
        this._motorImpulse = Math.Clamp(this._motorImpulse + num1, -high, high);
        float num2 = this._motorImpulse - motorImpulse;
        Vec2 vec2 = num2 * this._axis;
        float num3 = num2 * this._a1;
        float num4 = num2 * this._a2;
        linearVelocity1 -= this._invMass1 * vec2;
        angularVelocity1 -= this._invI1 * num3;
        linearVelocity2 += this._invMass2 * vec2;
        angularVelocity2 += this._invI2 * num4;
      }
      Vec2 vec2_1;
      vec2_1.X = (float) ((double) Vec2.Dot(this._perp, linearVelocity2 - linearVelocity1) + (double) this._s2 * (double) angularVelocity2 - (double) this._s1 * (double) angularVelocity1);
      vec2_1.Y = angularVelocity2 - angularVelocity1;
      Vec2 vec2_2;
      float num5;
      Vec2 vec2_3;
      float num6;
      if (this._enableLimit && this._limitState != LimitState.InactiveLimit)
      {
        float z = (float) ((double) Vec2.Dot(this._axis, linearVelocity2 - linearVelocity1) + (double) this._a2 * (double) angularVelocity2 - (double) this._a1 * (double) angularVelocity1);
        Vec3 vec3_1 = new Vec3(vec2_1.X, vec2_1.Y, z);
        Vec3 impulse = this._impulse;
        this._impulse += this._K.Solve33(-vec3_1);
        if (this._limitState == LimitState.AtLowerLimit)
          this._impulse.Z = Math.Max(this._impulse.Z, 0.0f);
        else if (this._limitState == LimitState.AtUpperLimit)
          this._impulse.Z = Math.Min(this._impulse.Z, 0.0f);
        Vec2 vec2_4 = this._K.Solve22(-vec2_1 - (this._impulse.Z - impulse.Z) * new Vec2(this._K.Col3.X, this._K.Col3.Y)) + new Vec2(impulse.X, impulse.Y);
        this._impulse.X = vec2_4.X;
        this._impulse.Y = vec2_4.Y;
        Vec3 vec3_2 = this._impulse - impulse;
        Vec2 vec2_5 = vec3_2.X * this._perp + vec3_2.Z * this._axis;
        float num1 = (float) ((double) vec3_2.X * (double) this._s1 + (double) vec3_2.Y + (double) vec3_2.Z * (double) this._a1);
        float num2 = (float) ((double) vec3_2.X * (double) this._s2 + (double) vec3_2.Y + (double) vec3_2.Z * (double) this._a2);
        vec2_2 = linearVelocity1 - this._invMass1 * vec2_5;
        num5 = angularVelocity1 - this._invI1 * num1;
        vec2_3 = linearVelocity2 + this._invMass2 * vec2_5;
        num6 = angularVelocity2 + this._invI2 * num2;
      }
      else
      {
        Vec2 vec2_4 = this._K.Solve22(-vec2_1);
        this._impulse.X += vec2_4.X;
        this._impulse.Y += vec2_4.Y;
        Vec2 vec2_5 = vec2_4.X * this._perp;
        float num1 = vec2_4.X * this._s1 + vec2_4.Y;
        float num2 = vec2_4.X * this._s2 + vec2_4.Y;
        vec2_2 = linearVelocity1 - this._invMass1 * vec2_5;
        num5 = angularVelocity1 - this._invI1 * num1;
        vec2_3 = linearVelocity2 + this._invMass2 * vec2_5;
        num6 = angularVelocity2 + this._invI2 * num2;
      }
      body1._linearVelocity = vec2_2;
      body1._angularVelocity = num5;
      body2._linearVelocity = vec2_3;
      body2._angularVelocity = num6;
    }

    internal override bool SolvePositionConstraints(float baumgarte)
    {
      Body body1 = this._body1;
      Body body2 = this._body2;
      Vec2 c1 = body1._sweep.C;
      float a1 = body1._sweep.A;
      Vec2 c2 = body2._sweep.C;
      float a2 = body2._sweep.A;
      float a3 = 0.0f;
      bool flag = false;
      float num1 = 0.0f;
      Mat22 A1 = new Mat22(a1);
      Mat22 A2 = new Mat22(a2);
      Vec2 vec2_1 = Math.Mul(A1, this._localAnchor1 - this._localCenter1);
      Vec2 a4 = Math.Mul(A2, this._localAnchor2 - this._localCenter2);
      Vec2 b = c2 + a4 - c1 - vec2_1;
      if (this._enableLimit)
      {
        this._axis = Math.Mul(A1, this._localXAxis1);
        this._a1 = Vec2.Cross(b + vec2_1, this._axis);
        this._a2 = Vec2.Cross(a4, this._axis);
        float a5 = Vec2.Dot(this._axis, b);
        if ((double) Math.Abs(this._upperTranslation - this._lowerTranslation) < 2.0 * (double) Settings.LinearSlop)
        {
          num1 = Math.Clamp(a5, -Settings.MaxLinearCorrection, Settings.MaxLinearCorrection);
          a3 = Math.Abs(a5);
          flag = true;
        }
        else if ((double) a5 <= (double) this._lowerTranslation)
        {
          num1 = Math.Clamp(a5 - this._lowerTranslation + Settings.LinearSlop, -Settings.MaxLinearCorrection, 0.0f);
          a3 = this._lowerTranslation - a5;
          flag = true;
        }
        else if ((double) a5 >= (double) this._upperTranslation)
        {
          num1 = Math.Clamp(a5 - this._upperTranslation - Settings.LinearSlop, 0.0f, Settings.MaxLinearCorrection);
          a3 = a5 - this._upperTranslation;
          flag = true;
        }
      }
      this._perp = Math.Mul(A1, this._localYAxis1);
      this._s1 = Vec2.Cross(b + vec2_1, this._perp);
      this._s2 = Vec2.Cross(a4, this._perp);
      Vec2 vec2_2 = new Vec2();
      vec2_2.X = Vec2.Dot(this._perp, b);
      vec2_2.Y = a2 - a1 - this._refAngle;
      float num2 = Math.Max(a3, Math.Abs(vec2_2.X));
      float num3 = Math.Abs(vec2_2.Y);
      Vec3 vec3;
      if (flag)
      {
        float invMass1 = this._invMass1;
        float invMass2 = this._invMass2;
        float invI1 = this._invI1;
        float invI2 = this._invI2;
        float x = (float) ((double) invMass1 + (double) invMass2 + (double) invI1 * (double) this._s1 * (double) this._s1 + (double) invI2 * (double) this._s2 * (double) this._s2);
        float num4 = (float) ((double) invI1 * (double) this._s1 + (double) invI2 * (double) this._s2);
        float num5 = (float) ((double) invI1 * (double) this._s1 * (double) this._a1 + (double) invI2 * (double) this._s2 * (double) this._a2);
        float y = invI1 + invI2;
        float num6 = (float) ((double) invI1 * (double) this._a1 + (double) invI2 * (double) this._a2);
        float z = (float) ((double) invMass1 + (double) invMass2 + (double) invI1 * (double) this._a1 * (double) this._a1 + (double) invI2 * (double) this._a2 * (double) this._a2);
        this._K.Col1.Set(x, num4, num5);
        this._K.Col2.Set(num4, y, num6);
        this._K.Col3.Set(num5, num6, z);
        vec3 = this._K.Solve33(-new Vec3()
        {
          X = vec2_2.X,
          Y = vec2_2.Y,
          Z = num1
        });
      }
      else
      {
        float invMass1 = this._invMass1;
        float invMass2 = this._invMass2;
        float invI1 = this._invI1;
        float invI2 = this._invI2;
        float x = (float) ((double) invMass1 + (double) invMass2 + (double) invI1 * (double) this._s1 * (double) this._s1 + (double) invI2 * (double) this._s2 * (double) this._s2);
        float num4 = (float) ((double) invI1 * (double) this._s1 + (double) invI2 * (double) this._s2);
        float y = invI1 + invI2;
        this._K.Col1.Set(x, num4, 0.0f);
        this._K.Col2.Set(num4, y, 0.0f);
        Vec2 vec2_3 = this._K.Solve22(-vec2_2);
        vec3.X = vec2_3.X;
        vec3.Y = vec2_3.Y;
        vec3.Z = 0.0f;
      }
      Vec2 vec2_4 = vec3.X * this._perp + vec3.Z * this._axis;
      float num7 = (float) ((double) vec3.X * (double) this._s1 + (double) vec3.Y + (double) vec3.Z * (double) this._a1);
      float num8 = (float) ((double) vec3.X * (double) this._s2 + (double) vec3.Y + (double) vec3.Z * (double) this._a2);
      Vec2 vec2_5 = c1 - this._invMass1 * vec2_4;
      float num9 = a1 - this._invI1 * num7;
      Vec2 vec2_6 = c2 + this._invMass2 * vec2_4;
      float num10 = a2 + this._invI2 * num8;
      body1._sweep.C = vec2_5;
      body1._sweep.A = num9;
      body2._sweep.C = vec2_6;
      body2._sweep.A = num10;
      body1.SynchronizeTransform();
      body2.SynchronizeTransform();
      return (double) num2 <= (double) Settings.LinearSlop && (double) num3 <= (double) Settings.AngularSlop;
    }
  }
}
