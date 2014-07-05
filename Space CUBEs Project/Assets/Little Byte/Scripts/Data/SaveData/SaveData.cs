// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.05.18
// Edited: 2014.06.20

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityClasses;
using UnityEngine;

namespace LittleByte.Data
{
    /// <summary>
    /// Save/Load data from binary files.
    /// </summary>
    public static class SaveData
    {
        #region Readonly Fields

        /// <summary>BinaryFormatter used for all saving and loading.</summary>
        private static readonly BinaryFormatter Formatter = new BinaryFormatter();

        private static readonly string DataPath = Application.persistentDataPath + "/Data/";

        #endregion

        #region Const Fields

        public const string FileExt = ".dat";
        public const string DefaultPath = @"Default/";

        #endregion

        #region Events

        public static EventHandler<ValueArgs> FileSavedEvent;

        #endregion

        #region Public Methods

        /// <summary>
        /// Save value to data.
        /// </summary>
        /// <param name="file">File used to save the value.</param>
        /// <param name="value">Value to save.</param>
        /// <param name="path">Path of folder containing the file.</param>
        public static void Save(string file, object value, string path = DefaultPath)
        {
            FileStream fileStream;
            GetFileStream(path, file, out fileStream);

            using (var writer = new BinaryWriter(fileStream))
            {
                using (var mStream = new MemoryStream())
                {
                    Formatter.Serialize(mStream, value);
                    writer.Write(mStream.ToArray());
                }
            }

            FileSavedEvent.Fire(value, new ValueArgs(path + file));
        }


        /// <summary>
        /// Save a Unity value to data.
        /// </summary>
        /// <param name="file">File used to save the value.</param>
        /// <param name="value">Value to save.</param>
        /// <param name="path">Path of folder containing the file.</param>
        public static void SaveUnity(string file, object value, string path = DefaultPath)
        {
            Save(file, UnityTypes.SCast(value), path);
        }


        /// <summary>
        /// Load saved data.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="file">File used to save the value.</param>
        /// <param name="path">Path of folder containing the file.</param>
        /// <param name="defaultValue">Default value to return if key is not found.</param>
        /// <returns>Value from data if found or default value.</returns>
        public static T Load<T>(string file, string path = DefaultPath, T defaultValue = default(T))
        {
            FileStream fileStream;
            if (!GetFileStream(path, file, out fileStream))
            {
                Save(file, defaultValue, path);
                return defaultValue;
            }

            using (var reader = new BinaryReader(fileStream))
            {
                byte[] bytes = reader.ReadBytes((int)reader.BaseStream.Length);
                using (var mStream = new MemoryStream(bytes))
                {
                    object value = Formatter.Deserialize(mStream);
                    if (value is IUnityClass)
                    {
                        value = ((IUnityClass)value).Cast();
                    }
                    else if (IsSList(value))
                    {
                        return LoadList<T>((IList)value);
                    }
                    return (T)value;
                }
            }
        }


