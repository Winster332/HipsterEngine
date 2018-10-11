using Box2DX.Collision;
using Box2DX.Common;
using System;

namespace Box2DX.Dynamics
{
  public class ContactSolver : IDisposable
  {
    public TimeStep _step;
    public ContactConstraint[] _constraints;
    public int _constraintCount;

    public ContactSolver(TimeStep step, Contact[] contacts, int contactCount)
    {
      this._step = step;
      this._constraintCount = 0;
      for (int index = 0; index < contactCount; ++index)
      {
        Box2DXDebug.Assert(contacts[index].IsSolid());
        this._constraintCount += contacts[index].GetManifoldCount();
      }
      this._constraints = new ContactConstraint[this._constraintCount];
      for (int index = 0; index < this._constraintCount; ++index)
        this._constraints[index] = new ContactConstraint();
      int index1 = 0;
      for (int index2 = 0; index2 < contactCount; ++index2)
      {
        Contact contact = contacts[index2];
        Shape shape1 = contact._shape1;
        Shape shape2 = contact._shape2;
        Body body1 = shape1.GetBody();
        Body body2 = shape2.GetBody();
        int manifoldCount = contact.GetManifoldCount();
        Manifold[] manifolds = contact.GetManifolds();
        float num1 = Settings.MixFriction(shape1.Friction, shape2.Friction);
        float num2 = Settings.MixRestitution(shape1.Restitution, shape2.Restitution);
        Vec2 linearVelocity1 = body1._linearVelocity;
        Vec2 linearVelocity2 = body2._linearVelocity;
        float angularVelocity1 = body1._angularVelocity;
        float angularVelocity2 = body2._angularVelocity;
        for (int index3 = 0; index3 < manifoldCount; ++index3)
        {
          Manifold manifold = manifolds[index3];
          Box2DXDebug.Assert(manifold.PointCount > 0);
          Vec2 normal = manifold.Normal;
          Box2DXDebug.Assert(index1 < this._constraintCount);
          ContactConstraint constraint = this._constraints[index1];
          constraint.Body1 = body1;
          constraint.Body2 = body2;
          constraint.Manifold = manifold;
          constraint.Normal = normal;
          constraint.PointCount = manifold.PointCount;
          constraint.Friction = num1;
          constraint.Restitution = num2;
          for (int index4 = 0; index4 < constraint.PointCount; ++index4)
          {
            ManifoldPoint point1 = manifold.Points[index4];
            ContactConstraintPoint point2 = constraint.Points[index4];
            point2.NormalImpulse = point1.NormalImpulse;
            point2.TangentImpulse = point1.TangentImpulse;
            point2.Separation = point1.Separation;
            point2.LocalAnchor1 = point1.LocalPoint1;
            point2.LocalAnchor2 = point1.LocalPoint2;
            point2.R1 = Box2DX.Common.Math.Mul(body1.GetXForm().R, point1.LocalPoint1 - body1.GetLocalCenter());
            point2.R2 = Box2DX.Common.Math.Mul(body2.GetXForm().R, point1.LocalPoint2 - body2.GetLocalCenter());
            float num3 = Vec2.Cross(point2.R1, normal);
            float num4 = Vec2.Cross(point2.R2, normal);
            float num5 = num3 * num3;
            float num6 = num4 * num4;
            float num7 = (float) ((double) body1._invMass + (double) body2._invMass + (double) body1._invI * (double) num5 + (double) body2._invI * (double) num6);
            Box2DXDebug.Assert((double) num7 > (double) Settings.FLT_EPSILON);
            point2.NormalMass = 1f / num7;
            float num8 = (float) ((double) body1._mass * (double) body1._invMass + (double) body2._mass * (double) body2._invMass) + (float) ((double) body1._mass * (double) body1._invI * (double) num5 + (double) body2._mass * (double) body2._invI * (double) num6);
            Box2DXDebug.Assert((double) num8 > (double) Settings.FLT_EPSILON);
            point2.EqualizedMass = 1f / num8;
            Vec2 b = Vec2.Cross(normal, 1f);
            float num9 = Vec2.Cross(point2.R1, b);
            float num10 = Vec2.Cross(point2.R2, b);
            float num11 = num9 * num9;
            float num12 = num10 * num10;
            float num13 = (float) ((double) body1._invMass + (double) body2._invMass + (double) body1._invI * (double) num11 + (double) body2._invI * (double) num12);
            Box2DXDebug.Assert((double) num13 > (double) Settings.FLT_EPSILON);
            point2.TangentMass = 1f / num13;
            point2.VelocityBias = 0.0f;
            if ((double) point2.Separation > 0.0)
            {
              point2.VelocityBias = -step.Inv_Dt * point2.Separation;
            }
            else
            {
              float num14 = Vec2.Dot(constraint.Normal, linearVelocity2 + Vec2.Cross(angularVelocity2, point2.R2) - linearVelocity1 - Vec2.Cross(angularVelocity1, point2.R1));
              if ((double) num14 < -(double) Settings.VelocityThreshold)
                point2.VelocityBias = -constraint.Restitution * num14;
            }
          }
          if (constraint.PointCount == 2)
          {
            ContactConstraintPoint point1 = constraint.Points[0];
            ContactConstraintPoint point2 = constraint.Points[1];
            float invMass1 = body1._invMass;
            float invI1 = body1._invI;
            float invMass2 = body2._invMass;
            float invI2 = body2._invI;
            float num3 = Vec2.Cross(point1.R1, normal);
            float num4 = Vec2.Cross(point1.R2, normal);
            float num5 = Vec2.Cross(point2.R1, normal);
            float num6 = Vec2.Cross(point2.R2, normal);
            float x = (float) ((double) invMass1 + (double) invMass2 + (double) invI1 * (double) num3 * (double) num3 + (double) invI2 * (double) num4 * (double) num4);
            float y = (float) ((double) invMass1 + (double) invMass2 + (double) invI1 * (double) num5 * (double) num5 + (double) invI2 * (double) num6 * (double) num6);
            float num7 = (float) ((double) invMass1 + (double) invMass2 + (double) invI1 * (double) num3 * (double) num5 + (double) invI2 * (double) num4 * (double) num6);
            if ((double) x * (double) x < 100.0 * ((double) x * (double) y - (double) num7 * (double) num7))
            {
              constraint.K.Col1.Set(x, num7);
              constraint.K.Col2.Set(num7, y);
              constraint.NormalMass = constraint.K.Invert();
            }
            else
              constraint.PointCount = 1;
          }
          ++index1;
        }
      }
      Box2DXDebug.Assert(index1 == this._constraintCount);
    }

