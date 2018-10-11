using System;
using HoleVortex.Desktop.Asserts;

namespace HoleVortex.Desktop
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Assert.Init(Environment.CurrentDirectory);
            Assert.Load(new[]
            {
                @"../../Asserts/Resources/button-pause.png",
                @"../../Asserts/Resources/button-resume.png",
                @"../../Asserts/Resources/circle-0.png",
                @"../../Asserts/Resources/circle-1.png",
                @"../../Asserts/Resources/circle-2.png",
                @"../../Asserts/Resources/circle-3.png",
                @"../../Asserts/Resources/circle-4.png",
                @"../../Asserts/Resources/circle-5.png",
                @"../../Asserts/Resources/circle-6.png",
                @"../../Asserts/Resources/circle-7.png"
            });
            
            using (var window = new AppWindow())
            {
                window.Run(60.0f, 60.0f);
            }
        }
    }
}