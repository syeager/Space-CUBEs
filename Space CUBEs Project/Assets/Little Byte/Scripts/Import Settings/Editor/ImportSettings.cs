// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.02
// Edited: 2014.09.04

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LittleByte.ImportSettings
{
    /// <summary>
    /// Base class for settings saved in folders and applied to their contents.
    /// </summary>
    public abstract class ImportSettings : ScriptableObject
    {
        #region Abstract Members

        /// <summary>Asset importer to listen for.</summary>
        public abstract Type AssetType { get; }

        /// <summary>Identifiable name.</summary>
        public abstract string Name { get; }


        /// <summary>
        /// Apply settings to the asset importer.
        /// </summary>
        /// <param name="assetImporter">Asset importer for current object being updated.</param>
        public abstract void Apply(AssetImporter assetImporter);

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Are these settings applicable to this asset importer?
        /// </summary>
        /// <param name="assetImporter">Asset importer for current object being updated.</param>
        /// <returns>True, if these import settings should be used.</returns>
        public virtual bool IsValid(AssetImporter assetImporter)
        {
            return AssetType == assetImporter.GetType();
        }


        /// <summary>
        /// Draw members in inspector.
        /// </summary>
        public virtual void OnGUI()
        {
            SerializedObject serializedSetting = new SerializedObject(this);
            FieldInfo[] fields = GetType().GetFields();
            foreach (FieldInfo field in fields)
            {
                EditorGUILayout.PropertyField(serializedSetting.FindProperty(field.Name), true);
            }
            serializedSetting.ApplyModifiedProperties();
        }

        #endregion
    } 
}