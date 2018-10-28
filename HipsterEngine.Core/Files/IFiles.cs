using System;
using System.Collections.Generic;
using System.Text;

namespace HipsterEngine.Core.Files
{
    public interface IFiles
    {
        bool Serialize<T>(T data, string path) where T : class;
        T Deserialize<T>(string path) where T : class;
        bool Exist(string path);
    }
}
