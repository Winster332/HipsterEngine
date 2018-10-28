using System;
using HipsterEngine.Core.Graphics;
using SkiaSharp;

namespace HoleVortex.Core.Models.Behaviors.Common
{
    public delegate void MeshPaintEventHandler();
    public delegate void MeshUpdateEventHandler();
    
    public abstract class Behavior
    {
        public Transform Transform { get; set; }
        public event MeshPaintEventHandler Paint;
        public event MeshPaintEventHandler Update;

        public Behavior()
        {
            Transform = new Transform();
        }

        public void Step()
        {
            Update?.Invoke();
        }

        public void Draw(Canvas canvas)
        {
            Transform.Bind(canvas);
            
            Paint?.Invoke();
            
            Transform.Unbind(canvas);
        }
    }
}