using System.Linq;

namespace HipsterEngine.Core.SVG
{
    public class SvgTransform
    {
        public float A { get; set; }
        public float B { get; set; }
        public float C { get; set; }
        public float D { get; set; }
        public float E { get; set; }
        public float F { get; set; }

        public SvgTransform()
        {
            A = 0; B = 0; C = 0; 
            D = 0; E = 0; F = 0;
        }

        public float GetAngle()
        {
            return (float) System.Math.Atan2(B, D);
        }

        public static SvgTransform Parse(string valueString)
        {
            var transform = new SvgTransform();

            var startIndex = valueString.IndexOf("(") + 1;
            var endIndex = valueString.IndexOf(")") - startIndex;
            var matrix = valueString.Substring(startIndex, endIndex).Split(' ').Select(float.Parse).ToArray();

            transform.A = matrix[0];
            transform.B = matrix[1];
            transform.C = matrix[2];
            transform.D = matrix[3];
            transform.E = matrix[4];
            transform.F = matrix[5];
            
            return transform;
        }
    }
}