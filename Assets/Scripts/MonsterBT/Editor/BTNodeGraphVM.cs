using MonsterBT.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MonsterBT.Editor
{
    public class BTNodeGraphVM : GraphView
    {
        private BehaviorTree behaviorTree;
        private Dictionary<BTNode, BTNodeVM> nodeViews;

        public BTNodeGraphVM()
        {
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/Scripts/MonsterBT/Editor/BTNodeGraphStyle.uss"));

            nodeViews = new Dictionary<BTNode, BTNodeVM>();

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddToClassList("node-graph-view");

            graphViewChanged += OnGraphViewChanged;
        }

        public void SetBehaviorTree(BehaviorTree tree)
        {
            behaviorTree = tree;
            PopulateView();
        }

        private void PopulateView()
        {
            // 暂时禁用事件监听
            graphViewChanged -= OnGraphViewChanged;

            // 清除现有内容
            DeleteElements(graphElements);
            nodeViews.Clear();

            // 重新启用事件监听
            graphViewChanged += OnGraphViewChanged;

            // 如果有行为树，创建节点视图
            if (behaviorTree?.RootNode != null)
            {
                CreateNodeView(behaviorTree.RootNode);
                CreateNodeViewsRecursive(behaviorTree.RootNode);
                CreateConnections();
            }
        }

        private void CreateNodeViewsRecursive(BTNode node)
        {
            if (node is CompositeNode composite)
            {
                foreach (var child in composite.Children)
                {
                    CreateNodeView(child);
                    CreateNodeViewsRecursive(child);
                }
            }
            else if (node is DecoratorNode decorator && decorator.Child != null)
            {
                CreateNodeView(decorator.Child);
                CreateNodeViewsRecursive(decorator.Child);
            }
            else if (node is RootNode root && root.Child != null)
            {
                CreateNodeView(root.Child);
                CreateNodeViewsRecursive(root.Child);
            }
        }

        private void CreateNodeView(BTNode node)
        {
            if (nodeViews.ContainsKey(node))
                return;

            var nodeView = new BTNodeVM(node);
            nodeViews[node] = nodeView;
            AddElement(nodeView);
        }

        private void CreateConnections()
        {
            foreach (var kvp in nodeViews)
            {
                var node = kvp.Key;
                var nodeView = kvp.Value;

                // 连接到子节点
                if (node is RootNode root && root.Child != null)
                {
                    ConnectNodes(nodeView, nodeViews[root.Child]);
                }
                else if (node is DecoratorNode decorator && decorator.Child != null)
                {
                    ConnectNodes(nodeView, nodeViews[decorator.Child]);
                }
                else if (node is CompositeNode composite)
                {
                    foreach (var child in composite.Children)
                    {
                        ConnectNodes(nodeView, nodeViews[child]);
                    }
                }
            }
        }

        private void ConnectNodes(BTNodeVM parentView, BTNodeVM childView)
        {
            if (parentView.OutputPort != null && childView.InputPort != null)
            {
                var edge = parentView.OutputPort.ConnectTo(childView.InputPort);
                AddElement(edge);
            }
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            // 处理删除的元素
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (var element in graphViewChange.elementsToRemove)
                {
                    if (element is BTNodeVM nodeView)
                    {
                        nodeViews.Remove(nodeView.Node);
                    }
                    else if (element is Edge edge)
                    {
                        // 断开连接
                        var parentView = edge.output.node as BTNodeVM;
                        var childView = edge.input.node as BTNodeVM;

                        if (parentView?.Node is RootNode rootNode)
                        {
                            rootNode.Child = null;
                        }
                        else if (parentView?.Node is DecoratorNode decoratorNode)
                        {
                            decoratorNode.Child = null;
                        }
                        else if (parentView?.Node is CompositeNode compositeNode && childView != null)
                        {
                            compositeNode.Children.Remove(childView.Node);
                        }
                    }
                }
            }

            // 处理创建的连接
            if (graphViewChange.edgesToCreate != null)
            {
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    var parentView = edge.output.node as BTNodeVM;
                    var childView = edge.input.node as BTNodeVM;

                    if (parentView?.Node is RootNode rootNode && childView != null)
                    {
                        rootNode.Child = childView.Node;
                    }
                    else if (parentView?.Node is DecoratorNode decoratorNode && childView != null)
                    {
                        decoratorNode.Child = childView.Node;
                    }
                    else if (parentView?.Node is CompositeNode compositeNode && childView != null)
                    {
                        compositeNode.Children.Add(childView.Node);
                    }
                }
            }

            return graphViewChange;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
                endPort.direction != startPort.direction &&
                endPort.node != startPort.node).ToList();
        }
    }
}