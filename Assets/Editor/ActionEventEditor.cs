
using UnityEditor;

[CustomEditor(typeof(ActionEvent))]
[CanEditMultipleObjects]
public class ActionEventEditor : Editor
{
    SerializedProperty effect;
    SerializedProperty hasValue;
    
    SerializedProperty valueEffectCalled;
    SerializedProperty voidEffectCalled;
    
    void OnEnable()
    {
        effect = serializedObject.FindProperty("Effect");
        hasValue = serializedObject.FindProperty("HasValue");
        valueEffectCalled = serializedObject.FindProperty("ValueEffectCalled");
        voidEffectCalled = serializedObject.FindProperty("VoidEffectCalled");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(effect);
        EditorGUILayout.PropertyField(hasValue);
        
        // Display relevant event
        EditorGUILayout.PropertyField(hasValue.boolValue ? valueEffectCalled : voidEffectCalled);

        serializedObject.ApplyModifiedProperties();
    }
}