namespace FlatPlant.Graph
{
    public class Joint
    {
        public Node From { get; set; }
        public Node To { get; set; }

        public Joint(Node from, Node to)
        {
            From = from;
            To = to;
        }
    }
}