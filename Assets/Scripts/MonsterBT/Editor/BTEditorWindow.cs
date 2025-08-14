using MonsterBT.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MonsterBT.Editor
{
    public class BTEditorWindow : EditorWindow
    {
        private BTGraphView graphView;
        private ObjectField behaviorTreeField;

        private BehaviorTree currentBehaviorTree;

        [MenuItem("Window/MonsterBT/BehaviorTree")]
        public static void ShowWindow()
        {
            var window = GetWindow<BTEditorWindow>();
            window.titleContent = new GUIContent("Monster BehaviorTree");
            window.minSize = new Vector2(1360, 1020);
        }

        public void OnEnable()
        {
            if (currentBehaviorTree != null && graphView != null)
                graphView.SetBehaviorTree(currentBehaviorTree);
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;

            // 加载主编辑器布局模板
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

            // 设置工具栏按钮事件
            var createButton = rootVisualElement.Q<Button>("create-button");
            var saveButton = rootVisualElement.Q<Button>("save-button");
            var autoLayoutButton = rootVisualElement.Q<Button>("auto-layout-button");
            var playButton = rootVisualElement.Q<Button>("play-button");
            var debugButton = rootVisualElement.Q<Button>("debug-button");

            // 注册事件
            if (behaviorTreeField != null)
            {
                behaviorTreeField.objectType = typeof(BehaviorTree);
                behaviorTreeField.RegisterValueChangedCallback(OnBehaviorTreeChanged);
            }

            createButton?.RegisterCallback<ClickEvent>(_ => CreateNewBehaviorTree());
            saveButton?.RegisterCallback<ClickEvent>(_ => SaveBehaviorTree());
            autoLayoutButton?.RegisterCallback<ClickEvent>(_ => AutoLayoutNodes());
            playButton?.RegisterCallback<ClickEvent>(_ => TogglePlayMode());
            debugButton?.RegisterCallback<ClickEvent>(_ => ToggleDebugMode());

            // 创建图形视图并添加到容器
            var graphContainer = rootVisualElement.Q<VisualElement>("graph-container");
            if (graphContainer != null)
            {
                graphView = new BTGraphView();
                graphContainer.Add(graphView);
            }

            // 设置节点库拖拽事件
            SetupNodeLibrary();
        }

        private void SetupNodeLibrary()
        {
            // 设置节点库中各节点项的拖拽功能
            var nodeItems = rootVisualElement.Query<VisualElement>(className: "node-list-item");
            nodeItems.ForEach(item =>
            {
                item.RegisterCallback<MouseDownEvent>(evt =>
                {
                    if (evt.button == 0) // 左键开始拖拽
                    {
                        StartNodeDrag(item, evt);
                    }
                });
            });
        }

        private void StartNodeDrag(VisualElement nodeItem, MouseDownEvent evt)
        {
            // 实现从节点库拖拽创建节点的功能
            var nodeType = nodeItem.name.Replace("-item", "");
            Debug.Log($"开始拖拽节点类型: {nodeType}");
        }

        private void AutoLayoutNodes()
        {
            if (graphView != null && currentBehaviorTree != null)
            {
                // 实现自动布局逻辑
                Debug.Log("执行自动布局");
            }
        }

        private void TogglePlayMode()
        {
            // 实现播放/停止行为树的功能
            Debug.Log("切换播放模式");
        }

        private void ToggleDebugMode()
        {
            // 实现调试模式切换
            Debug.Log("切换调试模式");
        }

        private void OnBehaviorTreeChanged(ChangeEvent<Object> evt)
        {
            currentBehaviorTree = evt.newValue as BehaviorTree;
            graphView.SetBehaviorTree(currentBehaviorTree);
        }

        private void CreateNewBehaviorTree()
        {
            // 默认设置
            var tree = CreateInstance<BehaviorTree>();
            var rootNode = CreateInstance<RootNode>();
            rootNode.name = "Root";
            rootNode.Position = new Vector2(400, 100);
            tree.RootNode = rootNode;
            tree.name = "New BehaviorTree";

            // 请求保存
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Behavior Tree",
                "NewBehaviorTree",
                "asset",
                "Please enter a file name to save the behavior tree to"
            );

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(tree, path);
                AssetDatabase.AddObjectToAsset(rootNode, tree);
                AssetDatabase.SaveAssets();

                behaviorTreeField.value = tree;
                currentBehaviorTree = tree;
                graphView.SetBehaviorTree(currentBehaviorTree);
            }
        }

        private void SaveBehaviorTree()
        {
            if (currentBehaviorTree == null)
                return;

            EditorUtility.SetDirty(currentBehaviorTree);
            AssetDatabase.SaveAssets();
            Debug.Log("Behavior Tree Saved!");
        }
    }
}