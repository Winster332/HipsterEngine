using System;
using System.Collections.Generic;
using HipsterEngine.Core.UI.Events.Mouse;
using SkiaSharp;

namespace HipsterEngine.Core.UI.Components
{
    public abstract class UIElement : IDisposable
    {
        public float X {
            get
            {
                if (Parent != null)
                    return Parent.X + LocalX;
                else return LocalX;
            }
            set => LocalX = value;
        }

        public float Y
        {
            get
            {
                if (Parent != null)
                    return Parent.Y + LocalY;
                else return LocalY;
            }
            set => LocalY = value;
        }

        public float LocalX { get; set; }
        public float LocalY { get; set; }
        public UIElement Parent { get; set; }
        public List<UIElement> Childs { get; set; }
        public SKPaint Paint { get; set; }
        public event MouseEventHandler Click;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseUp;

        protected UIElement()
        {
            LocalX = 0;
            LocalY = 0;
            Parent = null;
            Childs = new List<UIElement>();
        }

        public virtual void AddElement(UIElement element)
        {
            element.Parent = this;
            Childs.Add(element);
        }
        
        public virtual void Draw(SKCanvas canvas)
        {
            Childs.ForEach(element => element.Draw(canvas));
        }

        public bool OnMouseAction(MouseState mouseState)
        {
            if (IsIntersection(mouseState.X, mouseState.Y))
            {
                if (mouseState.Action == MouseAction.Up)
                {
                    MouseUp?.Invoke(this, mouseState);
                    Click?.Invoke(this, mouseState);
                }

                if (mouseState.Action == MouseAction.Down)
                {
                    MouseDown?.Invoke(this, mouseState);
                }

                if (mouseState.Action == MouseAction.Move)
                {
                    MouseMove?.Invoke(this, mouseState);
                }
                
                Childs.ForEach(e => e.OnMouseAction(mouseState));
            }

            return true;
        }

        public abstract bool IsIntersection(float x, float y);

        public virtual void Dispose()
        {
            Childs.Clear();
            Childs = null;
            
            Parent?.Dispose();
            Parent = null;
        }
    }
}