using Box2DX.Common;

namespace Box2DX.Dynamics
{
    public class MouseJointDef : JointDef
    {
        public Vec2 Target;
        public float MaxForce;
        public float FrequencyHz;
        public float DampingRatio;

        public MouseJointDef()
        {
            this.Type = JointType.MouseJoint;
            this.Target.Set(0.0f, 0.0f);
            this.MaxForce = 0.0f;
            this.FrequencyHz = 5f;
            this.DampingRatio = 0.7f;
        }
    }
}