// Decompiled with JetBrains decompiler
// Type: Box2DX.Dynamics.ContactManager
// Assembly: Box2DX, Version=2.0.1.28279, Culture=neutral, PublicKeyToken=null
// MVID: BD116DE6-476A-4E37-B8FC-78B19E349C03
// Assembly location: /Users/kusy/Documents/Новая папка/HipsterEngine/bin/Debug/Box2DX.dll

using Box2DX.Collision;
using Box2DX.Common;

namespace Box2DX.Dynamics
{
  public class ContactManager : PairCallback
  {
    public World _world;
    public NullContact _nullContact;
    public bool _destroyImmediate;

    public ContactManager()
    {
      this._world = (World) null;
      this._destroyImmediate = false;
    }

    public override object PairAdded(object proxyUserData1, object proxyUserData2)
    {
      Shape shape1_1 = proxyUserData1 as Shape;
      Shape shape2_1 = proxyUserData2 as Shape;
      Body body1 = shape1_1.GetBody();
      Body body2 = shape2_1.GetBody();
      if (body1.IsStatic() && body2.IsStatic() || shape1_1.GetBody() == shape2_1.GetBody() || body2.IsConnected(body1) || this._world._contactFilter != null && !this._world._contactFilter.ShouldCollide(shape1_1, shape2_1))
        return (object) this._nullContact;
      Contact contact = Contact.Create(shape1_1, shape2_1);
      if (contact == null)
        return (object) this._nullContact;
      Shape shape1_2 = contact.GetShape1();
      Shape shape2_2 = contact.GetShape2();
      Body body3 = shape1_2.GetBody();
      Body body4 = shape2_2.GetBody();
      contact._prev = (Contact) null;
      contact._next = this._world._contactList;
      if (this._world._contactList != null)
        this._world._contactList._prev = contact;
      this._world._contactList = contact;
      contact._node1.Contact = contact;
      contact._node1.Other = body4;
      contact._node1.Prev = (ContactEdge) null;
      contact._node1.Next = body3._contactList;
      if (body3._contactList != null)
        body3._contactList.Prev = contact._node1;
      body3._contactList = contact._node1;
      contact._node2.Contact = contact;
      contact._node2.Other = body3;
      contact._node2.Prev = (ContactEdge) null;
      contact._node2.Next = body4._contactList;
      if (body4._contactList != null)
        body4._contactList.Prev = contact._node2;
      body4._contactList = contact._node2;
      ++this._world._contactCount;
      return (object) contact;
    }

    public override void PairRemoved(object proxyUserData1, object proxyUserData2, object pairUserData)
    {
      if (pairUserData == null)
        return;
      Contact c = pairUserData as Contact;
      if (c == this._nullContact)
        return;
      this.Destroy(c);
    }

    public void Destroy(Contact c)
    {
      Shape shape1 = c.GetShape1();
      Shape shape2 = c.GetShape2();
      Body body1 = shape1.GetBody();
      Body body2 = shape2.GetBody();
      ContactPoint point1 = new ContactPoint();
      point1.Shape1 = shape1;
      point1.Shape2 = shape2;
      point1.Friction = Settings.MixFriction(shape1.Friction, shape2.Friction);
      point1.Restitution = Settings.MixRestitution(shape1.Restitution, shape2.Restitution);
      int manifoldCount = c.GetManifoldCount();
      if (manifoldCount > 0 && this._world._contactListener != null)
      {
        Manifold[] manifolds = c.GetManifolds();
        for (int index1 = 0; index1 < manifoldCount; ++index1)
        {
          Manifold manifold = manifolds[index1];
          point1.Normal = manifold.Normal;
          for (int index2 = 0; index2 < manifold.PointCount; ++index2)
          {
            ManifoldPoint point2 = manifold.Points[index2];
            point1.Position = body1.GetWorldPoint(point2.LocalPoint1);
            Vec2 velocityFromLocalPoint1 = body1.GetLinearVelocityFromLocalPoint(point2.LocalPoint1);
            Vec2 velocityFromLocalPoint2 = body2.GetLinearVelocityFromLocalPoint(point2.LocalPoint2);
            point1.Velocity = velocityFromLocalPoint2 - velocityFromLocalPoint1;
            point1.Separation = point2.Separation;
            point1.ID = point2.ID;
            this._world._contactListener.Remove(point1);
          }
        }
      }
      if (c._prev != null)
        c._prev._next = c._next;
      if (c._next != null)
        c._next._prev = c._prev;
      if (c == this._world._contactList)
        this._world._contactList = c._next;
      if (c._node1.Prev != null)
        c._node1.Prev.Next = c._node1.Next;
      if (c._node1.Next != null)
        c._node1.Next.Prev = c._node1.Prev;
      if (c._node1 == body1._contactList)
        body1._contactList = c._node1.Next;
      if (c._node2.Prev != null)
        c._node2.Prev.Next = c._node2.Next;
      if (c._node2.Next != null)
        c._node2.Next.Prev = c._node2.Prev;
      if (c._node2 == body2._contactList)
        body2._contactList = c._node2.Next;
      Contact.Destroy(c);
      --this._world._contactCount;
    }

    public void Collide()
    {
      for (Contact contact = this._world._contactList; contact != null; contact = contact.GetNext())
      {
        if (!contact.GetShape1().GetBody().IsSleeping() || !contact.GetShape2().GetBody().IsSleeping())
          contact.Update(this._world._contactListener);
      }
    }
  }
}
