using System;
using System.Collections.Generic;
using System.Linq;
using Box2DX.Common;
using ConsoleApplication2.UI.Components;
using ConsoleApplication2.UI.Components.Screens;
using HipsterEngine.Particles;
using SkiaSharp;
using Math = System.Math;

namespace FlatPlant.Screens
{
    public class LightScreen : Screen
    {
        public Vec2 MousePosition { get; set; }
        public List<Light> Lights { get; set; }
        public List<ObjectLight> Objects { get; set; }
        
        public override void OnLoad()
        {
            MousePosition = new Vec2(0, 0);
            
            Paint += OnPaint;
            Update += OnUpdate;
            
            HipsterEngine.Surface.Canvas.Camera.Y = Height / 2;
            HipsterEngine.Surface.Canvas.Camera.X = -Width / 2;
            
            HipsterEngine.Particles.AddParticleSystem(new ParticlesControllerFire(HipsterEngine));
            
            var isMove = false;
            MouseDown += (o, e) => { isMove = true; };
            MouseMove += (o, e) =>
            {
                MousePosition = new Vec2(e.X, e.Y);
                
                var ps = (ParticlesControllerFire)HipsterEngine.Particles.GetSystem(typeof(ParticlesControllerFire));
                ps.AddBlood(MousePosition.X, MousePosition.Y, new Vec2(), 5);
            };
            MouseUp += (o, e) => { isMove = false; };
            
            Init();
        }

        public float Angle = 0;

        public void Init()
        {
            Lights = new List<Light>();
            Objects = new List<ObjectLight>();
            var rand = new Random();
            
            for(var i = 0; i < 20; i++){
                var size = ((float)rand.NextDouble()*20) + 2;
                Objects.Add(new Block(new  Vec2((float)rand.NextDouble()*Width, (float) rand.NextDouble()*Height),size,size)); 
            }
            
            for(var i = 0; i < 20; i++){
                var size = ((float)rand.NextDouble()*20) + 2;
                Objects.Add(new Circle(new  Vec2((float)rand.NextDouble()*Width, (float) rand.NextDouble()*Height),size)); 
            }
            
            Lights.Add(new Light(new Vec2((Width/2),256), 300, 360, new SKColor(255,255,0, 150)));
        }

        public ResultFindDistance FindDistance(Light light, ObjectLight block, float angle, float rLen, bool start, float shorTest, ObjectLight closestBlock)
        {
            var x = 0.0f;
            var y = 0.0f;

            if (typeof(Circle) == block.GetType())
            {
                var o = (Circle) block;
                
                y = (block.Y + o.Radius) - light.Position.Y;
                x = (block.X + o.Radius) - light.Position.X;
            }
            else if (typeof(Block) == block.GetType())
            {
                var o = (Block) block;
                
                y = (block.Y + o.Width / 2) - light.Position.Y;
                x = (block.X + o.Height / 2) - light.Position.X;
            }

            var dist = (float)Math.Sqrt((y * y) + (x * x));

            if (light.Radius >= dist)
            {
                var rads = angle * (float)(Math.PI / 180);
                var pointPos = new Vec2(light.Position.X, light.Position.Y);

                pointPos.X += (float) (Math.Cos(rads) * dist);
                pointPos.Y += (float) (Math.Sin(rads) * dist);

                bool isIntersect = false;
            if (typeof(Circle) == block.GetType())
            {
                var o = (Circle) block;
                
                
                if (pointPos.X > o.Position.X && pointPos.X < o.Position.X + o.Radius * 2 &&
                    pointPos.Y > o.Position.Y && pointPos.Y < o.Position.Y + o.Radius * 2)
                {
                    isIntersect = true;
                }
                
            }
            else if (typeof(Block) == block.GetType())
            {
                var o = (Block) block;

                if (pointPos.X > o.Position.X && pointPos.X < o.Position.X + o.Width &&
                    pointPos.Y > o.Position.Y && pointPos.Y < o.Position.Y + o.Height)
                {
                    isIntersect = true;
                }
            }
                
                if (isIntersect)
                {
                    if(start || dist < shorTest)
                    {
                        start = false;
                        shorTest = dist;
                        rLen = dist;
                        closestBlock = block;
                    }
                    
                    return new ResultFindDistance(start, shorTest, rLen, closestBlock);
                }
            }
            
            return new ResultFindDistance(start, shorTest, rLen, closestBlock);
        }
        
        public List<Tuple<Vec2, Light>> SetupLight { get; set; } = new List<Tuple<Vec2, Light>>();

        public void ShineLight(Light light)
        {
            var curAngle = light.Angle - (light.AngleSpread / 2);
            var dynLen = light.Radius;
            var addTo = 1.0f / light.Radius;
            
            SetupLight.Clear();

            for (;curAngle < light.Angle + (light.AngleSpread / 2);
                curAngle += (addTo * (180 / (float) Math.PI)) * 2)
            {
                dynLen = light.Radius;
                
                var findDistRes = new ResultFindDistance();
                findDistRes.Start = true;
                findDistRes.ShorTest = 0;
                findDistRes.RLen = dynLen;
                findDistRes.ClosestBlock = new Block();
                
                for(var i = 0; i < Objects.Count; i++)
                {
                    findDistRes = FindDistance(light, Objects[i], curAngle, findDistRes.RLen, findDistRes.Start, findDistRes.ShorTest, findDistRes.ClosestBlock);
                }

                var rads = curAngle * (Math.PI / 180);
                var end = new Vec2(light.Position.X, light.Position.Y);
    
                findDistRes.ClosestBlock.IsVisible = true;
                end.X += (float) Math.Cos(rads) * findDistRes.RLen;
                end.Y += (float) Math.Sin(rads) * findDistRes.RLen;

                SetupLight.Add(new Tuple<Vec2, Light>(end, light));
            }
        }