        /// <summary>
        /// Load saved data.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="file">File used to save the value.</param>
        /// <param name="path">Path of folder containing the file.</param>
        /// <param name="defaultValue">Default value to return if key is not found.</param>
        /// <returns>Value from data if found or default value.</returns>
        public static T LoadUnity<T>(string file, string path = DefaultPath, T defaultValue = default(T))
        {
            T value = Load(file, path, defaultValue);
            return (T)UnityTypes.UnityCast(value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T LoadFromPath<T>(string path, T defaultValue = default(T))
        {
            int split = path.LastIndexOf('\\');
            string folderPath = path.Substring(0, split + 1);
            string file = path.Substring(split + 1, path.Length - split - 1);

            return Load(file, folderPath, defaultValue);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static object LoadFromPath(string path, object defaultValue = null)
        {
            int split = path.LastIndexOf('\\');
            string folderPath = path.Substring(0, split + 1);
            string file = path.Substring(split + 1, path.Length - split - 1);

            return Load(file, folderPath, defaultValue);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T LoadFromResources<T>(string path, T defaultValue = default(T))
        {
            var file = (TextAsset)Resources.Load(path);
            using (var mStream = new MemoryStream(file.bytes))
            {
                object value = Formatter.Deserialize(mStream);
                if (value is IUnityClass)
                {
                    value = ((IUnityClass)value).Cast();
                }
                else if (IsSList(value))
                {
                    return LoadList<T>((IList)value);
                }
                return (T)value;
            }
        }


        /// <summary>
        /// Get list of all files in Data folder.
        /// </summary>
        /// <returns>List of file titles in data folder. Doesn't contain paths or postfixes.</returns>
        public static string[] GetAllFiles()
        {
            return Directory.GetFiles(DataPath, "*" + FileExt, SearchOption.AllDirectories).Select(f => f.Remove(0, DataPath.Length).Replace(FileExt, "")).ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] GetFiles(string path)
        {
            path = DataPath + path;
            return Directory.GetFiles(path, "*" + FileExt, SearchOption.AllDirectories).Select(f => f.Remove(0, path.Length).Replace(FileExt, "")).ToArray();
        }


        /// <summary>
        /// Delete a specific file.
        /// </summary>
        /// <param name="file">File to delete.</param>
        /// <param name="path">Path of folder containing the file.</param>
        /// <returns>True, if a file was deleted.</returns>
        public static bool Delete(string file, string path = @"Default\")
        {
            path = DataPath + path;
            file = path + file + FileExt;
            if (Directory.Exists(path) && File.Exists(file))
            {
                File.Delete(file);

                if (Directory.GetFiles(path).Length == 0)
                {
                    Directory.Delete(path);
                }

                return true;
            }

            return false;
        }


        /// <summary>
        /// Delete a whole data folder.
        /// </summary>
        /// <param name="path">Folder path.</param>
        /// <returns>True, if a folder deleted.</returns>
        public static bool DeleteFolder(string path)
        {
            path = DataPath + path;
            if (Directory.Exists(path))
            {
                Directory.Delete(path);

                return true;
            }

            return false;
        }


        /// <summary>
        /// Delete all data files.
        /// </summary>
        public static void DeleteAll()
        {
            Directory.Delete(DataPath, true);
            Directory.CreateDirectory(DataPath);
        }


        /// <summary>
        /// Checks for the existance of a file.
        /// </summary>
        /// <param name="file">File to search for.</param>
        /// <param name="path">Folder path to search for file in.</param>
        /// <returns>True, if the file is found.</returns>
        public static bool Contains(string file, string path = DefaultPath)
        {
            path = DataPath + path;
            file = path + file + FileExt;
            return Directory.Exists(path) && File.Exists(file);
        }

        #endregion

        #region Private Methods

        private static void SaveList(string file, IList list, string path)
        {
            // convert
            List<object> converted = list.Cast<object>().Select(x => UnityTypes.UnityCast(x)).ToList();

            FileStream fileStream;
            GetFileStream(path, file, out fileStream);

            using (var writer = new BinaryWriter(fileStream))
            {
                using (var mStream = new MemoryStream())
                {
                    Formatter.Serialize(mStream, converted);
                    writer.Write(mStream.ToArray());
                }
            }

            FileSavedEvent.Fire(converted, new ValueArgs(DataPath + path + file + FileExt));
        }


        private static T LoadList<T>(IList list)
        {
            Type type = typeof(T).GetGenericParameterConstraints()[0];

            IList newList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
            foreach (object entry in list)
            {
                newList.Add(UnityTypes.SCast(entry));
            }
            return (T)newList;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        private static bool GetFileStream(string path, string file, out FileStream fileStream)
        {
            bool found = true;
            path = DataPath + path;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                found = false;
            }
            else if (!File.Exists(path + file + FileExt))
            {
                found = false;
            }

            fileStream = new FileStream(path + file + FileExt, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 8, FileOptions.RandomAccess);
            return found;
        }


        private static bool IsUnityList(object value)
        {
            if (value is IList)
            {
                IList list = (IList)value;
                if (list.Count > 0 && UnityTypes.IsUnity(list[0]))
                {
                    return true;
                }
            }

            return false;
        }


        private static bool IsSList(object value)
        {
            if (value is IList)
            {
                IList list = (IList)value;
                if (list.Count > 0 && list[0] is IUnityClass)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}