using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using MonsterBT.Runtime;

namespace MonsterBT.Editor
{
    public class BTVisualEditor : EditorWindow
    {
        private BehaviorTreeView behaviorTreeView;
        private ObjectField behaviorTreeField;
        private BehaviorTree currentBehaviorTree;

        [MenuItem("Window/MonsterBT/Behavior Tree Editor")]
        public static void ShowWindow()
        {
            BTVisualEditor wnd = GetWindow<BTVisualEditor>();
            wnd.titleContent = new GUIContent("Behavior Tree Editor");
            wnd.minSize = new Vector2(800, 600);
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            // 创建工具栏
            CreateToolbar(root);

            // 创建主视图
            behaviorTreeView = new BehaviorTreeView();
            root.Add(behaviorTreeView);
        }

        private void CreateToolbar(VisualElement root)
        {
            var toolbar = new Toolbar();
            toolbar.style.height = 30;

            // 行为树选择字段
            behaviorTreeField = new ObjectField("Behavior Tree")
            {
                objectType = typeof(BehaviorTree),
                style = { width = 300 }
            };
            behaviorTreeField.RegisterValueChangedCallback(OnBehaviorTreeChanged);
            toolbar.Add(behaviorTreeField);

            // 添加间隔
            toolbar.Add(new ToolbarSpacer());

            // 创建新行为树按钮
            var createButton = new ToolbarButton(() => CreateNewBehaviorTree())
            {
                text = "Create New"
            };
            toolbar.Add(createButton);

            // 保存按钮
            var saveButton = new ToolbarButton(() => SaveBehaviorTree())
            {
                text = "Save"
            };
            toolbar.Add(saveButton);

            root.Add(toolbar);
        }

        private void OnBehaviorTreeChanged(ChangeEvent<Object> evt)
        {
            currentBehaviorTree = evt.newValue as BehaviorTree;
            behaviorTreeView.SetBehaviorTree(currentBehaviorTree);
        }

        private void CreateNewBehaviorTree()
        {
            // 创建新的行为树资产
            var tree = ScriptableObject.CreateInstance<BehaviorTree>();
            var rootNode = ScriptableObject.CreateInstance<RootNode>();
            rootNode.name = "Root";
            rootNode.Position = new Vector2(400, 100);

            tree.RootNode = rootNode;
            tree.name = "NewBehaviorTree";

            // 保存到Assets文件夹
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Behavior Tree",
                "NewBehaviorTree",
                "asset",
                "Please enter a file name to save the behavior tree to");

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(tree, path);
                AssetDatabase.AddObjectToAsset(rootNode, tree);
                AssetDatabase.SaveAssets();

                behaviorTreeField.value = tree;
                currentBehaviorTree = tree;
                behaviorTreeView.SetBehaviorTree(tree);
            }
        }

        private void SaveBehaviorTree()
        {
            if (currentBehaviorTree != null)
            {
                EditorUtility.SetDirty(currentBehaviorTree);
                AssetDatabase.SaveAssets();
                Debug.Log("Behavior tree saved!");
            }
        }

        private void OnEnable()
        {
            // 如果有已选择的行为树，恢复显示
            if (currentBehaviorTree != null && behaviorTreeView != null)
            {
                behaviorTreeView.SetBehaviorTree(currentBehaviorTree);
            }
        }
    }
}