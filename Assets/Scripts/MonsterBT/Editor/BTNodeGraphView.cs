using System.Collections.Generic;
using System.Linq;
using MonsterBT.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MonsterBT.Editor
{
    public class BTNodeGraphView : GraphView
    {
        private BehaviorTree behaviorTree;
        private Dictionary<BTNode, BTNodeView> nodeViews;

        public BTNodeGraphView()
        {
            behaviorTree = null;
            nodeViews = new Dictionary<BTNode, BTNodeView>();

            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/Scripts/MonsterBT/Editor/BTNodeGraphStyle.uss"));

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var miniMap = new MiniMap();
            miniMap.name = "mini-map";
            miniMap.AddToClassList("mini-map");
            Add(miniMap);

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            name = "node-graph";
            AddToClassList("node-graph-view");

            graphViewChanged += OnGraphViewChanged;
            PopulateView();
        }

        public void SetBehaviorTree(BehaviorTree tree)
        {
            behaviorTree = tree;
            PopulateView();
        }

        private void PopulateView()
        {
            // 填充当前视图
            graphViewChanged -= OnGraphViewChanged;

            // 清除现有内容
            DeleteElements(graphElements);
            nodeViews.Clear();

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
                if (composite.Children == null || composite.Children.Count == 0)
                    return;

                foreach (var child in composite.Children)
                {
                    CreateNodeView(child);
                    CreateNodeViewsRecursive(child);
                }
            }
            else if (node is DecoratorNode decorator)
            {
                if (decorator.Child == null)
                    return;

                CreateNodeView(decorator.Child);
                CreateNodeViewsRecursive(decorator.Child);
            }
            else if (node is RootNode root)
            {
                if (root.Child == null)
                    return;

                CreateNodeView(root.Child);
                CreateNodeViewsRecursive(root.Child);
            }
        }

        private void CreateNodeView(BTNode node)
        {
            if (nodeViews.ContainsKey(node))
                return;

            var nodeView = new BTNodeView(node);
            nodeViews[node] = nodeView;
            AddElement(nodeView);
        }

        private void CreateConnections()
        {
            foreach (var (node, nodeView) in nodeViews)
            {
                // 连接到子节点
                if (node is RootNode root)
                {
                    if (root.Child == null)
                        return;

                    ConnectNodes(nodeView, nodeViews[root.Child]);
                }
                else if (node is DecoratorNode decorator)
                {
                    if (decorator.Child == null)
                        return;

                    ConnectNodes(nodeView, nodeViews[decorator.Child]);
                }
                else if (node is CompositeNode composite)
                {
                    if (composite.Children == null || composite.Children.Count == 0)
                        return;

                    foreach (var child in composite.Children)
                    {
                        ConnectNodes(nodeView, nodeViews[child]);
                    }
                }
            }
        }

        private void ConnectNodes(BTNodeView parentNode, BTNodeView childNode)
        {
            if (parentNode.OutputPort == null || childNode.InputPort == null)
                return;

            var edge = parentNode.OutputPort.ConnectTo(childNode.InputPort);
            AddElement(edge);
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            // 处理删除的元素
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (var element in graphViewChange.elementsToRemove)
                {
                    if (element is BTNodeView nodeView)
                    {
                        nodeViews.Remove(nodeView.Node);
                    }
                    else if (element is Edge edge)
                    {
                        // 断开连接
                        var parentView = edge.output.node as BTNodeView;
                        var childView = edge.input.node as BTNodeView;

                        switch (parentView?.Node)
                        {
                            case RootNode rootNode:
                                rootNode.Child = null;
                                break;
                            case DecoratorNode decoratorNode:
                                decoratorNode.Child = null;
                                break;
                            case CompositeNode compositeNode when childView != null:
                                compositeNode.Children.Remove(childView.Node);
                                break;
                        }
                    }
                }
            }

            // 处理创建的连接
            if (graphViewChange.edgesToCreate != null)
            {
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    var parentView = edge.output.node as BTNodeView;
                    var childView = edge.input.node as BTNodeView;

                    switch (parentView?.Node)
                    {
                        case RootNode rootNode when childView != null:
                            rootNode.Child = childView.Node;
                            break;
                        case DecoratorNode decoratorNode when childView != null:
                            decoratorNode.Child = childView.Node;
                            break;
                        case CompositeNode compositeNode when childView != null:
                            compositeNode.Children.Add(childView.Node);
                            break;
                    }
                }
            }

            return graphViewChange;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports
                .ToList()
                .Where(endPort =>
                    endPort.direction != startPort.direction &&
                    endPort.node != startPort.node)
                .ToList();
        }
    }
}