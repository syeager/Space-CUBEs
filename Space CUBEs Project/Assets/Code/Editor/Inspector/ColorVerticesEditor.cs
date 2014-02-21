// Steve Yeager
// 2.19.2014

using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// 
/// </summary>
[CustomEditor(typeof(ColorVertices))]
public class ColorVerticesEditor : Editor
{
    #region Private Fields

    private ColorVertices cv;

    #endregion


    #region Editor Overrides

    private void OnEnable()
    {
        cv = target as ColorVertices;
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("colors"), true);
        cv.Bake();

        serializedObject.ApplyModifiedProperties();
    }
    
    #endregion
}