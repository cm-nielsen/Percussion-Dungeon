using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(StringPair))]
public class StringPairDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position,
            GUIUtility.GetControlID(FocusType.Passive), label);

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        Rect nameRect = new Rect(position.x,
            position.y,
            position.width * 0.5f - 2,
            position.height);
        Rect valRect = new Rect(position.x + position.width * 0.5f + 2,
            position.y,
            position.width * 0.5f - 2,
            position.height);

        SerializedProperty name = property.FindPropertyRelative("name");
        SerializedProperty val = property.FindPropertyRelative("val");

        EditorGUI.PropertyField(nameRect, name, GUIContent.none);
        EditorGUI.PropertyField(valRect, val, GUIContent.none);

        EditorGUI.indentLevel = indent;
        
        EditorGUI.EndProperty();

        //base.OnGUI(position, property, label);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }
}
