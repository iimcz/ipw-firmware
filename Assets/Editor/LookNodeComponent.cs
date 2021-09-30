using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(LookNodeComponent))]
[CanEditMultipleObjects]
public class LookNodeEditor : Editor
{
    SerializedProperty parentNode;

    SerializedProperty onEnter;
    SerializedProperty onExit;
    SerializedProperty onActivate;

    void OnEnable()
    {
        parentNode = serializedObject.FindProperty("ParentNode");

        onEnter = serializedObject.FindProperty("OnEnter");
        onExit = serializedObject.FindProperty("OnExit");
        onActivate = serializedObject.FindProperty("OnActivate");
    }
    
    public override void OnInspectorGUI()
    {
        var node = target as NavigationNodeComponent;

        serializedObject.Update();
        EditorGUILayout.PropertyField(parentNode);

        EditorGUILayout.PropertyField(onEnter);
        EditorGUILayout.PropertyField(onExit);
        EditorGUILayout.PropertyField(onActivate);
        serializedObject.ApplyModifiedProperties();
    }

    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    public static void DrawHandles(LookNodeComponent t, GizmoType gizmoType)
    {
        Handles.color = Color.green;
        Handles.SphereHandleCap(0, t.transform.position, Quaternion.identity, 0.5f, EventType.Repaint);

        if (t.ParentNode != null) Handles.DrawLine(t.transform.position, t.ParentNode.transform.position);
    }
}
