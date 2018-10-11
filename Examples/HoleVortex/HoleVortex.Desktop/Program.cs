using System;
using System.Collections.Generic;
using System.IO;
using HipsterEngine.Core.Configurations;
using HipsterEngine.Core.Desktop;
using HoleVortex.Core.IO;
using HoleVortex.Core.Screens;

namespace HoleVortex.Desktop
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var assetsFiles = new List<string>()
            {
                @"../../../HoleVortex.Core/Assets/button_pause.png",
                @"../../../HoleVortex.Core/Assets/button_resume.png",
                @"../../../HoleVortex.Core/Assets/circle_0.png",
                @"../../../HoleVortex.Core/Assets/circle_1.png",
                @"../../../HoleVortex.Core/Assets/circle_2.png",
                @"../../../HoleVortex.Core/Assets/circle_3.png",
                @"../../../HoleVortex.Core/Assets/circle_4.png",
                @"../../../HoleVortex.Core/Assets/circle_5.png",
                @"../../../HoleVortex.Core/Assets/circle_6.png",
                @"../../../HoleVortex.Core/Assets/circle_7.png"
            };
            Assets.Init(Environment.CurrentDirectory);
            Assets.Load(GetStreams(assetsFiles));
            
            Builder.Create(new HipsterStartup(), 480, 720)
                .SetTargetFPS(60, 60)
                .Run(new MenuScreen());
        }

        public static Stream[] GetStreams(List<string> files)
        {
            var listStreams = new List<Stream>();
            
            files.ForEach(file =>
            {
                listStreams.Add(new FileStream(file, FileMode.Open));
            });

            return listStreams.ToArray();
        }
    }
}