using Naki3D.Common.Protocol;
using UnityEditor;

[CustomEditor(typeof(ActionEvent))]
[CanEditMultipleObjects]
public class ActionEventEditor : Editor
{
    SerializedProperty effect;
    SerializedProperty effectType;
    
    SerializedProperty voidEffectCalled;
    SerializedProperty intEffectCalled;
    SerializedProperty floatEffectCalled;
    SerializedProperty boolEffectCalled;
    SerializedProperty stringEffectCalled;
    SerializedProperty vector2EffectCalled;
    SerializedProperty vector3EffectCalled;
    
    void OnEnable()
    {
        effect = serializedObject.FindProperty("Effect");
        effectType = serializedObject.FindProperty("EffectType");

        voidEffectCalled = serializedObject.FindProperty("VoidEffectCalled");
        intEffectCalled = serializedObject.FindProperty("IntEffectCalled");
        floatEffectCalled = serializedObject.FindProperty("FloatEffectCalled");
        boolEffectCalled = serializedObject.FindProperty("BoolEffectCalled");
        stringEffectCalled = serializedObject.FindProperty("StringEffectCalled");

        vector2EffectCalled = serializedObject.FindProperty("Vector2EffectCalled");
        vector3EffectCalled = serializedObject.FindProperty("Vector3EffectCalled");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(effect);
        EditorGUILayout.PropertyField(effectType);

        // Display relevant event
        SerializedProperty field = (DataType)effectType.enumValueIndex switch
        {
            DataType.Void => voidEffectCalled,
            DataType.Integer => intEffectCalled,
            DataType.Float => floatEffectCalled,
            DataType.Bool => boolEffectCalled,
            DataType.String => stringEffectCalled,
            DataType.Vector2 => vector2EffectCalled,
            DataType.Vector3 => vector3EffectCalled,
            _ => null   // This should never happen
        };
        EditorGUILayout.PropertyField(field);

        serializedObject.ApplyModifiedProperties();
    }
}