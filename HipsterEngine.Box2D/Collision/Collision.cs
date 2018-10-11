// Decompiled with JetBrains decompiler
// Type: Box2DX.Collision.Collision
// Assembly: Box2DX, Version=2.0.1.28279, Culture=neutral, PublicKeyToken=null
// MVID: BD116DE6-476A-4E37-B8FC-78B19E349C03
// Assembly location: /Users/kusy/Documents/Новая папка/HipsterEngine/bin/Debug/Box2DX.dll

using Box2DX.Common;

namespace Box2DX.Collision
{
  public class Collision
  {
    public static int GJKIterations = 0;
    public static readonly byte NullFeature = Math.UCHAR_MAX;

    public static void CollideCircles(ref Manifold manifold, CircleShape circle1, XForm xf1, CircleShape circle2, XForm xf2)
    {
      manifold.PointCount = 0;
      Vec2 vec2_1 = Math.Mul(xf1, circle1.GetLocalPosition());
      Vec2 vec2_2 = Math.Mul(xf2, circle2.GetLocalPosition());
      Vec2 vec2_3 = vec2_2 - vec2_1;
      float x = Vec2.Dot(vec2_3, vec2_3);
      float radius1 = circle1.GetRadius();
      float radius2 = circle2.GetRadius();
      float num1 = radius1 + radius2;
      if ((double) x > (double) num1 * (double) num1)
        return;
      float num2;
      if ((double) x < (double) Settings.FLT_EPSILON)
      {
        num2 = -num1;
        manifold.Normal.Set(0.0f, 1f);
      }
      else
      {
        float num3 = Math.Sqrt(x);
        num2 = num3 - num1;
        float num4 = 1f / num3;
        manifold.Normal.X = num4 * vec2_3.X;
        manifold.Normal.Y = num4 * vec2_3.Y;
      }
      manifold.PointCount = 1;
      manifold.Points[0].ID.Key = 0U;
      manifold.Points[0].Separation = num2;
      Vec2 v = 0.5f * (vec2_1 + radius1 * manifold.Normal + (vec2_2 - radius2 * manifold.Normal));
      manifold.Points[0].LocalPoint1 = Math.MulT(xf1, v);
      manifold.Points[0].LocalPoint2 = Math.MulT(xf2, v);
    }

