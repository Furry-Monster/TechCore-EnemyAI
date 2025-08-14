using System.Collections.Generic;
using MonsterBT.Runtime;
using MonsterBT.Runtime.Actions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MonsterBT.Editor
{
    public class BTGraphView : VisualElement
    {
        private BehaviorTree behaviorTree;
        private readonly Dictionary<BTNode, BTNodeView> nodeViews = new Dictionary<BTNode, BTNodeView>();
        private VisualElement nodeContainer;
        private VisualElement connectionContainer;

        // 布局资源
        private VisualTreeAsset nodeLayout;
        private VisualTreeAsset contextLayout;

        // 交互状态
        private Vector2 lastMousePosition;
        private BTNodeView draggingNode;
        private bool isCreatingConnection;
        private BTNodeView connectionStart;

        public BTGraphView()
        {
            AddToClassList("graph-view");
            LoadTemplates();
            CreateViewStructure();
            RegisterCallbacks();
        }

        private void LoadTemplates()
        {
            // 加载 UXML 模板
            nodeLayout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/Scripts/MonsterBT/Editor/BTNodeTemplate.uxml");
            contextLayout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/Scripts/MonsterBT/Editor/ContextMenu.uxml");
        }

        private void CreateViewStructure()
        {
            // 创建连接线容器（在节点之下）
            connectionContainer = new VisualElement
            {
                name = "connection-container",
                pickingMode = PickingMode.Ignore,
                style = { position = Position.Absolute, top = 0, left = 0, right = 0, bottom = 0 }
            };
            Add(connectionContainer);

            // 创建节点容器
            nodeContainer = new VisualElement
            {
                name = "node-container",
                style = { flexGrow = 1 }
            };
            Add(nodeContainer);
        }

        private void RegisterCallbacks()
        {
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            RegisterCallback<MouseUpEvent>(OnMouseUp);
            RegisterCallback<KeyDownEvent>(OnKeyDown);
            RegisterCallback<ContextualMenuPopulateEvent>(OnContextMenu);
        }

        public void SetBehaviorTree(BehaviorTree tree)
        {
            behaviorTree = tree;
            RefreshView();
        }

        public void RefreshView()
        {
            ClearView();

            if (behaviorTree == null) return;

            CreateNodesView();
            UpdateConnections();
        }

        private void ClearView()
        {
            nodeContainer.Clear();
            connectionContainer.Clear();
            nodeViews.Clear();
        }

        private void CreateNodesView()
        {
            if (behaviorTree.RootNode != null)
            {
                CreateNodeView(behaviorTree.RootNode);
            }

            // 创建其他节点视图（如果有的话）
            // 这里需要根据你的 BehaviorTree 结构进行调整
        }

        private BTNodeView CreateNodeView(BTNode node)
        {
            if (nodeViews.ContainsKey(node))
                return nodeViews[node];

            var nodeView = new BTNodeView(node, nodeLayout);
            nodeViews[node] = nodeView;
            nodeContainer.Add(nodeView);

            // 设置节点位置
            nodeView.style.position = Position.Absolute;
            nodeView.style.left = node.Position.x;
            nodeView.style.top = node.Position.y;

            // 注册节点事件
            nodeView.RegisterCallback<MouseDownEvent>(evt => OnNodeMouseDown(evt, nodeView));
            nodeView.RegisterCallback<MouseMoveEvent>(evt => OnNodeMouseMove(evt, nodeView));
            nodeView.RegisterCallback<MouseUpEvent>(evt => OnNodeMouseUp(evt, nodeView));

            return nodeView;
        }

        private void UpdateConnections()
        {
            connectionContainer.Clear();

            // 绘制节点之间的连接线
            foreach (var kvp in nodeViews)
            {
                var node = kvp.Key;
                var nodeView = kvp.Value;

                // 根据节点类型绘制连接线
                if (node is CompositeNode composite)
                {
                    DrawConnectionsForComposite(composite, nodeView);
                }
                else if (node is DecoratorNode decorator)
                {
                    DrawConnectionsForDecorator(decorator, nodeView);
                }
            }
        }

        private void DrawConnectionsForComposite(CompositeNode composite, BTNodeView parentView)
        {
            // 这里需要根据你的 CompositeNode 实现来绘制连接线
            // 假设 CompositeNode 有一个 Children 属性
        }

        private void DrawConnectionsForDecorator(DecoratorNode decorator, BTNodeView parentView)
        {
            // 这里需要根据你的 DecoratorNode 实现来绘制连接线
            // 假设 DecoratorNode 有一个 Child 属性
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            lastMousePosition = evt.localMousePosition;

            if (evt.button == 0) // 左键
            {
                // 处理选择和拖拽
                Focus();
            }
            else if (evt.button == 1) // 右键
            {
                ShowContextMenu(evt.localMousePosition);
            }
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (draggingNode != null)
            {
                var delta = evt.localMousePosition - lastMousePosition;
                var newPos = draggingNode.GetPosition() + delta;
                draggingNode.SetPosition(newPos);
                draggingNode.GetBTNode().Position = newPos;
            }

            lastMousePosition = evt.localMousePosition;
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            draggingNode = null;
            isCreatingConnection = false;
            connectionStart = null;
        }

        private void OnNodeMouseDown(MouseDownEvent evt, BTNodeView nodeView)
        {
            if (evt.button == 0)
            {
                draggingNode = nodeView;
                evt.StopPropagation();
            }
        }

        private void OnNodeMouseMove(MouseMoveEvent evt, BTNodeView nodeView)
        {
            evt.StopPropagation();
        }

        private void OnNodeMouseUp(MouseUpEvent evt, BTNodeView nodeView)
        {
            evt.StopPropagation();
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.Delete:
                    DeleteSelectedNodes();
                    break;
                case KeyCode.C when evt.ctrlKey:
                    CopySelectedNodes();
                    break;
                case KeyCode.V when evt.ctrlKey:
                    PasteNodes();
                    break;
            }
        }

        private void OnContextMenu(ContextualMenuPopulateEvent evt)
        {
            ShowContextMenu(evt.localMousePosition);
        }

        private void ShowContextMenu(Vector2 position)
        {
            var menu = new GenericMenu();

            // 添加创建节点菜单项
            menu.AddItem(new GUIContent("Create Node/Composite/Selector"), false,
                () => CreateNode<SelectorNode>(position));
            menu.AddItem(new GUIContent("Create Node/Composite/Sequence"), false,
                () => CreateNode<SequenceNode>(position));
            menu.AddItem(new GUIContent("Create Node/Decorator/Inverter"), false,
                () => CreateNode<Inverter>(position));
            menu.AddItem(new GUIContent("Create Node/Action/Debug Log"), false,
                () => CreateNode<DebugLogAction>(position));
            menu.AddItem(new GUIContent("Create Node/Action/Wait"), false,
                () => CreateNode<WaitAction>(position));

            menu.ShowAsContext();
        }

        private void CreateNode<T>(Vector2 position) where T : BTNode
        {
            var node = ScriptableObject.CreateInstance<T>();
            node.Position = position;
            node.name = typeof(T).Name;

            // 添加到行为树资源
            AssetDatabase.AddObjectToAsset(node, behaviorTree);
            AssetDatabase.SaveAssets();

            // 创建可视化
            CreateNodeView(node);

            EditorUtility.SetDirty(behaviorTree);
        }

        private void DeleteSelectedNodes()
        {
            // 实现删除选中节点的逻辑
        }

        private void CopySelectedNodes()
        {
            // 实现复制选中节点的逻辑
        }

        private void PasteNodes()
        {
            // 实现粘贴节点的逻辑
        }
    }

}