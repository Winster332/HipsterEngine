using System;
using ConsoleApplication2.Graphics;
using SkiaSharp;

namespace ConsoleApplication2.UI.Components.Screens.Animations
{
    public class AnimationBlackWhite : IScreenAnimation
    {
        private int _from;
        private int _to;
        private float _current;
        private float _width;
        private float _height;
        private float _step;
        public SKPaint Paint { get; set; }
        public Action EndAnimation { get; set; }

        public AnimationBlackWhite()
        {
            Paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(0, 0, 0, 255)
            };
        }
        
        public void Start(int from, int to, float step, float width, float height, Action endAnimation)
        {
            EndAnimation = endAnimation;
            _from = from;
            _to = to;
            _step = step;
            _current = from;
            _width = width;
            _height = height;
        }
        
        public void Update()
        {
            if (_current <= _to)
            {
                _current += _step;
                Paint.Color = new SKColor(Paint.Color.Red, Paint.Color.Green, Paint.Color.Blue, (byte) _current);
            }
        }

        public void Draw(Canvas canvas)
        {
            canvas.DrawRect(0, 0, _width, _height, Paint);
        }
    }
}