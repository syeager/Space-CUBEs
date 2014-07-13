// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.07.12
// Edited: 2014.07.12

using System;
using System.Globalization;
using System.Linq;
using Annotations;
using UnityEditor;
using UnityEngine;

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

        private static GUIStyle buttonStyle;

        private static bool initial = true;

        private string search;

        #endregion

        #region Const Fields

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
            sceneNames = scenePaths.Select(path => System.IO.Path.GetFileNameWithoutExtension(path)).ToArray();
            initial = true;

            EditorWindow window = GetWindow<SceneLoader>("Scenes");
            window.minSize = window.maxSize = new Vector2(200f, scenePaths.Length * 20f + 20f + 16f);
        }


        [UsedImplicitly]
        private void OnGUI()
        {
            // create style
            buttonStyle = GUI.skin.button;
            buttonStyle.alignment = TextAnchor.MiddleLeft;

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
                        UnityEngine.Object scene = AssetDatabase.LoadAssetAtPath(GUI.GetNameOfFocusedControl(), typeof(UnityEngine.Object));
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
                    search = "";
                    initial = true;
                    Repaint();
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
            EditorGUILayout.LabelField(search);
        }


        private void ScenesGUI()
        {
            for (int i = 0; i < scenePaths.Length; i++)
            {
                string scenePath = scenePaths[i];
                GUI.SetNextControlName(scenePath);
                if (GUILayout.Button(sceneNames[i], buttonStyle))
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
                Close();
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

        #endregion
    }
}