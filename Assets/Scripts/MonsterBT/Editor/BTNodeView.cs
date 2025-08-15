using MonsterBT.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MonsterBT.Editor
{
    public sealed class BTNodeView : Node
    {
        public BTNode Node { get; }
        public Port InputPort { get; private set; }
        public Port OutputPort { get; private set; }

        public BTNodeView(BTNode node)
        {
            Node = node;

            SetupNodeContent();
            SetupPorts();
            SetupNodeStyle();

            SetPosition(new Rect(node.Position, Vector2.zero));
        }

        private void SetupNodeContent()
        {
            title = string.IsNullOrEmpty(Node.name) ? Node.GetType().Name : Node.name;

            var description = new Label(GetNodeDescription())
            {
                name = "description"
            };
            description.AddToClassList("node-description");

            mainContainer.Add(description);
        }

        private void SetupPorts()
        {
            // Input Part
            if (Node is not RootNode)
            {
                InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single,
                    typeof(bool));
                InputPort.portName = "";
                inputContainer.Add(InputPort);
            }

            // Output Part
            if (Node is RootNode or CompositeNode)
            {
                OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi,
                    typeof(bool));
            }
            else if (Node is DecoratorNode)
            {
                OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single,
                    typeof(bool));
            }

            if (OutputPort != null)
            {
                OutputPort.portName = "";
                outputContainer.Add(OutputPort);
            }
        }

        private void SetupNodeStyle()
        {
            switch (Node)
            {
                case RootNode:
                    AddToClassList("root-node");
                    break;
                case CompositeNode:
                    AddToClassList("composite-node");
                    break;
                case DecoratorNode:
                    AddToClassList("decorator-node");
                    break;
                case ActionNode:
                    AddToClassList("action-node");
                    break;
            }
        }

        private string GetNodeDescription()
        {
            return Node switch
            {
                RootNode => "Root of the behavior tree",
                SelectorNode => "Execute children until one succeeds",
                SequenceNode => "Execute children in sequence until one fails",
                Inverter => "Invert the result of child node",
                ActionNode action => $"Action: {action.GetType().Name}",
                _ => "Behavior tree node"
            };
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Node.Position = new Vector2(newPos.xMin, newPos.yMin);
            EditorUtility.SetDirty(Node);
        }
    }
}