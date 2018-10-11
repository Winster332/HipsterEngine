// Decompiled with JetBrains decompiler
// Type: Box2DX.Collision.PolygonShape
// Assembly: Box2DX, Version=2.0.1.28279, Culture=neutral, PublicKeyToken=null
// MVID: BD116DE6-476A-4E37-B8FC-78B19E349C03
// Assembly location: /Users/kusy/Documents/Новая папка/HipsterEngine/bin/Debug/Box2DX.dll

using Box2DX.Common;

namespace Box2DX.Collision
{
  public class PolygonShape : Shape, Box2DX.Collision.Collision.IGenericShape
  {
    private Vec2[] _vertices = new Vec2[Settings.MaxPolygonVertices];
    private Vec2[] _coreVertices = new Vec2[Settings.MaxPolygonVertices];
    private Vec2[] _normals = new Vec2[Settings.MaxPolygonVertices];
    private Vec2 _centroid;
    private OBB _obb;
    private int _vertexCount;

    public Vec2 GetCentroid()
    {
      return this._centroid;
    }

    public OBB GetOBB()
    {
      return this._obb;
    }

    public int VertexCount
    {
      get
      {
        return this._vertexCount;
      }
    }

    public Vec2[] GetVertices()
    {
      return this._vertices;
    }

    public Vec2[] GetCoreVertices()
    {
      return this._coreVertices;
    }

    public Vec2[] Normals
    {
      get
      {
        return this._normals;
      }
    }

    public Vec2 GetFirstVertex(XForm xf)
    {
      return Box2DX.Common.Math.Mul(xf, this._coreVertices[0]);
    }

    public Vec2 Centroid(XForm xf)
    {
      return Box2DX.Common.Math.Mul(xf, this._centroid);
    }

    public Vec2 Support(XForm xf, Vec2 d)
    {
      Vec2 b = Box2DX.Common.Math.MulT(xf.R, d);
      int index1 = 0;
      float num1 = Vec2.Dot(this._coreVertices[0], b);
      for (int index2 = 1; index2 < this._vertexCount; ++index2)
      {
        float num2 = Vec2.Dot(this._coreVertices[index2], b);
        if ((double) num2 > (double) num1)
        {
          index1 = index2;
          num1 = num2;
        }
      }
      return Box2DX.Common.Math.Mul(xf, this._coreVertices[index1]);
    }

    internal PolygonShape(ShapeDef def)
      : base(def)
    {
      Box2DXDebug.Assert(def.Type == ShapeType.PolygonShape);
      this._type = ShapeType.PolygonShape;
      PolygonDef polygonDef = (PolygonDef) def;
      this._vertexCount = polygonDef.VertexCount;
      Box2DXDebug.Assert(3 <= this._vertexCount && this._vertexCount <= Settings.MaxPolygonVertices);
      for (int index = 0; index < this._vertexCount; ++index)
        this._vertices[index] = polygonDef.Vertices[index];
      for (int index1 = 0; index1 < this._vertexCount; ++index1)
      {
        int index2 = index1;
        Vec2 a = this._vertices[index1 + 1 < this._vertexCount ? index1 + 1 : 0] - this._vertices[index2];
        Box2DXDebug.Assert((double) a.LengthSquared() > (double) Settings.FLT_EPSILON * (double) Settings.FLT_EPSILON);
        this._normals[index1] = Vec2.Cross(a, 1f);
        double num = (double) this._normals[index1].Normalize();
      }
      for (int index1 = 0; index1 < this._vertexCount; ++index1)
      {
        for (int index2 = 0; index2 < this._vertexCount; ++index2)
        {
          if (index2 != index1 && index2 != (index1 + 1) % this._vertexCount)
            Box2DXDebug.Assert((double) Vec2.Dot(this._normals[index1], this._vertices[index2] - this._vertices[index1]) < -(double) Settings.LinearSlop);
        }
      }
      for (int index = 1; index < this._vertexCount; ++index)
        Box2DXDebug.Assert(System.Math.Asin((double) Box2DX.Common.Math.Clamp(Vec2.Cross(this._normals[index - 1], this._normals[index]), -1f, 1f)) > (double) Settings.AngularSlop);
      this._centroid = PolygonShape.ComputeCentroid(polygonDef.Vertices, polygonDef.VertexCount);
      PolygonShape.ComputeOBB(out this._obb, this._vertices, this._vertexCount);
      for (int index1 = 0; index1 < this._vertexCount; ++index1)
      {
        int index2 = index1 - 1 >= 0 ? index1 - 1 : this._vertexCount - 1;
        int index3 = index1;
        Vec2 normal1 = this._normals[index2];
        Vec2 normal2 = this._normals[index3];
        Vec2 b1 = this._vertices[index1] - this._centroid;
        Vec2 b2 = new Vec2();
        b2.X = Vec2.Dot(normal1, b1) - Settings.ToiSlop;
        b2.Y = Vec2.Dot(normal2, b1) - Settings.ToiSlop;
        Box2DXDebug.Assert((double) b2.X >= 0.0);
        Box2DXDebug.Assert((double) b2.Y >= 0.0);
        this._coreVertices[index1] = new Mat22()
        {
          Col1 = {
            X = normal1.X
          },
          Col2 = {
            X = normal1.Y
          }
        }.Solve(b2) + this._centroid;
      }
    }

