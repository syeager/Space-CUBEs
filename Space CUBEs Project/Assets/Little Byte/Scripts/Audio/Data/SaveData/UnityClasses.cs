// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.08
// Edited: 2014.06.09

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityClasses
{
    public interface IUnityClass
    {
        object Cast();
    }


    public static class UnityTypes
    {
        private static readonly Dictionary<Type, Type> Surrogates = new Dictionary<Type, Type>
        {
            {typeof(sColor), typeof(Color)},
            {typeof(sVector2), typeof(Vector2)},
            {typeof(sVector3), typeof(Vector3)},
        };


        public static bool IsUnity(object obj)
        {
            return Surrogates.ContainsValue(obj.GetType());
        }


        public static bool IsUnity(Type type)
        {
            return Surrogates.ContainsValue(type);
        }


        public static object sCast(object obj)
        {
            Type objType = obj.GetType();

            if (objType == typeof(Color))
            {
                return (sColor)(Color)obj;
            }
            if (objType == typeof(Vector2))
            {
                return (sVector2)(Vector2)obj;
            }
            if (objType == typeof(Vector3))
            {
                return (sVector3)(Vector3)obj;
            }

            return obj;
        }


        public static object UnityCast(object obj)
        {
            Type objType = obj.GetType();

            if (objType == typeof(sColor))
            {
                return (Color)(sColor)obj;
            }
            if (objType == typeof(sVector2))
            {
                return (Vector2)(sVector2)obj;
            }
            if (objType == typeof(sVector3))
            {
                return (Vector3)(sVector3)obj;
            }

            return obj;
        }
    }


    [Serializable]
    public sealed class sVector2 : IUnityClass
    {
        public float x, y;

        public sVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator Vector2(sVector2 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static implicit operator sVector2(Vector2 v)
        {
            return new sVector2(v.x, v.y);
        }

        public object Cast()
        {
            return (Vector2)this;
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


    [Serializable]
    public sealed class sColor : IUnityClass
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public sColor(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static implicit operator Color(sColor c)
        {
            return new Color(c.r, c.g, c.b, c.a);
        }

        public static implicit operator sColor(Color c)
        {
            return new sColor(c.r, c.g, c.b, c.a);
        }

        public object Cast()
        {
            return (Color)this;
        }
    }
}