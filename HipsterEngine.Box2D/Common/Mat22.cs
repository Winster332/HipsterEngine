// Decompiled with JetBrains decompiler
// Type: Box2DX.Common.Mat22
// Assembly: Box2DX, Version=2.0.1.28279, Culture=neutral, PublicKeyToken=null
// MVID: BD116DE6-476A-4E37-B8FC-78B19E349C03
// Assembly location: /Users/kusy/Documents/Новая папка/HipsterEngine/bin/Debug/Box2DX.dll

namespace Box2DX.Common
{
  public struct Mat22
  {
    public Vec2 Col1;
    public Vec2 Col2;

    public Mat22(Vec2 c1, Vec2 c2)
    {
      this.Col1 = c1;
      this.Col2 = c2;
    }

    public Mat22(float a11, float a12, float a21, float a22)
    {
      this.Col1.X = a11;
      this.Col1.Y = a21;
      this.Col2.X = a12;
      this.Col2.Y = a22;
    }

    public Mat22(float angle)
    {
      float num1 = (float) System.Math.Cos((double) angle);
      float num2 = (float) System.Math.Sin((double) angle);
      this.Col1.X = num1;
      this.Col2.X = -num2;
      this.Col1.Y = num2;
      this.Col2.Y = num1;
    }

    public void Set(Vec2 c1, Vec2 c2)
    {
      this.Col1 = c1;
      this.Col2 = c2;
    }

    public void Set(float angle)
    {
      float num1 = (float) System.Math.Cos((double) angle);
      float num2 = (float) System.Math.Sin((double) angle);
      this.Col1.X = num1;
      this.Col2.X = -num2;
      this.Col1.Y = num2;
      this.Col2.Y = num1;
    }

    public void SetIdentity()
    {
      this.Col1.X = 1f;
      this.Col2.X = 0.0f;
      this.Col1.Y = 0.0f;
      this.Col2.Y = 1f;
    }

    public void SetZero()
    {
      this.Col1.X = 0.0f;
      this.Col2.X = 0.0f;
      this.Col1.Y = 0.0f;
      this.Col2.Y = 0.0f;
    }

    public float GetAngle()
    {
      return (float) System.Math.Atan2((double) this.Col1.Y, (double) this.Col1.X);
    }

    public Mat22 Invert()
    {
      float x1 = this.Col1.X;
      float x2 = this.Col2.X;
      float y1 = this.Col1.Y;
      float y2 = this.Col2.Y;
      Mat22 mat22 = new Mat22();
      float num1 = (float) ((double) x1 * (double) y2 - (double) x2 * (double) y1);
      Box2DXDebug.Assert((double) num1 != 0.0);
      /// TODO: DEBUG
      float num2 = 1f / num1;
      mat22.Col1.X = num2 * y2;
      mat22.Col2.X = -num2 * x2;
      mat22.Col1.Y = -num2 * y1;
      mat22.Col2.Y = num2 * x1;
      return mat22;
    }

    public Vec2 Solve(Vec2 b)
    {
      float x1 = this.Col1.X;
      float x2 = this.Col2.X;
      float y1 = this.Col1.Y;
      float y2 = this.Col2.Y;

      if (y1 == 0.0f)
        y1 = 0.05f;
      if (y2 == 0.0f)
        y2 = 0.05f;
      
      float num1 = (float) ((double) x1 * (double) y2 - (double) x2 * (double) y1);
  //    Box2DXDebug.Assert((double) num1 != 0.0);
      /// TODO: DEBUG
      float num2 = 0;
      if (num1 != 0.0)
        num2 = 1f / num1;
      
      return new Vec2()
      {
        X = num2 * (float) ((double) y2 * (double) b.X - (double) x2 * (double) b.Y),
        Y = num2 * (float) ((double) x1 * (double) b.Y - (double) y1 * (double) b.X)
      };
    }

    public static Mat22 Identity
    {
      get
      {
        return new Mat22(1f, 0.0f, 0.0f, 1f);
      }
    }

    public static Mat22 operator +(Mat22 A, Mat22 B)
    {
      Mat22 mat22 = new Mat22();
      mat22.Set(A.Col1 + B.Col1, A.Col2 + B.Col2);
      return mat22;
    }
  }
}
