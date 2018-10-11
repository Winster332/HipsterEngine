namespace Box2DX.Dynamics
{
    public class JointDef
    {
        public JointType Type;
        public object UserData;
        public Body Body1;
        public Body Body2;
        public bool CollideConnected;

        public JointDef()
        {
            this.Type = JointType.UnknownJoint;
            this.UserData = (object) null;
            this.Body1 = (Body) null;
            this.Body2 = (Body) null;
            this.CollideConnected = false;
        }
    }
}