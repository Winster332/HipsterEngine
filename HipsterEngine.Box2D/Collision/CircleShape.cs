// Decompiled with JetBrains decompiler
// Type: Box2DX.Collision.CircleShape
// Assembly: Box2DX, Version=2.0.1.28279, Culture=neutral, PublicKeyToken=null
// MVID: BD116DE6-476A-4E37-B8FC-78B19E349C03
// Assembly location: /Users/kusy/Documents/Новая папка/HipsterEngine/bin/Debug/Box2DX.dll

using Box2DX.Common;

namespace Box2DX.Collision
{
  public class CircleShape : Shape
  {
    private Vec2 _localPosition;
    private float _radius;

    internal CircleShape(ShapeDef def)
      : base(def)
    {
      Box2DXDebug.Assert(def.Type == ShapeType.CircleShape);
      CircleDef circleDef = (CircleDef) def;
      this._type = ShapeType.CircleShape;
      this._localPosition = circleDef.LocalPosition;
      this._radius = circleDef.Radius;
    }

    internal override void UpdateSweepRadius(Vec2 center)
    {
      this._sweepRadius = (this._localPosition - center).Length() + this._radius - Settings.ToiSlop;
    }

    public override bool TestPoint(XForm transform, Vec2 p)
    {
      Vec2 vec2_1 = transform.Position + Math.Mul(transform.R, this._localPosition);
      Vec2 vec2_2 = p - vec2_1;
      return (double) Vec2.Dot(vec2_2, vec2_2) <= (double) this._radius * (double) this._radius;
    }

    public override SegmentCollide TestSegment(XForm transform, out float lambda, out Vec2 normal, Segment segment, float maxLambda)
    {
      lambda = 0.0f;
      normal = Vec2.Zero;
      Vec2 vec2_1 = transform.Position + Math.Mul(transform.R, this._localPosition);
      Vec2 vec2_2 = segment.P1 - vec2_1;
      float num1 = Vec2.Dot(vec2_2, vec2_2) - this._radius * this._radius;
      if ((double) num1 < 0.0)
      {
        lambda = 0.0f;
        return SegmentCollide.StartInsideCollide;
      }
      Vec2 vec2_3 = segment.P2 - segment.P1;
      float num2 = Vec2.Dot(vec2_2, vec2_3);
      float num3 = Vec2.Dot(vec2_3, vec2_3);
      float x = (float) ((double) num2 * (double) num2 - (double) num3 * (double) num1);
      if ((double) x < 0.0 || (double) num3 < (double) Settings.FLT_EPSILON)
        return SegmentCollide.MissCollide;
      float num4 = (float) -((double) num2 + (double) Math.Sqrt(x));
      if (0.0 > (double) num4 || (double) num4 > (double) maxLambda * (double) num3)
        return SegmentCollide.MissCollide;
      float num5 = num4 / num3;
      lambda = num5;
      normal = vec2_2 + num5 * vec2_3;
      double num6 = (double) normal.Normalize();
      return SegmentCollide.HitCollide;
    }

    public override void ComputeAABB(out AABB aabb, XForm transform)
    {
      aabb = new AABB();
      Vec2 vec2 = transform.Position + Math.Mul(transform.R, this._localPosition);
      aabb.LowerBound.Set(vec2.X - this._radius, vec2.Y - this._radius);
      aabb.UpperBound.Set(vec2.X + this._radius, vec2.Y + this._radius);
    }

    public override void ComputeSweptAABB(out AABB aabb, XForm transform1, XForm transform2)
    {
      aabb = new AABB();
      Vec2 a = transform1.Position + Math.Mul(transform1.R, this._localPosition);
      Vec2 b = transform2.Position + Math.Mul(transform2.R, this._localPosition);
      Vec2 vec2_1 = Math.Min(a, b);
      Vec2 vec2_2 = Math.Max(a, b);
      aabb.LowerBound.Set(vec2_1.X - this._radius, vec2_1.Y - this._radius);
      aabb.UpperBound.Set(vec2_2.X + this._radius, vec2_2.Y + this._radius);
    }

    public override void ComputeMass(out MassData massData)
    {
      massData = new MassData();
      massData.Mass = this._density * Settings.Pi * this._radius * this._radius;
      massData.Center = this._localPosition;
      massData.I = massData.Mass * (0.5f * this._radius * this._radius + Vec2.Dot(this._localPosition, this._localPosition));
    }

    public Vec2 GetLocalPosition()
    {
      return this._localPosition;
    }

    public float GetRadius()
    {
      return this._radius;
    }
  }
}
