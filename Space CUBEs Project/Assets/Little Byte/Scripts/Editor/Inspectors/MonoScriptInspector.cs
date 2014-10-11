// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.05
// Edited: 2014.09.05

using System;
using Annotations;
using UnityEditor;
using UnityEngine;

namespace LittleByte
{
    /// <summary>
    /// 
    /// </summary>
    [CustomEditor(typeof(MonoScript), false)]
    public class MonoScriptInspector : Editor
    {
        #region Private Fields

        private bool isScriptableObject;

        #endregion

        #region Editor Overrides

        [UsedImplicitly]
        private void OnEnable()
        {
        }


        public override void OnInspectorGUI()
        {
            MonoScript script = (MonoScript)target;
            Type type = script.GetClass();
            if (!(type.IsSubclassOf(typeof(ScriptableObject)) && !type.IsAbstract && !type.IsSubclassOf(typeof(Editor)))) return;

            if (GUILayout.Button("Create Instance"))
            {
                ScriptableObject scriptableObject = CreateInstance(type);
                string assetPath = AssetDatabase.GetAssetPath(target);
                string path = AssetDatabase.GenerateUniqueAssetPath(assetPath.Substring(0, assetPath.Length - 3) + ".asset");
                AssetDatabase.CreateAsset(scriptableObject, path);

                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object)));
            }
        }

        #endregion
    } 
}