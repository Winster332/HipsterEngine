using Box2DX.Common;

namespace HipsterEngine.Core.Particles
{
    public abstract class Particle
    {
        public float X
        {
            get => _position.X;
            set => _position.X = value;
        }
        
        public float Y
        {
            get => _position.Y;
            set => _position.Y = value;
        }
        
        public float VelocityX
        {
            get => _velocity.X;
            set => _velocity.X = value;
        }
        
        public float VelocityY
        {
            get => _velocity.Y;
            set => _velocity.Y = value;
        }

        private Vec2 _position;
        private Vec2 _velocity;
        
        public int TTL { get; set; }
        public bool IsDead { get; set; }

        protected Particle()
        {
            _position = new Vec2();
            _velocity = new Vec2();
            TTL = -1;
            IsDead = false;
        }

        protected void UpdatePosition(float dt)
        {
            TTL -= 1;
            _position.X += _velocity.X;
            _position.Y += _velocity.Y;

            if (TTL <= 0)
                IsDead = true;
        }

        public abstract void Step(float dt);
    }
}