namespace Box2DX.Collision
{
    public class Bound
    {
        public ushort Value;
        public ushort ProxyId;
        public ushort StabbingCount;

        public bool IsLower
        {
            get
            {
                return ((int) this.Value & 1) == 0;
            }
        }

        public bool IsUpper
        {
            get
            {
                return ((int) this.Value & 1) == 1;
            }
        }

        public Bound Clone()
        {
            return new Bound()
            {
                Value = this.Value,
                ProxyId = this.ProxyId,
                StabbingCount = this.StabbingCount
            };
        }
    }
}