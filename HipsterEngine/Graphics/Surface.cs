namespace ConsoleApplication2.Graphics
{
    public class Surface
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public Canvas Canvas { get; set; }
        public float KSize { get; set; }

        public Surface()
        {
            Width = 0;
            Height = 0;
            Canvas = new Canvas(this);
        }

        public void OnResize(float width, float height)
        {
            Width = width;
            Height = height;
            KSize = Width / Height;
        }
    }
}