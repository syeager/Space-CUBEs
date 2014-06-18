// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.15
// Edited: 2014.06.15


using UnityEditor;
using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
//[CustomPropertyDrawer(typeof(Pool))]
public class PoolDrawer : PropertyDrawer
{
    //private Color cachedColor;

    #region PropertyDrawer Overrides
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // label
        //SerializedProperty prefab = property.FindPropertyRelative("prefab");
        //Object prefabRef = prefab.objectReferenceValue;
        //string prefabName;
        //if (prefabRef == null)
        //{
        //    GUIColor(Color.red);
        //    prefabName = "EMPTY";
        //}
        //else
        //{
        //    prefabName = prefabRef.name;
        //}
        //EditorGUI.PrefixLabel(position, new GUIContent(prefabName));
        //GUI.color = Color.blue;

        EditorGUI.PropertyField(position, property.FindPropertyRelative("prefab"));
        EditorGUI.PropertyField(position, property.FindPropertyRelative("preAllocate"));
        EditorGUI.PropertyField(position, property.FindPropertyRelative("allocateBlock"));
    }

    #endregion

    #region Private Methods

    private void GUIColor(Color color)
    {
        //cachedColor = GUI.color;
        GUI.color = color;
    }

    #endregion
}