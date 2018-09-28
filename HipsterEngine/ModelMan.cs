namespace ConsoleApplication2
{
    public class ModelMan
    {
        private PhysicsController Physics;

        public ModelMan(PhysicsController physics)
        {
            Physics = physics;

            var sizeHead = 30;
            
            var head = Physics.FactoryBody
                .CreateRigidCircle()
                .CreateCircleDef(0.2f, 0.2f, 0.2f, sizeHead)
                .CreateBodyDef(200, 100, 0, true, false)
                .Build(1);
            
            var body = Physics.FactoryBody
                .CreateRigidVertex()
                .CreateBox(0.2f, 0.2f, 30, 60, 0.2f)
                .CreateBodyDef(200, 170, 0, true, false)
                .Build(1);
            
            var hend1 = Physics.FactoryBody
                .CreateRigidVertex()
                .CreateBox(0.2f, 0.2f, 10, 30, 0.2f, false, -22)
                .CreateBodyDef(200, 150, 0, true, false)
                .Build(1);
            var hend2 = Physics.FactoryBody
                .CreateRigidVertex()
                .CreateBox(0.2f, 0.2f, 10, 30, 0.2f, false, -22)
                .CreateBodyDef(200, 120, 0, true, false)
                .Build(1);
            
            var knee = Physics.FactoryBody
                .CreateRigidVertex()
                .CreateBox(0.2f, 0.2f, 25, 36, 0.2f, false)
                .CreateBodyDef(200, 225, 0, true, false)
                .Build(1);
            
            var leg = Physics.FactoryBody
                .CreateRigidVertex()
                .CreateBox(0.2f, 0.2f, 25, 46, 0.2f)
                .CreateBodyDef(200, 260, 0, true, false)
                .Build(1);

            body.JointRevolute(hend1, 0, 0, false, -185, 60, 0, 0, -20, 0, -15);
            hend1.JointRevolute(hend2, 0, 0, false, -90, 1, 0, 0, 15, 0, -15);
            body.JointRevolute(head, 0, 0, false, -20, 25, 0, 0, -30, 0, 30);
            body.JointRevolute(knee, 0, 0, false, -125, 30, 0, 0, 30, 0, -17);
            knee.JointRevolute(leg, 0, 0, false, -1, 125, 0, 0, 36/2, 0, -23);
            //   body.JointDistance(head, 0, -30, 0, 15, false, 20, 0.5f, 20f);
            //   body.JointDistance(head, -30, -60, 0, 15, false, 20, 0.5f, 20f);
        }
    }
}