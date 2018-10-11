using System;
using System.Collections.Generic;
using System.IO;
using SkiaSharp;

namespace HoleVortex.Asserts
{
    public static class Assert
    {
        public static List<SKBitmap> Bitmaps;
        public static string PathToProfile { get; set; } = "profile.xml";
        
        public static void Load(string[] names)
        {
            Bitmaps = new List<SKBitmap>();

            foreach (var name in names)
            {
                using (var stream = new FileStream(name, FileMode.Open))
                {
                    Bitmaps.Add(SKBitmap.Decode(stream));
                }
            }
        }
    }
}