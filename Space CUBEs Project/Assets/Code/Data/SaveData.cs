// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.05.18
// Edited: 2014.06.03

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace LittleByte.Data
{
    /// <summary>
    /// Save/Load data from binary files.
    /// </summary>
    public static class SaveData
    {
        #region Readonly Fields

        private static readonly string DataPath = Application.persistentDataPath + "/Data/";
        private static readonly Dictionary<string, FileStream> Files = new Dictionary<string, FileStream>();
        private static readonly BinaryFormatter Formatter = new BinaryFormatter();

        private static readonly Dictionary<Type, Func<object, object>> UnityTypes = new Dictionary<Type, Func<object, object>>()
        {
            {typeof(Vector2), (o) => new SDVector2((Vector2)o)},
            {typeof(Vector3), (o) => new SDVector3((Vector3)o)},
            {typeof(Vector4), (o) => new SDVector4((Vector4)o)},
        };

        private static readonly Dictionary<Type, Func<object, object>> SDTypes = new Dictionary<Type, Func<object, object>>()
        {
            {typeof(SDVector2), (o) => ((SDVector2)o).ToVector2()},
            {typeof(SDVector3), (o) => ((SDVector3)o).ToVector3()},
            {typeof(SDVector4), (o) => ((SDVector4)o).ToVector4()},
        };

        #endregion

        #region Const Fields

        private const string FilePostfix = "Data.dat";
        public const string DefaultFile = "Default";

        #endregion

        #region Events

        public static EventHandler<ValueArgs> FileSavedEvent;

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Set up save data at the beginning of the game.
        /// </summary>
        public static void Initialize(params string[] files)
        {
            //GameMaster.QuitEvent -= OnQuit;
            //GameMaster.QuitEvent += OnQuit;

            //if (!Directory.Exists(DataPath))
            //{
            //    Directory.CreateDirectory(DataPath);
            //}

            //Close();
            //Files.Clear();

            // touch files already created
            string[] createdFiles = GetFiles();
            foreach (var file in createdFiles)
            {
                GetFileStream(file);
            }

            // create files
            foreach (var file in files)
            {
                GetFileStream(file);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Save value to data.
        /// </summary>
        /// <param name="key">Unique key in the file associated with the value.</param>
        /// <param name="value">Value to save.</param>
        /// <param name="file">File to save in. Default = "Default"</param>
        public static void Save(string key, object value, string file = DefaultFile)
        {
            // get all data in file
            Dictionary<string, object> data = LoadFileData(file);
            File.WriteAllText(DataPath + file + FilePostfix, string.Empty);

            bool found = false;

            using (var writer = new BinaryWriter(GetFileStream(file)))
            {
                foreach (var entry in data)
                {
                    // found key
                    if (string.Equals(key, entry.Key))
                    {
                        found = true;
                        Write(key, value, writer);
                    }
                    else
                    {
                        Write(entry.Key, entry.Value, writer);
                    }
                }

                // never found
                if (!found)
                {
                    Write(key, value, writer);
                }
            }

            FileSavedEvent.Fire(value, new ValueArgs(file));
        }


        /// <summary>
        /// Load saved data.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="key">Unique key in the file associated with the value.</param>
        /// <param name="file">File to load from. Default = "Default"</param>
        /// <param name="defaultValue">Default value to return if key is not found. Default = default(T)</param>
        /// <returns>Value from data if found or default value.</returns>
        public static T Load<T>(string key, string file = DefaultFile, T defaultValue = default(T))
        {
            using (var reader = new BinaryReader(GetFileStream(file)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    // found key
                    if (string.Equals(reader.ReadString(), key))
                    {
                        // get size of value
                        int size = reader.ReadInt32();
                        // get value
                        byte[] bytes = reader.ReadBytes(size);
                        using (var stream = new MemoryStream(bytes))
                        {
                            object obj = Formatter.Deserialize(stream);

                            // convert from SD to Unity
                            if (SDTypes.ContainsKey(obj.GetType()))
                            {
                                obj = (T)SDTypes[obj.GetType()](obj);
                            }

                            return (T)obj;
                        }
                    }
                    else
                    {
                        // get size of value
                        int size = reader.ReadInt32();
                        // get value
                        reader.ReadBytes(size);
                    }
                }

                // not found
                return defaultValue;
            }
        }


        /// <summary>
        /// Loads all data in a file.
        /// </summary>
        /// <param name="file">File to load data from.</param>
        /// <returns>Collection of keys and their saved data.</returns>
        public static Dictionary<string, object> LoadFileData(string file)
        {
            var data = new Dictionary<string, object>();

            using (var reader = new BinaryReader(GetFileStream(file)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    // get key
                    string key = reader.ReadString();
                    // get value size
                    int size = reader.ReadInt32();
                    // get value
                    byte[] bytes = reader.ReadBytes(size);
                    using (var stream = new MemoryStream(bytes))
                    {
                        object value = Formatter.Deserialize(stream);

                        // convert from SD to Unity
                        if (SDTypes.ContainsKey(value.GetType()))
                        {
                            value = SDTypes[value.GetType()](value);
                        }

                        data.Add(key, value);
                    }
                }
            }

            return data;
        }


        /// <summary>
        /// Get list of all files in Data folder.
        /// </summary>
        /// <returns>List of file titles in data folder. Don't contain path or postfix.</returns>
        public static string[] GetFiles()
        {
            return Directory.GetFiles(DataPath, "*.dat").Select(f => f.Remove(0, DataPath.Length).Replace(FilePostfix, "")).ToArray();
        }


        /// <summary>
        /// Delete a specific key in a file.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool DeleteKey(string key, string file = "Default")
        {
            // get all data in file
            Dictionary<string, object> data = LoadFileData(file);
            File.WriteAllText(DataPath + file + FilePostfix, string.Empty);

            bool found = false;

            using (var writer = new BinaryWriter(GetFileStream(file)))
            {
                foreach (var entry in data)
                {
                    // found key
                    if (string.Equals(key, entry.Key))
                    {
                        found = true;
                    }
                    else
                    {
                        Write(entry.Key, entry.Value, writer);
                    }
                }
            }

            return found;
        }


        /// <summary>
        /// Delete a whole data file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool DeleteFile(string file)
        {
            string path = DataPath + file + FilePostfix;
            if (!File.Exists(path))
            {
                return false;
            }

            File.Delete(path);
            return true;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="writer"></param>
        private static void Write(string key, object value, BinaryWriter writer)
        {
            // key
            writer.Write(key);

            // convert to SD from Unity
            if (UnityTypes.ContainsKey(value.GetType()))
            {
                value = UnityTypes[value.GetType()](value);
            }

            // serialize
            byte[] bytes;
            using (var serializeStream = new MemoryStream())
            {
                Formatter.Serialize(serializeStream, value);
                bytes = serializeStream.ToArray();
            }

            // size
            writer.Write(bytes.Length);

            // value
            writer.Write(bytes);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static FileStream GetFileStream(string file)
        {
            return new FileStream(DataPath + file + FilePostfix, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 8, FileOptions.RandomAccess);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void Close()
        {
            foreach (var file in Files)
            {
                file.Value.Close();
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnQuit(object sender, EventArgs args)
        {
            Close();
        }

        #endregion
    }
}

#region Structs

[Serializable]
public struct SDVector2
{
    public float x, y;


    public SDVector2(Vector2 vector2)
    {
        x = vector2.x;
        y = vector2.y;
    }


    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }
}

[Serializable]
public struct SDVector3
{
    public float x, y, z;


    public SDVector3(Vector3 vector3)
    {
        x = vector3.x;
        y = vector3.y;
        z = vector3.z;
    }


    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

[Serializable]
public struct SDVector4
{
    public float x, y, z, w;


    public SDVector4(Vector4 vector4)
    {
        x = vector4.x;
        y = vector4.y;
        z = vector4.z;
        w = vector4.w;
    }


    public Vector4 ToVector4()
    {
        return new Vector4(x, y, z, w);
    }
}

#endregion