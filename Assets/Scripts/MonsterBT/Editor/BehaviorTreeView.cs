using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using MonsterBT.Runtime;

namespace MonsterBT.Editor
{
    public class BehaviorTreeView : VisualElement
    {
        private BehaviorTree behaviorTree;
        private Dictionary<BTNode, NodeView> nodeViews = new Dictionary<BTNode, NodeView>();
        private List<Connection> connections = new List<Connection>();

        public BehaviorTreeView()
        {
            // 设置样式
            style.flexGrow = 1;
            style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);

            // 添加网格背景
            AddToClassList("behavior-tree-view");

            // 注册右键菜单
            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));

            // 注册鼠标事件用于绘制连接线
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            RegisterCallback<MouseUpEvent>(OnMouseUp);

            // 定期重绘连接线
            schedule.Execute(MarkDirtyRepaint).Every(16); // ~60fps
        }

        public void SetBehaviorTree(BehaviorTree tree)
        {
            behaviorTree = tree;
            RefreshView();
        }

        private void RefreshView()
        {
            // 清空当前视图
            Clear();
            nodeViews.Clear();
            connections.Clear();

            if (behaviorTree == null || behaviorTree.RootNode == null)
                return;

            // 递归创建节点视图
            CreateNodeView(behaviorTree.RootNode);
            CreateConnections();
        }

        private void CreateNodeView(BTNode node)
        {
            if (node == null || nodeViews.ContainsKey(node))
                return;

            var nodeView = new NodeView(node);
            nodeViews[node] = nodeView;
            Add(nodeView);

            // 递归创建子节点
            if (node is CompositeNode composite)
            {
                foreach (var child in composite.Children)
                {
                    CreateNodeView(child);
                }
            }
            else if (node is DecoratorNode decorator && decorator.Child != null)
            {
                CreateNodeView(decorator.Child);
            }
            else if (node is RootNode root && root.Child != null)
            {
                CreateNodeView(root.Child);
            }
        }

        private void CreateConnections()
        {
            connections.Clear();

            foreach (var kvp in nodeViews)
            {
                var node = kvp.Key;
                var nodeView = kvp.Value;

                // 为复合节点创建到子节点的连接
                if (node is CompositeNode composite)
                {
                    foreach (var child in composite.Children)
                    {
                        if (nodeViews.ContainsKey(child))
                        {
                            connections.Add(new Connection(nodeView, nodeViews[child]));
                        }
                    }
                }
                // 为装饰节点创建到子节点的连接
                else if (node is DecoratorNode decorator && decorator.Child != null)
                {
                    if (nodeViews.ContainsKey(decorator.Child))
                    {
                        connections.Add(new Connection(nodeView, nodeViews[decorator.Child]));
                    }
                }
                // 为根节点创建到子节点的连接
                else if (node is RootNode root && root.Child != null)
                {
                    if (nodeViews.ContainsKey(root.Child))
                    {
                        connections.Add(new Connection(nodeView, nodeViews[root.Child]));
                    }
                }
            }
        }

        private void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var mousePos = evt.localMousePosition;

            evt.menu.AppendAction("Create/Sequence Node", (_) => CreateNode<SequenceNode>(mousePos));
            evt.menu.AppendAction("Create/Selector Node", (_) => CreateNode<SelectorNode>(mousePos));
            evt.menu.AppendAction("Create/Inverter", (_) => CreateNode<Inverter>(mousePos));
            evt.menu.AppendSeparator();
            evt.menu.AppendAction("Create/Debug Log Action",
                (_) => CreateNode<Runtime.Actions.DebugLogAction>(mousePos));
            evt.menu.AppendAction("Create/Wait Action", (_) => CreateNode<Runtime.Actions.WaitAction>(mousePos));
            evt.menu.AppendAction("Create/Move To Target Action",
                (_) => CreateNode<Runtime.Actions.MoveToTargetAction>(mousePos));
        }

        private void CreateNode<T>(Vector2 position) where T : BTNode
        {
            if (behaviorTree == null) return;

            var node = ScriptableObject.CreateInstance<T>();
            node.Position = position;
            node.name = typeof(T).Name;

            // 将节点添加到行为树资产中
            AssetDatabase.AddObjectToAsset(node, behaviorTree);
            AssetDatabase.SaveAssets();

            // 创建视图
            var nodeView = new NodeView(node);
            nodeViews[node] = nodeView;
            Add(nodeView);

            EditorUtility.SetDirty(behaviorTree);
        }

        // 连接线绘制相关
        private NodeView connectingFrom;
        private Vector2 connectingToPosition;
        private bool isConnecting;

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.button == 0) // 左键
            {
                var targetNodeView = evt.target as NodeView;
                if (targetNodeView?.OutputPort != null && evt.target == targetNodeView.OutputPort)
                {
                    // 开始连接
                    connectingFrom = targetNodeView;
                    connectingToPosition = evt.localMousePosition;
                    isConnecting = true;
                    evt.StopPropagation();
                }
            }
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (isConnecting)
            {
                connectingToPosition = evt.localMousePosition;
                MarkDirtyRepaint();
            }
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            if (isConnecting && evt.button == 0)
            {
                var targetNodeView = GetNodeViewAtPosition(evt.localMousePosition);
                if (targetNodeView != null && targetNodeView != connectingFrom && targetNodeView.InputPort != null)
                {
                    // 创建连接
                    ConnectNodes(connectingFrom, targetNodeView);
                }

                isConnecting = false;
                connectingFrom = null;
                MarkDirtyRepaint();
            }
        }

        private NodeView GetNodeViewAtPosition(Vector2 position)
        {
            foreach (var nodeView in nodeViews.Values)
            {
                var rect = new Rect(nodeView.style.left.value.value, nodeView.style.top.value.value,
                    nodeView.style.width.value.value, nodeView.resolvedStyle.height);
                if (rect.Contains(position))
                {
                    return nodeView;
                }
            }
            return null;
        }

        private void ConnectNodes(NodeView from, NodeView to)
        {
            var fromNode = from.Node;
            var toNode = to.Node;

            // 根据节点类型建立连接
            if (fromNode is CompositeNode composite)
            {
                composite.AddChild(toNode);
            }
            else if (fromNode is DecoratorNode decorator)
            {
                decorator.Child = toNode;
            }
            else if (fromNode is RootNode root)
            {
                root.Child = toNode;
            }

            // 重新创建连接显示
            CreateConnections();
            EditorUtility.SetDirty(behaviorTree);
        }

        // 自定义绘制连接线
        // protected override void ImmediateRepaint()
        // {
        //     base.ImmediateRepaint();
        //
        //     // 绘制现有连接
        //     foreach (var connection in connections)
        //     {
        //         DrawConnection(connection);
        //     }
        //
        //     // 绘制正在连接的线
        //     if (isConnecting && connectingFrom != null)
        //     {
        //         DrawConnectingLine();
        //     }
        // }

        private void DrawConnection(Connection connection)
        {
            var fromPos = GetOutputPortWorldPosition(connection.From);
            var toPos = GetInputPortWorldPosition(connection.To);
            DrawBezierLine(fromPos, toPos, Color.white);
        }

        private void DrawConnectingLine()
        {
            var fromPos = GetOutputPortWorldPosition(connectingFrom);
            DrawBezierLine(fromPos, connectingToPosition, Color.yellow);
        }

        private Vector2 GetOutputPortWorldPosition(NodeView nodeView)
        {
            return new Vector2(
                nodeView.style.left.value.value + nodeView.style.width.value.value / 2,
                nodeView.style.top.value.value + nodeView.resolvedStyle.height
            );
        }

        private Vector2 GetInputPortWorldPosition(NodeView nodeView)
        {
            return new Vector2(
                nodeView.style.left.value.value + nodeView.style.width.value.value / 2,
                nodeView.style.top.value.value
            );
        }

        private void DrawBezierLine(Vector2 start, Vector2 end, Color color)
        {
            var startTangent = start + Vector2.up * 50f;
            var endTangent = end + Vector2.down * 50f;

            Handles.DrawBezier(start, end, startTangent, endTangent, color, null, 2f);
        }
    }

    // 连接数据结构
    public class Connection
    {
        public NodeView From { get; }
        public NodeView To { get; }

        public Connection(NodeView from, NodeView to)
        {
            From = from;
            To = to;
        }
    }
}