    public void Dispose()
    {
      this._constraints = (ContactConstraint[]) null;
    }

    public void InitVelocityConstraints(TimeStep step)
    {
      for (int index1 = 0; index1 < this._constraintCount; ++index1)
      {
        ContactConstraint constraint = this._constraints[index1];
        Body body1 = constraint.Body1;
        Body body2 = constraint.Body2;
        float invMass1 = body1._invMass;
        float invI1 = body1._invI;
        float invMass2 = body2._invMass;
        float invI2 = body2._invI;
        Vec2 normal = constraint.Normal;
        Vec2 vec2 = Vec2.Cross(normal, 1f);
        if (step.WarmStarting)
        {
          for (int index2 = 0; index2 < constraint.PointCount; ++index2)
          {
            ContactConstraintPoint point = constraint.Points[index2];
            point.NormalImpulse *= step.DtRatio;
            point.TangentImpulse *= step.DtRatio;
            Vec2 b = point.NormalImpulse * normal + point.TangentImpulse * vec2;
            body1._angularVelocity -= invI1 * Vec2.Cross(point.R1, b);
            body1._linearVelocity -= invMass1 * b;
            body2._angularVelocity += invI2 * Vec2.Cross(point.R2, b);
            body2._linearVelocity += invMass2 * b;
          }
        }
        else
        {
          for (int index2 = 0; index2 < constraint.PointCount; ++index2)
          {
            ContactConstraintPoint point = constraint.Points[index2];
            point.NormalImpulse = 0.0f;
            point.TangentImpulse = 0.0f;
          }
        }
      }
    }

