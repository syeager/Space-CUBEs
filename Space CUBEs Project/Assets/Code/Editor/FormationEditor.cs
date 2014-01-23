// Steve Yeager
// 1.12.2014

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

//[CustomEditor(typeof(Pattern))]
public class FormationEditor : Editor
{
    #region Private Fields

    private SerializedProperty positions;
    private int openPosition = -1;

    #endregion


    #region Editor Overrides

    private void OnEnable()
    {
        positions = serializedObject.FindProperty("positions");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // positions
        for (int i = 0; i < positions.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            {
                // toggle
                if (GUILayout.Button(openPosition == i ? "|" : "O", EditorStyles.miniButton, GUILayout.Width(25f)))
                {
                    openPosition = openPosition == i ? -1 : i;
                }

                // number
                EditorGUILayout.LabelField("ID: " + i, GUILayout.Width(50f));

                // position
                Vector2 newPosition = positions.GetArrayElementAtIndex(i).vector3Value;
                newPosition = EditorGUILayout.Vector2Field("", newPosition);
                positions.GetArrayElementAtIndex(i).vector3Value = newPosition;

                // delete
                if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(25f)))
                {
                    DeletePosition(i);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        // add position
        if (GUILayout.Button("Add Position"))
        {
            AddPosition();
        }

        serializedObject.ApplyModifiedProperties();
    }

    #endregion

    #region Private Methods

    private void AddPosition(int index = -1)
    {
        if (index == -1)
        {
            index = positions.arraySize;
        }

        positions.InsertArrayElementAtIndex(index);
    }


    private void DeletePosition(int index)
    {
        positions.DeleteArrayElementAtIndex(index);
    }


    private void DrawPlaceholders()
    {

    }

    #endregion
}