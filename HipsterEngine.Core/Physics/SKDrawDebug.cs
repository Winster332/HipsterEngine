using System;
using System.Linq;
using Box2DX.Common;
using Box2DX.Dynamics;
using HipsterEngine.Core.Graphics;
using SkiaSharp;

namespace HipsterEngine.Core.Physics
{
    public class SKDrawDebug : DebugDraw
    {
        public SKPaint paintCircleSolid { get; set; }
        private HipsterEngine _engine;
        private Canvas _canvas;

        public SKDrawDebug(HipsterEngine engine)
        {
            _engine = engine;
            _canvas = _engine.Surface.Canvas;

            Flags = DrawFlags.Aabb |
                    DrawFlags.Shape |
                    DrawFlags.Pair |
                    DrawFlags.Obb |
                    DrawFlags.Joint |
                    DrawFlags.CenterOfMass |
                    DrawFlags.CoreShape;
            paintCircleSolid = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Green,
                IsAntialias = true,
                StrokeWidth = 1
            };
        }
        
        public override void DrawPolygon(Vec2[] vertices, int vertexCount, Color color)
        {
            var c = new SKColor(0, 140, 220, 100);
            
            var path = new SKPath();
            path.MoveTo(vertices.First().X * PhysicsController.metric, 
                vertices.First().Y * PhysicsController.metric);
            
            for (var i = 0; i < vertexCount; i++)
            {
                var vertex = vertices[i];
                var x = vertex.X * PhysicsController.metric;
                var y = vertex.Y * PhysicsController.metric;
                
                path.LineTo(x, y);
            }

            path.Close();
            _canvas.Save();
            _canvas.Translate(_canvas.Camera.X, _canvas.Camera.Y);
          //  canvas.RotateDegrees(axis.Y);
            _canvas.DrawPath(path, new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                Color = c
            });
            _canvas.Restore();
        }

        public override void DrawSolidPolygon(Vec2[] vertices, int vertexCount, Color color)
        {
            var colorR = Convert.ToByte(color.R * 255);
            var colorG = Convert.ToByte(color.G * 255);
            var colorB = Convert.ToByte(color.B * 255);
            
            var c = new SKColor(colorR, colorG, colorB, 100);
            
            var path = new SKPath();
            path.MoveTo(vertices.First().X * PhysicsController.metric, 
                vertices.First().Y * PhysicsController.metric);
            
            for (var i = 0; i < vertexCount; i++)
            {
                var vertex = vertices[i];
                var x = vertex.X * PhysicsController.metric;
                var y = vertex.Y * PhysicsController.metric;
                
                path.LineTo(x, y);
            }

            path.Close();
            _canvas.Save();
          //  canvas.Translate(x, y);
          //  canvas.RotateDegrees(axis.Y);
            _canvas.Translate(_canvas.Camera.X, _canvas.Camera.Y);
            _canvas.DrawPath(path, new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = c
            });
            _canvas.Restore();
        }

        public override void DrawCircle(Vec2 center, float radius, Color color)
        {
            var x = center.X * PhysicsController.metric;
            var y = center.Y * PhysicsController.metric;
            var r = radius * PhysicsController.metric;
            
            _canvas.Save();
            _canvas.Translate(x + _canvas.Camera.X, y + _canvas.Camera.Y);
            _canvas.DrawCircle(0, 0, r, new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                Color = new SKColor(20, 20, 20)
            });
            _canvas.Restore();
        }

        public override void DrawSolidCircle(Vec2 center, float radius, Vec2 axis, Color color)
        {
            var x = center.X * PhysicsController.metric;
            var y = center.Y * PhysicsController.metric;
            var r = radius * PhysicsController.metric;
            var colorR = Convert.ToByte(color.R * 255);
            var colorG = Convert.ToByte(color.G * 255);
            var colorB = Convert.ToByte(color.B * 255);
            
            var c = new SKColor(colorR, colorG, colorB, 100);
            _canvas.Save();
            _canvas.Translate(x + _canvas.Camera.X, y + _canvas.Camera.Y);
            _canvas.RotateDegrees(axis.Y);
            
            _canvas.DrawCircle(0, 0, r, new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = c
            });
            _canvas.Restore();
        }

        public override void DrawSegment(Vec2 p1, Vec2 p2, Color color)
        {
            var x1 = p1.X * PhysicsController.metric;
            var y1 = p1.Y * PhysicsController.metric;
            var x2 = p2.X * PhysicsController.metric;
            var y2 = p2.Y * PhysicsController.metric;
            var colorR = Convert.ToByte(color.R * 255);
            var colorG = Convert.ToByte(color.G * 255);
            var colorB = Convert.ToByte(color.B * 255);
            
            var c = new SKColor(colorR, colorG, colorB, 100);
            _canvas.Save();
            _canvas.Translate(x1 + _canvas.Camera.X, y1 + _canvas.Camera.Y);
            _canvas.DrawCircle(0, 0, 5, new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = c
            });
            _canvas.Restore();
            
            _canvas.Save();
            _canvas.Translate(x2 + _canvas.Camera.X, y2 +_canvas.Camera.Y);
            _canvas.DrawCircle(0, 0, 5, new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = c
            });
            _canvas.Restore();
        }

        public override void DrawXForm(XForm xf)
        {
            var x = xf.Position.X * PhysicsController.metric;
            var y = xf.Position.Y * PhysicsController.metric;
            var angle = xf.R.GetAngle();
            
            _canvas.Save();
            _canvas.Translate(x + _canvas.Camera.X, y + _canvas.Camera.Y);
            _canvas.RotateRadians(angle);
            _canvas.DrawLine(0, 0, 0, 10, new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 3,
                IsAntialias = true,
                Color = new SKColor(0, 140, 210, 100)
            });
            _canvas.Restore();
        }
    }
}