        private void OnUpdate(double time, float dt)
        {
            HipsterEngine.Surface.Canvas.Camera.Update();

            Lights.First().Position = new Vec2(MousePosition.X, MousePosition.Y);
            ShineLight(Lights.First());
        }


        private void OnPaint(UIElement element, SKCanvas canvas)
        {
            canvas.Clear(SKColors.Black);

            canvas.DrawText($"Current screen: {GetType().Name} [FPS: {HipsterEngine.DeltaTime.GetFPS()}]", 10, 75,
                new SKPaint
                {
                    TextSize = 20,
                    Color = new SKColor(100, 100, 100)
                });

            canvas.DrawText($"Mouse: [{MousePosition.X}, {MousePosition.Y}]", 10, 25, new SKPaint
            {
                TextSize = 20,
                Color = new SKColor(100, 100, 100)
            });

            var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = new SKColor(100, 0, 0)
            };

            for (var i = 0; i < Objects.Count; i++)
            {
                var block = Objects[i];

                //    if (block.IsVisible)
                //    {
                //        paint.Color = new SKColor(100, 0, 0);
                //        canvas.DrawRect(block.Position.X, block.Position.Y, block.Width, block.Height, paint);
                //        block.IsVisible = false;
                //    }
                //    else
                //    {

                if (typeof(Circle) == block.GetType())
                {
                    var o = (Circle) block;

                    paint.Color = new SKColor(100, 0, 0, 50);
                    canvas.DrawCircle(block.Position.X + o.Radius, block.Position.Y + o.Radius, o.Radius, paint);
                }
                else if (typeof(Block) == block.GetType())
                {
                    var o = (Block) block;

                    paint.Color = new SKColor(100, 0, 0, 50);
                    canvas.DrawRect(block.Position.X, block.Position.Y, o.Width, o.Height, paint);
                }

                //    }
            }

            //  Angle += 0.01f;
            ////   for (var i = 0; i < Lights.Count; i++)
            //   {
            //       paint.Color = Lights[i].Color;
            //       Lights[i].Angle += 3;
            //       Lights[i].AddToX((float) Math.Sin(Angle + 3) * 2);
            //       Lights[i].AddToY((float) Math.Sin(Angle + 2) * 2);

            for (var i = 0; i < SetupLight.Count; i++)
            {
                var end = SetupLight[i].Item1;
                var light = SetupLight[i].Item2;

                var path = new SKPath();
                path.MoveTo(light.Position.X, light.Position.Y);
                path.LineTo(end.X, end.Y);
                path.Close();

                canvas.DrawPath(path, new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    IsAntialias = true,
                    Shader = SKShader.CreateRadialGradient(new SKPoint(light.Position.X, light.Position.Y),
                        light.Radius, new SKColor[]
                        {
                            new SKColor(50, 50, 50, 100),
                            new SKColor(0, 0, 0, 100),
                        }, new float[] {0.5f, light.Radius}, SKShaderTileMode.Clamp),
                    Color = new SKColor(200, 200, 200)
                });
            }


            //    }

            HipsterEngine.Surface.Canvas.Save();
            HipsterEngine.Particles.Draw(HipsterEngine.Surface.Canvas.GetSkiaCanvas());
            HipsterEngine.Surface.Canvas.Restore();
        }

        public class ResultFindDistance
        {
            public bool Start { get; set; }
            public float ShorTest { get; set; }
            public float RLen { get; set; }
            public ObjectLight ClosestBlock { get; set; }

            public ResultFindDistance()
            {
            }

            public ResultFindDistance(bool start, float shorTest, float rLen, ObjectLight closestBlock)
            {
                Start = start;
                ShorTest = shorTest;
                RLen = rLen;
                ClosestBlock = closestBlock;
            }
        }

        public class Light
        {
            public Vec2 Position { get; set; }
            public SKColor Color { get; set; }
            public float Radius { get; set; }
            public float AngleSpread { get; set; }
            public float Angle { get; set; }

            public void AddToX(float value)
            {
                Position = new Vec2(Position.X + value, Position.Y);
            }
            
            public void AddToY(float value)
            {
                Position = new Vec2(Position.Y, Position.Y + value);
            }

            public Light(Vec2 position, float radius, float angleSpread, SKColor color)
            {
                Position = position;
                Radius = radius;
                AngleSpread = angleSpread;
                Color = color;
                Angle = (float)new Random().NextDouble() * 180;
            }
        }

        public class ObjectLight
        {
            public Vec2 Position { get; set; }
            public float X => Position.X;
            public float Y => Position.Y;
            public bool IsVisible { get; set; }
        }
        
        public class Circle : ObjectLight
        {
            public float Radius { get; set; }

            public Circle()
            {
            }

            public Circle(Vec2 position, float radius)
            {
                Position = position;
                Radius = radius;
                IsVisible = false;
            }
        }

        public class Block : ObjectLight
        {
            public float Width { get; set; }
            public float Height { get; set; }

            public Block()
            {
            }

            public Block(Vec2 position, float width, float height)
            {
                Position = position;
                Width = width;
                Height = height;
                IsVisible = false;
            }
        }
    }
}