    public static void CollidePolygonAndCircle(ref Manifold manifold, PolygonShape polygon, XForm xf1, CircleShape circle, XForm xf2)
    {
      manifold.PointCount = 0;
      Vec2 v1 = Math.Mul(xf2, circle.GetLocalPosition());
      Vec2 vec2_1 = Math.MulT(xf1, v1);
      int index1 = 0;
      float num1 = -Settings.FLT_MAX;
      float radius = circle.GetRadius();
      int vertexCount = polygon.VertexCount;
      Vec2[] vertices = polygon.GetVertices();
      Vec2[] normals = polygon.Normals;
      for (int index2 = 0; index2 < vertexCount; ++index2)
      {
        float num2 = Vec2.Dot(normals[index2], vec2_1 - vertices[index2]);
        if ((double) num2 > (double) radius)
          return;
        if ((double) num2 > (double) num1)
        {
          num1 = num2;
          index1 = index2;
        }
      }
      if ((double) num1 < (double) Settings.FLT_EPSILON)
      {
        manifold.PointCount = 1;
        manifold.Normal = Math.Mul(xf1.R, normals[index1]);
        manifold.Points[0].ID.Features.IncidentEdge = (byte) index1;
        manifold.Points[0].ID.Features.IncidentVertex = Box2DX.Collision.Collision.NullFeature;
        manifold.Points[0].ID.Features.ReferenceEdge = (byte) 0;
        manifold.Points[0].ID.Features.Flip = (byte) 0;
        Vec2 v2 = v1 - radius * manifold.Normal;
        manifold.Points[0].LocalPoint1 = Math.MulT(xf1, v2);
        manifold.Points[0].LocalPoint2 = Math.MulT(xf2, v2);
        manifold.Points[0].Separation = num1 - radius;
      }
      else
      {
        int index2 = index1;
        int index3 = index2 + 1 < vertexCount ? index2 + 1 : 0;
        Vec2 b = vertices[index3] - vertices[index2];
        float num2 = b.Normalize();
        Box2DXDebug.Assert((double) num2 > (double) Settings.FLT_EPSILON);
        float num3 = Vec2.Dot(vec2_1 - vertices[index2], b);
        Vec2 vec2_2;
        if ((double) num3 <= 0.0)
        {
          vec2_2 = vertices[index2];
          manifold.Points[0].ID.Features.IncidentEdge = Box2DX.Collision.Collision.NullFeature;
          manifold.Points[0].ID.Features.IncidentVertex = (byte) index2;
        }
        else if ((double) num3 >= (double) num2)
        {
          vec2_2 = vertices[index3];
          manifold.Points[0].ID.Features.IncidentEdge = Box2DX.Collision.Collision.NullFeature;
          manifold.Points[0].ID.Features.IncidentVertex = (byte) index3;
        }
        else
        {
          vec2_2 = vertices[index2] + num3 * b;
          manifold.Points[0].ID.Features.IncidentEdge = (byte) index1;
          manifold.Points[0].ID.Features.IncidentVertex = Box2DX.Collision.Collision.NullFeature;
        }
        Vec2 v2 = vec2_1 - vec2_2;
        float num4 = v2.Normalize();
        if ((double) num4 > (double) radius)
          return;
        manifold.PointCount = 1;
        manifold.Normal = Math.Mul(xf1.R, v2);
        Vec2 v3 = v1 - radius * manifold.Normal;
        manifold.Points[0].LocalPoint1 = Math.MulT(xf1, v3);
        manifold.Points[0].LocalPoint2 = Math.MulT(xf2, v3);
        manifold.Points[0].Separation = num4 - radius;
        manifold.Points[0].ID.Features.ReferenceEdge = (byte) 0;
        manifold.Points[0].ID.Features.Flip = (byte) 0;
      }
    }

    public static int ProcessTwo(out Vec2 x1, out Vec2 x2, ref Vec2[] p1s, ref Vec2[] p2s, ref Vec2[] points)
    {
      Vec2 a = -points[1];
      Vec2 b = points[0] - points[1];
      float num1 = b.Normalize();
      float num2 = Vec2.Dot(a, b);
      if ((double) num2 <= 0.0 || (double) num1 < (double) Settings.FLT_EPSILON)
      {
        x1 = p1s[1];
        x2 = p2s[1];
        p1s[0] = p1s[1];
        p2s[0] = p2s[1];
        points[0] = points[1];
        return 1;
      }
      float num3 = num2 / num1;
      x1 = p1s[1] + num3 * (p1s[0] - p1s[1]);
      x2 = p2s[1] + num3 * (p2s[0] - p2s[1]);
      return 2;
    }

