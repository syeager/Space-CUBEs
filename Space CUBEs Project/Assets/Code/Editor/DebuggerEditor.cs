using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Debugger))]
public class DebuggerEditor : Editor
{
    #region Private Fields

    private SerializedObject sObject;
    private Debugger myDebugger;
    private string[] logTypes;

    #endregion


    #region Editor Overrides

    private void OnEnable()
    {
        sObject = new SerializedObject(target);
        myDebugger = (Debugger) target;
        logTypes = Enum.GetNames(typeof(Debugger.LogTypes));
    }


    public override void OnInspectorGUI()
    {
        sObject.Update();

        EditorGUILayout.PropertyField(sObject.FindProperty("overwrite"), new GUIContent("Overwrite Saved Logs"));

        EditorGUILayout.LabelField("Toggle Log Types");
        EditorGUI.indentLevel++;
        SerializedProperty logFlags = sObject.FindProperty("logFlags");
        for (int i = 0; i < myDebugger.logFlags.Length; i++)
        {
            EditorGUILayout.PropertyField(logFlags.GetArrayElementAtIndex(i), new GUIContent(logTypes[i]));
        }
        EditorGUI.indentLevel--;

        sObject.ApplyModifiedProperties();
    }

    #endregion
}