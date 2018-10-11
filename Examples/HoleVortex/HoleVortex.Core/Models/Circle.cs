namespace HoleVortex.Core.Models
{
    public class Circle
    {
        public float Angle { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Radius { get; set; }
        public float AngularVelacity { get; set; }
        
        public Circle(float x, float y, float radius, float angle = 0, float angularVelacity = 0)
        {
            X = x;
            Y = y;
            Radius = radius;
            Angle = angle;
            AngularVelacity = angularVelacity;
        }

        public void Update()
        {
            Angle += AngularVelacity;
        }
    }
}