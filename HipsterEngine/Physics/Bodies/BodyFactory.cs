namespace ConsoleApplication2.Physics.Bodies
{
    public class BodyFactory
    {
        private PhysicsController _physics;
        
        public BodyFactory(PhysicsController physics)
        {
            _physics = physics;
        }

        public RigidBodyCircle CreateRigidCircle()
        {
            var body = new RigidBodyCircle(_physics);
            return body;
        }
        
        public RigidBodyVertex CreateRigidVertex()
        {
            var body = new RigidBodyVertex(_physics);
            return body;
        }
    }
}