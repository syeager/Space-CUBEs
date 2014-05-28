// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.01
// Edited: 2014.05.27

using System;
using Annotations;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Debugger))]
public class DebuggerEditor : Creator<Debugger>
{
    #region Private Fields

    private Debugger myDebugger;
    private string[] logTypes;
    private SerializedProperty logSaving;
    private SerializedProperty showTime;
    private SerializedProperty logFlags;
    private SerializedProperty fps;
    private SerializedProperty fpsPrefab;
    private SerializedProperty fpsWarning;
    private SerializedProperty lowFPS;
    private SerializedProperty consoleLine;
    private SerializedProperty consoleLinePrefab;

    #endregion

    #region Const Fields

    private const string FPSName = "_FPS";
    private const string ConsoleLineName = "_ConsoleLine";

    #endregion

    #region Creator Methods

    [MenuItem("GameObject/Singletons/Debugger", false, 3)]
    public static void Create()
    {
        Create("___Debugger");
    }

    #endregion

    #region Editor Overrides

    [UsedImplicitly]
    private void OnEnable()
    {
        myDebugger = (Debugger)target;
        logTypes = Enum.GetNames(typeof(Debugger.LogTypes));
        logSaving = serializedObject.FindProperty("logSaving");
        showTime = serializedObject.FindProperty("showTime");
        logFlags = serializedObject.FindProperty("logFlags");
        fps = serializedObject.FindProperty("fps");
        fpsPrefab = serializedObject.FindProperty("fpsPrefab");
        fpsWarning = serializedObject.FindProperty("fpsWarning");
        lowFPS = serializedObject.FindProperty("lowFPS");
        consoleLine = serializedObject.FindProperty("consoleLine");
        consoleLinePrefab = serializedObject.FindProperty("consoleLinePrefab");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        FPS();
        ConsoleLine();
        EditorGUILayout.Space();
        LoggingOptions();
        EditorGUILayout.Space();
        Logs();

        serializedObject.ApplyModifiedProperties();
    }

    #endregion

    #region Private Methods

    private void FPS()
    {
        // FPS GUI
        if (GUILayout.Button((myDebugger.fps == null ? "Show" : "Hide") + " FPS"))
        {
            if (fps.objectReferenceValue == null)
            {
                fps.objectReferenceValue = (Instantiate(fpsPrefab.objectReferenceValue, new Vector3(0.99f, 0.99f, 0f), Quaternion.identity) as GameObject).GetComponent<HUDFPS>();
                fps.objectReferenceValue.name = FPSName;
                Debugger.Log("FPS created.", fps.objectReferenceValue);
            }
            else
            {
                DestroyImmediate((fps.objectReferenceValue as HUDFPS).gameObject);
                fps.objectReferenceValue = null;
            }
        }

        // FPS warning
        EditorGUILayout.PropertyField(fpsWarning, new GUIContent("FPS Warning"));
        if (fpsWarning.boolValue)
        {
            lowFPS.intValue = EditorGUILayout.IntSlider("Low FPS", lowFPS.intValue, 1, GameTime.targetFPS);
        }
    }


    private void ConsoleLine()
    {
        if (GUILayout.Button((myDebugger.consoleLine == null ? "Show" : "Hide") + " ConsoleLine"))
        {
            if (consoleLine.objectReferenceValue == null)
            {
                consoleLine.objectReferenceValue = Instantiate(consoleLinePrefab.objectReferenceValue, new Vector3(0.5f, 0.01f, 0f), Quaternion.identity);
                consoleLine.objectReferenceValue.name = ConsoleLineName;
                Debugger.Log("ConsoleLine created.", consoleLine.objectReferenceValue);
            }
            else
            {
                DestroyImmediate(consoleLine.objectReferenceValue);
                consoleLine.objectReferenceValue = null;
            }
        }
    }


    private void LoggingOptions()
    {
        EditorGUILayout.PropertyField(logSaving, new GUIContent("Log Saving"));
        EditorGUILayout.PropertyField(showTime);
    }


    private void Logs()
    {
        // all
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("All Log Types");
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
    }

    #endregion
}