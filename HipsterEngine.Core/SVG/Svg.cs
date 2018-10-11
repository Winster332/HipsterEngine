using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HipsterEngine.Core.SVG
{
    public class Svg
    {
        public List<SvgStyle> Styles { get; set; }
        public List<SvgElement> Elements { get; set; }

        public Svg()
        {
            Styles = new List<SvgStyle>();
            Elements = new List<SvgElement>();
        }
        
        public static Svg Load(string pathToFile)
        {
            var svg = new Svg();
            XDocument document = null;

            using (var stream = new FileStream(pathToFile, FileMode.Open))
            {
                document = XDocument.Load(stream);
            }

            var svgElement = document.Root;
            //var line = svgElement.Descendants("{http://www.w3.org/2000/svg}");
            var nodes = svgElement.Nodes();
            foreach (var node in nodes)
            {
                if (node.GetType() == typeof(XText))
                {
                }
                if (node.GetType() == typeof(XElement))
                {
                    svg.SolveElements((XElement) node);
                }
            }

            return svg;
        }

        private void SolveElements(XText element)
        {
           // var name = element.Name.LocalName;
            Console.WriteLine("");
            
            if (element.NextNode.GetType() == typeof(XElement))
                SolveElements((XElement) element.NextNode);
        }

        private void SolveElements(XElement element)
        {
            var name = element.Name.LocalName;

            if (name == "style")
            {
                var value = element.Value;
                var styles = value.Split('\n').Where(x => x != "").ToList();
                styles.ForEach(styleString =>
                {
                    var style = SvgStyle.Parse(styleString);
                    Styles.Add(style);
                });
            }

            if (name == "rect")
            {
                var svgRect = CreateRect(element);
                Elements.Add(svgRect);
            }

            if (name == "circle")
            {
                var svgCircle = CreateCircle(element);
                Elements.Add(svgCircle);
            }

            if (name == "path")
            {
                var svgPath = CreatePath(element);
                Elements.Add(svgPath);
            }

            if (name == "g")
            {
                var svgGroup = CreateGroup(element);
                Elements.Add(svgGroup);
            }
            
            if (name == "line")
            {
                var svgLine = CreateLine(element);
                Elements.Add(svgLine);
            }
            
            if (name == "ellipse")
            {
                var svgLine = CreateEllipse(element);
                Elements.Add(svgLine);
            }
            
            if (name == "polygon")
            {
                var svgPolygon = CreatePolygon(element);
                Elements.Add(svgPolygon);
            }
        }
        
        private SvgElement CreateElement(XElement element)
        {
            SvgElement svgElement = null;
            var name = element.Name.LocalName;

            if (name == "rect")
            {
                svgElement = CreateRect(element);
            }

            if (name == "circle")
            {
                svgElement = CreateCircle(element);
            }

            if (name == "path")
            {
                svgElement = CreatePath(element);
            }

            if (name == "g")
            {
                svgElement = CreateGroup(element);
            }
            
            if (name == "line")
            {
                svgElement = CreateLine(element);
            }
            
            if (name == "ellipse")
            {
                svgElement = CreateEllipse(element);
            }
            if (name == "polygon")
            {
                svgElement = CreatePolygon(element);
            }

            return svgElement;
        }
        
        private SvgGroup CreateGroup(XElement xelement)
        {
            var svgGroup = new SvgGroup();

            foreach (var node in xelement.Nodes())
            {
                if (node.GetType() == typeof(XElement))
                {
                    var element = CreateElement((XElement) node);
                    
                    svgGroup.AddElement(element);
                }
            }

            return svgGroup;
        }
        
        private SvgElement CreatePolygon(XElement xelement)
        {
            var svgElement = new SvgPolygon();

            foreach (var xAttribute in xelement.Attributes())
            {
                var attributeName = xAttribute.Name.LocalName;
                var attributeValue = xAttribute.Value;

                switch (attributeName)
                {
                    case "class": svgElement.Style = GetStyle(attributeValue); break;
                    case "points": svgElement.Points = SvgPolygon.ParsePoints(attributeValue); break;
                }
            }

            return svgElement;
        }

        private SvgElement CreatePath(XElement xelement)
        {
            var svgElement = new SvgPath();

            foreach (var xAttribute in xelement.Attributes())
            {
                var attributeName = xAttribute.Name.LocalName;
                var attributeValue = xAttribute.Value;

                switch (attributeName)
                {
                    case "class": svgElement.Style = GetStyle(attributeValue); break;
                    case "d": svgElement.ParseCommands(attributeValue); break;
                }
            }

            return svgElement;
        }
        
        private SvgElement CreateLine(XElement xelement)
        {
            var svgElement = new SvgLine();
            
            foreach (var xAttribute in xelement.Attributes())
            {
                var attributeName = xAttribute.Name.LocalName;
                var attributeValue = xAttribute.Value;
                
                switch (attributeName)
                {
                    case "class": svgElement.Style = GetStyle(attributeValue); break;
                    case "x1": svgElement.X1 = float.Parse(attributeValue); break;
                    case "y1": svgElement.Y1 = float.Parse(attributeValue); break;
                    case "x2": svgElement.X2 = float.Parse(attributeValue); break;
                    case "y2": svgElement.Y2 = float.Parse(attributeValue); break;
                }
            }

            return svgElement;
        }
        
        private SvgElement CreateEllipse(XElement xelement)
        {
            var svgElement = new SvgEllipse();
            
            foreach (var xAttribute in xelement.Attributes())
            {
                var attributeName = xAttribute.Name.LocalName;
                var attributeValue = xAttribute.Value;
                
                switch (attributeName)
                {
                    case "class": svgElement.Style = GetStyle(attributeValue); break;
                    case "cx": svgElement.X = float.Parse(attributeValue); break;
                    case "cy": svgElement.Y = float.Parse(attributeValue); break;
                    case "rx": svgElement.RadiusX = float.Parse(attributeValue); break;
                    case "ry": svgElement.RadiusY = float.Parse(attributeValue); break;
                    case "transform": svgElement.Transform = SvgTransform.Parse(attributeValue); break;
                }
            }

            return svgElement;
        }

        private SvgElement CreateRect(XElement xelement)
        {
            var svgElement = new SvgRect();
            
            foreach (var xAttribute in xelement.Attributes())
            {
                var attributeName = xAttribute.Name.LocalName;
                var attributeValue = xAttribute.Value;
                
                switch (attributeName)
                {
                    case "class": svgElement.Style = GetStyle(attributeValue); break;
                    case "x": svgElement.X = float.Parse(attributeValue); break;
                    case "y": svgElement.Y = float.Parse(attributeValue); break;
                    case "width": svgElement.Width = float.Parse(attributeValue); break;
                    case "height": svgElement.Height = float.Parse(attributeValue); break;
                    case "transform": svgElement.Transform = SvgTransform.Parse(attributeValue); break;
                }
            }

            return svgElement;
        }

        private SvgCircle CreateCircle(XElement xelement)
        {
            var svgCircle = new SvgCircle();
            
            foreach (var xAttribute in xelement.Attributes())
            {
                var attributeName = xAttribute.Name.LocalName;
                var attributeValue = xAttribute.Value;
                
                switch (attributeName)
                {
                    case "class": svgCircle.Style = GetStyle(attributeValue); break;
                    case "cx": svgCircle.X = float.Parse(attributeValue); break;
                    case "cy": svgCircle.Y = float.Parse(attributeValue); break;
                    case "r": svgCircle.Radius = float.Parse(attributeValue); break;
                }
            }

            return svgCircle;
        }

        public SvgStyle GetStyle(string name)
        {
            var style = Styles.First(s => s.Name == name);

            return style;
        }
    }
}