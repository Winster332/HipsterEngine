using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Android.Content.Res;
using HoleVortex.Android.Models;
using SkiaSharp;

namespace HoleVortex.Android.Asserts
{
    public static class Assert
    {
        public static List<SKBitmap> Bitmaps;
        public static string PathToProfile { get; set; } = "profile.xml";
        public static string StoragePath { get; set; }
        
        public static void Init(string storagePath)
        {
            StoragePath = storagePath;
        }
        
        public static void Load(AssetManager assets, string[] names)
        {
            Bitmaps = new List<SKBitmap>();

            foreach (var name in names)
            {
                using (var stream = assets.Open(name))
                {
                    Bitmaps.Add(SKBitmap.Decode(stream));
                }

                //    using (var stream = new FileStream(name, FileMode.Open))
                //    {
                //        Bitmaps.Add(SKBitmap.Decode(stream));
                //    }
            }

            var bitmaps = Bitmaps;
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