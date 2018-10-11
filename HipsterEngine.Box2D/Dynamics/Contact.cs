// Decompiled with JetBrains decompiler
// Type: Box2DX.Dynamics.Contact
// Assembly: Box2DX, Version=2.0.1.28279, Culture=neutral, PublicKeyToken=null
// MVID: BD116DE6-476A-4E37-B8FC-78B19E349C03
// Assembly location: /Users/kusy/Documents/Новая папка/HipsterEngine/bin/Debug/Box2DX.dll

using Box2DX.Collision;
using System;

namespace Box2DX.Dynamics
{
  public abstract class Contact
  {
    public static ContactRegister[][] s_registers = new ContactRegister[2][];
    public static bool s_initialized = false;
    public Contact.CollisionFlags _flags;
    public int _manifoldCount;
    public Contact _prev;
    public Contact _next;
    public ContactEdge _node1;
    public ContactEdge _node2;
    public Shape _shape1;
    public Shape _shape2;
    public float _toi;

    public Contact()
    {
      this._shape1 = (Shape) null;
      this._shape2 = (Shape) null;
    }

    public Contact(Shape s1, Shape s2)
    {
      this._flags = (Contact.CollisionFlags) 0;
      if (s1.IsSensor || s2.IsSensor)
        this._flags |= Contact.CollisionFlags.NonSolid;
      this._shape1 = s1;
      this._shape2 = s2;
      this._manifoldCount = 0;
      this._prev = (Contact) null;
      this._next = (Contact) null;
      this._node1 = new ContactEdge();
      this._node1.Contact = (Contact) null;
      this._node1.Prev = (ContactEdge) null;
      this._node1.Next = (ContactEdge) null;
      this._node1.Other = (Body) null;
      this._node2 = new ContactEdge();
      this._node2.Contact = (Contact) null;
      this._node2.Prev = (ContactEdge) null;
      this._node2.Next = (ContactEdge) null;
      this._node2.Other = (Body) null;
    }

    public static void AddType(ContactCreateFcn createFcn, ContactDestroyFcn destoryFcn, ShapeType type1, ShapeType type2)
    {
      Box2DXDebug.Assert(ShapeType.UnknownShape < type1 && type1 < ShapeType.ShapeTypeCount);
      Box2DXDebug.Assert(ShapeType.UnknownShape < type2 && type2 < ShapeType.ShapeTypeCount);
      if (Contact.s_registers[(int) type1] == null)
        Contact.s_registers[(int) type1] = new ContactRegister[2];
      Contact.s_registers[(int) type1][(int) type2].CreateFcn = createFcn;
      Contact.s_registers[(int) type1][(int) type2].DestroyFcn = destoryFcn;
      Contact.s_registers[(int) type1][(int) type2].Primary = true;
      if (type1 == type2)
        return;
      Contact.s_registers[(int) type2][(int) type1].CreateFcn = createFcn;
      Contact.s_registers[(int) type2][(int) type1].DestroyFcn = destoryFcn;
      Contact.s_registers[(int) type2][(int) type1].Primary = false;
    }

    public static void InitializeRegisters()
    {
      Contact.AddType(new ContactCreateFcn(CircleContact.Create), new ContactDestroyFcn(CircleContact.Destroy), ShapeType.CircleShape, ShapeType.CircleShape);
      Contact.AddType(new ContactCreateFcn(PolyAndCircleContact.Create), new ContactDestroyFcn(PolyAndCircleContact.Destroy), ShapeType.PolygonShape, ShapeType.CircleShape);
      Contact.AddType(new ContactCreateFcn(PolygonContact.Create), new ContactDestroyFcn(PolygonContact.Destroy), ShapeType.PolygonShape, ShapeType.PolygonShape);
    }

    public static Contact Create(Shape shape1, Shape shape2)
    {
      if (!Contact.s_initialized)
      {
        Contact.InitializeRegisters();
        Contact.s_initialized = true;
      }
      ShapeType type1 = shape1.GetType();
      ShapeType type2 = shape2.GetType();
      Box2DXDebug.Assert(ShapeType.UnknownShape < type1 && type1 < ShapeType.ShapeTypeCount);
      Box2DXDebug.Assert(ShapeType.UnknownShape < type2 && type2 < ShapeType.ShapeTypeCount);
      ContactCreateFcn createFcn = Contact.s_registers[(int) type1][(int) type2].CreateFcn;
      if (createFcn == null)
        return (Contact) null;
      if (Contact.s_registers[(int) type1][(int) type2].Primary)
        return createFcn(shape1, shape2);
      Contact contact = createFcn(shape2, shape1);
      for (int index = 0; index < contact.GetManifoldCount(); ++index)
      {
        Manifold manifold = contact.GetManifolds()[index];
        manifold.Normal = -manifold.Normal;
      }
      return contact;
    }

    public static void Destroy(Contact contact)
    {
      Box2DXDebug.Assert(Contact.s_initialized);
      if (contact.GetManifoldCount() > 0)
      {
        contact.GetShape1().GetBody().WakeUp();
        contact.GetShape2().GetBody().WakeUp();
      }
      ShapeType type1 = contact.GetShape1().GetType();
      ShapeType type2 = contact.GetShape2().GetType();
      Box2DXDebug.Assert(ShapeType.UnknownShape < type1 && type1 < ShapeType.ShapeTypeCount);
      Box2DXDebug.Assert(ShapeType.UnknownShape < type2 && type2 < ShapeType.ShapeTypeCount);
      Contact.s_registers[(int) type1][(int) type2].DestroyFcn(contact);
    }

    public abstract Manifold[] GetManifolds();

    public int GetManifoldCount()
    {
      return this._manifoldCount;
    }

    public bool IsSolid()
    {
      return (this._flags & Contact.CollisionFlags.NonSolid) == (Contact.CollisionFlags) 0;
    }

    public Contact GetNext()
    {
      return this._next;
    }

    public Shape GetShape1()
    {
      return this._shape1;
    }

    public Shape GetShape2()
    {
      return this._shape2;
    }

    public void Update(ContactListener listener)
    {
      int manifoldCount1 = this.GetManifoldCount();
      this.Evaluate(listener);
      int manifoldCount2 = this.GetManifoldCount();
      Body body1 = this._shape1.GetBody();
      Body body2 = this._shape2.GetBody();
      if (manifoldCount2 == 0 && manifoldCount1 > 0)
      {
        body1.WakeUp();
        body2.WakeUp();
      }
      if (body1.IsStatic() || body1.IsBullet() || body2.IsStatic() || body2.IsBullet())
        this._flags &= ~Contact.CollisionFlags.Slow;
      else
        this._flags |= Contact.CollisionFlags.Slow;
    }

    public abstract void Evaluate(ContactListener listener);

    [Flags]
    public enum CollisionFlags
    {
      NonSolid = 1,
      Slow = 2,
      Island = 4,
      Toi = 8,
    }
  }
}
