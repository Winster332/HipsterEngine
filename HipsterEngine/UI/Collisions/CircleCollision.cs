﻿using System;
using ConsoleApplication2.UI.Components;

namespace ConsoleApplication2.UI.Collisions
{
    public class CircleCollision : UIElement
    {
        public float Radius { get; set; }
        public float Angle { get; set; }

        public CircleCollision()
        {
            X = 0;
            Y = 0;
            Radius = 0;
        }

        protected override bool IsIntersection(float x, float y)
        {
            var distance = Math.Sqrt(Math.Pow(X - x, 2) + Math.Pow(Y - y, 2));

            return distance <= Radius;
        }
    }
}