using System;
using ConsoleApplication2.Graphics;
using FlatPlant.Extensions;
using SkiaSharp;

namespace FlatPlant.Models
{
    public abstract class Model
    {
        protected Canvas _canvas;
        protected ConsoleApplication2.HipsterEngine _engine;
        public SKPaint Paint { get; set; }
        public Random Rand { get; set; }
        public TimerWatch Timer { get; set; }

        public Model(ConsoleApplication2.HipsterEngine engine)
        {
            _engine = engine;
            _canvas = _engine.Surface.Canvas;
            Timer = new TimerWatch();
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