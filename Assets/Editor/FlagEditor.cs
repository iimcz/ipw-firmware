using System;
using Codice.Client.GameUI.Explorer;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FlagComponent))]
public class FlagEditor : Editor
{
    private SerializedProperty flagTarget;
    private SerializedProperty text;
    private SerializedProperty textMesh;
    
    void OnEnable()
    {
        flagTarget = serializedObject.FindProperty("Target");
        text = serializedObject.FindProperty("Text");
        textMesh = serializedObject.FindProperty("TextMesh");
    }

    public override void OnInspectorGUI()
    {
        var flag = target as FlagComponent;
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(flagTarget);
        EditorGUILayout.PropertyField(text, true);
        EditorGUILayout.PropertyField(textMesh);
        
        if (GUILayout.Button("Align flag above target") && flag.Target != null)
        { 
            flag.transform.position =
                new Vector3(flag.Target.position.x, flag.transform.position.y, flag.Target.position.z);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    public static void DrawHandles(FlagComponent t, GizmoType gizmoType)
    {
        if (t == null || t.Target == null) return;
        Handles.DrawLine(t.transform.position, t.Target.transform.position);
    }
}
