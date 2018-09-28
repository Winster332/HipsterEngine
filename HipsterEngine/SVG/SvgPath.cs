using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestOpenTK
{
    public class SvgPath : SvgElement
    {
        public SvgStyle Style { get; set; }
        public List<SvgPathCommand> Commands { get; set; }

        public SvgPath()
        {
            Commands = new List<SvgPathCommand>();
        }
        
        public void ParseCommands(string textPath)
        {
            var separators = @"(?=[A-Za-z])";
            var tokens = Regex.Split(textPath, separators).Where(t => !string.IsNullOrEmpty(t));

            foreach (var token in tokens){
                var pathCommand = SvgPathCommand.Parse(token);
                Commands.Add(pathCommand);
                Console.WriteLine(pathCommand.command);

                switch (pathCommand.command)
                {
                    case 'M': break; // Move
                    case 'l': break; // l
                    case 'z': break; // end path
                    case 'c': break; // bezie
                }
            }
        }
    }

    public class SvgPathCommand
    {
        public char command { get; private set; }
        public float[] arguments { get; private set; }

        public SvgPathCommand(char command, params float[] arguments)
        {
            this.command = command;
            this.arguments = arguments;
        }
        
        public static SvgPathCommand Parse(string SVGpathstring)
        {
            var cmd = SVGpathstring.Take(1).Single();
            var remainingargs = SVGpathstring.Substring(1);

            var argSeparators = @"[\s,]|(?=-)";
            var splitArgs = Regex
                .Split(remainingargs, argSeparators)
                .Where(t => !string.IsNullOrEmpty(t));

            var floatArgs = splitArgs.Select(arg => float.Parse(arg)).ToArray();
            return new SvgPathCommand(cmd, floatArgs);
        }
    }
}