namespace TestPhysics
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            using (var window = new AppWindow())
            {
                window.Run(60.0f, 60.0f);
            }
        }
    }
}