// Little Byte Games

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using LittleByte.Extensions;
using UnityClasses;
using UnityEngine;
using Profiler = LittleByte.Debug.Profiler;

namespace LittleByte.Data
{
    /// <summary>
    /// Save/Load data from binary files.
    /// </summary>
    public static class SaveData
    {
        #region Const Fields

        /// <summary>BinaryFormatter used for all saving and loading.</summary>
        private static readonly BinaryFormatter Formatter = new BinaryFormatter();

        /// <summary>Parent folder location for all data.</summary>
        private static readonly string DataPath = Application.persistentDataPath + Slash + "Data" + Slash;

        /// <summary>Path for the backed up data.</summary>
        private static readonly string BackupPath = Application.persistentDataPath + Slash + "Backup" + Slash;

        /// <summary>File extension for data files.</summary>
        public const string FileExt = ".dat";

        /// <summary>Default folder to save data if none is specified.</summary>
        public const string DefaultPath = "Default";

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        /// <summary>Slash used in folder paths.</summary>
        public const string Slash = "\\";
#else
        /// <summary>Slash used in folder paths.</summary>
        public const string Slash = "/";
#endif

        /// <summary>Prefix for backup files.</summary>
        public const string BackupName = "Backup-";

        #endregion

        #region Events

        /// <summary>Fired when any data is saved.</summary>
        public static EventHandler<FileSavedArgs> FileSavedEvent;

        #endregion

        #region Save Methods

        /// <summary>
        /// Save value to data.
        /// </summary>
        /// <param name="file">File used to save the value.</param>
        /// <param name="value">Value to save.</param>
        /// <param name="path">Path of folder containing the file.</param>
        public static void Save(string file, object value, string path = DefaultPath)
        {
            if (!path.EndsWith(Slash))
            {
                path += Slash;
            }

            FileStream fileStream;
            GetFileStream(path, file, out fileStream);

            using (var writer = new BinaryWriter(fileStream))
            {
                if (UnityTypes.IsUnity(value))
                {
                    value = UnityTypes.sCast(value);
                }
                using (var mStream = new MemoryStream())
                {
                    Formatter.Serialize(mStream, value);
                    writer.Write(mStream.ToArray());
                }
            }

            FileSavedEvent.Fire(value, new FileSavedArgs(path, file));
        }

        /// <summary>
        /// Save a Unity value to data.
        /// </summary>
        /// <param name="file">File used to save the value.</param>
        /// <param name="value">Value to save.</param>
        /// <param name="path">Path of folder containing the file.</param>
        public static void SaveUnity(string file, object value, string path = DefaultPath)
        {
            Save(file, UnityTypes.UnityCast(value), path);
        }

        /// <summary>
        /// Save all contents of a file.
        /// </summary>
        /// <param name="path">Path for the file.</param>
        /// <param name="data">All contents for the file.</param>
        private static void SaveFile(string path, byte[] data)
        {
            string directoryPath = DataPath + path.Substring(0, path.LastIndexOf(Slash));
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            var fileStream = new FileStream(DataPath + path, FileMode.Create);
            fileStream.Write(data, 0, data.Length);
            fileStream.Dispose();
        }

        #endregion

        #region Load Methods

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
            if (!path.EndsWith(Slash))
            {
                path += Slash;
            }
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
        /// Load data from a path and file.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="path">Path and file.</param>
        /// <param name="defaultValue">Default value to return if key is not found.</param>
        /// <returns>Value from data if found or default value.</returns>
        public static T LoadFromPath<T>(string path, T defaultValue = default(T))
        {
            int split = path.LastIndexOf(Slash);
            string folderPath = path.Substring(0, split + 1);
            string file = path.Substring(split + 1, path.Length - split - 1);

            return Load(file, folderPath, defaultValue);
        }

        /// <summary>
        /// Load data from the Resources folder.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="path">Path in Resources folder to asset.</param>
        /// <param name="defaultValue">Default value to return if key is not found.</param>
        /// <returns>Value from data if found or default value.</returns>
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
        /// Checks for the existance of a file.
        /// </summary>
        /// <param name="file">File to search for.</param>
        /// <param name="path">Folder path to search for file in.</param>
        /// <returns>True, if the file is found.</returns>
        public static bool Contains(string file, string path = DefaultPath)
        {
            path = DataPath + path;
            file = path + Slash + file + FileExt;
            return Directory.Exists(path) && File.Exists(file);
        }

        /// <summary>
        /// Save all data to a byte[].
        /// </summary>
        /// <returns>All data serialized.</returns>
        public static byte[] ExportGameState()
        {
            int prefixLength = DataPath.Length;
            using (var profiler = new Profiler("ExportGameState"))
            {
                string[] files = Directory.GetFiles(DataPath, "*" + FileExt, SearchOption.AllDirectories);
                object[] allFiles = new object[files.Length * 2];
                for (int i = 0, j = 0; i < allFiles.Length; i++, j++)
                {
                    allFiles[i] = files[j].Substring(prefixLength, files[j].Length - prefixLength);
                    i++;
                    BinaryReader reader = new BinaryReader(new FileStream(files[j], FileMode.Open));
                    allFiles[i] = reader.ReadBytes((int)reader.BaseStream.Length);
                }

                using (var mStream = new MemoryStream())
                {
                    Formatter.Serialize(mStream, allFiles);
                    byte[] data = mStream.ToArray();
                    return data;
                }
            }
        }

