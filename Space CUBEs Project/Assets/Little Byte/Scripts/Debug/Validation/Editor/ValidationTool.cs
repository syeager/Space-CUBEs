// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.13
// Edited: 2014.06.30

using System;
using System.Collections.Generic;
using Annotations;
using LittleByte.Debug.Attributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Runs validation checks in Edit or Play Mode.
/// </summary>
public class ValidationTool : EditorWindow
{
    #region Private Fields

    /// <summary>All gameObjects and their exceptions after validation.</summary>
    private Dictionary<GameObject, List<ExceptionObject>> allExceptions;

    /// <summary>Amount of gameObjects with exceptions.</summary>
    private int gameObjectCount;

    /// <summary>Amount of objects with exceptions.</summary>
    private int objectCount;

    /// <summary>Total number of exceptions.</summary>
    private int exceptionsCount;

    /// <summary>Flags for foldouts.</summary>
    private bool[] gameObjectToggles;

    /// <summary>Exceptions scroll position.</summary>
    private Vector2 scrollPosition;

    #endregion

    #region Readonly Fields

    /// <summary>GUIStyle used for exception messages.</summary>
    private static readonly GUIStyle Style = new GUIStyle
    {
        wordWrap = true,
        normal = new GUIStyleState
        {
            textColor = Color.red
        },
        padding = new RectOffset(0, 4, 0, 0)
    };

    /// <summary>GUIStyle used for exception messages.</summary>
    private static readonly GUIStyle NoExceptionsStyle = new GUIStyle
    {
        wordWrap = true,
        normal = new GUIStyleState
        {
            textColor = Color.green
        },
        padding = new RectOffset(0, 4, 0, 0)
    };

    #endregion

    #region Editor Overrides

    [MenuItem("Tools/Run Validation &V")]
    private static void RunValidation()
    {
        var window = GetWindow<ValidationTool>("Validation");
        window.Run();
    }


    [UsedImplicitly]
    private void OnGUI()
    {
        // close
        if (Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
        {
            Close();
        }

        // info
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label(string.Format("GameObjects: {0}", gameObjectCount));
            GUILayout.Label(string.Format("Objects: {0}", objectCount));
            GUILayout.Label(string.Format("Exceptions: {0}", exceptionsCount));
        }
        GUILayout.EndHorizontal();

        // toggle buttons
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Open All"))
            {
                ToggleFoldouts(true);
            }
            if (GUILayout.Button("Close All"))
            {
                ToggleFoldouts(false);
            }
        }
        GUILayout.EndHorizontal();

        // no exceptions
        if (allExceptions.Count == 0)
        {
            GUILayout.Label("No exceptions found.", NoExceptionsStyle);
            return;
        }

        // exceptions
        int cursor = 0;
        GUILayout.Space(10f);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        {
            foreach (var exception in allExceptions)
            {
                GUILayout.BeginHorizontal();
                {
                    gameObjectToggles[cursor] = EditorGUILayout.Foldout(gameObjectToggles[cursor], exception.Key.name);

                    // open
                    if (GUILayout.Button("O", EditorStyles.miniButtonLeft, GUILayout.Width(25f)))
                    {
                        ToggleFoldout(cursor, exception.Value, true);
                    }
                    // close
                    if (GUILayout.Button("|", EditorStyles.miniButtonRight, GUILayout.Width(25f)))
                    {
                        ToggleFoldout(cursor, exception.Value, false);
                    }

                    // set inspector
                    if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(50f)))
                    {
                        EditorGUIUtility.PingObject(exception.Key);
                        Selection.activeObject = exception.Key;
                    }
                }
                GUILayout.EndHorizontal();