    internal override void UpdateSweepRadius(Vec2 center)
    {
      this._sweepRadius = 0.0f;
      for (int index = 0; index < this._vertexCount; ++index)
        this._sweepRadius = Box2DX.Common.Math.Max(this._sweepRadius, (this._coreVertices[index] - center).Length());
    }

    public override bool TestPoint(XForm xf, Vec2 p)
    {
      Vec2 vec2 = Box2DX.Common.Math.MulT(xf.R, p - xf.Position);
      for (int index = 0; index < this._vertexCount; ++index)
      {
        if ((double) Vec2.Dot(this._normals[index], vec2 - this._vertices[index]) > 0.0)
          return false;
      }
      return true;
    }

    public override SegmentCollide TestSegment(XForm xf, out float lambda, out Vec2 normal, Segment segment, float maxLambda)
    {
      lambda = 0.0f;
      normal = Vec2.Zero;
      float num1 = 0.0f;
      float num2 = maxLambda;
      Vec2 vec2 = Box2DX.Common.Math.MulT(xf.R, segment.P1 - xf.Position);
      Vec2 b = Box2DX.Common.Math.MulT(xf.R, segment.P2 - xf.Position) - vec2;
      int index1 = -1;
      for (int index2 = 0; index2 < this._vertexCount; ++index2)
      {
        float num3 = Vec2.Dot(this._normals[index2], this._vertices[index2] - vec2);
        float num4 = Vec2.Dot(this._normals[index2], b);
        if ((double) num4 == 0.0)
        {
          if ((double) num3 < 0.0)
            return SegmentCollide.MissCollide;
        }
        else if ((double) num4 < 0.0 && (double) num3 < (double) num1 * (double) num4)
        {
          num1 = num3 / num4;
          index1 = index2;
        }
        else if ((double) num4 > 0.0 && (double) num3 < (double) num2 * (double) num4)
          num2 = num3 / num4;
        if ((double) num2 < (double) num1)
          return SegmentCollide.MissCollide;
      }
      Box2DXDebug.Assert(0.0 <= (double) num1 && (double) num1 <= (double) maxLambda);
      if (index1 >= 0)
      {
        lambda = num1;
        normal = Box2DX.Common.Math.Mul(xf.R, this._normals[index1]);
        return SegmentCollide.HitCollide;
      }
      lambda = 0.0f;
      return SegmentCollide.StartInsideCollide;
    }

    public override void ComputeAABB(out AABB aabb, XForm xf)
    {
      Vec2 vec2_1 = Box2DX.Common.Math.Mul(Box2DX.Common.Math.Abs(Box2DX.Common.Math.Mul(xf.R, this._obb.R)), this._obb.Extents);
      Vec2 vec2_2 = xf.Position + Box2DX.Common.Math.Mul(xf.R, this._obb.Center);
      aabb.LowerBound = vec2_2 - vec2_1;
      aabb.UpperBound = vec2_2 + vec2_1;
    }

    public override void ComputeSweptAABB(out AABB aabb, XForm transform1, XForm transform2)
    {
      AABB aabb1;
      this.ComputeAABB(out aabb1, transform1);
      AABB aabb2;
      this.ComputeAABB(out aabb2, transform2);
      aabb.LowerBound = Box2DX.Common.Math.Min(aabb1.LowerBound, aabb2.LowerBound);
      aabb.UpperBound = Box2DX.Common.Math.Max(aabb1.UpperBound, aabb2.UpperBound);
    }

