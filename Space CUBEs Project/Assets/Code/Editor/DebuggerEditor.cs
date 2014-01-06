using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Debugger))]
public class DebuggerEditor : Editor
{
    #region Private Fields

    private Debugger myDebugger;
    private string[] logTypes;

    #endregion


    #region Editor Overrides

    private void OnEnable()
    {
        myDebugger = (Debugger)target;
        logTypes = Enum.GetNames(typeof(Debugger.LogTypes));
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // FPS
        if (GUILayout.Button((myDebugger.FPS == null ? "Show" : "Hide") + " FPS"))
        {
            if (serializedObject.FindProperty("FPS").objectReferenceValue == null)
            {
                serializedObject.FindProperty("FPS").objectReferenceValue = Instantiate(serializedObject.FindProperty("FPS_Prefab").objectReferenceValue, new Vector3(0.99f, 0.99f, 0f), Quaternion.identity);
            }
            else
            {
                DestroyImmediate(serializedObject.FindProperty("FPS").objectReferenceValue);
                serializedObject.FindProperty("FPS").objectReferenceValue = null;
            }
        }

        // ConsoleLine
        if (GUILayout.Button((myDebugger.ConsoleLine == null ? "Show" : "Hide") + " ConsoleLine"))
        {
            if (serializedObject.FindProperty("ConsoleLine").objectReferenceValue == null)
            {
                serializedObject.FindProperty("ConsoleLine").objectReferenceValue = Instantiate(serializedObject.FindProperty("ConsoleLine_Prefab").objectReferenceValue, new Vector3(0.5f, 0.01f, 0f), Quaternion.identity);
            }
            else
            {
                DestroyImmediate(serializedObject.FindProperty("ConsoleLine").objectReferenceValue);
                serializedObject.FindProperty("ConsoleLine").objectReferenceValue = null;
            }
        }

        // overwrite logs
        EditorGUILayout.PropertyField(serializedObject.FindProperty("overwrite"), new GUIContent("Overwrite Saved Logs"));

        // log types
        EditorGUILayout.LabelField("Toggle Log Types");
        EditorGUI.indentLevel++;
        SerializedProperty logFlags = serializedObject.FindProperty("logFlags");
        for (int i = 0; i < myDebugger.logFlags.Length; i++)
        {
            EditorGUILayout.PropertyField(logFlags.GetArrayElementAtIndex(i), new GUIContent(logTypes[i]));
        }
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }

    #endregion
}