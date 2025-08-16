using System;
using System.Collections.Generic;
using System.Linq;
using MonsterBT.Runtime;
using MonsterBT.Runtime.Actions;
using MonsterBT.Runtime.Conditions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Blackboard = UnityEditor.Experimental.GraphView.Blackboard;
using Object = UnityEngine.Object;

namespace MonsterBT.Editor
{
    public class BTNodeGraphView : GraphView
    {
        private BehaviorTree behaviorTree;
        private Dictionary<BTNode, BTNodeView> nodeViews;

        #region General GraphView Methods

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
            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            var miniMap = new MiniMap
            {
                name = "mini-map",
            };
            miniMap.AddToClassList("mini-map");
            Add(miniMap);

            var blackboard = new Blackboard
            {
                name = "blackboard",
                title = "Variables"
            };
            blackboard.AddToClassList("blackboard");
            Add(blackboard);

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            name = "node-graph";
            AddToClassList("node-graph-view");

            graphViewChanged += OnGraphViewChanged;

            // 使用更可靠的事件监听方式
            this.schedule.Execute(CheckSelection).Every(100);

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

            if (behaviorTree?.RootNode != null)
            {
                CreateNodeViewFromNode(behaviorTree.RootNode);
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
                    CreateNodeViewFromNode(child);
                    CreateNodeViewsRecursive(child);
                }
            }
            else if (node is DecoratorNode decorator)
            {
                if (decorator.Child == null)
                    return;

                CreateNodeViewFromNode(decorator.Child);
                CreateNodeViewsRecursive(decorator.Child);
            }
            else if (node is RootNode root)
            {
                if (root.Child == null)
                    return;

                CreateNodeViewFromNode(root.Child);
                CreateNodeViewsRecursive(root.Child);
            }
        }

        private BTNodeView CreateNodeViewFromNode(BTNode node)
        {
            if (nodeViews.ContainsKey(node))
                return null;

            var nodeView = new BTNodeView(node);
            nodeViews[node] = nodeView;
            AddElement(nodeView);
            return nodeView;
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

        #endregion

        #region ContextMenu Methods

        private BTNode copiedNode;
        private Vector2 mousePosition;

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            mousePosition = evt.localMousePosition;

            switch (evt.target)
            {
                case GraphView:
                    BuildGraphContextMenu(evt);
                    break;
                case BTNodeView nodeView:
                    BuildNodeContextMenu(evt, nodeView);
                    break;
            }
        }

        private void BuildGraphContextMenu(ContextualMenuPopulateEvent evt)
        {
            #region Node Ops

            // 复合节点
            evt.menu.AppendAction("Create Node/Composite/Sequence", _ => CreateNode<SequenceNode>(),
                DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Create Node/Composite/Selector", _ => CreateNode<SelectorNode>(),
                DropdownMenuAction.AlwaysEnabled);
            // 装饰器节点
            evt.menu.AppendAction("Create Node/Decorator/Inverter", _ => CreateNode<Inverter>(),
                DropdownMenuAction.AlwaysEnabled);
            // 行为节点
            evt.menu.AppendAction("Create Node/Action/Move To Target",
                _ => CreateNode<MoveToTargetAction>(), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Create Node/Action/Wait", _ => CreateNode<WaitAction>(),
                DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Create Node/Action/Debug Log",
                _ => CreateNode<DebugLogAction>(), DropdownMenuAction.AlwaysEnabled);
            // 条件节点
            evt.menu.AppendAction("Create Node/Condition/Distance Check",
                _ => CreateNode<DistanceCondition>(), DropdownMenuAction.AlwaysEnabled);

            #endregion

            #region Blackboard Ops

            // Blackboard变量管理
            evt.menu.AppendAction("Blackboard/Add Boolean", _ => AddBlackboardVariable("NewBool", typeof(bool)),
                DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Blackboard/Add Float", _ => AddBlackboardVariable("NewFloat", typeof(float)),
                DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Blackboard/Add Vector3", _ => AddBlackboardVariable("NewVector3", typeof(Vector3)),
                DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Blackboard/Add GameObject",
                _ => AddBlackboardVariable("NewGameObject", typeof(GameObject)), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Blackboard/Add String", _ => AddBlackboardVariable("NewString", typeof(string)),
                DropdownMenuAction.AlwaysEnabled);

            #endregion

            // 粘贴功能
            evt.menu.AppendAction("Paste", _ => PasteNode(),
                action => copiedNode == null
                    ? DropdownMenuAction.AlwaysDisabled(action)
                    : DropdownMenuAction.AlwaysEnabled(action));
        }

        private void BuildNodeContextMenu(ContextualMenuPopulateEvent evt, BTNodeView nodeView)
        {
            // 基本操作
            evt.menu.AppendAction("Copy", _ => CopyNode(nodeView), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Cut", _ => CutNode(nodeView), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Duplicate", _ => DuplicateNode(nodeView), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Delete", _ => DeleteNode(nodeView), DropdownMenuAction.AlwaysEnabled);
        }

        public void CreateNode<T>() where T : BTNode
        {
            if (behaviorTree == null)
                return;

            var node = ScriptableObject.CreateInstance<T>();
            node.name = typeof(T).Name;

            AssetDatabase.AddObjectToAsset(node, behaviorTree);

            var nodeView = CreateNodeViewFromNode(node);
            nodeView.SetPosition(new Rect(mousePosition, Vector2.zero));

            EditorUtility.SetDirty(behaviorTree);
            AssetDatabase.SaveAssets();
        }

        private void CopyNode(BTNodeView nodeView)
        {
            copiedNode = nodeView.Node;
        }

        private void CutNode(BTNodeView nodeView)
        {
            CopyNode(nodeView);
            DeleteNode(nodeView);
        }

        private void DuplicateNode(BTNodeView nodeView)
        {
            if (behaviorTree == null)
                return;

            var originalNode = nodeView.Node;
            var duplicatedNode = Object.Instantiate(originalNode);
            duplicatedNode.name = originalNode.name + " (Copy)";

            AssetDatabase.AddObjectToAsset(duplicatedNode, behaviorTree);

            var duplicatedView = CreateNodeViewFromNode(duplicatedNode);
            var originalPos = nodeView.GetPosition();
            duplicatedView.SetPosition(new Rect(originalPos.x + 200, originalPos.y, originalPos.width,
                originalPos.height));

            EditorUtility.SetDirty(behaviorTree);
            AssetDatabase.SaveAssets();
        }

        private void DeleteNode(BTNodeView nodeView)
        {
            if (nodeView.Node is RootNode)
            {
                Debug.LogWarning("Cannot delete root node!");
                return;
            }

            RemoveElement(nodeView);
            nodeViews.Remove(nodeView.Node);

            Object.DestroyImmediate(nodeView.Node, true);

            EditorUtility.SetDirty(behaviorTree);
            AssetDatabase.SaveAssets();
        }

        private void PasteNode()
        {
            if (copiedNode == null || behaviorTree == null)
                return;

            var pastedNode = Object.Instantiate(copiedNode);
            pastedNode.name = copiedNode.name + " (Paste)";

            AssetDatabase.AddObjectToAsset(pastedNode, behaviorTree);

            var pastedView = CreateNodeViewFromNode(pastedNode);
            pastedView.SetPosition(new Rect(mousePosition, Vector2.zero));

            EditorUtility.SetDirty(behaviorTree);
            AssetDatabase.SaveAssets();
        }

        private void AddBlackboardVariable(string varName, Type varType)
        {
            var blackboard = this.Q<Blackboard>();
            if (blackboard == null)
                return;

            var field = new BlackboardField { text = varName, typeText = GetTypeDisplayName(varType) };
            blackboard.Add(field);
        }

        private string GetTypeDisplayName(Type type)
        {
            if (type == typeof(bool)) return "bool";
            if (type == typeof(float)) return "float";
            if (type == typeof(Vector3)) return "Vector3";
            if (type == typeof(GameObject)) return "GameObject";
            if (type == typeof(string)) return "string";

            return type.Name;
        }

        #endregion

        #region Inspector Notifications

        public event Action<BTNode> OnNodeSelected;
        public event Action OnNodeDeselected;

        private BTNode lastSelectedNode;

        private void CheckSelection()
        {
            var selectedNodes = selection.OfType<BTNodeView>().ToList();
            var currentSelectedNode = selectedNodes.Count == 1 ? selectedNodes[0].Node : null;

            // 检查选择是否发生变化
            if (currentSelectedNode != lastSelectedNode)
            {
                lastSelectedNode = currentSelectedNode;

                if (currentSelectedNode != null)
                {
                    OnNodeSelected?.Invoke(currentSelectedNode);
                }
                else
                {
                    OnNodeDeselected?.Invoke();
                }
            }
        }

        #endregion

    }
}