    public static int ProcessThree(out Vec2 x1, out Vec2 x2, ref Vec2[] p1s, ref Vec2[] p2s, ref Vec2[] points)
    {
      Vec2 vec2_1 = points[0];
      Vec2 vec2_2 = points[1];
      Vec2 vec2_3 = points[2];
      Vec2 vec2_4 = vec2_2 - vec2_1;
      Vec2 b1 = vec2_3 - vec2_1;
      Vec2 b2 = vec2_3 - vec2_2;
      float num1 = -Vec2.Dot(vec2_1, vec2_4);
      float num2 = Vec2.Dot(vec2_2, vec2_4);
      float num3 = -Vec2.Dot(vec2_1, b1);
      float num4 = Vec2.Dot(vec2_3, b1);
      float num5 = -Vec2.Dot(vec2_2, b2);
      float num6 = Vec2.Dot(vec2_3, b2);
      if ((double) num4 <= 0.0 && (double) num6 <= 0.0)
      {
        x1 = p1s[2];
        x2 = p2s[2];
        p1s[0] = p1s[2];
        p2s[0] = p2s[2];
        points[0] = points[2];
        return 1;
      }
      Box2DXDebug.Assert((double) num1 > 0.0 || (double) num3 > 0.0);
      Box2DXDebug.Assert((double) num2 > 0.0 || (double) num5 > 0.0);
      float num7 = Vec2.Cross(vec2_4, b1);
      float num8 = num7 * Vec2.Cross(vec2_1, vec2_2);
      Box2DXDebug.Assert((double) num8 > 0.0 || (double) num1 > 0.0 || (double) num2 > 0.0);
      float num9 = num7 * Vec2.Cross(vec2_2, vec2_3);
      if ((double) num9 <= 0.0 && (double) num5 >= 0.0 && (double) num6 >= 0.0 && (double) num5 + (double) num6 > 0.0)
      {
        Box2DXDebug.Assert((double) num5 + (double) num6 > 0.0);
        float num10 = num5 / (num5 + num6);
        x1 = p1s[1] + num10 * (p1s[2] - p1s[1]);
        x2 = p2s[1] + num10 * (p2s[2] - p2s[1]);
        p1s[0] = p1s[2];
        p2s[0] = p2s[2];
        points[0] = points[2];
        return 2;
      }
      float num11 = num7 * Vec2.Cross(vec2_3, vec2_1);
      if ((double) num11 <= 0.0 && (double) num3 >= 0.0 && (double) num4 >= 0.0 && (double) num3 + (double) num4 > 0.0)
      {
        Box2DXDebug.Assert((double) num3 + (double) num4 > 0.0);
        float num10 = num3 / (num3 + num4);
        x1 = p1s[0] + num10 * (p1s[2] - p1s[0]);
        x2 = p2s[0] + num10 * (p2s[2] - p2s[0]);
        p1s[1] = p1s[2];
        p2s[1] = p2s[2];
        points[1] = points[2];
        return 2;
      }
      float num12 = num9 + num11 + num8;
      Box2DXDebug.Assert((double) num12 > 0.0);
      float num13 = 1f / num12;
      float num14 = num9 * num13;
      float num15 = num11 * num13;
      float num16 = 1f - num14 - num15;
      x1 = num14 * p1s[0] + num15 * p1s[1] + num16 * p1s[2];
      x2 = num14 * p2s[0] + num15 * p2s[1] + num16 * p2s[2];
      return 3;
    }

    public static bool InPoints(Vec2 w, Vec2[] points, int pointCount)
    {
      float num = 100f * Settings.FLT_EPSILON;
      for (int index = 0; index < pointCount; ++index)
      {
        Vec2 vec2_1 = Math.Abs(w - points[index]);
        Vec2 vec2_2 = Math.Max(Math.Abs(w), Math.Abs(points[index]));
        if ((double) vec2_1.X < (double) num * ((double) vec2_2.X + 1.0) && (double) vec2_1.Y < (double) num * ((double) vec2_2.Y + 1.0))
          return true;
      }
      return false;
    }

