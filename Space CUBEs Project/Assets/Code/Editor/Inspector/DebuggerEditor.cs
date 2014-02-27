using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Debugger))]
public class DebuggerEditor : Editor
{
    #region Private Fields

    private Debugger myDebugger;
    private string[] logTypes;
    private SerializedProperty overwrite;
    private SerializedProperty showTime;
    private SerializedProperty logFlags;

    #endregion


    #region Editor Overrides

    private void OnEnable()
    {
        myDebugger = (Debugger)target;
        logTypes = Enum.GetNames(typeof(Debugger.LogTypes));
        overwrite = serializedObject.FindProperty("overwrite");
        showTime = serializedObject.FindProperty("showTime");
        logFlags = serializedObject.FindProperty("logFlags");
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
        EditorGUILayout.PropertyField(overwrite, new GUIContent("Overwrite Saved Logs"));
        EditorGUILayout.PropertyField(showTime);

        // all
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("All");
            if (GUILayout.Button("On"))
            {
                for (int i = 0; i < myDebugger.logFlags.Length; i++)
                {
                    logFlags.GetArrayElementAtIndex(i).boolValue = true;
                }
            }
            if (GUILayout.Button("Off"))
            {
                for (int i = 0; i < myDebugger.logFlags.Length; i++)
                {
                    logFlags.GetArrayElementAtIndex(i).boolValue = false;
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        // log types
        EditorGUILayout.LabelField("Toggle Log Types");
        EditorGUI.indentLevel++;
        for (int i = 0; i < myDebugger.logFlags.Length; i++)
        {
            EditorGUILayout.PropertyField(logFlags.GetArrayElementAtIndex(i), new GUIContent(logTypes[i]));
        }
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }

    #endregion
}