        /// <summary>
        /// Save the current game state in a backup file.
        /// </summary>
        public static void Backup()
        {
            if (!Directory.Exists(BackupPath))
            {
                Directory.CreateDirectory(BackupPath);
            }

            byte[] bytes = ExportGameState();
            const string date = BackupName + "{0:yyyy-MM-dd_hh-mm-ss-tt}";
            using (var writer = new BinaryWriter(new FileStream(BackupPath + string.Format(date, DateTime.Now) + FileExt, FileMode.OpenOrCreate)))
            {
                writer.Write(bytes);
            }
        }

        /// <summary>
        /// Restore the game state from serialized data.
        /// </summary>
        /// <param name="data">Serialized data to restore.</param>
        public static void RestoreGameState(byte[] data)
        {
            using (var profiler = new Profiler("RestoreGameState"))
            {
                object[] allData = (object[])Formatter.Deserialize(new MemoryStream(data));
                string path = string.Empty;
                for (int i = 0; i < allData.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        path = (string)allData[i];
                    }
                    else
                    {
                        var fileData = (byte[])allData[i];
                        SaveFile(path, fileData);
                    }
                }
            }
        }

        /// <summary>
        /// Get the latest backup file.
        /// </summary>
        /// <returns>Serialized data from the latest backup file.</returns>
        public static byte[] GetLatestBackup()
        {
            if (!Directory.Exists(BackupPath))
            {
                return null;
            }

            DirectoryInfo backupDirectory = new DirectoryInfo(BackupPath);

            FileInfo newestFile = backupDirectory.GetFiles().OrderByDescending(f => f.CreationTime).FirstOrDefault();

            if (newestFile == null)
            {
                return null;
            }

            BinaryReader reader = new BinaryReader(new FileStream(newestFile.FullName, FileMode.Open));
            byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);
            return data;
        }

        #endregion

        #region File Methods

        /// <summary>
        /// Create file stream for a file.
        /// </summary>
        /// <param name="path">Path of the folder for the file.</param>
        /// <param name="file">File name.</param>
        /// <param name="fileStream">File stream to create.</param>
        /// <returns>True, if a file was found.</returns>
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

        /// <summary>
        /// Get list of all files in Data folder.
        /// </summary>
        /// <returns>List of file titles in data folder. Doesn't contain paths or postfixes.</returns>
        public static string[] GetAllFiles()
        {
            return Directory.GetFiles(DataPath, "*" + FileExt, SearchOption.AllDirectories).Select(f => f.Remove(0, DataPath.Length).Replace(FileExt, "")).ToArray();
        }

        /// <summary>
        /// Get list of all files in this folder.
        /// </summary>
        /// <param name="path">Path to the folder.</param>
        /// <returns>List of file titles in the folder. Doesn't contain paths or postfixes.</returns>
        public static string[] GetFiles(string path)
        {
            path = DataPath + path;
            return Directory.GetFiles(path, "*" + FileExt).Select(f => f.Remove(0, path.Length + 1).Replace(FileExt, "")).ToArray();
        }

        #endregion

        #region Unity Methods

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
            return (T)UnityTypes.sCast(value);
        }

        private static bool IsUnityList(object value)
        {
            var list = value as IList;
            if (list != null)
            {
                if (list.Count > 0 && UnityTypes.IsUnity(list[0]))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsSList(object value)
        {
            IList list = value as IList;
            if (list != null)
            {
                if (list.Count > 0 && list[0] is IUnityClass)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Deletion Methods

        /// <summary>
        /// Delete a specific file.
        /// </summary>
        /// <param name="file">File to delete.</param>
        /// <param name="path">Path of folder containing the file.</param>
        /// <returns>True, if a file was deleted.</returns>
        public static bool DeleteFile(string file, string path = DefaultPath)
        {
            path = DataPath + path + Slash;
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

        #endregion

        #region Private Methods

        private static void SaveList(string file, IList list, string path)
        {
            // convert
            List<object> converted = list.Cast<object>().Select(x => UnityTypes.sCast(x)).ToList();

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

            //FileSavedEvent.Fire(converted, new FileSavedArgs(DataPath + path + file + FileExt));
        }

        private static T LoadList<T>(IList list)
        {
            Type type = typeof(T).GetGenericParameterConstraints()[0];

            IList newList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
            foreach (object entry in list)
            {
                newList.Add(UnityTypes.UnityCast(entry));
            }
            return (T)newList;
        }

        #endregion
    }

    /// <summary>
    /// Args for file saving.
    /// </summary>
    public class FileSavedArgs : EventArgs
    {
        public readonly string folder;
        public readonly string file;

        public string Path
        {
            get { return folder + SaveData.Slash + file; }
        }

        public FileSavedArgs(string folder, string file)
        {
            this.folder = folder;
            this.file = file;
        }
    }
}