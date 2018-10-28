using System.Collections.Generic;
using HipsterEngine.Core.UI.Components.Screens;
using HipsterEngine.UI.Components;

namespace HipsterEngine.UI
{
    public class UIController
    {
        private Element RootElement { get; set; }
        private Screen Screen { get; set; }
        private Core.HipsterEngine _engine;
        
        public UIController(Core.HipsterEngine engine)
        {
            _engine = engine;
        }

        public void SetContentView(Element element)
        {
            RootElement = element;
        }

        public void Draw()
        {
            RootElement?.OnDraw(_engine.Surface.Canvas);
        }

        public void SetScreen(Screen screen)
        {
            Screen = screen;
        }
    }
}