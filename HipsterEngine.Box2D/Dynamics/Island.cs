using Box2DX.Collision;
using Box2DX.Common;
using System;

namespace Box2DX.Dynamics
{
  public class Island : IDisposable
  {
    public ContactListener _listener;
    public Body[] _bodies;
    public Contact[] _contacts;
    public Joint[] _joints;
    public Position[] _positions;
    public Velocity[] _velocities;
    public int _bodyCount;
    public int _jointCount;
    public int _contactCount;
    public int _bodyCapacity;
    public int _contactCapacity;
    public int _jointCapacity;
    public int _positionIterationCount;

    public Island(int bodyCapacity, int contactCapacity, int jointCapacity, ContactListener listener)
    {
      this._bodyCapacity = bodyCapacity;
      this._contactCapacity = contactCapacity;
      this._jointCapacity = jointCapacity;
      this._bodyCount = 0;
      this._contactCount = 0;
      this._jointCount = 0;
      this._listener = listener;
      this._bodies = new Body[bodyCapacity];
      this._contacts = new Contact[contactCapacity];
      this._joints = new Joint[jointCapacity];
      this._velocities = new Velocity[this._bodyCapacity];
      this._positions = new Position[this._bodyCapacity];
    }

    public void Dispose()
    {
      this._positions = (Position[]) null;
      this._velocities = (Velocity[]) null;
      this._joints = (Joint[]) null;
      this._contacts = (Contact[]) null;
      this._bodies = (Body[]) null;
    }

    public void Clear()
    {
      this._bodyCount = 0;
      this._contactCount = 0;
      this._jointCount = 0;
    }

    public void Solve(TimeStep step, Vec2 gravity, bool allowSleep)
    {
      for (int index = 0; index < this._bodyCount; ++index)
      {
        Body body = this._bodies[index];
        if (!body.IsStatic())
        {
          body._linearVelocity += step.Dt * (gravity + body._invMass * body._force);
          body._angularVelocity += step.Dt * body._invI * body._torque;
          body._force.Set(0.0f, 0.0f);
          body._torque = 0.0f;
          body._linearVelocity *= Box2DX.Common.Math.Clamp((float) (1.0 - (double) step.Dt * (double) body._linearDamping), 0.0f, 1f);
          body._angularVelocity *= Box2DX.Common.Math.Clamp((float) (1.0 - (double) step.Dt * (double) body._angularDamping), 0.0f, 1f);
          if ((double) Vec2.Dot(body._linearVelocity, body._linearVelocity) > (double) Settings.MaxLinearVelocitySquared)
          {
            double num = (double) body._linearVelocity.Normalize();
            body._linearVelocity *= Settings.MaxLinearVelocity;
          }
          if ((double) body._angularVelocity * (double) body._angularVelocity > (double) Settings.MaxAngularVelocitySquared)
            body._angularVelocity = (double) body._angularVelocity >= 0.0 ? Settings.MaxAngularVelocity : -Settings.MaxAngularVelocity;
        }
      }
      ContactSolver contactSolver = new ContactSolver(step, this._contacts, this._contactCount);
      contactSolver.InitVelocityConstraints(step);
      for (int index = 0; index < this._jointCount; ++index)
        this._joints[index].InitVelocityConstraints(step);
      for (int index1 = 0; index1 < step.VelocityIterations; ++index1)
      {
        for (int index2 = 0; index2 < this._jointCount; ++index2)
          this._joints[index2].SolveVelocityConstraints(step);
        contactSolver.SolveVelocityConstraints();
      }
      contactSolver.FinalizeVelocityConstraints();
      for (int index = 0; index < this._bodyCount; ++index)
      {
        Body body = this._bodies[index];
        if (!body.IsStatic())
        {
          body._sweep.C0 = body._sweep.C;
          body._sweep.A0 = body._sweep.A;
          body._sweep.C += step.Dt * body._linearVelocity;
          body._sweep.A += step.Dt * body._angularVelocity;
          body.SynchronizeTransform();
        }
      }
      for (int index1 = 0; index1 < step.PositionIterations; ++index1)
      {
        bool flag1 = contactSolver.SolvePositionConstraints(Settings.ContactBaumgarte);
        bool flag2 = true;
        for (int index2 = 0; index2 < this._jointCount; ++index2)
        {
          bool flag3 = this._joints[index2].SolvePositionConstraints(Settings.ContactBaumgarte);
          flag2 = flag2 && flag3;
        }
        if (flag1 && flag2)
          break;
      }
      this.Report(contactSolver._constraints);
      if (!allowSleep)
        return;
      float a = Settings.FLT_MAX;
      float num1 = Settings.LinearSleepTolerance * Settings.LinearSleepTolerance;
      float num2 = Settings.AngularSleepTolerance * Settings.AngularSleepTolerance;
      for (int index = 0; index < this._bodyCount; ++index)
      {
        Body body = this._bodies[index];
        if ((double) body._invMass != 0.0)
        {
          if ((body._flags & Body.BodyFlags.AllowSleep) == (Body.BodyFlags) 0)
          {
            body._sleepTime = 0.0f;
            a = 0.0f;
          }
          if ((body._flags & Body.BodyFlags.AllowSleep) == (Body.BodyFlags) 0 || (double) body._angularVelocity * (double) body._angularVelocity > (double) num2 || (double) Vec2.Dot(body._linearVelocity, body._linearVelocity) > (double) num1)
          {
            body._sleepTime = 0.0f;
            a = 0.0f;
          }
          else
          {
            body._sleepTime += step.Dt;
            a = Box2DX.Common.Math.Min(a, body._sleepTime);
          }
        }
      }
      if ((double) a >= (double) Settings.TimeToSleep)
      {
        for (int index = 0; index < this._bodyCount; ++index)
        {
          Body body = this._bodies[index];
          body._flags |= Body.BodyFlags.Sleep;
          body._linearVelocity = Vec2.Zero;
          body._angularVelocity = 0.0f;
        }
      }
    }