    public static float DistanceGeneric<T1, T2>(out Vec2 x1, out Vec2 x2, T1 shape1_, XForm xf1, T2 shape2_, XForm xf2)
    {
      Box2DX.Collision.Collision.IGenericShape genericShape1 = (object) shape1_ as Box2DX.Collision.Collision.IGenericShape;
      Box2DX.Collision.Collision.IGenericShape genericShape2 = (object) shape2_ as Box2DX.Collision.Collision.IGenericShape;
      if (genericShape1 == null || genericShape2 == null)
        Box2DXDebug.Assert(false, "Can not cast some parameters to IGenericShape");
      Vec2[] p1s = new Vec2[3];
      Vec2[] p2s = new Vec2[3];
      Vec2[] points = new Vec2[3];
      int pointCount = 0;
      x1 = genericShape1.GetFirstVertex(xf1);
      x2 = genericShape2.GetFirstVertex(xf2);
      float x = 0.0f;
      int num1 = 20;
      for (int index1 = 0; index1 < num1; ++index1)
      {
        Vec2 vec2_1 = x2 - x1;
        Vec2 vec2_2 = genericShape1.Support(xf1, vec2_1);
        Vec2 vec2_3 = genericShape2.Support(xf2, -vec2_1);
        x = Vec2.Dot(vec2_1, vec2_1);
        Vec2 vec2_4 = vec2_3 - vec2_2;
        float num2 = Vec2.Dot(vec2_1, vec2_4);
        if ((double) x - (double) num2 <= 0.00999999977648258 * (double) x || Box2DX.Collision.Collision.InPoints(vec2_4, points, pointCount))
        {
          if (pointCount == 0)
          {
            x1 = vec2_2;
            x2 = vec2_3;
          }
          Box2DX.Collision.Collision.GJKIterations = index1;
          return Math.Sqrt(x);
        }
        switch (pointCount)
        {
          case 0:
            p1s[0] = vec2_2;
            p2s[0] = vec2_3;
            points[0] = vec2_4;
            x1 = p1s[0];
            x2 = p2s[0];
            ++pointCount;
            break;
          case 1:
            p1s[1] = vec2_2;
            p2s[1] = vec2_3;
            points[1] = vec2_4;
            pointCount = Box2DX.Collision.Collision.ProcessTwo(out x1, out x2, ref p1s, ref p2s, ref points);
            break;
          case 2:
            p1s[2] = vec2_2;
            p2s[2] = vec2_3;
            points[2] = vec2_4;
            pointCount = Box2DX.Collision.Collision.ProcessThree(out x1, out x2, ref p1s, ref p2s, ref points);
            break;
        }
        if (pointCount == 3)
        {
          Box2DX.Collision.Collision.GJKIterations = index1;
          return 0.0f;
        }
        float a = -Settings.FLT_MAX;
        for (int index2 = 0; index2 < pointCount; ++index2)
          a = Math.Max(a, Vec2.Dot(points[index2], points[index2]));
        if ((double) x <= 100.0 * (double) Settings.FLT_EPSILON * (double) a)
        {
          Box2DX.Collision.Collision.GJKIterations = index1;
          Vec2 vec2_5 = x2 - x1;
          return Math.Sqrt(Vec2.Dot(vec2_5, vec2_5));
        }
      }
      Box2DX.Collision.Collision.GJKIterations = num1;
      return Math.Sqrt(x);
    }

    public static float DistanceCC(out Vec2 x1, out Vec2 x2, CircleShape circle1, XForm xf1, CircleShape circle2, XForm xf2)
    {
      Vec2 vec2_1 = Math.Mul(xf1, circle1.GetLocalPosition());
      Vec2 vec2_2 = Math.Mul(xf2, circle2.GetLocalPosition());
      Vec2 vec2_3 = vec2_2 - vec2_1;
      float num1 = Vec2.Dot(vec2_3, vec2_3);
      float num2 = circle1.GetRadius() - Settings.ToiSlop;
      float num3 = circle2.GetRadius() - Settings.ToiSlop;
      float num4 = num2 + num3;
      if ((double) num1 > (double) num4 * (double) num4)
      {
        float num5 = vec2_3.Normalize() - num4;
        x1 = vec2_1 + num2 * vec2_3;
        x2 = vec2_2 - num3 * vec2_3;
        return num5;
      }
      if ((double) num1 > (double) Settings.FLT_EPSILON * (double) Settings.FLT_EPSILON)
      {
        double num5 = (double) vec2_3.Normalize();
        x1 = vec2_1 + num2 * vec2_3;
        x2 = x1;
        return 0.0f;
      }
      x1 = vec2_1;
      x2 = x1;
      return 0.0f;
    }

