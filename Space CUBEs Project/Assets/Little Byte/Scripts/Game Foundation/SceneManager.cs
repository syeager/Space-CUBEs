// Little Byte Games
// Author: Steve Yeager
// Created: 2014.07.31
// Edited: 2014.08.15

using System;
using System.Collections.Generic;
using UnityEngine;

namespace LittleByte
{
    /// <summary>
    /// Singleton to transition between scenes.
    /// </summary>
    public static class SceneManager
    {
        #region Const Fields

        public const string LoadScreen = "Load Screen";

        #endregion

        #region Properties

        /// <summary>The scene loaded previously to this scene.</summary>
        public static string PreviousScene { get; private set; }

        /// <summary>The scene to load after the Loading Screen.</summary>
        public static string NextScene { get; private set; }

        public static Dictionary<string, object> Payload { get; private set; }

        #endregion

        #region Constructors

        static SceneManager()
        {
            Payload = new Dictionary<string, object>();
        }

        #endregion

        #region Loading Methods

        /// <summary>
        /// Load the next scene and cache data.
        /// </summary>
        /// <param name="nextScene">Name of next scene.</param>
        /// <param name="loadScreen">Should the Loading Screen be loaded first?</param>
        /// <param name="clean">Should the game be cleaned up?</param>
        public static AsyncOperation LoadScene(string nextScene, bool loadScreen = false, bool clean = false)
        {
            PreviousScene = Application.loadedLevelName;
            NextScene = nextScene;

            Application.LoadLevel(loadScreen ? LoadScreen : nextScene);
            if (clean)
            {
                GC.Collect();
                return Resources.UnloadUnusedAssets();
            }

            return null;
        }


        public static AsyncOperation LoadScenePayload(string nextScene, Dictionary<string, object> payload, bool loadScreen, bool clean)
        {
            Payload = payload ?? new Dictionary<string, object>();
            return LoadScene(nextScene, loadScreen, clean);
        }


        public static AsyncOperation LoadScenePayload(string nextScene, string key, object value, bool loadScreen, bool clean)
        {
            Payload = new Dictionary<string, object>{{key, value}};
            return LoadScene(nextScene, loadScreen, clean);
        }


        /// <summary>
        /// Reloads the current scene without the Loading Screen.
        /// </summary>
        public static void ReloadScene(bool load = false, bool unload = false, bool collect = false)
        {
            if (unload) Resources.UnloadUnusedAssets();
            if (collect) GC.Collect();
            Application.LoadLevel(Application.loadedLevel);
        }

        #endregion
    }
}