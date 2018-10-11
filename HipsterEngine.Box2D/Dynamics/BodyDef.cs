using Box2DX.Collision;
using Box2DX.Common;

namespace Box2DX.Dynamics
{
    public class BodyDef
    {
        public MassData MassData;
        public object UserData;
        public Vec2 Position;
        public float Angle;
        public float LinearDamping;
        public float AngularDamping;
        public bool AllowSleep;
        public bool IsSleeping;
        public bool FixedRotation;
        public bool IsBullet;

        public BodyDef()
        {
            this.MassData = new MassData();
            this.MassData.Center.SetZero();
            this.MassData.Mass = 0.0f;
            this.MassData.I = 0.0f;
            this.UserData = (object) null;
            this.Position = new Vec2();
            this.Position.Set(0.0f, 0.0f);
            this.Angle = 0.0f;
            this.LinearDamping = 0.0f;
            this.AngularDamping = 0.0f;
            this.AllowSleep = true;
            this.IsSleeping = false;
            this.FixedRotation = false;
            this.IsBullet = false;
        }
    }
}