    public static float DistancePC(out Vec2 x1, out Vec2 x2, PolygonShape polygon, XForm xf1, CircleShape circle, XForm xf2)
    {
      float num1 = Box2DX.Collision.Collision.DistanceGeneric<PolygonShape, Box2DX.Collision.Collision.Point>(out x1, out x2, polygon, xf1, new Box2DX.Collision.Collision.Point()
      {
        p = Math.Mul(xf2, circle.GetLocalPosition())
      }, XForm.Identity);
      float num2 = circle.GetRadius() - Settings.ToiSlop;
      float num3;
      if ((double) num1 > (double) num2)
      {
        num3 = num1 - num2;
        Vec2 vec2 = x2 - x1;
        double num4 = (double) vec2.Normalize();
        x2 -= num2 * vec2;
      }
      else
      {
        num3 = 0.0f;
        x2 = x1;
      }
      return num3;
    }

    public static float Distance(out Vec2 x1, out Vec2 x2, Shape shape1, XForm xf1, Shape shape2, XForm xf2)
    {
      x1 = new Vec2();
      x2 = new Vec2();
      ShapeType type1 = shape1.GetType();
      ShapeType type2 = shape2.GetType();
      if (type1 == ShapeType.CircleShape && type2 == ShapeType.CircleShape)
        return Box2DX.Collision.Collision.DistanceCC(out x1, out x2, (CircleShape) shape1, xf1, (CircleShape) shape2, xf2);
      if (type1 == ShapeType.PolygonShape && type2 == ShapeType.CircleShape)
        return Box2DX.Collision.Collision.DistancePC(out x1, out x2, (PolygonShape) shape1, xf1, (CircleShape) shape2, xf2);
      if (type1 == ShapeType.CircleShape && type2 == ShapeType.PolygonShape)
        return Box2DX.Collision.Collision.DistancePC(out x2, out x1, (PolygonShape) shape2, xf2, (CircleShape) shape1, xf1);
      if (type1 == ShapeType.PolygonShape && type2 == ShapeType.PolygonShape)
        return Box2DX.Collision.Collision.DistanceGeneric<PolygonShape, PolygonShape>(out x1, out x2, (PolygonShape) shape1, xf1, (PolygonShape) shape2, xf2);
      return 0.0f;
    }

    public static float TimeOfImpact(Shape shape1, Sweep sweep1, Shape shape2, Sweep sweep2)
    {
      float sweepRadius1 = shape1.GetSweepRadius();
      float sweepRadius2 = shape2.GetSweepRadius();
      Box2DXDebug.Assert((double) sweep1.T0 == (double) sweep2.T0);
      Box2DXDebug.Assert(1.0 - (double) sweep1.T0 > (double) Settings.FLT_EPSILON);
      float t0 = sweep1.T0;
      Vec2 vec2_1 = sweep1.C - sweep1.C0;
      Vec2 vec2_2 = sweep2.C - sweep2.C0;
      float a1 = sweep1.A - sweep1.A0;
      float a2 = sweep2.A - sweep2.A0;
      float num1 = 0.0f;
      int num2 = 20;
      int num3 = 0;
      Vec2 zero = Vec2.Zero;
      float num4 = 0.0f;
      while (true)
      {
        float t = (1f - num1) * t0 + num1;
        XForm xf1;
        sweep1.GetXForm(out xf1, t);
        XForm xf2;
        sweep2.GetXForm(out xf2, t);
        Vec2 x1;
        Vec2 x2;
        float num5 = Box2DX.Collision.Collision.Distance(out x1, out x2, shape1, xf1, shape2, xf2);
        if (num3 == 0)
          num4 = (double) num5 <= 2.0 * (double) Settings.ToiSlop ? Math.Max(0.05f * Settings.ToiSlop, num5 - 0.5f * Settings.ToiSlop) : 1.5f * Settings.ToiSlop;
        if ((double) num5 - (double) num4 >= 0.0500000007450581 * (double) Settings.ToiSlop && num3 != num2)
        {
          Vec2 a3 = x2 - x1;
          double num6 = (double) a3.Normalize();
          float a4 = (float) ((double) Vec2.Dot(a3, vec2_1 - vec2_2) + (double) Math.Abs(a1) * (double) sweepRadius1 + (double) Math.Abs(a2) * (double) sweepRadius2);
          if ((double) Math.Abs(a4) >= (double) Settings.FLT_EPSILON)
          {
            float num7 = (num5 - num4) / a4;
            float num8 = num1 + num7;
            if ((double) num8 >= 0.0 && 1.0 >= (double) num8)
            {
              if ((double) num8 >= (1.0 + 100.0 * (double) Settings.FLT_EPSILON) * (double) num1)
              {
                num1 = num8;
                ++num3;
              }
              else
                goto label_10;
            }
            else
              goto label_6;
          }
          else
            break;
        }
        else
          goto label_10;
      }
      num1 = 1f;
      goto label_10;
label_6:
      num1 = 1f;
label_10:
      return num1;
    }

