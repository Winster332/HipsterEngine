using System.Collections.Generic;
using System.IO;
using HoleVortex.Core.Models;
using SkiaSharp;

namespace HoleVortex.Core.IO
{
    public static class Assets
    {
        public static List<SKBitmap> Bitmaps;
        public static string PathToProfile { get; set; } = "profile.xml";
        public static string StoragePath { get; set; }
        public static SKTypeface Typeface { get; set; }
        
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
            
            Typeface = SKTypeface.FromFile("../../../HoleVortex.Core/Assets/Fonts/123458.ttf");
        }

        public static Profile GetProfile(HipsterEngine.Core.HipsterEngine engine)
        {
            var fileName = Path.Combine(StoragePath, PathToProfile);
            var profile = engine.Files.Deserialize<Profile>(fileName);

            return profile;
        }

        public static bool SaveProfile(HipsterEngine.Core.HipsterEngine engine, Profile profile)
        {
            var fileName = Path.Combine(StoragePath, PathToProfile);
            var result = engine.Files.Serialize(profile, fileName);

            return result;
        }
        
        public static bool ExistProfile()
        {
            var fileName = Path.Combine(StoragePath, PathToProfile);

            return File.Exists(fileName);
        }
    }
}