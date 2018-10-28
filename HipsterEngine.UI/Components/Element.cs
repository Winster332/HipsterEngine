using System.Collections.Generic;
using HipsterEngine.Core.Graphics;
using HipsterEngine.Core.UI.Components.Screens;

namespace HipsterEngine.UI.Components
{
    public delegate void PaintElementEventHandler(Element element, Canvas canvas);
    
    public abstract class Element
    {
        public float X
        {
            get
            {
                if (Parent == null)
                {
                    return LocalX;
                }
                
                return Parent.X + LocalX;
            }
            set => LocalX = value;
        }
        public float Y
        {
            get
            {
                if (Parent == null)
                {
                    return LocalY;
                }
                
                return Parent.Y + LocalY;
            }
            set => LocalY = value;
        }
        public float LocalX { get; set; }
        public float LocalY { get; set; }
        public event PaintElementEventHandler Paint;
        public List<Element> Childs { get; set; }
        public Element Parent { get; set; }
        public Style Style { get; set; }
        private Core.HipsterEngine _engine;

        public abstract void UseClip(Canvas canvas);

        public Element(Core.HipsterEngine engine)
        {
            _engine = engine;
            Childs = new List<Element>();
            Style = new Style(this);
        }

        public void OnDraw(Canvas canvas)
        {
            Paint?.Invoke(this, canvas);

            for (var i = 0; i < Childs.Count; i++)
            {
                Childs[i].OnDraw(canvas);
            }
        }

        public virtual void AddView(Element element)
        {
            element.Parent = this;
            Childs.Add(element);
        }

        public void RemoveView(Element element)
        {
            Childs.Remove(element);
        }

        public override string ToString()
        {
            return GetType().FullName;
        }
    }
}