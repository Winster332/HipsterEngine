using System;
using System.Collections.Generic;
using System.Linq;
using Box2DX.Common;
using Box2DX.Dynamics;
using ConsoleApplication2;
using ConsoleApplication2.Physics.Bodies;
using ConsoleApplication2.UI.Animations;
using ConsoleApplication2.UI.Components;
using ConsoleApplication2.UI.Components.Buttons;
using ConsoleApplication2.UI.Components.GamePad;
using ConsoleApplication2.UI.Components.Layout;
using ConsoleApplication2.UI.Components.Screens;
using ConsoleApplication2.UI.Events;
using FlatPlant.Extensions;
using FlatPlant.Graph;
using FlatPlant.Models;
using FlatPlant.Models.Planets;
using FlatPlant.Models.Robots;
using FlatPlant.Models.Robots.Arms;
using FlatPlant.Models.Robots.Bodies;
using FlatPlant.Models.Robots.Transmissions;
using HipsterEngine.Particles;
using SkiaSharp;
using Math = System.Math;

namespace FlatPlant.Screens
{
    public class GameScreen : Screen
    {
        public SKDrawDebug dd;
        public Vec2 MousePosition { get; set; }
        public List<Tree> Trees { get; set; }
        public PlanetEarth Earth { get; set; }
        public List<IRobot> Robots { get; set; }
        public MoveControl mControl { get; set; }
        public AttackControl aControl { get; set; }

        public override void OnLoad()
        {
            Paint += OnPaint;

            dd = new SKDrawDebug(HipsterEngine);

            HipsterEngine.Physics.Initialize(-1000, -1000, 1000, 1000, 0, 0f, true);
            HipsterEngine.Physics.GetWorld().SetDebugDraw(dd);

            MousePosition = new Vec2(0, 0);


            HipsterEngine.Surface.Canvas.Camera.SetScale(0.5f);

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
                        .Build(1f);
                }
            };
            MouseMove += (o, e) =>
            {
                MousePosition = new Vec2(e.X, e.Y);

                var ps = (ParticlesControllerFire) HipsterEngine.Particles.GetSystem(typeof(ParticlesControllerFire));
                ps.AddBlood(MousePosition.X - HipsterEngine.Surface.Canvas.Camera.X, MousePosition.Y + HipsterEngine.Surface.Canvas.Camera.Y, new Vec2(), 5);

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

            Trees = new List<Tree>();


            InitializeBall();
            
            Robots = new List<IRobot>();
            var robot = new Robot(HipsterEngine, Earth);
            robot.Initialize(new TwoWheels(), new Box2(), new Gun1())
                .Build(Width / 2+200, 120, 90, 20);
            
            var robot1 = new Robot(HipsterEngine, Earth);
            robot1.Initialize(new TwoWheels(), new Box2(), new Gun1())
                .Build(Width / 2-200, 120, 90, 20);
            Robots.Add(robot);
            Robots.Add(robot1);
            
            aControl = new AttackControl(HipsterEngine, 70);
            mControl = new MoveControl(HipsterEngine);
            mControl.Tracker.Move += (sender, vec2) =>
            {
                ValueX = vec2.X; 
                Robots.First().Transmission.Move(-ValueX / 10);

                if (ValueX > 0)
                {
                    ((Gun1) Robots.First().Arms).SetAngleRad(90);
                }
                else if (ValueX < 0)
                {
                    ((Gun1) Robots.First().Arms).SetAngleRad(-90);
                }
                else if (ValueX == 0)
                {
               //     ((Gun1) Robots.First().Arms).SetAngleRad(180);
                }
            };
            aControl.Click += (sender, args) =>
            {
                Robots.First().Arms.Attack();
            };
            
            Timer = new TimeWatch();
            Timer.Tick += tick => { ((Gun1) Robots.First().Arms).SetAngleRad(tick); };
        }
        private TimeWatch Timer { get; set; }
        public float ValueX { get; set; } = 0;

        public void InitializeBall()
        {
            float x = HipsterEngine.Surface.Width / 2;
            float y = HipsterEngine.Surface.Height / 2;

            Earth = new PlanetEarth(HipsterEngine, x, y, 200);

            var tree = AddTree(Earth.RigidBody.GetX(), Earth.RigidBody.GetY() - 200, 3, 45);
            Earth.RigidBody.JointRevolute(tree.Root.RigidBody, 0, 0, false, 0, 0, 0);
        }

        public Tree AddTree(float x, float y, int layers, float size)
        {
            var tree = new Tree(HipsterEngine, x, y, 40, layers, size);

            Trees.Add(tree);

            return tree;
        }

        private void OnPaint(UIElement element, SKCanvas canvas)
        {
            Timer.Update();
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

            canvas.DrawText($"Life: {ValueX}", 10, 50, new SKPaint
            {
                TextSize = 20,
                Color = new SKColor(100, 100, 100)
            });

            var deltaTime = HipsterEngine.DeltaTime.GetDeltaTime();
            mControl.Update();
            mControl.Draw();
            aControl.Draw();

            HipsterEngine.Surface.Canvas.Save();
            HipsterEngine.Surface.Canvas.Translate(HipsterEngine.Surface.Canvas.Camera.X,
                HipsterEngine.Surface.Canvas.Camera.Y);
            HipsterEngine.Surface.Canvas.Scale(HipsterEngine.Surface.Canvas.Camera.ScaleX, HipsterEngine.Surface.Canvas.Camera.ScaleY);
            Earth.Step();
            Earth.Draw();
            Trees.ForEach(t =>
            {
                t.Step();
                t.Draw();
            });
            Robots.ForEach(r =>
            {
                r.Update(1, 1);
                r.Draw(HipsterEngine.Surface.Canvas);
            });

            HipsterEngine.Physics.Step(deltaTime, 20);
            HipsterEngine.Particles.Draw(HipsterEngine.Surface.Canvas.GetSkiaCanvas());
            HipsterEngine.Surface.Canvas.Restore();
        }
    }
}