    public static bool TestOverlap(AABB a, AABB b)
    {
      Vec2 vec2_1 = b.LowerBound - a.UpperBound;
      Vec2 vec2_2 = a.LowerBound - b.UpperBound;
      return (double) vec2_1.X <= 0.0 && (double) vec2_1.Y <= 0.0 && ((double) vec2_2.X <= 0.0 && (double) vec2_2.Y <= 0.0);
    }

    public static int ClipSegmentToLine(out Box2DX.Collision.Collision.ClipVertex[] vOut, Box2DX.Collision.Collision.ClipVertex[] vIn, Vec2 normal, float offset)
    {
      if (vIn.Length != 2)
        Box2DXDebug.ThrowBox2DXException("vIn should contain 2 element, but contains " + vIn.Length.ToString());
      vOut = new Box2DX.Collision.Collision.ClipVertex[2];
      int index = 0;
      float num1 = Vec2.Dot(normal, vIn[0].V) - offset;
      float num2 = Vec2.Dot(normal, vIn[1].V) - offset;
      if ((double) num1 <= 0.0)
        vOut[index++] = vIn[0];
      if ((double) num2 <= 0.0)
        vOut[index++] = vIn[1];
      if ((double) num1 * (double) num2 < 0.0)
      {
        float num3 = num1 / (num1 - num2);
        vOut[index].V = vIn[0].V + num3 * (vIn[1].V - vIn[0].V);
        vOut[index].ID = (double) num1 <= 0.0 ? vIn[1].ID : vIn[0].ID;
        ++index;
      }
      return index;
    }

    public static float EdgeSeparation(PolygonShape poly1, XForm xf1, int edge1, PolygonShape poly2, XForm xf2)
    {
      int vertexCount1 = poly1.VertexCount;
      Vec2[] vertices1 = poly1.GetVertices();
      Vec2[] normals = poly1.Normals;
      int vertexCount2 = poly2.VertexCount;
      Vec2[] vertices2 = poly2.GetVertices();
      Box2DXDebug.Assert(0 <= edge1 && edge1 < vertexCount1);
      Vec2 vec2_1 = Math.Mul(xf1.R, normals[edge1]);
      Vec2 b = Math.MulT(xf2.R, vec2_1);
      int index1 = 0;
      float num1 = Settings.FLT_MAX;
      for (int index2 = 0; index2 < vertexCount2; ++index2)
      {
        float num2 = Vec2.Dot(vertices2[index2], b);
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          index1 = index2;
        }
      }
      Vec2 vec2_2 = Math.Mul(xf1, vertices1[edge1]);
      return Vec2.Dot(Math.Mul(xf2, vertices2[index1]) - vec2_2, vec2_1);
    }

