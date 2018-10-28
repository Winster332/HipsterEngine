using System;
using System.IO;
using System.Xml.Serialization;

namespace HipsterEngine.Core.Files
{
    public class Files : IFiles
    {
        public bool Serialize<T>(T data, string path) where T : class
        {
            try
            {
                var serializer = new XmlSerializer(data.GetType());

                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    serializer.Serialize(stream, data);
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        public T Deserialize<T>(string path) where T : class
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                T res = null;

                using (Stream stream = new FileStream(path, FileMode.Open))
                {
                    res = (T) serializer.Deserialize(stream);
                }
                
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        public bool Exist(string path) => File.Exists(path);
    }
}