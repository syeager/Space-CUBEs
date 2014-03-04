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
    private Color[] allColors;
    private SerializedProperty colors;

    #endregion


    #region Editor Overrides

    private void OnEnable()
    {
        cv = target as ColorVertices;
        allColors = CUBE.LoadColors();
        colors = serializedObject.FindProperty("colors");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PrefixLabel("Colors");
        EditorGUI.indentLevel++;
        for (int i = 0; i < colors.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUI.backgroundColor = allColors[cv.colors[i]];
                if (GUILayout.Button("Piece " + (i + 1)))
                {
                    ColorSelector.OpenSelector(allColors, cv, i);
                }

            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel--;
    }
    
    #endregion
}