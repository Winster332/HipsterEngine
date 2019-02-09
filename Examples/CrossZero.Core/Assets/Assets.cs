using System.Collections.Generic;
using System.IO;
using SkiaSharp;

namespace CrossZero.Core.Assets
{
    public static class Assets
    {
        public static List<SKBitmap> Bitmaps;
        public static string StoragePath { get; set; }
        
        public static void Init(string storagePath)
        {
            StoragePath = storagePath;
        }
        
        public static void Load(Stream[] streams)
        {
            Bitmaps = new List<SKBitmap>();

            foreach (var stream in streams)
            {
                Bitmaps.Add(SKBitmap.Decode(stream));
                stream.Close();
            }
        }
    }
}