// Steve Yeager
// 5.18.2014

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
        };

        private static readonly Dictionary<Type, Func<object, object>> SDTypes = new Dictionary<Type, Func<object, object>>()
        {
            {typeof(SDVector2), (o) => ((SDVector2)o).ToVector2()},
            {typeof(SDVector3), (o) => ((SDVector3)o).ToVector3()},
        };

        #endregion

        #region Const Fields

        private const string FilePostfix = "Data.dat";
        public const string DefaultFile = "Default";

        #endregion

#if UNITY_EDITOR

        #region Events

        public static EventHandler<ValueArgs> FileSavedEvent;

        #endregion

#endif

        #region Initialization Methods

        /// <summary>
        /// 
        /// </summary>
        public static void Initialize()
        {
            GameMaster.QuitEvent -= OnQuit;
            GameMaster.QuitEvent += OnQuit;

            Close();
            Files.Clear();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="file"></param>
        public static void Save(string key, object value, string file = DefaultFile)
        {
            // get all data in file
            Dictionary<string, object> data = LoadFileData(file);

            bool found = false;
#if UNITY_EDITOR
            using (BinaryWriter writer = new BinaryWriter(GetFileStream(file)))
#else
            BinaryWriter writer = new BinaryWriter(GetFileStream(file));
#endif
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

#if UNITY_EDITOR
            FileSavedEvent.Fire(value, new ValueArgs(file));
#endif
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="file"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T Load<T>(string key, string file = DefaultFile, T defaultValue = default(T))
        {
#if UNITY_EDITOR
            using (BinaryReader reader = new BinaryReader(GetFileStream(file)))
#else
            BinaryReader reader = new BinaryReader(GetFileStream(file));
#endif
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
                        using (MemoryStream stream = new MemoryStream(bytes))
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
                //reader.Close();

                // not found
                return default(T);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Dictionary<string, object> LoadFileData(string file)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

#if UNITY_EDITOR
            using (BinaryReader reader = new BinaryReader(GetFileStream(file)))
#else
            BinaryReader reader = new BinaryReader(GetFileStream(file));
#endif
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    // get key
                    string key = reader.ReadString();
                    // get value size
                    int size = reader.ReadInt32();
                    // get value
                    byte[] bytes = reader.ReadBytes(size);
                    using (MemoryStream stream = new MemoryStream(bytes))
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
            //reader.Close();

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
            using (MemoryStream serializeStream = new MemoryStream())
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
#if !UNITY_EDITOR
            FileStream fileStream;
            if (Files.TryGetValue(file, out fileStream))
            {
                fileStream.Position = 0;
                return fileStream;
            }
#endif

            FileStream stream = new FileStream(DataPath + file + FilePostfix, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
#if !UNITY_EDITOR
            Files.Add(file, stream);
#endif
            return stream;
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

#endregion