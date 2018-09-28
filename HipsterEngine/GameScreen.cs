using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Box2DX.Common;
using Box2DX.Dynamics;
using ConsoleApplication2.UI.Components;
using ConsoleApplication2.UI.Components.Buttons;
using ConsoleApplication2.UI.Components.Layout;
using ConsoleApplication2.UI.Components.Screens;
using ConsoleApplication2.UI.Components.Texts;
using ConsoleApplication2.UI.Events;
using HipsterEngine.Particles;
using SkiaSharp;
using TestOpenTK;

namespace ConsoleApplication2
{
    public class GameScreen : Screen
    {
        public SKDrawDebug dd;
        public PointF MousePosition { get; set; }
        
        public override void OnLoad()
        {
            Paint += OnPaint;
            
            dd = new SKDrawDebug(HipsterEngine);
            
            HipsterEngine.Physics.Initialize(-1000, -1000, 1000, 1000, 0, 1, true);
            HipsterEngine.Physics.GetWorld().SetDebugDraw(dd);
         //   HipsterEngine.Physics.LoadSvg(@"C:\Users\Winster332\Desktop\test.svg");
            
            MousePosition = new PointF(0, 0);
            
         //   pc.AddRect(120, 0, 50, 50, 0, 0.2f, 0.2f, 0.2f, 1);
            HipsterEngine.Physics.FactoryBody.
                CreateRigidVertex()
                .CreateBox(0.2f, 0.2f, HipsterEngine.Surface.Width - 50, 50, 0.0f)
                .CreateBodyDef(HipsterEngine.Surface.Width / 2, HipsterEngine.Surface.Height - 50, 0, true, false)
                .Build(0);//.AddRect(Width / 2 - 10, Height - 50, Width - 30, 50, 0, 0, 0.2f, 0.2f, 0);
            HipsterEngine.Surface.Canvas.Camera.SetScale(-0.1f);
         //   pc.AddCircle(100, 100, 50, 0, 0.2f, 0.2f, 0.2f, 1);
        //    pc.AddCircle(90, 300, 50, 0, 0f, 0.2f, 0.2f, 0);
            var car = new ModelCar(HipsterEngine.Physics);
            var man = new ModelMan(HipsterEngine.Physics);
            
        //    var circle = HipsterEngine.Physics.FactoryBody
        //        .CreateRigidCircle()
        //        .CreateCircleDef(0.2f, 0.2f, 0.2f, 0.05f)
        //        .CreateBodyDef(0.5f, 0.5f, 0, true, false)
        //        .Build(1);

            
          //  circle.ContactAdd += point =>
          //  {
          //      var ps = (ParticlesControllerFire)HipsterEngine.Particles.GetSystem(typeof(ParticlesControllerFire));
          //      var x = point.Position.X * PhysicsController.metric;
          //      var y = point.Position.Y * PhysicsController.metric;
                
          //      ps.AddBlood(x, y, new Vec2(), 5);
          //  };
         //   HipsterEngine.Surface.Canvas.Camera.SetTargetBody(circle);
            HipsterEngine.Surface.Canvas.Camera.Y = Height / 2;
            HipsterEngine.Surface.Canvas.Camera.X = -Width / 2;
            
            HipsterEngine.Particles.AddParticleSystem(new ParticlesControllerFire(HipsterEngine));
            
            var isMove = false;
            var mouse = new Vec2();
            var listMouses = new List<Vec2>();
            MouseJoint joint = null;
            Body body = null;

            MouseDown += (o, e) =>
            {
                isMove = true;

                if (e.Button == MouseButton.Right)
                {
                    HipsterEngine.Physics.FactoryBody
                        .CreateRigidVertex()
                        .CreateBox(0.2f, 0.2f, 50, 50, 0.2f)
                        .CreateBodyDef(e.X, e.Y, 0, true, false)
                        .Build(0.3f);
                }
            };
            MouseMove += (o, e) =>
            {
                MousePosition = new PointF(e.X, e.Y);
          //      HipsterEngine.Surface.Canvas.Camera.X = -e.X / 5.0f + 80 - Width / 2;
           //     HipsterEngine.Surface.Canvas.Camera.Y = e.Y / 5.0f - 80 + Height / 2;

                
                var ps = (ParticlesControllerFire)HipsterEngine.Particles.GetSystem(typeof(ParticlesControllerFire));
                ps.AddBlood(MousePosition.X, MousePosition.Y, new Vec2(), 5);
                
                if (isMove)
                {
                    mouse = new Vec2(e.X / PhysicsController.metric, e.Y / PhysicsController.metric);

                    if (listMouses.Count >= 10)
                        listMouses.RemoveAt(0);
                    listMouses.Add(mouse);

                    body = HipsterEngine.Physics.GetRayBody(e.X, e.Y);

                    if (body != null && joint == null)
                    {
                        joint = HipsterEngine.Physics.AddJointMouse(body, mouse);
                    }

                    joint?.SetTarget(mouse);
                }
            };
            MouseUp += (o, e) =>
            {
                if (joint != null)
                {
                    HipsterEngine.Physics.GetWorld().DestroyJoint(joint);
                    joint = null;
                    body = null;
                }

                isMove = false;
                listMouses.Clear();
            };
            
            
            
            
            KeyDown += (o, e) =>
            {
                if (e == Keys.R)
                {
                    //pc.AddRect(MousePosition.X, MousePosition.Y, 50, 50, 0, 0.2f, 0.2f, 0.2f, 1);
                }
                
                if (e == Keys.T)
                {
                   // pc.AddRect(MousePosition.X, MousePosition.Y, 50, 50, 0, 0, 0.2f, 0.2f, 0);
                }
                
                if (e == Keys.C)
                {
                //    pc.AddCircle(MousePosition.X, MousePosition.Y, 25, 0, 0.2f, 0.2f, 0.2f, 1);
                }
                
                if (e == Keys.V)
                {
                //    pc.AddCircle(MousePosition.X, MousePosition.Y, 25, 0, 0f, 0.2f, 0.2f, 0);
                }

                if (e == Keys.Space)
                {
                    Enabled = !Enabled;
                }
            };
            var layout = new AbsoluteLayout
            {
                X = 100,
                Y = 100,
                Width = 200,
                Height = 200
            };
            
            layout.AddElement(new RectButton(0, 0, 80, 50, "BUTTON"));
         //   AddView(layout);
         //   AddView(new TextBox { X = 100, Y = 100, Text = "Color: RED" });

        }



