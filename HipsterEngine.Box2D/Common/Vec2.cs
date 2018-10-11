namespace Box2DX.Common
{
  public struct Vec2
  {
    public float X;
    public float Y;

    public Vec2(float x, float y)
    {
      this.X = x;
      this.Y = y;
    }

    public void SetZero()
    {
      this.X = 0.0f;
      this.Y = 0.0f;
    }

    public void Set(float x, float y)
    {
      this.X = x;
      this.Y = y;
    }

    public float Length()
    {
      return (float) System.Math.Sqrt((double) this.X * (double) this.X + (double) this.Y * (double) this.Y);
    }

    public float LengthSquared()
    {
      return (float) ((double) this.X * (double) this.X + (double) this.Y * (double) this.Y);
    }

    public float Normalize()
    {
      float num1 = this.Length();
      if ((double) num1 < (double) Math.FLOAT32_EPSILON)
        return 0.0f;
      float num2 = 1f / num1;
      this.X *= num2;
      this.Y *= num2;
      return num1;
    }

    public bool IsValid
    {
      get
      {
        return Math.IsValid(this.X) && Math.IsValid(this.Y);
      }
    }

    public static Vec2 operator -(Vec2 v1)
    {
      Vec2 vec2 = new Vec2();
      vec2.Set(-v1.X, -v1.Y);
      return vec2;
    }

    public static Vec2 operator +(Vec2 v1, Vec2 v2)
    {
      Vec2 vec2 = new Vec2();
      vec2.Set(v1.X + v2.X, v1.Y + v2.Y);
      return vec2;
    }

    public static Vec2 operator -(Vec2 v1, Vec2 v2)
    {
      Vec2 vec2 = new Vec2();
      vec2.Set(v1.X - v2.X, v1.Y - v2.Y);
      return vec2;
    }

    public static Vec2 operator *(Vec2 v1, float a)
    {
      Vec2 vec2 = new Vec2();
      vec2.Set(v1.X * a, v1.Y * a);
      return vec2;
    }

    public static Vec2 operator *(float a, Vec2 v1)
    {
      Vec2 vec2 = new Vec2();
      vec2.Set(v1.X * a, v1.Y * a);
      return vec2;
    }

    public static bool operator ==(Vec2 a, Vec2 b)
    {
      return (double) a.X == (double) b.X && (double) a.Y == (double) b.Y;
    }

    public static bool operator !=(Vec2 a, Vec2 b)
    {
      return (double) a.X != (double) b.X && (double) a.Y != (double) b.Y;
    }

    public static Vec2 Zero
    {
      get
      {
        return new Vec2(0.0f, 0.0f);
      }
    }

    public static float Dot(Vec2 a, Vec2 b)
    {
      return (float) ((double) a.X * (double) b.X + (double) a.Y * (double) b.Y);
    }

    public static float Cross(Vec2 a, Vec2 b)
    {
      return (float) ((double) a.X * (double) b.Y - (double) a.Y * (double) b.X);
    }

    public static Vec2 Cross(Vec2 a, float s)
    {
      Vec2 vec2 = new Vec2();
      vec2.Set(s * a.Y, -s * a.X);
      return vec2;
    }

    public static Vec2 Cross(float s, Vec2 a)
    {
      Vec2 vec2 = new Vec2();
      vec2.Set(-s * a.Y, s * a.X);
      return vec2;
    }

    public static float Distance(Vec2 a, Vec2 b)
    {
      return (a - b).Length();
    }

    public static float DistanceSquared(Vec2 a, Vec2 b)
    {
      Vec2 vec2 = a - b;
      return Vec2.Dot(vec2, vec2);
    }
  }
}
