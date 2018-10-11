namespace Box2DX.Common
{
    public struct Sweep
    {
        public Vec2 LocalCenter;
        public Vec2 C0;
        public Vec2 C;
        public float A0;
        public float A;
        public float T0;

        public void GetXForm(out XForm xf, float t)
        {
            xf = new XForm();
            if (1.0 - (double) this.T0 > (double) Math.FLOAT32_EPSILON)
            {
                float num = (float) (((double) t - (double) this.T0) / (1.0 - (double) this.T0));
                xf.Position = (1f - num) * this.C0 + num * this.C;
                float angle = (float) ((1.0 - (double) num) * (double) this.A0 + (double) num * (double) this.A);
                xf.R.Set(angle);
            }
            else
            {
                xf.Position = this.C;
                xf.R.Set(this.A);
            }
            xf.Position -= Math.Mul(xf.R, this.LocalCenter);
        }

        public void Advance(float t)
        {
            if ((double) this.T0 >= (double) t || 1.0 - (double) this.T0 <= (double) Math.FLOAT32_EPSILON)
                return;
            float num = (float) (((double) t - (double) this.T0) / (1.0 - (double) this.T0));
            this.C0 = (1f - num) * this.C0 + num * this.C;
            this.A0 = (float) ((1.0 - (double) num) * (double) this.A0 + (double) num * (double) this.A);
            this.T0 = t;
        }
    }
}