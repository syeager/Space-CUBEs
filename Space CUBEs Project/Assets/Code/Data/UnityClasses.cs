// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.08
// Edited: 2014.06.09

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UnityClasses
{
    public interface IUnityClass
    {
        object Cast();
    }


    public static class UnityTypes
    {
        private static readonly Type[] Types =
        {
            typeof(Vector3)
        };

        private static readonly Type[] EditorTypes =
        {
            typeof(int),
            typeof(float),
            typeof(Vector3),
        };


        private static readonly Dictionary<Type, Type> Surogates = new Dictionary<Type, Type>
        {
            {typeof(sVector3), typeof(Vector3)}
        };


        public static bool IsUnity(object obj)
        {
            return Types.Contains(obj.GetType());
        }


        public static bool IsUnity(Type type)
        {
            return Types.Contains(type);
        }


        public static bool IsEditor(object obj)
        {
            return EditorTypes.Contains(obj.GetType());
        }


        public static Type GetUnityType(object obj)
        {
            return Surogates[obj.GetType()];
        }


        public static object UnityCast(object obj)
        {
            Type objType = obj.GetType();

            if (objType == typeof(Vector3))
            {
                return (sVector3)(Vector3)obj;
            }

            return obj;
        }


        public static object SCast(object obj)
        {
            Debug.Log(obj.GetType());
            Type objType = obj.GetType();

            if (objType == typeof(sVector3))
            {
                return (Vector3)(sVector3)obj;
            }

            return obj;
        }
    }


    [Serializable]
    public sealed class sVector3 : IUnityClass
    {
        public float x, y, z;


        public sVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }


        public static implicit operator Vector3(sVector3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }


        public static implicit operator sVector3(Vector3 v)
        {
            return new sVector3(v.x, v.y, v.z);
        }

        public object Cast()
        {
            return (Vector3)this;
        }
    }
}