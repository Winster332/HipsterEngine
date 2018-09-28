using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsoleApplication2.Graphics;
using ConsoleApplication2.UI.Components;
using ConsoleApplication2.UI.Components.Screens;
using ConsoleApplication2.UI.Events;
using SkiaSharp;

namespace HoleVortex.Screens
{
    public class ParticlesScreen : Screen
    {
        public ParticlesController pController { get; set; }
        
        public override void OnLoad()
        {
            Paint += OnPaint;
            MouseMove += OnMouseMove;
            
            pController = new ParticlesController(Width, Height);
            Console.WriteLine("create particles");
            pController.Generate(200000, (int)Width, (int)Height);
            Console.WriteLine("particles created");
        }

        private void OnMouseMove(UIElement element, MouseState mousestate)
        {
            pController.QTime.MouseX = mousestate.X;
            pController.QTime.MouseY = mousestate.Y;
        }

        private void OnPaint(UIElement element, SKCanvas canvas)
        {
            canvas.DrawText(pController.Particles.Count.ToString(), 100, 100, new SKPaint
            {
                TextSize = 60,
                IsAntialias = true,
                Color = new SKColor(180, 100, 50)
            });
            pController.Step(canvas);
        }

        public class ParticlesController
        {
            public List<Particle> Particles { get; set; }
            public QuantTime QTime { get; set; }
            public SKPaint Paint { get; set; }
            
            public ParticlesController(float width, float height)
            {
                Paint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    IsAntialias = true,
                    Color = new SKColor(150, 150, 150)
                };
                Particles = new List<Particle>();
                QTime = new QuantTime();
                QTime.Width = width;
                QTime.Height = height;
                QTime.Paint = Paint;
            }


            public void Generate(int count, int widthMax, int heightMax)
            {
                var rand = new Random();
                var countKernel = 4;
                var maxParticles = count / countKernel;

                for (var j = 0; j < countKernel; j++)
                {
              //      Task.Run(() =>
              //      {
                        for (var i = 0; i < maxParticles; i++)
                        {
                            var p = new Particle(QTime);
                            p.Radius = (float)rand.Next(1, 5) / 2.0f;
                            p.X = rand.Next(0, widthMax);
                            p.Y = rand.Next(0, heightMax);
                            p.VelacityX = (float) rand.Next(-10, 10) / 10.0f;
                            p.VelacityY = (float) rand.Next(-10, 10) / 10.0f;

                            Particles.Add(p);
                        }
               //     });
                }
            }

            public void Step(SKCanvas canvas)
            {
                QTime.Canvas = canvas;
                QTime.Time += 0.01f;
            }
        }
        
        public class Particle
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Radius { get; set; }
            public float VelacityX;
            public float VelacityY;
            private QuantTime _quantTime;

            public Particle(QuantTime quantTime)
            {
                _quantTime = quantTime;
                _quantTime.Quant += QuantTimeOnQuant;
            }

            private void QuantTimeOnQuant()
            {
                Step();
            }

            public void Step()
            {
                X += VelacityX;
                Y += VelacityY;
                
                _quantTime.Canvas.DrawCircle(X, Y, Radius, _quantTime.Paint);

                if (X <= 0)
                {
                    VelacityX *= -1;
                    X = 0;
                }

                if (Y <= 0)
                {
                    Y = 0;
                    VelacityY *= -1;
                }

                if (X >= _quantTime.Width)
                {
                    X = _quantTime.Width;
                    VelacityX *= -1;
                }

                if (Y >= _quantTime.Height)
                {
                    VelacityY *= -1;
                    Y = _quantTime.Height;
                }

           //     var mx = _quantTime.MouseX - _quantTime.Width / 2;
           //     var my = _quantTime.MouseY - _quantTime.Height / 2;
          //      var distance = (float) Math.Sqrt(Math.Pow(X + mx, 2) - Math.Pow(Y + my, 2));
          //      var angle = (float) Math.Atan2(Y - my, X - mx);
               // var vectorX = (float) Math.Cos(angle);
               // var vectorY = (float) Math.Sin(angle);

               // VelacityX = vectorX * distance;
               // VelacityY = vectorY * distance;
                //VelacityX = (vectorX * distance) / 1000.0f;
                //VelacityY = (vectorY * distance) / 1000.0f;
           //       VelacityX = Lerp(VelacityX, mx, 0.005f);
           //       VelacityY = Lerp(VelacityY, my, 0.005f);
            }
            
            public float Lerp(float a, float b, float p)
            {
                return (b - a) * p + a;
                // return a + t * (b - a); 
            }
        }

        public delegate void QuantTimeEventHandler();
        public class QuantTime
        {
            public float Time
            {
                get => _time;
                set
                {
                    Quant?.Invoke();
                    _time = value;
                }
            }

            public float MouseX = 0;
            public float MouseY = 0;
            public float _time;
            public float Width;
            public float Height;
            public SKPaint Paint;
            public SKCanvas Canvas { get; set; }
            public event QuantTimeEventHandler Quant;
        }
    }
}