using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TestOpenTK
{
    public class SvgPolygon : SvgElement
    {
        public SvgStyle Style { get; set; }
        public List<PointF> Points { get; set; }

        public SvgPolygon()
        {
            Points = new List<PointF>();
        }

        public static List<PointF> ParsePoints(string valueText)
        {
            var points = valueText.Split(' ')
                .Where(e => e != "")
                .Select(x => x.Split(','))
                .Select(d => new PointF(float.Parse(d[0]), float.Parse(d[1]))).ToList();

            return points;
        }

        public SizeF ExtractSize()
        {
            var size = new SizeF(0, 0);

            for (var i = 0; i < Points.Count; i++)
            {
                var point = Points[i];

                if (point.X > size.Width)
                    size.Width = point.X;
                
                if (point.Y > size.Height)
                    size.Height = point.Y;
            }

            return size;
        }
    }
}