                if (gameObjectToggles[cursor])
                {
                    EditorGUI.indentLevel++;
                    foreach (ExceptionObject exceptionObject in exception.Value)
                    {
                        exceptionObject.open = EditorGUILayout.Foldout(exceptionObject.open, exceptionObject.script.GetType().ToString());
                        if (exceptionObject.open)
                        {
                            EditorGUI.indentLevel++;
                            for (int i = 0; i < exceptionObject.exceptions.Length; i++)
                            {
                                EditorGUILayout.LabelField(string.Format("{0}: {1}", i, exceptionObject.exceptions[i].Message), Style);
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                    EditorGUI.indentLevel--;
                }

                cursor++;
            }
        }
        GUILayout.EndScrollView();

        // run validation
        GUILayout.FlexibleSpace();
        GUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Run Validation", GUILayout.MaxWidth(200f)))
            {
                RunValidation();
            }
            GUILayout.FlexibleSpace();
        }

        GUILayout.EndHorizontal();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Runs validation on all objects in scene.
    /// </summary>
    public void Run()
    {
        Reset();
        Sort(Validate(FindObjectsOfType(typeof(Object))));
    }


    /// <summary>
    /// Run validation check.
    /// </summary>
    /// <param name="objects">Objects to run check on.</param>
    /// <returns>Objects and their corresponding Exceptions.</returns>
    public static Dictionary<Object, List<Exception>> Validate(Object[] objects)
    {
        Dictionary<Object, List<Exception>> objectExceptions = new Dictionary<Object, List<Exception>>();

        foreach (Object obj in objects)
        {
            List<Exception> exceptions = ValidateAttributes.Validate(obj);
            if (exceptions.Count > 0)
            {
                objectExceptions.Add(obj, exceptions);
            }
        }

        ValidationAttribute.Clear();
        return objectExceptions;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Reset counts and allExceptions.
    /// </summary>
    private void Reset()
    {
        allExceptions = new Dictionary<GameObject, List<ExceptionObject>>();
        gameObjectCount = 0;
        objectCount = 0;
        exceptionsCount = 0;
    }


    /// <summary>
    /// Toggle all foldouts.
    /// </summary>
    /// <param name="open">Should the foldouts be open?</param>
    private void ToggleFoldouts(bool open)
    {
        int i = 0;
        foreach (var go in allExceptions)
        {
            gameObjectToggles[i] = open;
            i++;

            foreach (ExceptionObject obj in go.Value)
            {
                obj.open = open;
            }
        }
    }


    /// <summary>
    /// Toggle foldouts for a specific GameObject.
    /// </summary>
    /// <param name="index">GameObject index.</param>
    /// <param name="exceptionsObjects">The GameObject's list of exceptions.</param>
    /// <param name="open">Should the foldouts be open?</param>
    private void ToggleFoldout(int index, List<ExceptionObject> exceptionsObjects, bool open)
    {
        gameObjectToggles[index] = open;
        foreach (ExceptionObject obj in exceptionsObjects)
        {
            obj.open = open;
        }
    }


    /// <summary>
    /// Sort Objects and their Exceptions into allExceptions. Also calculates counts.
    /// </summary>
    /// <param name="objectExceptions">Exceptions gotten from validation.</param>
    private void Sort(Dictionary<Object, List<Exception>> objectExceptions)
    {
        foreach (var objectException in objectExceptions)
        {
            GameObject gameObject = (objectException.Key as Component).gameObject;
            if (!allExceptions.ContainsKey(gameObject))
            {
                gameObjectCount++;
                allExceptions.Add(gameObject, new List<ExceptionObject>());
            }
            allExceptions[gameObject].Add(new ExceptionObject(objectException.Key, objectException.Value.ToArray()));

            objectCount++;
            exceptionsCount += objectException.Value.Count;
        }

        gameObjectToggles = new bool[allExceptions.Count];
    }

    #endregion

    #region Classes

    /// <summary>
    /// Holds an Object and its Exceptions. Also has a flag for foldouts.
    /// </summary>
    private sealed class ExceptionObject
    {
        public readonly Exception[] exceptions;
        public readonly Object script;
        public bool open;


        public ExceptionObject(Object script, Exception[] exceptions)
        {
            this.script = script;
            this.exceptions = exceptions;
        }
    }

    #endregion
}