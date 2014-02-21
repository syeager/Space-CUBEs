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
    #region 
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Color"))
        {
            ColorVertices cv = target as ColorVertices;
            cv.Bake();
        }
    }
    
    #endregion
}