    public static float FindMaxSeparation(ref int edgeIndex, PolygonShape poly1, XForm xf1, PolygonShape poly2, XForm xf2)
    {
      int vertexCount = poly1.VertexCount;
      Vec2[] normals = poly1.Normals;
      Vec2 v = Math.Mul(xf2, poly2.GetCentroid()) - Math.Mul(xf1, poly1.GetCentroid());
      Vec2 b = Math.MulT(xf1.R, v);
      int edge1_1 = 0;
      float num1 = -Settings.FLT_MAX;
      for (int index = 0; index < vertexCount; ++index)
      {
        float num2 = Vec2.Dot(normals[index], b);
        if ((double) num2 > (double) num1)
        {
          num1 = num2;
          edge1_1 = index;
        }
      }
      float num3 = Box2DX.Collision.Collision.EdgeSeparation(poly1, xf1, edge1_1, poly2, xf2);
      if ((double) num3 > 0.0)
        return num3;
      int edge1_2 = edge1_1 - 1 >= 0 ? edge1_1 - 1 : vertexCount - 1;
      float num4 = Box2DX.Collision.Collision.EdgeSeparation(poly1, xf1, edge1_2, poly2, xf2);
      if ((double) num4 > 0.0)
        return num4;
      int edge1_3 = edge1_1 + 1 < vertexCount ? edge1_1 + 1 : 0;
      float num5 = Box2DX.Collision.Collision.EdgeSeparation(poly1, xf1, edge1_3, poly2, xf2);
      if ((double) num5 > 0.0)
        return num5;
      int num6;
      int num7;
      float num8;
      if ((double) num4 > (double) num3 && (double) num4 > (double) num5)
      {
        num6 = -1;
        num7 = edge1_2;
        num8 = num4;
      }
      else if ((double) num5 > (double) num3)
      {
        num6 = 1;
        num7 = edge1_3;
        num8 = num5;
      }
      else
      {
        edgeIndex = edge1_1;
        return num3;
      }
      float num9;
      while (true)
      {
        int edge1_4 = num6 != -1 ? (num7 + 1 < vertexCount ? num7 + 1 : 0) : (num7 - 1 >= 0 ? num7 - 1 : vertexCount - 1);
        num9 = Box2DX.Collision.Collision.EdgeSeparation(poly1, xf1, edge1_4, poly2, xf2);
        if ((double) num9 <= 0.0)
        {
          if ((double) num9 > (double) num8)
          {
            num7 = edge1_4;
            num8 = num9;
          }
          else
            goto label_20;
        }
        else
          break;
      }
      return num9;
label_20:
      edgeIndex = num7;
      return num8;
    }

    public static void FindIncidentEdge(out Box2DX.Collision.Collision.ClipVertex[] c, PolygonShape poly1, XForm xf1, int edge1, PolygonShape poly2, XForm xf2)
    {
      int vertexCount1 = poly1.VertexCount;
      Vec2[] normals1 = poly1.Normals;
      int vertexCount2 = poly2.VertexCount;
      Vec2[] vertices = poly2.GetVertices();
      Vec2[] normals2 = poly2.Normals;
      Box2DXDebug.Assert(0 <= edge1 && edge1 < vertexCount1);
      Vec2 a = Math.MulT(xf2.R, Math.Mul(xf1.R, normals1[edge1]));
      int num1 = 0;
      float num2 = Settings.FLT_MAX;
      for (int index = 0; index < vertexCount2; ++index)
      {
        float num3 = Vec2.Dot(a, normals2[index]);
        if ((double) num3 < (double) num2)
        {
          num2 = num3;
          num1 = index;
        }
      }
      int index1 = num1;
      int index2 = index1 + 1 < vertexCount2 ? index1 + 1 : 0;
      c = new Box2DX.Collision.Collision.ClipVertex[2];
      c[0].V = Math.Mul(xf2, vertices[index1]);
      c[0].ID.Features.ReferenceEdge = (byte) edge1;
      c[0].ID.Features.IncidentEdge = (byte) index1;
      c[0].ID.Features.IncidentVertex = (byte) 0;
      c[1].V = Math.Mul(xf2, vertices[index2]);
      c[1].ID.Features.ReferenceEdge = (byte) edge1;
      c[1].ID.Features.IncidentEdge = (byte) index2;
      c[1].ID.Features.IncidentVertex = (byte) 1;
    }

