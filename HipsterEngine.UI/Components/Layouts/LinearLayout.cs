namespace HipsterEngine.UI.Components.Layouts
{
    public enum LayoutOrientation { Horizontal, Vertical }
    
    public class LinearLayout : Layout
    {
        public LayoutOrientation Orientation { get; set; }
        
        public LinearLayout(Core.HipsterEngine engine) : base(engine)
        {
            Orientation = LayoutOrientation.Horizontal;
        }
    }
}