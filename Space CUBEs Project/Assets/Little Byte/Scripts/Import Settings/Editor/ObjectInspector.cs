// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.02
// Edited: 2014.09.04

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Annotations;
using UnityEditor;
using UnityEngine;

namespace LittleByte.ImportSettings
{
    [CustomEditor(typeof(UnityEngine.Object), false)]
    public class ObjectInspector : Editor
    {
        #region Private Fields

        private ObjectInspector activeInspector;

        #endregion

        #region Const Fields

        private const string IsValid = "IsValid";

        #endregion

        #region Editor Overrides

        [UsedImplicitly]
        private void OnEnable()
        {
            string path = AssetDatabase.GetAssetPath(target);
            IEnumerable<Type> types = Assembly.GetCallingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(ObjectInspector)));
            Type validType = types.FirstOrDefault(t => (bool)t.GetMethod(IsValid, BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { path }));

            if (validType != null)
            {
                activeInspector = (ObjectInspector)CreateEditor(target, validType);
            }
        }


        public override void OnInspectorGUI()
        {
            if (activeInspector == null) return;

            GUI.enabled = true;
            activeInspector.OnInspectorGUI();
        }

        #endregion
    } 
}