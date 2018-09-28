using System;
using System.Collections.Generic;
using System.Linq;
using Box2DX.Common;
using ConsoleApplication2.Graphics;
using ConsoleApplication2.Physics.Bodies;
using FlatPlant.Extensions;
using FlatPlant.Graph;
using HipsterEngine.Particles;
using SkiaSharp;
using Math = System.Math;

namespace FlatPlant.Models
{
    public class Tree : Model
    {
        public float X { get; set; }
        public float Y { get; set; }
        public List<Node> Nodes { get; set; }
        public Branch Root { get; set; }
        public float Theta { get; set; }
        public List<RigidBody> Leaves { get; set; }
        private float _angle { get; set; }
        
        public Tree(ConsoleApplication2.HipsterEngine engine, float x, float y, float angle, int layers, float size) : base(engine)
        {
            X = x;
            Y = y;
            _angle = angle / 180.0f * (float) Math.PI;

            Leaves = new List<RigidBody>();
            Nodes = new List<Node>();
            
            Paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                StrokeWidth = 2,
                Color = new SKColor(100, 100, 100)
            };
            
            Theta = (float)(Math.PI / 2) / 3;

            float width = 7;
            Root = new Branch(engine, X, Y, width, true, 0.1f);
            
            var next = (Branch) Root.AddNode(new Branch(engine, X, Y-size, width, true, 0.1f));
            Root.RigidBody.JointRevolute(next.RigidBody, 0, 0, false, -0, 0, 0);

            Generate(next, 6, 0, layers, -30, size, new Vec2(0, -70));
            Generate(next, 6, 0, layers, 30, size, new Vec2(0, -70));
        }

        private short index = 0;
        public Branch Generate(Branch parent, int depth, int i, int nMax, float angle, float length, Vec2 branchVec)
        {
          //  var parentAngle = new Vec2(parent.X, parent.Y).GetAngle() * 180 / (float)Math.PI;
            var p1 = GetAngleVector(new Vec2(parent.X, parent.Y), angle, length);
            index++;

            var stroke = parent.Paint.StrokeWidth - 1;
            var particle = new Branch(_engine, p1.X, p1.Y, stroke, true, stroke / 5000, false, index);
         //   particle.Paint.StrokeWidth -= 10 - nMax;
            particle.RigidBody.JointRevolute(parent.RigidBody, 0, 0, false, -0, 0, 0f);
            parent.AddNode(particle);
            
            if (i < nMax)
            {
                var a = Generate(particle, 0, i, nMax-1, angle + 30, length - 10, new Vec2());
                var b = Generate(particle, 0, i, nMax-1, angle - 30, length - 10, new Vec2());
                
                //a.RigidBody.JointRevolute(b.RigidBody, 0, 0, false, -1, 1, 0);
              //  _engine.Physics.AddCircle(a.X, a.Y, 10, 0, 0.2f, 0.2f, 0.2f, 1);
              //  _engine.Physics.AddCircle(b.X, b.Y, 10, 0, 0.2f, 0.2f, 0.2f, 1);
            }
            else
            {
           //     var f = _engine.Physics.FactoryBody.CreateRigidVertex()
           //         .CreateBox(0.2f, 0.2f, 10, 10, 0.2f, true)
           //         .CreateBodyDef(particle.X, particle.Y, 0, true, false)
           //         .Build(0.001f);
           //     f.JointRevolute(particle.RigidBody, 0, 0, false, 0, 0, 0);
                Leaves.Add(particle.RigidBody);
            }

            return particle;
        }
        
        public Vec2 GetAngleVector(Vec2 center, float angle, float lenth)
        {
            var a = angle / 180.0f * (float)Math.PI;
            var v = new Vec2
            {
                X = (float) Math.Sin(a) * lenth,
                Y = -(float) Math.Cos(a) * lenth
            };
            v += center;

            return v;
        }

        public void Step()
        {
            base.Step();
        }

        public override void Draw()
        {
            var ppp = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                StrokeWidth = 2,
                Color = new SKColor(0, 140, 220)
            };

            var stages = Root.GetNextNodes(j =>
            {
            });

            stages.ForEach(s =>
            {
                s.Nodes.ForEach(n =>
                {
                    var branch = (Branch) n;
                    
                    for (var i = 0; i < n.Output.Count; i++)
                    {
                        var b1 = (Branch) n;
                        var b2 = (Branch) n.Output[i].To;

                        var pos1 = new Vec2(b1.RigidBody.GetX(), b1.RigidBody.GetY());
                        var pos2 = new Vec2(b2.RigidBody.GetX(), b2.RigidBody.GetY());
                        
                        _canvas.DrawLine(pos1.X, pos1.Y, pos2.X, pos2.Y, b2.Paint);
                    }

                   // _canvas.DrawCircle(n.X, n.Y, 5, ppp);
                });
            });
        }
    }
}