// Steve Yeager
// 5.19.2014

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace LittleByte.Data
{
    /// <summary>
    /// Serialize/Deserialize unity structs and classes.
    /// </summary>
    public static class UnitySerializer
    {
        #region Public Methods

        public static bool Serializable(Type type)
        {
            bool answer =
                type == typeof(Vector2) ||
                type == typeof(Vector3);

            return answer;
        }

        public static byte[] Serialize(object value)
        {
            Type type = value.GetType();
            if (type == typeof(Vector2))
            {
                return SerializeVector2((Vector2)value);
            }
            if (type == typeof(Vector3))
            {
                return SerializeVector3((Vector3)value);
            }

            throw Debugger.LogException(
                new NotSupportedException(string.Format("Type \"{0}\" is not supported for Unity serialization.",
                    type.Name)));
        }


        public static T Deserialize<T>(byte[] bytes)
        {
            Type type = typeof(T);
            if (type == typeof(Vector2))
            {
                return (T)(object)DeserializeVector2(bytes);
            }
            if (type == typeof(Vector3))
            {
                return (T)(object)DeserializeVector3(bytes);
            }

            throw Debugger.LogException(
                new NotSupportedException(string.Format("Type \"{0}\" is not supported for Unity serialization.",
                    type.Name)));
        }

        #endregion

        #region Vector Methods

        private static byte[] SerializeVector2(Vector2 vector)
        {
            float[] info =
            {
                vector.x,
                vector.y
            };

            using (MemoryStream stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, info);
                return stream.ToArray();
            }
        }


        private static Vector2 DeserializeVector2(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                float[] info = (float[])new BinaryFormatter().Deserialize(stream);
                return new Vector2(info[0], info[1]);
            }
        }


        private static byte[] SerializeVector3(Vector3 vector)
        {
            float[] info =
            {
                vector.x,
                vector.y,
                vector.z
            };

            using (MemoryStream stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, info);
                return stream.ToArray();
            }
        }


        private static Vector3 DeserializeVector3(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                float[] info = (float[])new BinaryFormatter().Deserialize(stream);
                return new Vector3(info[0], info[1], info[2]);
            }
        }

        #endregion
    }
}