// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.01
// Edited: 2014.10.30

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Annotations;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LittleByte.UnityExtensions
{
    /// <summary>
    /// Get all scene paths and provide buttons for loading them.
    /// </summary>
    public class SceneLoader : EditorWindow
    {
        #region Private Fields

        private static string[] scenePaths;
        private static string[] sceneNames;

        private static bool initial = true;

        private string search;

        #endregion

        #region Const Fields

        private static readonly GUIStyle ButtonStyle = new GUIStyle("button") {alignment = TextAnchor.MiddleLeft};
        private static readonly GUIStyle ToolbarStyle = new GUIStyle("Toolbar");
        private static readonly GUIStyle SearchbarStyle = new GUIStyle("ToolbarSeachTextField");
        private static readonly GUIStyle CancelButtonStyle = new GUIStyle("ToolbarSeachCancelButton");

        private const string SceneExt = ".unity";

        #endregion

        #region EditorWindow Overrides

        [UsedImplicitly]
        [MenuItem("Tools/Scene Loader &S", true, 200)]
        private static bool Valid()
        {
            return !Application.isPlaying;
        }


        [UsedImplicitly]
        [MenuItem("Tools/Scene Loader &S", false, 200)]
        private static void Init()
        {
            scenePaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(SceneExt)).OrderByDescending(path => path).Reverse().ToArray();
            sceneNames = scenePaths.Select(path => Path.GetFileNameWithoutExtension(path)).ToArray();
            initial = true;

            EditorWindow window = GetWindow<SceneLoader>("Scenes");
            window.minSize = window.maxSize = new Vector2(200f, scenePaths.Length * 20f + 20f + 16f);
        }


        [UsedImplicitly]
        private void OnGUI()
        {
            // set current scene if not moved
            if (initial)
            {
                GUI.FocusControl(EditorApplication.currentScene);
                Repaint();
            }

            // key presses
            if (Event.current.isKey && Event.current.type == EventType.KeyDown)
            {
                initial = false;
                KeyCode key = Event.current.keyCode;

                // close
                if (key == KeyCode.Escape)
                {
                    Close();
                }

                // up
                if (key == KeyCode.UpArrow)
                {
                    int currentFocus = Array.IndexOf(scenePaths, GUI.GetNameOfFocusedControl());
                    int focus = currentFocus - 1;
                    if (focus < 0)
                    {
                        focus = scenePaths.Length - 1;
                    }
                    GUI.FocusControl(scenePaths[focus]);
                    Repaint();
                }

                // down
                if (key == KeyCode.DownArrow)
                {
                    int currentFocus = Array.IndexOf(scenePaths, GUI.GetNameOfFocusedControl());
                    int focus = currentFocus + 1;
                    if (focus >= scenePaths.Length)
                    {
                        focus = 0;
                    }
                    GUI.FocusControl(scenePaths[focus]);
                    Repaint();
                }

                // load/select
                if (key == KeyCode.Return)
                {
                    if (Event.current.control)
                    {
                        // select
                        Object scene = AssetDatabase.LoadAssetAtPath(GUI.GetNameOfFocusedControl(), typeof(Object));
                        Selection.activeObject = scene;
                        EditorGUIUtility.PingObject(scene);
                    }
                    else
                    {
                        // load
                        LoadScene(GUI.GetNameOfFocusedControl(), !Event.current.shift);
                    }
                }

                // search
                char character = Event.current.character;
                if (character >= 97 && character <= 122)
                {
                    search += character;
                    Search();
                }

                // clear search
                if (key == KeyCode.Delete)
                {
                    ClearSearch();
                }

                // backspace search
                if (key == KeyCode.Backspace)
                {
                    if (search.Length > 0)
                    {
                        search = search.Substring(0, search.Length - 1);
                        if (search.Length == 0)
                        {
                            initial = true;
                        }
                        else
                        {
                            Search();
                        }
                    }
                    else
                    {
                        initial = true;
                        EditorApplication.Beep();
                    }
                }
            }

            SearchGUI();
            ScenesGUI();
        }

        #endregion

        #region GUI Methods

        private void SearchGUI()
        {
            EditorGUILayout.BeginHorizontal(ToolbarStyle);
            {
                EditorGUILayout.TextField(search, SearchbarStyle);

                if (GUILayout.Button("", CancelButtonStyle))
                {
                    ClearSearch();
                }
            }
            EditorGUILayout.EndHorizontal();
        }


        private void ScenesGUI()
        {
            for (int i = 0; i < scenePaths.Length; i++)
            {
                string scenePath = scenePaths[i];
                GUI.SetNextControlName(scenePath);
                if (GUILayout.Button(sceneNames[i], ButtonStyle))
                {
                    LoadScene(scenePath, true);
                }
            }
        }

        #endregion

        #region Private Methods

        private void LoadScene(string scenePath, bool save)
        {
            if (!save || EditorApplication.SaveCurrentSceneIfUserWantsTo())
            {
                EditorApplication.OpenScene(scenePath);
            }
        }


        private void Search()
        {
            string[] compatible = sceneNames.Where(name => name.Replace(" ", "").StartsWith(search, true, CultureInfo.CurrentCulture)).ToArray();
            if (compatible.Length > 0)
            {
                int path = Array.IndexOf(sceneNames, compatible[0]);
                GUI.FocusControl(scenePaths[path]);
            }
            else
            {
                initial = true;
                EditorApplication.Beep();
            }
            Repaint();
        }


        private void ClearSearch()
        {
            search = "";
            initial = true;
            Repaint();
        }

        #endregion
    }
}