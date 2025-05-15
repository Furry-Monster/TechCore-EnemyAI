using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviorTreeEditor : EditorWindow
{
    [MenuItem("Tools/MyBehaviorTree/Editor")]
    public static void ShowWindow()
    {
        GetWindow<BehaviorTreeEditor>("Behavior Tree Editor");
    }

    void CreateGUI()
    {
        var graphView = new BehaviorTreeGraphView(); // Replace GraphView with a concrete implementation
        rootVisualElement.Add(graphView);

        var toolbar = new Toolbar();
        var saveButton = new ToolbarButton(() => SaveTree()) { text = "Save" };
        toolbar.Add(saveButton);
        rootVisualElement.Add(toolbar);
    }

    void SaveTree()
    {
        // TODO: Serialize to ScriptableObject
        Debug.Log("Tree Saved");
    }
}

// Concrete implementation of GraphView
public class BehaviorTreeGraphView : GraphView
{
    public BehaviorTreeGraphView()
    {
        // Setup zoom and other configurations
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        // Add grid background
        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
    }
}
