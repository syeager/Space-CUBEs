// Little Byte Games
// Author: Steve Yeager
// Created: 2014.08.24
// Edited: 2014.08.24

using UnityEditor;

[CustomEditor(typeof(LevelSelectButton))]
public class LevelSelectButtonInspector : UIButtonEditor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("level"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("nameLabel"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("disabledTextColor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("infoBackground"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("highScoreLabel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("bestTimeLabel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("medal"));

        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }
}