    public void SolveTOI(ref TimeStep subStep)
    {
      ContactSolver contactSolver = new ContactSolver(subStep, this._contacts, this._contactCount);
      for (int index = 0; index < subStep.VelocityIterations; ++index)
        contactSolver.SolveVelocityConstraints();
      for (int index = 0; index < this._bodyCount; ++index)
      {
        Body body = this._bodies[index];
        if (!body.IsStatic())
        {
          body._sweep.C0 = body._sweep.C;
          body._sweep.A0 = body._sweep.A;
          body._sweep.C += subStep.Dt * body._linearVelocity;
          body._sweep.A += subStep.Dt * body._angularVelocity;
          body.SynchronizeTransform();
        }
      }
      float baumgarte = 0.75f;
      int num = 0;
      while (num < subStep.PositionIterations && !contactSolver.SolvePositionConstraints(baumgarte))
        ++num;
      this.Report(contactSolver._constraints);
    }

    public void Add(Body body)
    {
      Box2DXDebug.Assert(this._bodyCount < this._bodyCapacity);
      body._islandIndex = this._bodyCount;
      this._bodies[this._bodyCount++] = body;
    }

    public void Add(Contact contact)
    {
      Box2DXDebug.Assert(this._contactCount < this._contactCapacity);
      this._contacts[this._contactCount++] = contact;
    }

    public void Add(Joint joint)
    {
      Box2DXDebug.Assert(this._jointCount < this._jointCapacity);
      this._joints[this._jointCount++] = joint;
    }

    public void Report(ContactConstraint[] constraints)
    {
      if (this._listener == null)
        return;
      for (int index1 = 0; index1 < this._contactCount; ++index1)
      {
        Contact contact = this._contacts[index1];
        ContactConstraint constraint = constraints[index1];
        ContactResult point1 = new ContactResult();
        point1.Shape1 = contact.GetShape1();
        point1.Shape2 = contact.GetShape2();
        Body body = point1.Shape1.GetBody();
        int manifoldCount = contact.GetManifoldCount();
        Manifold[] manifolds = contact.GetManifolds();
        for (int index2 = 0; index2 < manifoldCount; ++index2)
        {
          Manifold manifold = manifolds[index2];
          point1.Normal = manifold.Normal;
          for (int index3 = 0; index3 < manifold.PointCount; ++index3)
          {
            ManifoldPoint point2 = manifold.Points[index3];
            ContactConstraintPoint point3 = constraint.Points[index3];
            point1.Position = body.GetWorldPoint(point2.LocalPoint1);
            point1.NormalImpulse = point3.NormalImpulse;
            point1.TangentImpulse = point3.TangentImpulse;
            point1.ID = point2.ID;
            this._listener.Result(point1);
          }
        }
      }
    }
  }
}
