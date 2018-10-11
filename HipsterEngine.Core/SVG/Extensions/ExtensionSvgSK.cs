using System;
using SkiaSharp;

namespace HipsterEngine.Core.SVG.Extensions
{
    public static class ExtensionSvgSK
    {
        public static SKPaint ToSKPaint(this SvgStyle style)
        {
            var paint = new SKPaint();
            paint.IsAntialias = true;

            if (style.Fill != null)
            {
                paint.Style = SKPaintStyle.Fill;
                paint.Color = style.Fill.ToSKColor();
            }

            if (style.Stroke != null)
            {
                paint.Style = SKPaintStyle.Stroke;
                paint.Color = style.Stroke.ToSKColor();
                paint.StrokeWidth = style.StrokeThickness;
            }

            return paint;
        }

        public static SKColor ToSKColor(this SvgColor color)
        {
            var skColor = new SKColor((byte) color.R, (byte) color.G, (byte) color.B, (byte) color.A);

            return skColor;
        }

        public static SKPath ToSKPath(this SvgPath svgPath)
        {
            var path = new SKPath();
            
            svgPath.Commands.ForEach(command =>
            {
                var arguments = command.arguments;
                var name = command.command;

                switch (name)
                {
                    case 'M': path.MoveTo(arguments[0], arguments[1]); break;
                    case 'C': path.RCubicTo(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]); break;
                    case 'c': path.RCubicTo(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]); break;
                    case 'H': path.LineTo(arguments[0], 0); break;
                    case 'h': path.LineTo(arguments[0], 0); break;
                    case 'v': path.LineTo(0, arguments[0]); break;
                    case 'z': path.Close(); break;
                    case 'L': path.LineTo(arguments[0], arguments[1]); break;
                    case 'l': path.LineTo(arguments[0], arguments[1]); break;
                    case 's': path.QuadTo(arguments[0], arguments[1], arguments[2], arguments[3]); break;
                    case 'S': path.QuadTo(arguments[0], arguments[1], arguments[2], arguments[3]); break;
                }
                
                Console.WriteLine(name);
            });

            return path;
        }
    }
}