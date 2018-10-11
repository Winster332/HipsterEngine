namespace Box2DX.Common
{
    public struct Mat33
    {
        public Vec3 Col1;
        public Vec3 Col2;
        public Vec3 Col3;

        public Mat33(Vec3 c1, Vec3 c2, Vec3 c3)
        {
            this.Col1 = c1;
            this.Col2 = c2;
            this.Col3 = c3;
        }

        public void SetZero()
        {
            this.Col1.SetZero();
            this.Col2.SetZero();
            this.Col3.SetZero();
        }

        public Vec3 Solve33(Vec3 b)
        {
            float num1 = Vec3.Dot(this.Col1, Vec3.Cross(this.Col2, this.Col3));
            Box2DXDebug.Assert((double) num1 != 0.0);
            float num2 = 1f / num1;
            return new Vec3()
            {
                X = num2 * Vec3.Dot(b, Vec3.Cross(this.Col2, this.Col3)),
                Y = num2 * Vec3.Dot(this.Col1, Vec3.Cross(b, this.Col3)),
                Z = num2 * Vec3.Dot(this.Col1, Vec3.Cross(this.Col2, b))
            };
        }

        public Vec2 Solve22(Vec2 b)
        {
            float x1 = this.Col1.X;
            float x2 = this.Col2.X;
            float y1 = this.Col1.Y;
            float y2 = this.Col2.Y;
            float num1 = (float) ((double) x1 * (double) y2 - (double) x2 * (double) y1);
            Box2DXDebug.Assert((double) num1 != 0.0);
            float num2 = 1f / num1;
            return new Vec2()
            {
                X = num2 * (float) ((double) y2 * (double) b.X - (double) x2 * (double) b.Y),
                Y = num2 * (float) ((double) x1 * (double) b.Y - (double) y1 * (double) b.X)
            };
        }
    }
}