    public static void CollidePolygons(ref Manifold manifold, PolygonShape polyA, XForm xfA, PolygonShape polyB, XForm xfB)
    {
      manifold.PointCount = 0;
      int edgeIndex1 = 0;
      float maxSeparation1 = Box2DX.Collision.Collision.FindMaxSeparation(ref edgeIndex1, polyA, xfA, polyB, xfB);
      if ((double) maxSeparation1 > 0.0)
        return;
      int edgeIndex2 = 0;
      float maxSeparation2 = Box2DX.Collision.Collision.FindMaxSeparation(ref edgeIndex2, polyB, xfB, polyA, xfA);
      if ((double) maxSeparation2 > 0.0)
        return;
      float num1 = 0.98f;
      float num2 = 1f / 1000f;
      PolygonShape poly1;
      PolygonShape poly2;
      XForm xform;
      XForm xf2;
      int edge1;
      byte num3;
      if ((double) maxSeparation2 > (double) num1 * (double) maxSeparation1 + (double) num2)
      {
        poly1 = polyB;
        poly2 = polyA;
        xform = xfB;
        xf2 = xfA;
        edge1 = edgeIndex2;
        num3 = (byte) 1;
      }
      else
      {
        poly1 = polyA;
        poly2 = polyB;
        xform = xfA;
        xf2 = xfB;
        edge1 = edgeIndex1;
        num3 = (byte) 0;
      }
      Box2DX.Collision.Collision.ClipVertex[] c;
      Box2DX.Collision.Collision.FindIncidentEdge(out c, poly1, xform, edge1, poly2, xf2);
      int vertexCount = poly1.VertexCount;
      Vec2[] vertices = poly1.GetVertices();
      Vec2 v1 = vertices[edge1];
      Vec2 v2 = edge1 + 1 < vertexCount ? vertices[edge1 + 1] : vertices[0];
      Vec2 vec2_1 = v2 - v1;
      Vec2 vec2_2 = Math.Mul(xform.R, v2 - v1);
      double num4 = (double) vec2_2.Normalize();
      Vec2 a = Vec2.Cross(vec2_2, 1f);
      Vec2 b1 = Math.Mul(xform, v1);
      Vec2 b2 = Math.Mul(xform, v2);
      float num5 = Vec2.Dot(a, b1);
      float offset1 = -Vec2.Dot(vec2_2, b1);
      float offset2 = Vec2.Dot(vec2_2, b2);
      Box2DX.Collision.Collision.ClipVertex[] vOut1;
      Box2DX.Collision.Collision.ClipVertex[] vOut2;
      if (Box2DX.Collision.Collision.ClipSegmentToLine(out vOut1, c, -vec2_2, offset1) < 2 || Box2DX.Collision.Collision.ClipSegmentToLine(out vOut2, vOut1, vec2_2, offset2) < 2)
        return;
      manifold.Normal = num3 != (byte) 0 ? -a : a;
      int index1 = 0;
      for (int index2 = 0; index2 < Settings.MaxManifoldPoints; ++index2)
      {
        float num6 = Vec2.Dot(a, vOut2[index2].V) - num5;
        if ((double) num6 <= 0.0)
        {
          ManifoldPoint point = manifold.Points[index1];
          point.Separation = num6;
          point.LocalPoint1 = Math.MulT(xfA, vOut2[index2].V);
          point.LocalPoint2 = Math.MulT(xfB, vOut2[index2].V);
          point.ID = vOut2[index2].ID;
          point.ID.Features.Flip = num3;
          ++index1;
        }
      }
      manifold.PointCount = index1;
    }

    public interface IGenericShape
    {
      Vec2 Support(XForm xf, Vec2 v);

      Vec2 GetFirstVertex(XForm xf);
    }

    public class Point : Box2DX.Collision.Collision.IGenericShape
    {
      public Vec2 p;

      public Vec2 Support(XForm xf, Vec2 v)
      {
        return this.p;
      }

      public Vec2 GetFirstVertex(XForm xf)
      {
        return this.p;
      }
    }

    public struct ClipVertex
    {
      public Vec2 V;
      public ContactID ID;
    }
  }
}