    public void SolveVelocityConstraints()
    {
      for (int index1 = 0; index1 < this._constraintCount; ++index1)
      {
        ContactConstraint constraint = this._constraints[index1];
        Body body1 = constraint.Body1;
        Body body2 = constraint.Body2;
        float angularVelocity1 = body1._angularVelocity;
        float angularVelocity2 = body2._angularVelocity;
        Vec2 linearVelocity1 = body1._linearVelocity;
        Vec2 linearVelocity2 = body2._linearVelocity;
        float invMass1 = body1._invMass;
        float invI1 = body1._invI;
        float invMass2 = body2._invMass;
        float invI2 = body2._invI;
        Vec2 normal = constraint.Normal;
        Vec2 b1 = Vec2.Cross(normal, 1f);
        float friction = constraint.Friction;
        Box2DXDebug.Assert(constraint.PointCount == 1 || constraint.PointCount == 2);
        if (constraint.PointCount == 1)
        {
          ContactConstraintPoint point = constraint.Points[0];
          float num1 = Vec2.Dot(linearVelocity2 + Vec2.Cross(angularVelocity2, point.R2) - linearVelocity1 - Vec2.Cross(angularVelocity1, point.R1), normal);
          float num2 = (float) (-(double) point.NormalMass * ((double) num1 - (double) point.VelocityBias));
          float num3 = Box2DX.Common.Math.Max(point.NormalImpulse + num2, 0.0f);
          Vec2 b2 = (num3 - point.NormalImpulse) * normal;
          linearVelocity1 -= invMass1 * b2;
          angularVelocity1 -= invI1 * Vec2.Cross(point.R1, b2);
          linearVelocity2 += invMass2 * b2;
          angularVelocity2 += invI2 * Vec2.Cross(point.R2, b2);
          point.NormalImpulse = num3;
        }
        else
        {
          ContactConstraintPoint point1 = constraint.Points[0];
          ContactConstraintPoint point2 = constraint.Points[1];
          Vec2 v1 = new Vec2(point1.NormalImpulse, point2.NormalImpulse);
          Box2DXDebug.Assert((double) v1.X >= 0.0 && (double) v1.Y >= 0.0);
          Vec2 a1 = linearVelocity2 + Vec2.Cross(angularVelocity2, point1.R2) - linearVelocity1 - Vec2.Cross(angularVelocity1, point1.R1);
          Vec2 a2 = linearVelocity2 + Vec2.Cross(angularVelocity2, point2.R2) - linearVelocity1 - Vec2.Cross(angularVelocity1, point2.R1);
          float num1 = Vec2.Dot(a1, normal);
          float num2 = Vec2.Dot(a2, normal);
          Vec2 v2;
          v2.X = num1 - point1.VelocityBias;
          v2.Y = num2 - point2.VelocityBias;
          v2 -= Box2DX.Common.Math.Mul(constraint.K, v1);
          Vec2 vec2_1 = -Box2DX.Common.Math.Mul(constraint.NormalMass, v2);
          if ((double) vec2_1.X >= 0.0 && (double) vec2_1.Y >= 0.0)
          {
            Vec2 vec2_2 = vec2_1 - v1;
            Vec2 b2 = vec2_2.X * normal;
            Vec2 b3 = vec2_2.Y * normal;
            linearVelocity1 -= invMass1 * (b2 + b3);
            angularVelocity1 -= invI1 * (Vec2.Cross(point1.R1, b2) + Vec2.Cross(point2.R1, b3));
            linearVelocity2 += invMass2 * (b2 + b3);
            angularVelocity2 += invI2 * (Vec2.Cross(point1.R2, b2) + Vec2.Cross(point2.R2, b3));
            point1.NormalImpulse = vec2_1.X;
            point2.NormalImpulse = vec2_1.Y;
          }
          else
          {
            vec2_1.X = -point1.NormalMass * v2.X;
            vec2_1.Y = 0.0f;
            float num3 = constraint.K.Col1.Y * vec2_1.X + v2.Y;
            if ((double) vec2_1.X >= 0.0 && (double) num3 >= 0.0)
            {
              Vec2 vec2_2 = vec2_1 - v1;
              Vec2 b2 = vec2_2.X * normal;
              Vec2 b3 = vec2_2.Y * normal;
              linearVelocity1 -= invMass1 * (b2 + b3);
              angularVelocity1 -= invI1 * (Vec2.Cross(point1.R1, b2) + Vec2.Cross(point2.R1, b3));
              linearVelocity2 += invMass2 * (b2 + b3);
              angularVelocity2 += invI2 * (Vec2.Cross(point1.R2, b2) + Vec2.Cross(point2.R2, b3));
              point1.NormalImpulse = vec2_1.X;
              point2.NormalImpulse = vec2_1.Y;
            }
            else
            {
              vec2_1.X = 0.0f;
              vec2_1.Y = -point2.NormalMass * v2.Y;
              float num4 = constraint.K.Col2.X * vec2_1.Y + v2.X;
              if ((double) vec2_1.Y >= 0.0 && (double) num4 >= 0.0)
              {
                Vec2 vec2_2 = vec2_1 - v1;
                Vec2 b2 = vec2_2.X * normal;
                Vec2 b3 = vec2_2.Y * normal;
                linearVelocity1 -= invMass1 * (b2 + b3);
                angularVelocity1 -= invI1 * (Vec2.Cross(point1.R1, b2) + Vec2.Cross(point2.R1, b3));
                linearVelocity2 += invMass2 * (b2 + b3);
                angularVelocity2 += invI2 * (Vec2.Cross(point1.R2, b2) + Vec2.Cross(point2.R2, b3));
                point1.NormalImpulse = vec2_1.X;
                point2.NormalImpulse = vec2_1.Y;
              }
              else
              {
                vec2_1.X = 0.0f;
                vec2_1.Y = 0.0f;
                if ((double) v2.X >= 0.0 && (double) v2.Y >= 0.0)
                {
                  Vec2 vec2_2 = vec2_1 - v1;
                  Vec2 b2 = vec2_2.X * normal;
                  Vec2 b3 = vec2_2.Y * normal;
                  linearVelocity1 -= invMass1 * (b2 + b3);
                  angularVelocity1 -= invI1 * (Vec2.Cross(point1.R1, b2) + Vec2.Cross(point2.R1, b3));
                  linearVelocity2 += invMass2 * (b2 + b3);
                  angularVelocity2 += invI2 * (Vec2.Cross(point1.R2, b2) + Vec2.Cross(point2.R2, b3));
                  point1.NormalImpulse = vec2_1.X;
                  point2.NormalImpulse = vec2_1.Y;
                }
              }
            }
          }
        }
        for (int index2 = 0; index2 < constraint.PointCount; ++index2)
        {
          ContactConstraintPoint point = constraint.Points[index2];
          float num1 = Vec2.Dot(linearVelocity2 + Vec2.Cross(angularVelocity2, point.R2) - linearVelocity1 - Vec2.Cross(angularVelocity1, point.R1), b1);
          float num2 = point.TangentMass * -num1;
          float high = friction * point.NormalImpulse;
          float num3 = Box2DX.Common.Math.Clamp(point.TangentImpulse + num2, -high, high);
          Vec2 b2 = (num3 - point.TangentImpulse) * b1;
          linearVelocity1 -= invMass1 * b2;
          angularVelocity1 -= invI1 * Vec2.Cross(point.R1, b2);
          linearVelocity2 += invMass2 * b2;
          angularVelocity2 += invI2 * Vec2.Cross(point.R2, b2);
          point.TangentImpulse = num3;
        }
        body1._linearVelocity = linearVelocity1;
        body1._angularVelocity = angularVelocity1;
        body2._linearVelocity = linearVelocity2;
        body2._angularVelocity = angularVelocity2;
      }
    }

