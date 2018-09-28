using System.Collections.Generic;
using System.Linq;
using ConsoleApplication2.Physics.Bodies;

namespace FlatPlant.Models
{
    public class Rope : Model
    {
        public List<RigidBody> Nodes { get; set; }
        public RigidBody Begin => Nodes.First();
        public RigidBody End => Nodes.Last();
        public float Length { get; set; }
        
        public Rope(ConsoleApplication2.HipsterEngine engine, float x, float y, float length) : base(engine)
        {
            Length = length;
            Nodes = new List<RigidBody>();
            
            Generate(x, y);
        }

        public void Generate(float x, float y)
        {
            var radius = 10;

            var parent = _engine.Physics.FactoryBody
                    .CreateRigidCircle()
                    .CreateCircleDef(0.2f, 0.2f, 0.2f, 10)
                    .CreateBodyDef(x, y, 0, true, false)
                    .Build(1);
            Nodes.Add(parent);
            
            for (var i = 1; i < Length; i+=radius*2)
            {
                var node = _engine.Physics.FactoryBody
                    .CreateRigidCircle()
                    .CreateCircleDef(0.2f, 0.2f, 0.2f, 10)
                    .CreateBodyDef(x, y + i, 0, true, false)
                    .Build(0.01f);

                parent.JointDistance(node, 0, 0, 0, 0, false, 10.0f, (radius * 2) / 30.0f, 0);

                Nodes.Add(node);
                parent = node;
            }
        }

        public override void Draw()
        {
        }
    }
}