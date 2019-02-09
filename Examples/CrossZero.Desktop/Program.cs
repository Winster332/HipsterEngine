using System;
using System.Collections.Generic;
using System.IO;
using CrossZero.Core.Assets;
using CrossZero.Core.Screens;
using HipsterEngine.Core.Configurations;
using HipsterEngine.Core.Desktop;

namespace CrossZero.Desktop
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var assetsFiles = new List<string>()
            {
                @"../../../CrossZero.Core/Assets/background.png"
            };
            Assets.Init(Environment.CurrentDirectory);
            Assets.Load(GetStreams(assetsFiles));
            
            Builder.Create(new HipsterStartup(), 1024, 720)
                .SetTargetFPS(60, 60)
                .Run(new StartScreen());
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