using System.Collections.Generic;
using System.Linq;
using Box2DX.Common;

namespace HipsterEngine.Core.SVG
{
    public class SvgPolygon : SvgElement
    {
        public SvgStyle Style { get; set; }
        public List<Vec2> Points { get; set; }

        public SvgPolygon()
        {
            Points = new List<Vec2>();
        }

        public static List<Vec2> ParsePoints(string valueText)
        {
            var points = valueText.Split(' ')
                .Where(e => e != "")
                .Select(x => x.Split(','))
                .Select(d => new Vec2(float.Parse(d[0]), float.Parse(d[1]))).ToList();

            return points;
        }

        public Vec2 ExtractSize()
        {
            var size = new Vec2(0, 0);

            for (var i = 0; i < Points.Count; i++)
            {
                var point = Points[i];

                if (point.X > size.X)
                    size.X = point.X;
                
                if (point.Y > size.Y)
                    size.Y = point.Y;
            }

            return size;
        }
    }
}