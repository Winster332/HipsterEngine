namespace ConsoleApplication2.UI.Events
{
    public enum MouseAction { Down, Move, Up, PressedMove }
    public enum MouseButton { Left, Right }
    
    public class MouseState
    {
        public float X { get; set; }
        public float Y { get; set; }
        public MouseAction? Action { get; set; }
        public MouseButton? Button { get; set; }

        public MouseState()
        {
            Action = null;
            Button = null;
            X = -1;
            Y = -1;
        }
        
        public MouseState(float x, float y, MouseAction action, MouseButton button)
        {
            Action = action;
            Button = button;
            X = x;
            Y = y;
        }
    }
}