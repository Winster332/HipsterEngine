namespace ConsoleApplication2.FS
{
    public interface IFiles
    {
        bool Serialize<T>(T data, string path) where T : class;
        T Deserialize<T>(string path) where T : class;
        bool Exist(string path);
    }
}