namespace HipsterEngine.Core.Particles
{
    public class CircleParticle : Particle
    {
        public float Radius { get; set; }
        public float RadiusVelocity { get; set; }
        public float Angle { get; set; }
        public float AngularVelocity { get; set; }
        
        public CircleParticle(float radius, float radiusVelocity, float x, float y, 
            float velacityX, float velacityY, float angle, float angularVelocity, int ttl)
        {
            Radius = radius;
            RadiusVelocity = radiusVelocity;
            X = x;
            Y = y;
            VelocityX = velacityX;
            VelocityY = velacityY;
            Angle = angle;
            AngularVelocity = angularVelocity;
            TTL = ttl;
        }

        public override void Step(float dt)
        {
            if (!IsDead)
            {
                Angle += AngularVelocity;
                Radius -= RadiusVelocity;

                UpdatePosition(dt);

                if (Radius <= RadiusVelocity || TTL <= 0)
                {
                    IsDead = true;
                }
            }
        }
    }
}