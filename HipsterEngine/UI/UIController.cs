﻿using System;
using System.Collections.Generic;
using ConsoleApplication2.UI.Components;
using ConsoleApplication2.UI.Events;
using SkiaSharp;

namespace ConsoleApplication2.UI
{
    public class UIController : IDisposable
    {
        public List<UIElement> Elements { get; set; }
        
        public UIController()
        {
            Elements = new List<UIElement>();
        }

        public void AddElement(UIElement element)
        {
            Elements.Add(element);
        }
        
        public void Step(SKCanvas canvas)
        {
            Elements.ForEach(element =>
            {
                element.Draw(canvas);
            });
        }

        public void SendMouse(MouseState mouseState)
        {
            for (var i = 0; i < Elements.Count; i++)
            {
                var element = Elements[i];
                element.OnMouseAction(mouseState);
            }
        }

        public void RemoveElement(UIElement element)
        {
            Elements.Remove(element);
        }

        public void Dispose()
        {
            Elements.Clear();
        }
    }
}