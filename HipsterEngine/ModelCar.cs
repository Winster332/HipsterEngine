namespace ConsoleApplication2
{
    public class ModelCar
    {
        private PhysicsController Physics;
        
        public ModelCar(PhysicsController physics)
        {
            Physics = physics;
            
            var rect = Physics.FactoryBody
                .CreateRigidVertex()
                .CreateBox(0.2f, 0.2f, 250, 40, 0.2f)
                .CreateBodyDef(200, 0, 0, true, false)
                .Build(1);
            
            var circle = Physics.FactoryBody
                .CreateRigidCircle()
                .CreateCircleDef(0.2f, 0.2f, 0.2f, 50)
                .CreateBodyDef(100, 100, 0, true, false)
                .Build(1);
            
            var circle1 = Physics.FactoryBody
                .CreateRigidCircle()
                .CreateCircleDef(0.2f, 0.2f, 0.2f, 50)
                .CreateBodyDef(300, 100, 0, true, false)
                .Build(1);

            rect.JointDistance(circle, -100, 0, 0, 0, false, 30, 3, 0.2f);
            rect.JointDistance(circle, 0, 0, 0, 0, false, 10, 6, 1);
            
            rect.JointDistance(circle1, 100, 0, 0, 0, false, 30, 3, 1);
            rect.JointDistance(circle1, 0, 0, 0, 0, false, 10, 6, 0.2f);
            
            circle.JointDistance(circle1, 0, 0, 0, 0, false, 50, 5, 1);
        }
    }
}