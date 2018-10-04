using System;
using FlatPlant.Extensions;
using SkiaSharp;

namespace HoleVortex.Screens.UI
{
    public class LayoutTop : IDisposable
    {
        public Label LabelBalls { get; set; }
        public ButtonPause BtnPause { get; set; }
        private ConsoleApplication2.HipsterEngine _engine;
        private int Balls { get; set; }
        private TimerWatch Timer;
        
        public LayoutTop(ConsoleApplication2.HipsterEngine engine)
        {
            _engine = engine;
            LabelBalls = new Label(_engine, _engine.Surface.Width - 40, -15, "1", new SKPaint
            {
                IsAntialias = true,
                TextAlign = SKTextAlign.Center,
                TextSize = 20,
                Color = new SKColor(150, 150, 150)
            });
            Balls = 1;
            Timer = new TimerWatch();
            Timer.Tick += TimerOnTick;
            Timer.Start(1, 40);
            
            BtnPause = new ButtonPause(_engine, 30, -15, 15);
            BtnPause.Click += (element, state) =>
            {
                if (BtnPause.TextureId == 0)
                {
                    BtnPause.TextureId = 1;
                    
                    _engine.Screens.CurrentScreen.OnPaused();
                }
                else if (BtnPause.TextureId == 1)
                {
                    BtnPause.TextureId = 0;
                    
                    _engine.Screens.CurrentScreen.OnResume();
                }
            };

            _engine.Screens.CurrentScreen.MouseDown += (element, state) => BtnPause.OnMouseAction(state);
            _engine.Screens.CurrentScreen.MouseMove += (element, state) => BtnPause.OnMouseAction(state);
            _engine.Screens.CurrentScreen.MouseUp += (element, state) => BtnPause.OnMouseAction(state);
        }

        private void TimerOnTick(int counttick)
        {
            LabelBalls.Y += 1;
            BtnPause.Y += 1;
        }

        public void IncrementBalls()
        {
            Balls++;
            LabelBalls.Text = Balls.ToString();
        }
        
        public void Update()
        {
            Timer.Update();
        }
        
        public void Draw()
        {
            LabelBalls.Draw();
            BtnPause.Draw(_engine.Surface.Canvas.GetSkiaCanvas());
        }

        public void Dispose()
        {
            LabelBalls?.Dispose();
            BtnPause?.Dispose();
        }
    }
}