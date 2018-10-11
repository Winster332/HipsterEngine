// Decompiled with JetBrains decompiler
// Type: Box2DX.Dynamics.CircleContact
// Assembly: Box2DX, Version=2.0.1.28279, Culture=neutral, PublicKeyToken=null
// MVID: BD116DE6-476A-4E37-B8FC-78B19E349C03
// Assembly location: /Users/kusy/Documents/Новая папка/HipsterEngine/bin/Debug/Box2DX.dll

using Box2DX.Collision;
using Box2DX.Common;

namespace Box2DX.Dynamics
{
  public class CircleContact : Contact
  {
    public Manifold _manifold = new Manifold();

    public override Manifold[] GetManifolds()
    {
      return new Manifold[1]{ this._manifold };
    }

    public CircleContact(Shape s1, Shape s2)
      : base(s1, s2)
    {
      Box2DXDebug.Assert(this._shape1.GetType() == ShapeType.CircleShape);
      Box2DXDebug.Assert(this._shape2.GetType() == ShapeType.CircleShape);
      this._manifold.PointCount = 0;
      this._manifold.Points[0].NormalImpulse = 0.0f;
      this._manifold.Points[0].TangentImpulse = 0.0f;
    }

    public override void Evaluate(ContactListener listener)
    {
      Body body1 = this._shape1.GetBody();
      Body body2 = this._shape2.GetBody();
      Manifold manifold = this._manifold.Clone();
      Box2DX.Collision.Collision.CollideCircles(ref this._manifold, (CircleShape) this._shape1, body1.GetXForm(), (CircleShape) this._shape2, body2.GetXForm());
      ContactPoint point1 = new ContactPoint();
      point1.Shape1 = this._shape1;
      point1.Shape2 = this._shape2;
      point1.Friction = Settings.MixFriction(this._shape1.Friction, this._shape2.Friction);
      point1.Restitution = Settings.MixRestitution(this._shape1.Restitution, this._shape2.Restitution);
      if (this._manifold.PointCount > 0)
      {
        this._manifoldCount = 1;
        ManifoldPoint point2 = this._manifold.Points[0];
        if (manifold.PointCount == 0)
        {
          point2.NormalImpulse = 0.0f;
          point2.TangentImpulse = 0.0f;
          if (listener == null)
            return;
          point1.Position = body1.GetWorldPoint(point2.LocalPoint1);
          Vec2 velocityFromLocalPoint1 = body1.GetLinearVelocityFromLocalPoint(point2.LocalPoint1);
          Vec2 velocityFromLocalPoint2 = body2.GetLinearVelocityFromLocalPoint(point2.LocalPoint2);
          point1.Velocity = velocityFromLocalPoint2 - velocityFromLocalPoint1;
          point1.Normal = this._manifold.Normal;
          point1.Separation = point2.Separation;
          point1.ID = point2.ID;
          listener.Add(point1);
        }
        else
        {
          ManifoldPoint point3 = manifold.Points[0];
          point2.NormalImpulse = point3.NormalImpulse;
          point2.TangentImpulse = point3.TangentImpulse;
          if (listener != null)
          {
            point1.Position = body1.GetWorldPoint(point2.LocalPoint1);
            Vec2 velocityFromLocalPoint1 = body1.GetLinearVelocityFromLocalPoint(point2.LocalPoint1);
            Vec2 velocityFromLocalPoint2 = body2.GetLinearVelocityFromLocalPoint(point2.LocalPoint2);
            point1.Velocity = velocityFromLocalPoint2 - velocityFromLocalPoint1;
            point1.Normal = this._manifold.Normal;
            point1.Separation = point2.Separation;
            point1.ID = point2.ID;
            listener.Persist(point1);
          }
        }
      }
      else
      {
        this._manifoldCount = 0;
        if (manifold.PointCount > 0 && listener != null)
        {
          ManifoldPoint point2 = manifold.Points[0];
          point1.Position = body1.GetWorldPoint(point2.LocalPoint1);
          Vec2 velocityFromLocalPoint1 = body1.GetLinearVelocityFromLocalPoint(point2.LocalPoint1);
          Vec2 velocityFromLocalPoint2 = body2.GetLinearVelocityFromLocalPoint(point2.LocalPoint2);
          point1.Velocity = velocityFromLocalPoint2 - velocityFromLocalPoint1;
          point1.Normal = manifold.Normal;
          point1.Separation = point2.Separation;
          point1.ID = point2.ID;
          listener.Remove(point1);
        }
      }
    }

    public new static Contact Create(Shape shape1, Shape shape2)
    {
      return (Contact) new CircleContact(shape1, shape2);
    }

    public new static void Destroy(Contact contact)
    {
      contact = (Contact) null;
    }
  }
}