        private void OnPaint(UIElement element, SKCanvas canvas)
        {
            HipsterEngine.Surface.Canvas.Camera.Update();

            canvas.DrawText($"Current screen: {GetType().Name} [FPS: {HipsterEngine.DeltaTime.GetFPS()}]", 10, 75, new SKPaint
            {
                TextSize = 20,
                Color = new SKColor(100, 100, 100)
            });

            canvas.DrawText($"Mouse: [{MousePosition.X}, {MousePosition.Y}]", 10, 25, new SKPaint
            {
                TextSize = 20,
                Color = new SKColor(100, 100, 100)
            });

            canvas.DrawText($"Life: {Enabled}", 10, 50, new SKPaint
            {
                TextSize = 20,
                Color = new SKColor(100, 100, 100)
            });

            canvas.DrawText("Hipster Engine",
                Width / 2 + HipsterEngine.Surface.Canvas.Camera.X, Height / 2 + HipsterEngine.Surface.Canvas.Camera.Y, 
                new SKPaint
            {
                TextAlign = SKTextAlign.Center,
                TextSize = 60,
                Color = new SKColor(250, 80, 80)
            });
            
            var deltaTime = HipsterEngine.DeltaTime.GetDeltaTime();
            
            HipsterEngine.Surface.Canvas.Save();
          //  HipsterEngine.Surface.Canvas.Translate(HipsterEngine.Surface.Canvas.Camera.X, HipsterEngine.Surface.Canvas.Camera);
          //  HipsterEngine.Surface.Canvas.Scale(1, 1);
            HipsterEngine.Physics.Step(deltaTime, 20);
           // Particles.Step(deltaTime);
            HipsterEngine.Particles.Draw(HipsterEngine.Surface.Canvas.GetSkiaCanvas());
            HipsterEngine.Surface.Canvas.Restore();
        }
    }
}