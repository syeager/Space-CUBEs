// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.08
// Edited: 2014.06.08

using System;
using System.IO;
using System.Linq;
using ProtoBuf;
using UnityEngine;

namespace LittleByte.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class Protobuf
    {
        #region Readonly Fields

        private static readonly string DataPath = Application.persistentDataPath + "/ProtoData/";

        #endregion

        #region Const Fields

        private const string FileExt = ".proto";
        public const string DefaultPath = @"Default\";

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
            using (FileStream fStream = fileStream)
            {
                Serializer.Serialize(fStream, value);
            }

            FileSavedEvent.Fire(value, new ValueArgs(path));
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

            using (FileStream fStream = fileStream)
            {
                return Serializer.Deserialize<T>(fStream);
            }
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
        /// <typeparam name="T"></typeparam>
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
        /// Get list of all files in Data folder.
        /// </summary>
        /// <returns>List of file titles in data folder. Doesn't contain paths or postfixes.</returns>
        public static string[] GetFiles()
        {
            return Directory.GetFiles(DataPath, "*.proto", SearchOption.AllDirectories).Select(f => f.Remove(0, DataPath.Length).Replace(FileExt, "")).ToArray();
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

        #endregion
    }
}