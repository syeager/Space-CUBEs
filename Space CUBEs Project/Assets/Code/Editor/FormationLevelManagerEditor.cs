// Steve Yeager
// 1.12.214

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

//[CustomEditor(typeof(PatternLevelManager))]
public class FormationLevelManagerEditor : Editor
{
    #region Private Fields

    private SerializedProperty patterns;
    private int openPattern = -1;
    private bool patternsToggle;
    private Dictionary<Enemy.Classes, GameObject> EnemyPrefabs;
    private Formation[] patternRefs;

    #endregion

    #region Const Fields

    private const string PATTERNSPATH = "Assets/Patterns/";

    #endregion

    #region Editor Overrides

    private void OnEnable()
    {
        patterns = serializedObject.FindProperty("patterns");
        EnemyPrefabs = UtilityForEditor.GetEnemyPrefabs();
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // inherited fields
        EditorGUILayout.PropertyField(serializedObject.FindProperty("log"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("timeScale"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rankLimits"), true);

        // patterns
        patternsToggle = EditorGUILayout.Foldout(patternsToggle, "Patterns");
        if (patternsToggle)
        {
            for (int i = 0; i < patterns.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    // toggle
                    if (GUILayout.Button(openPattern == i ? "|" : "O"))
                    {
                        openPattern = openPattern == i ? -1 : i;
                    }

                    // pattern
                    

                    // spawn type


                    // enemies
                }
                EditorGUILayout.EndHorizontal();
            }

            // add pattern
            if (GUILayout.Button("Add Pattern"))
            {
                AddPattern();
            }
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    #endregion

    #region Private Methods

    private void AddPattern()
    {
        patterns.InsertArrayElementAtIndex(patterns.arraySize);
    }

    #endregion
}