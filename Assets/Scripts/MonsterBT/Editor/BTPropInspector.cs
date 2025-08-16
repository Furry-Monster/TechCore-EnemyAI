using System.Linq;
using System.Reflection;
using MonsterBT.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MonsterBT.Editor
{
    public class BTPropInspector : VisualElement
    {
        private BTNode currentNode;
        private ScrollView contentScrollView;
        private Label emptyStateLabel;

        public BTPropInspector()
        {
            LoadLayoutAndStyles();
            InitializeElements();
            ShowEmptyState();
        }

        #region Public Methods

        public void SetSelectedNode(BTNode node)
        {
            if (currentNode == node) return;

            currentNode = node;
            RefreshInspector();
        }

        public void ClearSelection()
        {
            currentNode = null;
            RefreshInspector();
        }

        #endregion

        #region Layout Creation

        private void LoadLayoutAndStyles()
        {
            // 加载UXML布局
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/Scripts/MonsterBT/Editor/BTPropInspectorLayout.uxml");
            if (uxml != null)
            {
                uxml.CloneTree(this);
            }

            // 加载USS样式
            var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/Scripts/MonsterBT/Editor/BTPropInspectorStyle.uss");
            if (uss != null)
            {
                styleSheets.Add(uss);
            }
        }

        private void InitializeElements()
        {
            contentScrollView = this.Q<ScrollView>("content-scroll");
            emptyStateLabel = this.Q<Label>("empty-state");
        }

        #endregion

        #region Content Management

        private void RefreshInspector()
        {
            contentScrollView.Clear();

            if (currentNode == null)
            {
                ShowEmptyState();
                return;
            }

            HideEmptyState();
            BuildInspectorForNode(currentNode);
        }

        private void ShowEmptyState()
        {
            emptyStateLabel.style.display = DisplayStyle.Flex;
        }

        private void HideEmptyState()
        {
            emptyStateLabel.style.display = DisplayStyle.None;
        }

        private void BuildInspectorForNode(BTNode node)
        {
            // 节点名称
            var nameField = new TextField("Name")
            {
                value = node.name
            };
            nameField.AddToClassList("inspector-field");
            nameField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(node, "Change Node Name");
                node.name = evt.newValue;
                EditorUtility.SetDirty(node);
            });
            contentScrollView.Add(nameField);

            // 节点描述
            var descriptionField = new TextField("Description")
            {
                value = node.Description ?? "",
                multiline = true
            };
            descriptionField.AddToClassList("description-field");
            descriptionField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(node, "Change Node Description");
                node.Description = evt.newValue;
                EditorUtility.SetDirty(node);
            });
            contentScrollView.Add(descriptionField);

            // 自定义属性
            BuildCustomProperties(node);
        }

        private void BuildCustomProperties(BTNode node)
        {
            var fields = GetEditableFields(node);
            if (fields.Length == 0) return;

            // 添加分隔符
            var separator = new VisualElement();
            separator.AddToClassList("field-separator");
            contentScrollView.Add(separator);

            foreach (var field in fields)
            {
                var fieldEditor = CreateFieldEditor(field, node);
                if (fieldEditor != null)
                {
                    fieldEditor.AddToClassList("inspector-field");
                    contentScrollView.Add(fieldEditor);
                }
            }
        }

        #endregion

        #region Field Editors

        private FieldInfo[] GetEditableFields(BTNode node)
        {
            return node.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.IsPublic || f.GetCustomAttribute<SerializeField>() != null)
                .Where(f => !IsIgnoredField(f))
                .ToArray();
        }

        private bool IsIgnoredField(FieldInfo field)
        {
            var ignoredFields = new[] { "name", "hideFlags", "description", "position" };
            return ignoredFields.Contains(field.Name.ToLower()) ||
                   field.Name.StartsWith("m_");
        }

        private VisualElement CreateFieldEditor(FieldInfo field, BTNode node)
        {
            var fieldType = field.FieldType;
            var fieldName = ObjectNames.NicifyVariableName(field.Name);

            return fieldType switch
            {
                var t when t == typeof(string) => CreateStringField(field, node, fieldName),
                var t when t == typeof(float) => CreateFloatField(field, node, fieldName),
                var t when t == typeof(int) => CreateIntField(field, node, fieldName),
                var t when t == typeof(bool) => CreateBoolField(field, node, fieldName),
                var t when t == typeof(Vector3) => CreateVector3Field(field, node, fieldName),
                var t when t.IsEnum => CreateEnumField(field, node, fieldName),
                _ => null
            };
        }

        private TextField CreateStringField(FieldInfo field, BTNode node, string displayName)
        {
            var textField = new TextField(displayName)
            {
                value = (string)field.GetValue(node) ?? ""
            };

            textField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(node, $"Change {displayName}");
                field.SetValue(node, evt.newValue);
                EditorUtility.SetDirty(node);
            });

            return textField;
        }

        private FloatField CreateFloatField(FieldInfo field, BTNode node, string displayName)
        {
            var floatField = new FloatField(displayName)
            {
                value = (float)field.GetValue(node)
            };

            floatField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(node, $"Change {displayName}");
                field.SetValue(node, evt.newValue);
                EditorUtility.SetDirty(node);
            });

            return floatField;
        }

        private IntegerField CreateIntField(FieldInfo field, BTNode node, string displayName)
        {
            var intField = new IntegerField(displayName)
            {
                value = (int)field.GetValue(node)
            };

            intField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(node, $"Change {displayName}");
                field.SetValue(node, evt.newValue);
                EditorUtility.SetDirty(node);
            });

            return intField;
        }

        private Toggle CreateBoolField(FieldInfo field, BTNode node, string displayName)
        {
            var toggle = new Toggle(displayName)
            {
                value = (bool)field.GetValue(node)
            };

            toggle.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(node, $"Change {displayName}");
                field.SetValue(node, evt.newValue);
                EditorUtility.SetDirty(node);
            });

            return toggle;
        }

        private Vector3Field CreateVector3Field(FieldInfo field, BTNode node, string displayName)
        {
            var vector3Field = new Vector3Field(displayName)
            {
                value = (Vector3)field.GetValue(node)
            };

            vector3Field.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(node, $"Change {displayName}");
                field.SetValue(node, evt.newValue);
                EditorUtility.SetDirty(node);
            });

            return vector3Field;
        }

        private EnumField CreateEnumField(FieldInfo field, BTNode node, string displayName)
        {
            var enumValue = (System.Enum)field.GetValue(node);
            var enumField = new EnumField(displayName, enumValue);

            enumField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(node, $"Change {displayName}");
                field.SetValue(node, evt.newValue);
                EditorUtility.SetDirty(node);
            });

            return enumField;
        }

        #endregion

    }
}