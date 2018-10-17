using System.Collections.Generic;

namespace FlatPlant.Graph
{
    public class Stage
    {
        public int LevelDepth { get; set; }
        public List<Node> Nodes { get; set; }

        public Stage(int levelDepth)
        {
            LevelDepth = levelDepth;
            Nodes = new List<Node>();
        }
        
        public Stage(int levelDepth, Node node)
        {
            LevelDepth = levelDepth;
            Nodes = new List<Node>();
            Nodes.Add(node);
        }
        
        public Stage(int levelDepth, List<Node> nodes)
        {
            LevelDepth = levelDepth;
            Nodes = nodes;
        }
    }
}