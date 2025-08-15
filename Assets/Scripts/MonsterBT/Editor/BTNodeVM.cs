using MonsterBT.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MonsterBT.Editor
{
    public class BTNodeVM : Node
    {
        public BTNode Node { get; }
        public Port InputPort { get; private set; }
        public Port OutputPort { get; private set; }

        private VisualElement nodeRoot;
        private Label nodeTitle;
        private Label nodeDescription;
        private VisualElement nodeIcon;
        private VisualElement stateIndicator;

        public BTNodeVM(BTNode node)
        {
            Node = node;

            // 加载节点模板
            var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/Scripts/MonsterBT/Editor/BTNodeLayout.uxml");

            if (template != null)
            {
                nodeRoot = template.CloneTree();
                Add(nodeRoot);

                SetupNodeElements();
                SetupPorts();
                SetupNodeData();
                SetupNodeStyle();
            }

            // 设置位置
            SetPosition(new Rect(node.Position, Vector2.zero));
        }

        private void SetupNodeElements()
        {
            // 获取UI元素引用
            nodeTitle = nodeRoot.Q<Label>("node-title");
            nodeDescription = nodeRoot.Q<Label>("node-description");
            nodeIcon = nodeRoot.Q<VisualElement>("node-icon");
            stateIndicator = nodeRoot.Q<VisualElement>("node-state-indicator");
        }

        private void SetupPorts()
        {
            var inputPortContainer = nodeRoot.Q<VisualElement>("input-port");
            var outputPortContainer = nodeRoot.Q<VisualElement>("output-port");

            // 根据节点类型创建端口
            if (!(Node is RootNode))
            {
                InputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                InputPort.portName = "";
                if (inputPortContainer != null)
                {
                    inputPortContainer.Add(InputPort);
                }
            }
            else
            {
                // 隐藏根节点的输入端口
                if (inputPortContainer != null)
                {
                    inputPortContainer.style.display = DisplayStyle.None;
                }
            }

            // 输出端口
            if (Node is RootNode || Node is CompositeNode)
            {
                OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }
            else if (Node is DecoratorNode)
            {
                OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single,
                    typeof(bool));
            }

            if (OutputPort != null)
            {
                OutputPort.portName = "";
                if (outputPortContainer != null)
                {
                    outputPortContainer.Add(OutputPort);
                }
            }
            else
            {
                // 隐藏叶子节点的输出端口
                if (outputPortContainer != null)
                {
                    outputPortContainer.style.display = DisplayStyle.None;
                }
            }
        }

        private void SetupNodeData()
        {
            // 设置节点标题
            if (nodeTitle != null)
            {
                nodeTitle.text = string.IsNullOrEmpty(Node.name) ? Node.GetType().Name : Node.name;
            }

            // 设置节点描述
            if (nodeDescription != null)
            {
                nodeDescription.text = GetNodeDescription();
            }

            // 设置状态指示器
            UpdateStateIndicator();
        }

        private void SetupNodeStyle()
        {
            // 根据节点类型设置样式类
            if (Node is RootNode)
            {
                nodeRoot.AddToClassList("root");
            }
            else if (Node is CompositeNode)
            {
                nodeRoot.AddToClassList("composite");
            }
            else if (Node is DecoratorNode)
            {
                nodeRoot.AddToClassList("decorator");
            }
            else if (Node is ActionNode)
            {
                nodeRoot.AddToClassList("action");
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

        private void UpdateStateIndicator()
        {
            if (stateIndicator == null) return;

            // 清除现有状态类
            stateIndicator.RemoveFromClassList("running");
            stateIndicator.RemoveFromClassList("success");
            stateIndicator.RemoveFromClassList("failure");

            // 根据节点状态设置样式
            switch (Node.State)
            {
                case BTNodeState.Running:
                    stateIndicator.AddToClassList("running");
                    break;
                case BTNodeState.Success:
                    stateIndicator.AddToClassList("success");
                    break;
                case BTNodeState.Failure:
                    stateIndicator.AddToClassList("failure");
                    break;
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            // 更新节点数据中的位置
            Node.Position = new Vector2(newPos.xMin, newPos.yMin);

            // 标记为脏数据
            if (Node != null)
            {
                EditorUtility.SetDirty(Node);
            }
        }

        public void RefreshNode()
        {
            SetupNodeData();
            UpdateStateIndicator();
        }
    }
}