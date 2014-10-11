// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.02
// Edited: 2014.09.05

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Annotations;
using LittleByte.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LittleByte.ImportSettings
{
    /// <summary>
    /// Draw import settings in the inspector for a folder.
    /// </summary>
    public class FolderInspector : ObjectInspector
    {
        #region Private Fields

        private static Type[] importTypes;

        private List<SettingGroup> settingGroups;

        private class SettingGroup : IComparable<SettingGroup>
        {
            public ImportSettings importSetting;
            public bool toggle;


            public SettingGroup(ImportSettings importSetting, bool toggle)
            {
                this.importSetting = importSetting;
                this.toggle = toggle;
            }


            public int CompareTo(SettingGroup other)
            {
                return importSetting.Name.CompareTo(other.importSetting.Name);
            }
        }

        private SettingsPopup openWindow;

        private string targetPath;

        #endregion

        #region Const Fields

        private const string SettingsFolder = "_ImportSettings";
        private const string SettingsKey = "Import Settings:";
        private const char GUIDSep = ',';

        private const float ButtonWidth = 30f;

        #endregion

        #region Editor Overrides

        [UsedImplicitly]
        private void OnEnable()
        {
            targetPath = AssetDatabase.GetAssetPath(target);

            importTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(ImportSettings)) && !t.IsAbstract).OrderBy(t => t.Name).ToArray();
            LoadImportSettings();
        }


        public override void OnInspectorGUI()
        {
            GUI.enabled = openWindow == null;

            DropSettings();
            AddSettingsButton();
            ImportSettingsGUI();

            // inspector starts disabled
            GUI.enabled = true;
        }

        #endregion

        #region ObjectInspector Overrides

        /// <summary>
        /// Is this inspector valid for the current object.
        /// </summary>
        /// <param name="path">Unity path to Object.</param>
        /// <returns>True if it is a directory.</returns>
        public static bool IsValid(string path)
        {
            return Directory.Exists(path);
        }

        #endregion

        #region GUI Methods

        private void DropSettings()
        {
            ImportSettings settings = (ImportSettings)EditorGUILayout.ObjectField(new GUIContent("Import Settings", "Drap settings to add or override settings for this folder."), null, typeof(ImportSettings), false);
            if (settings != null)
            {
                SettingGroup oldSettings = settingGroups.FirstOrDefault(sg => sg.importSetting.AssetType == settings.AssetType);
                if (oldSettings != null)
                {
                    if (oldSettings.importSetting == settings)
                    {
                        return;
                    }
                    else
                    {
                        RemoveSetting(settingGroups.FindIndex(sg => sg.importSetting == oldSettings.importSetting));
                    }
                }
                AddSetting(new SettingGroup(settings, true), AssetDatabase.GetAssetPath(settings));
            }
        }


        private void AddSettingsButton()
        {
            GUI.enabled = settingGroups.Count != importTypes.Length;
            if (GUILayout.Button("Add Settings", EditorStyles.toolbarButton))
            {
                openWindow = EditorWindow.GetWindow<SettingsPopup>(true, "Import Settings");
                openWindow.ShowPopup();
                openWindow.Initialize(this, importTypes.Where(t => settingGroups.All(s => s.importSetting.GetType() != t)).Select(t => t.Name).ToArray());
            }
            GUI.enabled = true;
        }


        private void ImportSettingsGUI()
        {
            // draw all settings
            if (settingGroups != null && settingGroups.Count > 0)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < settingGroups.Count; i++)
                {
                    EditorGUILayout.BeginVertical("box");
                    {
                        SettingGroup settingGroup = settingGroups[i];

                        EditorGUILayout.BeginHorizontal();
                        {
                            settingGroup.toggle = EditorGUILayout.Foldout(settingGroup.toggle, settingGroup.importSetting.Name);

                            // type
                            GUILayout.FlexibleSpace();
                            GUI.enabled = false;
                            GUILayout.Label("[" + settingGroup.importSetting.AssetType.Name.Substring(0, settingGroup.importSetting.AssetType.Name.Length - 8) + "]", EditorStyles.miniLabel);
                            GUI.enabled = true;

                            // select
                            if (GUILayout.Button(new GUIContent("○", "Ping"), EditorStyles.miniButtonLeft, GUILayout.Width(ButtonWidth)))
                            {
                                EditorGUIUtility.PingObject(settingGroup.importSetting);
                                return;
                            }

                            // reimport
                            if (GUILayout.Button(new GUIContent("↓", "Reimport"), EditorStyles.miniButtonMid, GUILayout.Width(ButtonWidth)))
                            {
                                AssetDatabase.ImportAsset(targetPath, ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate);
                                return;
                            }

                            // delete
                            if (GUILayout.Button(new GUIContent("-", "Delete asset if in local " + SettingsFolder + " folder or remove if not."), EditorStyles.miniButtonRight, GUILayout.Width(ButtonWidth)))
                            {
                                RemoveSetting(i);
                                return;
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        if (settingGroup.toggle)
                        {
                            GUILayout.Space(4f);
                            settingGroup.importSetting.OnGUI();
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUI.indentLevel--;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a new import setting.
        /// </summary>
        /// <param name="setting">Type name.</param>
        public void CreateSetting(string setting)
        {
            // get/create settings folder
            string path = targetPath + "/" + SettingsFolder;
            Object folder = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            if (folder == null)
            {
                AssetDatabase.CreateFolder(targetPath, SettingsFolder);
                AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            }

            // create asset
            SettingGroup createdSettings = new SettingGroup((ImportSettings)CreateInstance(setting), false);
            string assetPath = path + "/" + setting + ".asset";
            AssetDatabase.CreateAsset(createdSettings.importSetting, assetPath);

            // add
            AddSetting(createdSettings, assetPath);
        }

        #endregion

        #region Private Methods

        private void LoadImportSettings()
        {
            settingGroups = new List<SettingGroup>();

            AssetImporter importer = AssetImporter.GetAtPath(targetPath);
            if (importer == null || string.IsNullOrEmpty(importer.userData))
            {
                return;
            }

            List<string> data = importer.userData.Split('\n').ToList();
            int index = data.FindIndex(d => d.Contains(SettingsKey));
            if (index != -1)
            {
                IEnumerable<string> ids = data[index].Substring(SettingsKey.Length).Split(GUIDSep).Where(s => !string.IsNullOrEmpty(s));
                foreach (string id in ids)
                {
                    ImportSettings importSetting = (ImportSettings)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(id), typeof(ImportSettings));
                    if (importSetting == null)
                    {
                        DeleteGUID(id);
                    }
                    else
                    {
                        settingGroups.Add(new SettingGroup(importSetting, false));
                    }
                }
            }
        }


        private void RemoveSetting(int index)
        {
            string assetPath = AssetDatabase.GetAssetPath(settingGroups[index].importSetting);

            // save
            DeleteGUID(AssetDatabase.AssetPathToGUID(assetPath));

            // delete
            if (assetPath == targetPath + "/" + SettingsFolder + "/" + settingGroups[index].importSetting.name + ".asset")
            {
                AssetDatabase.DeleteAsset(assetPath);
            }

            // remove from lists
            settingGroups.RemoveAt(index);

            // delete folder if applicable
            string folderPath = targetPath + "/" + SettingsFolder;
            Object settingsFolder = AssetDatabase.LoadAssetAtPath(folderPath, typeof(Object));
            if (settingsFolder != null && Directory.GetFiles(folderPath).Length == 0)
            {
                AssetDatabase.DeleteAsset(folderPath);
            }

            AssetDatabase.Refresh();
        }


        private void DeleteGUID(string guid)
        {
            AssetImporter importer = AssetImporter.GetAtPath(targetPath);
            List<string> data = importer.userData.Split('\n').ToList();
            int dataIndex = data.FindIndex(d => d.Contains(SettingsKey));
            data[dataIndex] = data[dataIndex].Replace(guid + GUIDSep, string.Empty);
            importer.userData = data.Join('\n');
        }


        private void AddSetting(SettingGroup setting, string assetPath)
        {
            // add to lists
            int sortIndex = settingGroups.Count;
            for (int i = 0; i < settingGroups.Count; i++)
            {
                if (setting.CompareTo(settingGroups[i]) == -1)
                {
                    sortIndex = i;
                    break;
                }
            }
            settingGroups.Insert(sortIndex, setting);

            // save
            AssetImporter importer = AssetImporter.GetAtPath(targetPath);
            List<string> data = importer.userData.Split('\n').ToList();
            int index = data.FindIndex(d => d.Contains(SettingsKey));
            if (index == -1)
            {
                index = data.Count - 1;
            }
            IEnumerable<string> tempList = GetSettingGUIDs(targetPath);
            List<string> guids;
            if (tempList != null)
            {
                guids = tempList.ToList();
                guids.Insert(sortIndex, AssetDatabase.AssetPathToGUID(assetPath));
            }
            else
            {
                guids = new List<string> { AssetDatabase.AssetPathToGUID(assetPath) };
            }
            data[index] = SettingsKey + guids.Join(GUIDSep);
            importer.userData = data.Join('\n');
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Get all import setting GUIDs for folder.
        /// </summary>
        /// <param name="path">Path to folder object.</param>
        /// <returns>List of Unity GUIDs.</returns>
        public static IEnumerable<string> GetSettingGUIDs(string path)
        {
            AssetImporter importer = AssetImporter.GetAtPath(path);
            if (importer == null || string.IsNullOrEmpty(importer.userData))
            {
                return null;
            }

            List<string> data = importer.userData.Split('\n').ToList();
            int index = data.FindIndex(d => d.Contains(SettingsKey));
            return index != -1 ? data[index].Substring(SettingsKey.Length).Split(GUIDSep).Where(s => !string.IsNullOrEmpty(s)) : null;
        }

        #endregion
    }

    /// <summary>
    /// Popup with all possible import settings.
    /// </summary>
    public class SettingsPopup : EditorWindow
    {
        #region Private Fields

        private FolderInspector folder;
        private string[] types;

        #endregion

        #region Const Fields

        private const float Width = 200f;
        private const float Height = 20f;
        private const string Postfix = "Settings";

        #endregion

        #region EditorWindow Overrides

        [UsedImplicitly]
        private void OnGUI()
        {
            foreach (string type in types)
            {
                GUI.SetNextControlName(type);
                if (GUILayout.Button(type.Replace(Postfix, string.Empty), EditorStyles.miniButton, GUILayout.Height(Height)))
                {
                    folder.CreateSetting(type);
                    Close();
                }
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(FolderInspector folder, string[] types)
        {
            this.folder = folder;
            this.types = types;
            minSize = maxSize = new Vector2(Width, types.Length * Height + 6);
        }

        #endregion
    } 
}