using System;
using System.Collections.Generic;
using System.Linq;

namespace FlatPlant.Graph
{
    public class Node
    {
        public float X { get; set; }
        public float Y { get; set; }
        public List<Joint> Inputs { get; set; }
        public List<Joint> Output { get; set; }

        public Node(float x, float y)
        {
            X = x;
            Y = y;
            
            Inputs = new List<Joint>();
            Output = new List<Joint>();
        }
        
        public Node()
        {
            X = 0;
            Y = 0;
            
            Inputs = new List<Joint>();
            Output = new List<Joint>();
        }

        public Node AddNode(Node node)
        {
            node.Inputs.Add(new Joint(node, this));
            Output.Add(new Joint(this, node));

            return node;
        }
        
        public Node[] AddNode(Node[] nodes)
        {
            for (var i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                
                AddNode(node);
            }

            return nodes;
        }
        
        public List<Stage> GetNextNodes(Action<Joint> callBack = null)
        {
            var listStages = new List<Stage>();
            listStages.Add(new Stage(0, this));

            GetReflectNextStages(listStages, callBack);

            return listStages;
        }

        private void GetReflectNextStages(List<Stage> stages, Action<Joint> callBack = null)
        {
            var prevStage = stages.Last();
            var nodesPresentStage = new List<Node>();

            for (var iNode = 0; iNode < prevStage.Nodes.Count; iNode++)
            {
                var node = prevStage.Nodes[iNode];

                for (var iJoint = 0; iJoint < node.Output.Count; iJoint++)
                {
                    var joint = node.Output[iJoint];
                    
                    nodesPresentStage.Add(joint.To);
                    
                    callBack?.Invoke(joint);
                }
            }

            if (nodesPresentStage.Count > 0)
            {
                stages.Add(new Stage(prevStage.LevelDepth++, nodesPresentStage));
                
                GetReflectNextStages(stages);
            }
        }
    }
}