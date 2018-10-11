using System;
using System.Collections.Generic;
using HipsterEngine.Core.UI.Components;
using HipsterEngine.Core.UI.Components.Screens;
using SimplePhysics.Physics;
using SimplePhysics.Physics.Collision;
using SimplePhysics.Physics.Common;
using SkiaSharp;

namespace SimplePhysics.Screens
{
    public class GameScreen : Screen
    {
        public World world;
        public Edge edge1, edge2;
        public override void OnLoad()
        {
            Paint += OnPaint;
            
            world = new World(new Vec2(0, 0));
            world.AddCircle(100, 350, 50).Shape.Velacity = new Vec2(1, 0);
            world.AddCircle(350, 300, 50).Shape.Velacity = new Vec2(-1f, 0);
            
            edge1 = new Edge(100, 100, 200, 200);
            edge2 = new Edge(100, 200, 200, 100);
        }

        private void OnPaint(UIElement element, SKCanvas canvas)
        {
            var paint1 = new SKPaint
            {
                Color = new SKColor(150, 150, 150),
                TextSize = 60,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            edge1.From.X++;
            edge1.To.X++;
            edge1.From.Y++;
            edge1.Draw(canvas, paint1);
            edge2.Draw(canvas, paint1);

            var point = Edge.GetPointCollide(edge1, edge2);
            canvas.DrawCircle(point.X, point.Y, 10, paint1);
            Console.WriteLine($"{point.X}, {point.Y}");
            
            //world.Step(b =>
            //{
             //   var shape = b.Shape;

              //  if (shape.GetType() == typeof(CircleShape))
               // {
                //    var circleShape = (CircleShape) shape;
                    
            //        canvas.DrawCircle(circleShape.X, circleShape.Y, circleShape.Radius, paint1);
            //    }
           // });
            
            var paint = new SKPaint
            {
                Color = new SKColor(150, 150, 150),
                TextSize = 60,
                IsAntialias = true
            };
            
            canvas.DrawText($"::: {point.X} {point.Y}", 100, 100, paint);
        }
    }
}