    public void FinalizeVelocityConstraints()
    {
      for (int index1 = 0; index1 < this._constraintCount; ++index1)
      {
        ContactConstraint constraint = this._constraints[index1];
        Manifold manifold = constraint.Manifold;
        for (int index2 = 0; index2 < constraint.PointCount; ++index2)
        {
          manifold.Points[index2].NormalImpulse = constraint.Points[index2].NormalImpulse;
          manifold.Points[index2].TangentImpulse = constraint.Points[index2].TangentImpulse;
        }
      }
    }

    public bool SolvePositionConstraints(float baumgarte)
    {
      float a1 = 0.0f;
      for (int index1 = 0; index1 < this._constraintCount; ++index1)
      {
        ContactConstraint constraint = this._constraints[index1];
        Body body1 = constraint.Body1;
        Body body2 = constraint.Body2;
        float num1 = body1._mass * body1._invMass;
        float num2 = body1._mass * body1._invI;
        float num3 = body2._mass * body2._invMass;
        float num4 = body2._mass * body2._invI;
        Vec2 normal = constraint.Normal;
        for (int index2 = 0; index2 < constraint.PointCount; ++index2)
        {
          ContactConstraintPoint point = constraint.Points[index2];
          Vec2 a2 = Box2DX.Common.Math.Mul(body1.GetXForm().R, point.LocalAnchor1 - body1.GetLocalCenter());
          Vec2 a3 = Box2DX.Common.Math.Mul(body2.GetXForm().R, point.LocalAnchor2 - body2.GetLocalCenter());
          Vec2 vec2 = body1._sweep.C + a2;
          float b1 = Vec2.Dot(body2._sweep.C + a3 - vec2, normal) + point.Separation;
          a1 = Box2DX.Common.Math.Min(a1, b1);
          float num5 = baumgarte * Box2DX.Common.Math.Clamp(b1 + Settings.LinearSlop, -Settings.MaxLinearCorrection, 0.0f);
          Vec2 b2 = -point.EqualizedMass * num5 * normal;
          body1._sweep.C -= num1 * b2;
          body1._sweep.A -= num2 * Vec2.Cross(a2, b2);
          body1.SynchronizeTransform();
          body2._sweep.C += num3 * b2;
          body2._sweep.A += num4 * Vec2.Cross(a3, b2);
          body2.SynchronizeTransform();
        }
      }
      return (double) a1 >= -1.5 * (double) Settings.LinearSlop;
    }
  }
}
