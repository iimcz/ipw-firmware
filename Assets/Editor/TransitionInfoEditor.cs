using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TransitionInfo))]
public class TransitionInfoEditor : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 4f + 6;
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(rect, label, property);
        
        var labelRect = new Rect(rect.x, rect.y, rect.width, 16);
        var nextNodeRect = new Rect(rect.x, rect.y + 18, rect.width, 16);
        var typeRect = new Rect(rect.x, rect.y + 36, rect.width, 16);
        var infoRect = new Rect(rect.x, rect.y + 54, rect.width, 16);

        var nextNodeProperty = property.FindPropertyRelative("NextNode");
        var nextNodeObject = nextNodeProperty.objectReferenceValue as NavigationNodeComponent;
        if (nextNodeObject != null)
        {
            EditorGUI.LabelField(labelRect, label.text + $" ({nextNodeObject.gameObject.name})");
        }
        else
        {
            EditorGUI.LabelField(labelRect, label);
        }

        var movementTypeProperty = property.FindPropertyRelative("MovementType");
        var enumValue = (TransitionInfo.MovementTypeEnum) movementTypeProperty.intValue;

        EditorGUI.PropertyField(nextNodeRect, nextNodeProperty);
        EditorGUI.PropertyField(typeRect, movementTypeProperty);

        switch (enumValue)
        {
            case TransitionInfo.MovementTypeEnum.AnimationCurve:
                EditorGUI.PropertyField(infoRect, property.FindPropertyRelative("Curve"));
                break;
            case TransitionInfo.MovementTypeEnum.ConstantSpeed:
                EditorGUI.PropertyField(infoRect, property.FindPropertyRelative("Speed"));
                break;
            case TransitionInfo.MovementTypeEnum.ConstantTime:
                EditorGUI.PropertyField(infoRect, property.FindPropertyRelative("TravelTime"));
                break;
            case TransitionInfo.MovementTypeEnum.Teleport:
            case TransitionInfo.MovementTypeEnum.Vignette:
                break;
            default:
                throw new NotImplementedException();
        }

        EditorGUI.EndProperty();
    }
}
