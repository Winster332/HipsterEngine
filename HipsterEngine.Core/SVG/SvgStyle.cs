using System;
using System.Collections.Generic;
using System.Linq;

namespace HipsterEngine.Core.SVG
{
    public class SvgStyle
    {
        public string Name { get; set; }
        public SvgColor Fill { get; set; }
        public SvgColor Stroke { get; set; }
        public int StrokeThickness { get; set; }
        
        public static SvgStyle Parse(string stringStyle)
        {
            var svgStyle = new SvgStyle();
            var styleProps = svgStyle.ExtractProperties(stringStyle);
            svgStyle.Name = svgStyle.ExtractName(stringStyle);
            
            styleProps.ForEach(prop =>
            {
                var name = prop.Item1;
                var value = prop.Item2;

                svgStyle.UseProperty(name, value);
            });

            return svgStyle;
        }

        private void UseProperty(string name, string value)
        {
            switch (name)
            {
                case "fill":                Fill = SvgColor.Parse(value);            break;
                case "stroke":              Stroke = SvgColor.Parse(value);          break;
                case "stroke-miterlimit":   StrokeThickness = Int32.Parse(value);    break;
            }
        }

        private List<Tuple<string, string>> ExtractProperties(string valueStyle)
        {
            var startIndex = valueStyle.IndexOf("{", StringComparison.Ordinal) + 1;
            var endIndex = valueStyle.IndexOf("}", StringComparison.Ordinal) - startIndex;
            var stylePropString = valueStyle.Substring(startIndex, endIndex);
            var props = stylePropString.Split(';').Where(x => x != "");

            var extractKayValue = new Func<string, Tuple<string, string>>(value =>
            {
                var startForValueProp = value.IndexOf(":") + 1;
                var propName = value.Substring(0, value.IndexOf(":"));
                var propValue = value.Substring(startForValueProp, value.Length - startForValueProp);
                
                return new Tuple<string, string>(propName, propValue);
            });

            var listProps = new List<Tuple<string, string>>();

            foreach (var propStrnig in props)
            {
                var tupleProp = extractKayValue(propStrnig);
                var styleName = tupleProp.Item1;
                var styleValue = tupleProp.Item2;
                
                listProps.Add(new Tuple<string, string>(styleName, styleValue));
            }

            return listProps;
        }

        private string ExtractName(string valueStyle)
        {
            var startIndex = valueStyle.IndexOf(".", StringComparison.Ordinal) + 1;
            var endIndex = valueStyle.IndexOf("{", StringComparison.Ordinal) - startIndex;
            var styleName = valueStyle.Substring(startIndex, endIndex);

            return styleName;
        }
    }
}