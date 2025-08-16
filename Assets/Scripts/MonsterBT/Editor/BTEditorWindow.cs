using System;
using MonsterBT.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace MonsterBT.Editor
{
    public class BTEditorWindow : EditorWindow
    {
        private BTNodeGraphView graphView;
        private BTPropInspector inspector;
        private ObjectField behaviorTreeField;

        private BehaviorTree currentBehaviorTree;

        #region UI Methods

        [MenuItem("Window/MonsterBT/BehaviorTree")]
        public static void ShowWindow()
        {
            var window = GetWindow<BTEditorWindow>();
            window.titleContent = new GUIContent("Monster BehaviorTree");
            window.minSize = new Vector2(1920, 1080);
        }

        public void OnEnable()
        {
            if (currentBehaviorTree != null && graphView != null)
                graphView.SetBehaviorTree(currentBehaviorTree);
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;

            var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/Scripts/MonsterBT/Editor/BTEditorLayout.uxml");

            if (template != null)
            {
                template.CloneTree(root);
                SetupUIElements();
            }
        }

        private void SetupUIElements()
        {
            // 获取UI元素引用
            behaviorTreeField = rootVisualElement.Q<ObjectField>("behavior-tree-field");
            var createButton = rootVisualElement.Q<Button>("create-button");
            var saveButton = rootVisualElement.Q<Button>("save-button");
            var autoLayoutButton = rootVisualElement.Q<Button>("auto-layout-button");
            var playButton = rootVisualElement.Q<Button>("play-button");
            var debugButton = rootVisualElement.Q<Button>("debug-button");

            // 注册事件
            if (behaviorTreeField != null)
            {
                behaviorTreeField.objectType = typeof(BehaviorTree);
                behaviorTreeField?.RegisterValueChangedCallback(OnBehaviorTreeChanged);
            }
            createButton?.RegisterCallback<ClickEvent>(OnCreateNewBehaviorTree);
            saveButton?.RegisterCallback<ClickEvent>(OnSaveBehaviorTree);
            autoLayoutButton?.RegisterCallback<ClickEvent>(OnAutoLayoutNodes);
            playButton?.RegisterCallback<ClickEvent>(OnTogglePlayMode);
            debugButton?.RegisterCallback<ClickEvent>(OnToggleDebugMode);

            // 创建Graph视图
            var graphContainer = rootVisualElement.Q<VisualElement>("graph-container");
            if (graphContainer != null)
            {
                graphView = new BTNodeGraphView();
                graphContainer.Add(graphView);
            }

            // 创建Inspector视图
            var propContainer = rootVisualElement.Q<VisualElement>("property-container");
            if (propContainer != null)
            {
                inspector = new BTPropInspector();
                propContainer.Add(inspector);
            }

            // 连接GraphView和Inspector
            if (graphView != null && inspector != null)
            {
                graphView.OnNodeSelected += inspector.SetSelectedNode;
                graphView.OnNodeDeselected += inspector.ClearSelection;
            }

            // 设置节点库添加事件
            var nodeItems = rootVisualElement.Query<VisualElement>(className: "node-list-item");
            nodeItems.ForEach(item =>
            {
                item.RegisterCallback<ClickEvent>(OnClickSpawnNode);
                item.RegisterCallback<MouseDownEvent>(OnDragSpawnNode);
            });
        }

        #endregion

        #region Toolbar Callbacks

        private void OnBehaviorTreeChanged(ChangeEvent<Object> changeEvent)
        {
            currentBehaviorTree = changeEvent.newValue as BehaviorTree;
            graphView.SetBehaviorTree(currentBehaviorTree);
        }

        private void OnAutoLayoutNodes(ClickEvent evt)
        {
            if (graphView != null && currentBehaviorTree != null)
            {
                // TODO:实现自动布局逻辑
                Debug.Log("执行自动布局");
            }
        }

        private void OnTogglePlayMode(ClickEvent evt)
        {
            // TODO:实现播放/停止行为树的功能
            Debug.Log("切换播放模式");
        }

        private void OnToggleDebugMode(ClickEvent evt)
        {
            // TODO:实现调试模式切换
            Debug.Log("切换调试模式");
        }

        private void OnCreateNewBehaviorTree(ClickEvent evt)
        {
            // 默认设置
            var tree = CreateInstance<BehaviorTree>();
            var rootNode = CreateInstance<RootNode>();
            rootNode.name = "Root";
            rootNode.Position = new Vector2(400, 100);
            tree.RootNode = rootNode;
            tree.name = "New BehaviorTree";

            // 保存路径选择
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Behavior Tree",
                "NewBehaviorTree",
                "asset",
                "Please enter a file name to save the behavior tree to"
            );

            if (string.IsNullOrEmpty(path))
                return;

            // 保存到Asset文件中
            AssetDatabase.CreateAsset(tree, path);
            AssetDatabase.AddObjectToAsset(rootNode, tree);
            AssetDatabase.SaveAssets();

            behaviorTreeField.value = tree;
            currentBehaviorTree = tree;
            graphView.SetBehaviorTree(currentBehaviorTree);
        }

        private void OnSaveBehaviorTree(ClickEvent evt)
        {
            if (currentBehaviorTree == null)
                return;

            EditorUtility.SetDirty(currentBehaviorTree);
            AssetDatabase.SaveAssets();
            Debug.Log("Behavior tree saved.");
        }

        #endregion

        #region Library Callbacks

        private void OnClickSpawnNode(ClickEvent evt)
        {
        }

        private void OnDragSpawnNode(MouseDownEvent evt)
        {
        }

        #endregion

        #region Inspector Callbacks

        #endregion

    }
}