using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Base.Editor
{
    [CustomPropertyDrawer(typeof(Stat), true)]
    public class StatEditor : PropertyDrawer
    {
        const float TitleHeight = 19;
        const float PostTitleSpace = 0;
        const float RightSpace = 1;
        const float ArrowMargin = 5;
        const float IsEnableWidth = 20;
        
        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            int initialIndent = EditorGUI.indentLevel;
            float initialFieldWidth = EditorGUIUtility.fieldWidth;
            float initialLabelWidth = EditorGUIUtility.labelWidth;
            
            EditorGUI.indentLevel = 0;
            EditorGUIUtility.fieldWidth = 60f;

            Rect titleRect = position;
            titleRect.width -= IsEnableWidth;
            // titleRect.x += 7f;

            property.isExpanded = true;
            
            position = EditorGUI.PrefixLabel(titleRect, GUIUtility.GetControlID(FocusType.Passive), label);

            // Rect statNameRect = new Rect(position.x, position.y, position.width * .4f - 5, position.height);
            // Rect statValueRect = new Rect(position.x + position.width * .4f, position.y, position.width * .2f , position.height);
            // Rect isUpgradableRect = new Rect(statValueRect.x + position.width * .2f + 5, position.y, position.width * .15f, position.height);
            // Rect incrementRect = new Rect(isUpgradableRect.x + position.width * .15f + 5, position.y, position.width * .2f, position.height);
            
            Rect statValueRect = new Rect(position.x, position.y, position.width * .2f , position.height);
            Rect isUpgradableRect = new Rect(statValueRect.x + position.width * .2f + 5, position.y, position.width * .15f, position.height);
            Rect incrementRect = new Rect(isUpgradableRect.x + position.width * .1f, position.y, position.width * .2f, position.height);
            
            Rect isDecrementRect = new Rect(isUpgradableRect.x + position.width * .4f + 10, position.y, position.width * .15f, position.height);
            
            Rect decrementRect = new Rect(isDecrementRect.x + position.width * .1f, position.y, position.width * .2f, position.height);

            if (property.isExpanded)
            {
                SerializedProperty baseValue = property.FindPropertyRelative("baseValue");
                SerializedProperty isUpgradable = property.FindPropertyRelative("isUpgradable");
                SerializedProperty increment = property.FindPropertyRelative("increment");
                SerializedProperty isDecrement = property.FindPropertyRelative("isDecrement");
                SerializedProperty decrement = property.FindPropertyRelative("decrement");
                
                baseValue.floatValue = EditorGUI.FloatField(statValueRect, new GUIContent {tooltip = "The value of stat"}, baseValue.floatValue);
                isUpgradable.boolValue = EditorGUI.Toggle(isUpgradableRect, new GUIContent {tooltip = "Is this stat upgradable?"}, isUpgradable.boolValue);
                isDecrement.boolValue = EditorGUI.Toggle(isDecrementRect, isDecrement.boolValue);

                if (isUpgradable.boolValue)
                {
                    increment.floatValue = EditorGUI.FloatField(incrementRect, new GUIContent {tooltip = "Amount increase when upgrade"}, increment.floatValue);
                }
                else
                {
                    Rect labelRect = new Rect(isUpgradableRect.x + position.width * .065f, isUpgradableRect.y, position.width * .4f, position.height);
                    GUI.Label(labelRect, "Is Upgradable?");
                }

                if (isDecrement.boolValue)
                {
                    decrement.floatValue = EditorGUI.FloatField(decrementRect, decrement.floatValue);
                }
                else
                {
                    Rect labelRect = new Rect(isDecrementRect.x + position.width * .075f, isUpgradableRect.y, position.width * .4f, position.height);
                    GUI.Label(labelRect, "Is Decrement?");
                }
            }

            EditorGUI.EndProperty();
        }
    }
}

