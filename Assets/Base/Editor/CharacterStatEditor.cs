#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Base.CharacterStats;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CharacterStat), true)]
public class CharacterStatEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.indentLevel = 0;
        EditorGUIUtility.fieldWidth = 60f;

        Rect titleRect = position;
        titleRect.width -= 20f;

        position = EditorGUI.PrefixLabel(titleRect, GUIUtility.GetControlID(FocusType.Passive), label);

        property.isExpanded = true;

        if (property.isExpanded)
        {
            SerializedProperty baseValue = property.FindPropertyRelative("BaseValue");
            Rect valueRect = new Rect(position.x, position.y, position.width, position.height);
            EditorGUI.FloatField(valueRect, baseValue.floatValue);
        }
        
        EditorGUI.EndProperty();
    }
}

#endif
