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
        serializedObject.Update();
        EditorGUILayout.PropertyField(parentNode);

        EditorGUILayout.PropertyField(onEnter);
        EditorGUILayout.PropertyField(onExit);
        EditorGUILayout.PropertyField(onActivate);
        serializedObject.ApplyModifiedProperties();
    }

    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy | GizmoType.Pickable)]
    public static void DrawGizmos(LookNodeComponent t, GizmoType gizmoType)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(t.transform.position, 0.5f);

        if (t.ParentNode != null) Gizmos.DrawLine(t.transform.position, t.ParentNode.transform.position);
    }
}
