using System;
using FlatPlant.Extensions;
using HipsterEngine.Core.Graphics;
using HipsterEngine.Core.UI.Animations;
using SkiaSharp;

namespace FlatPlant.Models
{
    public abstract class Model
    {
        protected Canvas _canvas;
        protected HipsterEngine.Core.HipsterEngine _engine;
        public SKPaint Paint { get; set; }
        public Random Rand { get; set; }
        public TimeWatch Timer { get; set; }

        public Model(HipsterEngine.Core.HipsterEngine engine)
        {
            _engine = engine;
            _canvas = _engine.Surface.Canvas;
            Timer = new TimeWatch();
            Rand = new Random();
            
            Paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                StrokeWidth = 2,
                Color = new SKColor(100, 100, 100)
            };
        }

        public void Step()
        {
            Timer.Update();
        }

        public abstract void Draw();
    }
}