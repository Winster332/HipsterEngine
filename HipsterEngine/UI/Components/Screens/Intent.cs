namespace ConsoleApplication2.UI.Components.Screens
{
    public class Intent
    {
        public Screen From { get; set; }
        public Screen To { get; set; }
        public IScreenAnimation Animation { get; set; }
        public object Data { get; set; }
    }
}