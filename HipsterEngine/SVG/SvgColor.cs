using System;
using System.Collections.Generic;
using System.Linq;

namespace TestOpenTK
{
    public class SvgColor
    {
        public int A { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        
        public SvgColor(int a, int r, int g, int b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }
        
        public SvgColor()
        {
            A = 0;
            R = 0;
            G = 0;
            B = 0;
        }

        public static SvgColor Parse(string color)
        {
            SvgColor svgColor = null;
            
            if (color == "none")
            {
                return svgColor;
            }
            else
            {
                svgColor = new SvgColor();
            }

            if (color[0] == '#')
            {
                color = color.Substring(1, color.Length - 1);
            }

            var listColor = new List<int>();
            
            for (var i = 0; i < color.Length; i+=2)
            {
                var stringColor = $"{color[i]}{color[i+1]}";
                var intColor = Convert.ToInt32(stringColor, 16);
                listColor.Add(intColor);
            }

            if (listColor.Count == 3)
            {
                svgColor.A = 255;
                svgColor.R = listColor[0];
                svgColor.G = listColor[1];
                svgColor.B = listColor[2];
            }
            else if (listColor.Count == 4)
            {
                svgColor.A = listColor[0];
                svgColor.R = listColor[1];
                svgColor.G = listColor[2];
                svgColor.B = listColor[3];
            }

            return svgColor;
        }

        public override string ToString()
        {
            return $"{A},{R},{G},{B}";
        }
    }
}