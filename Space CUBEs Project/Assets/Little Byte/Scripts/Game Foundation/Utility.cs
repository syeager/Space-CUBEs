// Little Byte Games
// Author: Steve Yeager
// Created: 2014.01.11
// Edited: 2014.10.11

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

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

    public static Vector3 RotateVector(Vector3 vector, Quaternion rotation)
    {
        Matrix4x4 rot = new Matrix4x4();
        rot.SetTRS(Vector3.zero, rotation, Vector3.one);
        return rot.MultiplyVector(vector);
    }


    public static Vector3 RotateVector(Vector3 vector, float angle, Vector3 axis)
    {
        return RotateVector(vector, Quaternion.AngleAxis(angle, axis));
    }


    /// <summary>
    /// Caculate a random Vector3 between 2 vectors.
    /// </summary>
    /// <param name="bl">Small vector.</param>
    /// <param name="tr">Big vector.</param>
    /// <returns>Random Vector3.</returns>
    public static Vector3 RandomVector3(Vector3 bl, Vector3 tr)
    {
        return new Vector3(Random.Range(bl.x, tr.x), Random.Range(bl.y, tr.y), Random.Range(bl.z, tr.z));
    }

    #endregion

    #region Clone Methods

    public static T DeepClone<T>(T obj)
    {
        using (var ms = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;

            return (T)formatter.Deserialize(ms);
        }
    }

    #endregion

    #region Editor Methods

#if UNITY_EDITOR
    public static IEnumerable<T> LoadObjects<T>(string path, bool recursive) where T : Component
    {
        string[] files = recursive ? Directory.GetFiles(path, "*.*", SearchOption.AllDirectories) : Directory.GetFiles(path);
        List<T> objects = new List<T>();

        for (int i = 0; i < files.Length; i++)
        {
            Object obj = AssetDatabase.LoadAssetAtPath(files[i], typeof(GameObject));
            if (obj != null && PrefabUtility.GetPrefabType(obj) == PrefabType.Prefab)
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


    public static T LoadObject<T>(string path) where T : Object
    {
        return (T)AssetDatabase.LoadAssetAtPath("Assets/" + path, typeof(Object));
    }


    public static T LoadObjectFromGUID<T>(string GUID) where T : Object
    {
        return (T)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(GUID), typeof(T));
    }
#endif

    #endregion

    #region NGUI Methods

    /// <summary>
    /// Reposition children in grid.
    /// </summary>
    /// <param name="grid">Grid to update.</param>
    /// <param name="scrollBar">Scrollbar to update.</param>
    ///  <param name="scrollView">ScrollView to update.</param>
    public static IEnumerator UpdateScrollView(UIGrid grid, UIScrollBar scrollBar, UIScrollView scrollView)
    {
        yield return new WaitForEndOfFrame();

        grid.Reposition();
        scrollView.ResetPosition();
    }

    #endregion
}