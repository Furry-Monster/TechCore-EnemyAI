using MonsterBT.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace MonsterBT.Editor
{
    // 节点视图类
    public class BTNodeView : VisualElement
    {
        private BTNode btNode;
        private Label titleLabel;
        private Label descriptionLabel;
        private VisualElement stateIndicator;

        public BTNodeView(BTNode node, VisualTreeAsset template)
        {
            btNode = node;

            // 从模板创建UI
            if (template != null)
            {
                template.CloneTree(this);
                SetupNodeView();
            }
            else
            {
                CreateFallbackView();
            }

            UpdateNodeAppearance();
        }

        private void SetupNodeView()
        {
            titleLabel = this.Q<Label>("node-title");
            descriptionLabel = this.Q<Label>("node-description");
            stateIndicator = this.Q<VisualElement>("state-indicator");

            if (titleLabel != null)
                titleLabel.text = btNode.NodeName;

            if (descriptionLabel != null)
                descriptionLabel.text = GetNodeDescription();
        }

        private void CreateFallbackView()
        {
            AddToClassList("bt-node");

            titleLabel = new Label(btNode.NodeName);
            titleLabel.AddToClassList("bt-node-title");
            Add(titleLabel);

            descriptionLabel = new Label(GetNodeDescription());
            descriptionLabel.AddToClassList("bt-node-content");
            Add(descriptionLabel);
        }

        private void UpdateNodeAppearance()
        {
            // 根据节点类型添加样式类
            string nodeType = GetNodeType();
            AddToClassList($"bt-node-{nodeType}");

            if (titleLabel != null)
                titleLabel.AddToClassList($"bt-node-title-{nodeType}");

            // 更新状态指示器
            if (stateIndicator != null)
                UpdateStateIndicator();
        }

        private string GetNodeType()
        {
            if (btNode is RootNode) return "root";
            if (btNode is CompositeNode) return "composite";
            if (btNode is DecoratorNode) return "decorator";
            if (btNode is ActionNode) return "action";

            return "unknown";
        }

        private string GetNodeDescription()
        {
            return btNode.GetType().Name;
        }

        private void UpdateStateIndicator()
        {
            if (stateIndicator == null) return;

            stateIndicator.RemoveFromClassList("state-running");
            stateIndicator.RemoveFromClassList("state-success");
            stateIndicator.RemoveFromClassList("state-failure");
            stateIndicator.RemoveFromClassList("state-idle");

            switch (btNode.State)
            {
                case BTNodeState.Running:
                    stateIndicator.AddToClassList("state-running");
                    break;
                case BTNodeState.Success:
                    stateIndicator.AddToClassList("state-success");
                    break;
                case BTNodeState.Failure:
                    stateIndicator.AddToClassList("state-failure");
                    break;
                default:
                    stateIndicator.AddToClassList("state-idle");
                    break;
            }
        }

        public BTNode GetBTNode() => btNode;

        public Vector2 GetPosition() => new Vector2(style.left.value.value, style.top.value.value);

        public void SetPosition(Vector2 position)
        {
            style.left = position.x;
            style.top = position.y;
        }
    }
}