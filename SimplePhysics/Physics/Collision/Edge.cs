using SimplePhysics.Physics.Common;
using SkiaSharp;

namespace SimplePhysics.Physics.Collision
{
    public class Edge
    {
        public Vec2 From { get; set; }
        public Vec2 To { get; set; }

        public Edge()
        {
            From = new Vec2();
            To = new Vec2();
        }

        public Edge(float fromX, float fromY, float toX, float toY)
        {
            From = new Vec2(fromX, fromY);
            To = new Vec2(toX, toY);
        }

        public void Draw(SKCanvas canvas, SKPaint paint)
        {
            canvas.DrawCircle(From.X, From.Y, 10, paint);
            canvas.DrawCircle(To.X, To.Y, 10, paint);
            canvas.DrawLine(From.X, From.Y, To.X, To.Y, paint);
        }

        public static Vec2 GetPointCollide(Edge e1, Edge e2)
        {
            var point = new Vec2();
            var A = e1.From;
            var B = e1.To;
            var C = e2.From;
            var D = e2.To;

            float xo = A.X, yo = A.Y;
            float p = B.X - A.X, q = B.Y - A.Y, r;
 
            float x1 = C.X, y1 = C.Y;
            float p1 = D.X - C.X, q1 = D.Y - C.Y, r1;
 
            float x = (xo * q * p1 - x1 * q1 * p - yo * p * p1 + y1 * p * p1) /
                       (q * p1 - q1 * p);
            float y = (yo * p * q1 - y1 * p1 * q - xo * q * q1 + x1 * q * q1) /
                       (p * q1 - p1 * q);

            point.X = x;
            point.Y = y;

            return point;
        }
    }
}