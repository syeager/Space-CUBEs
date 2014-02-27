﻿// Steve Yeager
// 1.11.2014

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds static methods for general jobs.
/// </summary>
public static class Utility
{
    #region String Methods

    /// <summary>
    /// Converts string to Vector3.
    /// </summary>
    /// <param name="vectorString">String to convert. Must be in format (#, #, #)</param>
    /// <param name="sep">Char used to separate values. Default is comma.</param>
    /// <returns>Vector3 representation of the string.</returns>
    public static Vector3 ParseV3(string vectorString, char sep = ',')
    {
        Vector3 vector;
        vectorString = vectorString.Substring(1, vectorString.Length - 2).Replace(" ", "");
        string[] split = vectorString.Split(sep);
        vector.x = float.Parse(split[0]);
        vector.y = float.Parse(split[1]);
        vector.z = float.Parse(split[2]);

        return vector;
    }


    public static Color ParseColor(string colorString)
    {
        Color color;
        colorString = colorString.Substring(5, colorString.Length - 6).Replace(" ", "");
        string[] split = colorString.Split(',');
        color.r = float.Parse(split[0]);
        color.g = float.Parse(split[1]);
        color.b = float.Parse(split[2]);
        color.a = float.Parse(split[3]);

        return color;
    }

    #endregion

    #region Math Methods

    public static float SinZero(float angle)
    {
        float value = Mathf.Sin(angle * Mathf.PI / 180f);
        if (Mathf.Abs(value) <= 0.000001f)
        {
            value = 0f;
        }
        return value;
    }


    public static float CosZero(float angle)
    {
        float value = Mathf.Cos(angle * Mathf.PI / 180f);
        if (Mathf.Abs(value) <= 0.000001f)
        {
            value = 0f;
        }
        return value;
    }


    public static Matrix4x4 RotationMatrixX(float angle)
    {
        float c = CosZero(angle);
        float s = SinZero(angle);

        return new Matrix4x4
        {
            m00 = 1,
            m01 = 0,
            m02 = 0,
            m03 = 0,
            m10 = 0,
            m11 = c,
            m12 = -s,
            m13 = 0,
            m20 = 0,
            m21 = s,
            m22 = c,
            m23 = 0,
            m30 = 0,
            m31 = 0,
            m32 = 0,
            m33 = 1
        };
    }


    public static Matrix4x4 RotationMatrixY(float angle)
    {
        float c = CosZero(angle);
        float s = SinZero(angle);

        return new Matrix4x4
        {
            m00 = c,
            m01 = 0,
            m02 = s,
            m03 = 0,
            m10 = 0,
            m11 = 1,
            m12 = 0,
            m13 = 0,
            m20 = -s,
            m21 = 0,
            m22 = c,
            m23 = 0,
            m30 = 0,
            m31 = 0,
            m32 = 0,
            m33 = 1
        };
    }


    public static Matrix4x4 RotationMatrixZ(float angle)
    {
        float c = CosZero(angle);
        float s = SinZero(angle);

        return new Matrix4x4
        {
            m00 = c,
            m01 = -s,
            m02 = 0,
            m03 = 0,
            m10 = s,
            m11 = c,
            m12 = 0,
            m13 = 0,
            m20 = 0,
            m21 = 0,
            m22 = 1,
            m23 = 0,
            m30 = 0,
            m31 = 0,
            m32 = 0,
            m33 = 1
        };
    }


    public static Matrix4x4 RotationMatrixAroundPoint(Vector3 axis, float angle)
    {
        float c = CosZero(angle);
        float s = SinZero(angle);

        return new Matrix4x4
        {
            m00 = c + (1 - c) * axis.x * axis.x,
            m01 = (1 - c) * axis.x * axis.y - s * axis.z,
            m02 = (1 - c) * axis.x * axis.z + s * axis.y,
            m03 = 0,
            m10 = (1 - c) * axis.x * axis.y + s * axis.z,
            m11 = c + (1 - c) * axis.y * axis.y,
            m12 = (1 - c) * axis.y * axis.z - s * axis.x,
            m13 = 0,
            m20 = (1 - c) * axis.x * axis.z - s * axis.y,
            m21 = (1 - c) * axis.y * axis.z + s * axis.x,
            m22 = c + (1 - c) * axis.z * axis.z,
            m23 = 0,
            m30 = 0,
            m31 = 0,
            m32 = 0,
            m33 = 1
        };
    }

    #endregion

    #region Editor Methods

#if UNITY_EDITOR
    public static IEnumerable<T> LoadObjects<T>(string path) where T : Component
    {
        string[] files = System.IO.Directory.GetFiles(path);
        List<T> objects = new List<T>();

        for (int i = 0; i < files.Length; i++)
        {
            UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(files[i], typeof(GameObject));
            if (obj != null && UnityEditor.PrefabUtility.GetPrefabType(obj) == UnityEditor.PrefabType.Prefab)
            {
                T comp = (obj as GameObject).GetComponent<T>();
                if (comp != null)
                {
                    objects.Add(comp);
                }
            }
        }

        return objects;
    }
#endif


#if UNITY_EDITOR
    public static T LoadObject<T>(string path) where T : Component
    {
        string[] files = System.IO.Directory.GetFiles(path);

        for (int i = 0; i < files.Length; i++)
        {
            UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(files[i], typeof(GameObject));
            if (obj != null && UnityEditor.PrefabUtility.GetPrefabType(obj) == UnityEditor.PrefabType.Prefab)
            {
                T comp = (obj as GameObject).GetComponent<T>();
                if (comp != null)
                {
                    return comp;
                }
            }
        }

        return null;
    }
#endif

    #endregion

    #region NGUI Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="scrollBar"></param>
    public static IEnumerator UpdateScrollView(UIGrid grid, UIScrollBar scrollBar, bool setToZero = true)
    {
        yield return new WaitForEndOfFrame();
        grid.Reposition();
        if (setToZero)
        {
            scrollBar.value = 0f;
            scrollBar.ForceUpdate();
        }
    }

    #endregion
}