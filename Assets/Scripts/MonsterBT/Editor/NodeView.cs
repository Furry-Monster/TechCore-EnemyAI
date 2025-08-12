using UnityEngine;
using UnityEngine.UIElements;
using MonsterBT.Runtime;

namespace MonsterBT.Editor
{
    public class NodeView : VisualElement
    {
        public BTNode Node { get; private set; }
        public Port InputPort { get; private set; }
        public Port OutputPort { get; private set; }

        private Label titleLabel;
        private VisualElement content;

        public NodeView(BTNode node) : base()
        {
            Node = node;

            // 设置基本样式
            AddToClassList("node-view");
            style.position = Position.Absolute;
            style.left = node.Position.x;
            style.top = node.Position.y;
            style.width = 150;
            style.minHeight = 80;
            style.backgroundColor = GetNodeColor(node);
            style.borderTopWidth = 2;
            style.borderBottomWidth = 2;
            style.borderLeftWidth = 2;
            style.borderRightWidth = 2;
            style.borderTopColor = Color.black;
            style.borderBottomColor = Color.black;
            style.borderLeftColor = Color.black;
            style.borderRightColor = Color.black;
            style.borderTopLeftRadius = 5;
            style.borderTopRightRadius = 5;
            style.borderBottomLeftRadius = 5;
            style.borderBottomRightRadius = 5;

            CreatePorts();
            CreateTitle();
            CreateContent();

            // 支持拖拽
            this.AddManipulator(new Draggable(OnDragUpdate));
        }

        private void CreatePorts()
        {
            // 输入端口（除了根节点）
            if (!(Node is RootNode))
            {
                InputPort = new Port
                {
                    name = "input",
                    style = { position = Position.Absolute, top = -5, left = -5 }
                };
                InputPort.AddToClassList("input-port");
                Add(InputPort);
            }

            // 输出端口（非动作节点）
            if (Node is CompositeNode || Node is DecoratorNode || Node is RootNode)
            {
                OutputPort = new Port
                {
                    name = "output",
                    style = { position = Position.Absolute, bottom = -5, left = style.width.value.value / 2 - 5 }
                };
                OutputPort.AddToClassList("output-port");
                Add(OutputPort);
            }
        }

        private void CreateTitle()
        {
            titleLabel = new Label(Node.NodeName)
            {
                style =
                {
                    unityTextAlign = TextAnchor.MiddleCenter,
                    paddingTop = 4,
                    paddingBottom = 4,
                    backgroundColor = Color.black * 0.1f,
                    color = Color.white,
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            };
            Add(titleLabel);
        }

        private void CreateContent()
        {
            content = new VisualElement()
            {
                name = "content",
                style =
                {
                    paddingTop = 4,
                    paddingBottom = 4,
                    paddingLeft = 4,
                    paddingRight = 4
                }
            };

            // 显示节点状态
            var stateLabel = new Label($"State: {Node.State}")
            {
                name = "state-label",
                style =
                {
                    fontSize = 10,
                    color = GetStateColor(Node.State),
                    unityTextAlign = TextAnchor.MiddleCenter
                }
            };
            content.Add(stateLabel);

            Add(content);
        }

        private void OnDragUpdate(Vector2 delta)
        {
            Vector2 newPos = new Vector2(style.left.value.value + delta.x, style.top.value.value + delta.y);
            style.left = newPos.x;
            style.top = newPos.y;

            // 更新节点位置
            Node.Position = newPos;
        }

        private Color GetNodeColor(BTNode node)
        {
            return node switch
            {
                RootNode => new Color(0.3f, 0.5f, 0.3f, 1f), // 绿色
                CompositeNode => new Color(0.3f, 0.3f, 0.5f, 1f), // 蓝色
                DecoratorNode => new Color(0.5f, 0.4f, 0.3f, 1f), // 棕色
                ActionNode => new Color(0.5f, 0.3f, 0.3f, 1f), // 红色
                _ => Color.gray
            };
        }

        private Color GetStateColor(BTNodeState state)
        {
            return state switch
            {
                BTNodeState.Running => Color.yellow,
                BTNodeState.Success => Color.green,
                BTNodeState.Failure => Color.red,
                _ => Color.white
            };
        }

        public void UpdateState()
        {
            var stateLabel = content.Q<Label>("state-label");
            if (stateLabel != null)
            {
                stateLabel.text = $"State: {Node.State}";
                stateLabel.style.color = GetStateColor(Node.State);
            }
        }
    }

    // 简单的拖拽处理器
    public class Draggable : MouseManipulator
    {
        private System.Action<Vector2> onDrag;
        private Vector2 startMousePos;
        private Vector2 startElementPos;
        private bool isDragging;

        public Draggable(System.Action<Vector2> onDrag)
        {
            this.onDrag = onDrag;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.button == 0) // 左键
            {
                startMousePos = evt.mousePosition;
                startElementPos = new Vector2(target.style.left.value.value, target.style.top.value.value);
                isDragging = true;
                target.CaptureMouse();
                evt.StopPropagation();
            }
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (isDragging)
            {
                Vector2 delta = evt.mousePosition - startMousePos;
                onDrag?.Invoke(delta);
                evt.StopPropagation();
            }
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            if (isDragging && evt.button == 0)
            {
                isDragging = false;
                target.ReleaseMouse();
                evt.StopPropagation();
            }
        }
    }

    // 简单的端口视觉元素
    public class Port : VisualElement
    {
        public Port()
        {
            style.width = 10;
            style.height = 10;
            style.backgroundColor = Color.white;
            style.borderTopWidth = 1;
            style.borderBottomWidth = 1;
            style.borderLeftWidth = 1;
            style.borderRightWidth = 1;
            style.borderTopColor = Color.black;
            style.borderBottomColor = Color.black;
            style.borderLeftColor = Color.black;
            style.borderRightColor = Color.black;
            style.borderTopLeftRadius = 5;
            style.borderTopRightRadius = 5;
            style.borderBottomLeftRadius = 5;
            style.borderBottomRightRadius = 5;
        }
    }
}