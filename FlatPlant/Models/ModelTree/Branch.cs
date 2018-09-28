using System;
using System.Collections.Generic;
using System.Linq;
using Box2DX.Common;
using ConsoleApplication2.Graphics;
using ConsoleApplication2.Physics.Bodies;
using FlatPlant.Extensions;
using FlatPlant.Graph;
using SkiaSharp;
using Math = System.Math;

namespace FlatPlant.Models
{
    public class Branch : Node
    {
        public SKPaint Paint { get; set; }
        public Branch Parent => (Branch) Inputs.FirstOrDefault()?.To;
        public RigidBodyCircle RigidBody { get; set; }
        private ConsoleApplication2.HipsterEngine _engine;
        
        public Branch(ConsoleApplication2.HipsterEngine engine, float x, float y, float strokeWidth, 
            bool isPhysics = true, float mass = 1, bool isSensor = false, short gIndex = 1) : base(x, y)
        {
            _engine = engine;
            
            Paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                StrokeWidth = strokeWidth,
                Color = new SKColor(100, 100, 100)
            };

            if (isPhysics)
            {
                RigidBody = (RigidBodyCircle) _engine.Physics.FactoryBody
                    .CreateRigidCircle()
                    .CreateCircleDef(0.2f, 0.2f, 0.2f, 5, gIndex)
                    .CreateBodyDef(X, Y, 0, true, isSensor)
                    .Build(mass);
            }
        }

        public float GetAngle()
        {
            return (float) Math.Atan2(Y, X);
        }
        
        public float GetAngle2(Branch branch)
        {
            return (float) Math.Atan2(Y - branch.Y, X - branch.X);
        }
        
        public Vec2 GetAngleVector(float angle, float lenth)
        {
            var a = angle / 180.0f * (float)Math.PI;
            var v = new Vec2
            {
                X = (float) Math.Sin(a) * lenth,
                Y = -(float) Math.Cos(a) * lenth
            };

            return v;
        }
        
        public void Step()
        {
            Output.ForEach(o => ((Branch)o.To).Step());
        }
    }
}