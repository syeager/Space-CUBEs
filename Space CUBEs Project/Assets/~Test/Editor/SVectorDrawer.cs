// Steve Yeager
// 

using UnityClasses;
using UnityEditor;
using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
//[CustomPropertyDrawer(typeof(sVector3))]
public class SVectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PrefixLabel(position, new GUIContent("hello"));
    }
}