    public override void ComputeMass(out MassData massData)
    {
      Box2DXDebug.Assert(this._vertexCount >= 3);
      Vec2 vec2_1 = new Vec2();
      vec2_1.Set(0.0f, 0.0f);
      float num1 = 0.0f;
      float num2 = 0.0f;
      Vec2 vec2_2 = new Vec2(0.0f, 0.0f);
      float num3 = 0.3333333f;
      for (int index = 0; index < this._vertexCount; ++index)
      {
        Vec2 vec2_3 = vec2_2;
        Vec2 vertex = this._vertices[index];
        Vec2 vec2_4 = index + 1 < this._vertexCount ? this._vertices[index + 1] : this._vertices[0];
        Vec2 a = vertex - vec2_3;
        Vec2 b = vec2_4 - vec2_3;
        float num4 = Vec2.Cross(a, b);
        float num5 = 0.5f * num4;
        num1 += num5;
        vec2_1 += num5 * num3 * (vec2_3 + vertex + vec2_4);
        float x1 = vec2_3.X;
        float y1 = vec2_3.Y;
        float x2 = a.X;
        float y2 = a.Y;
        float x3 = b.X;
        float y3 = b.Y;
        float num6 = (float) ((double) num3 * (0.25 * ((double) x2 * (double) x2 + (double) x3 * (double) x2 + (double) x3 * (double) x3) + ((double) x1 * (double) x2 + (double) x1 * (double) x3)) + 0.5 * (double) x1 * (double) x1);
        float num7 = (float) ((double) num3 * (0.25 * ((double) y2 * (double) y2 + (double) y3 * (double) y2 + (double) y3 * (double) y3) + ((double) y1 * (double) y2 + (double) y1 * (double) y3)) + 0.5 * (double) y1 * (double) y1);
        num2 += num4 * (num6 + num7);
      }
      massData.Mass = this._density * num1;
      Box2DXDebug.Assert((double) num1 > (double) Settings.FLT_EPSILON);
      Vec2 vec2_5 = vec2_1 * (1f / num1);
      massData.Center = vec2_5;
      massData.I = this._density * num2;
    }

    public static Vec2 ComputeCentroid(Vec2[] vs, int count)
    {
      Box2DXDebug.Assert(count >= 3);
      Vec2 vec2_1 = new Vec2();
      vec2_1.Set(0.0f, 0.0f);
      float num1 = 0.0f;
      Vec2 vec2_2 = new Vec2(0.0f, 0.0f);
      float num2 = 0.3333333f;
      for (int index = 0; index < count; ++index)
      {
        Vec2 vec2_3 = vec2_2;
        Vec2 v = vs[index];
        Vec2 vec2_4 = index + 1 < count ? vs[index + 1] : vs[0];
        float num3 = 0.5f * Vec2.Cross(v - vec2_3, vec2_4 - vec2_3);
        num1 += num3;
        vec2_1 += num3 * num2 * (vec2_3 + v + vec2_4);
      }
      Box2DXDebug.Assert((double) num1 > (double) Settings.FLT_EPSILON);
      return vec2_1 * (1f / num1);
    }

    public static void ComputeOBB(out OBB obb, Vec2[] vs, int count)
    {
      obb = new OBB();
      Box2DXDebug.Assert(count <= Settings.MaxPolygonVertices);
      Vec2[] vec2Array = new Vec2[Settings.MaxPolygonVertices + 1];
      for (int index = 0; index < count; ++index)
        vec2Array[index] = vs[index];
      vec2Array[count] = vec2Array[0];
      float num1 = Settings.FLT_MAX;
      for (int index1 = 1; index1 <= count; ++index1)
      {
        Vec2 vec2 = vec2Array[index1 - 1];
        Vec2 a1 = vec2Array[index1] - vec2;
        Box2DXDebug.Assert((double) a1.Normalize() > (double) Settings.FLT_EPSILON);
        Vec2 a2 = new Vec2(-a1.Y, a1.X);
        Vec2 a3 = new Vec2(Settings.FLT_MAX, Settings.FLT_MAX);
        Vec2 a4 = new Vec2(-Settings.FLT_MAX, -Settings.FLT_MAX);
        for (int index2 = 0; index2 < count; ++index2)
        {
          Vec2 b1 = vec2Array[index2] - vec2;
          Vec2 b2 = new Vec2();
          b2.X = Vec2.Dot(a1, b1);
          b2.Y = Vec2.Dot(a2, b1);
          a3 = Box2DX.Common.Math.Min(a3, b2);
          a4 = Box2DX.Common.Math.Max(a4, b2);
        }
        float num2 = (float) (((double) a4.X - (double) a3.X) * ((double) a4.Y - (double) a3.Y));
        if ((double) num2 < 0.949999988079071 * (double) num1)
        {
          num1 = num2;
          obb.R.Col1 = a1;
          obb.R.Col2 = a2;
          Vec2 v = 0.5f * (a3 + a4);
          obb.Center = vec2 + Box2DX.Common.Math.Mul(obb.R, v);
          obb.Extents = 0.5f * (a4 - a3);
        }
      }
      Box2DXDebug.Assert((double) num1 < (double) Settings.FLT_MAX);
    }
  }
}
