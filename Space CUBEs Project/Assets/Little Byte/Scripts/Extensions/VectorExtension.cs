// Little Byte Games
// Author: Steve Yeager
// Created: 2014.01.01
// Edited: 2014.09.20

using UnityEngine;

public static class VectorExtension
{

    #region Vector3 Overrides

     public static Vector3 Round(this Vector3 v)
    {
        return new Vector3(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
    }


    public static float Area(this Vector3 v)
    {
        return v.x * v.y * v.z;
    }


    public static Vector3 To(this Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }


    public static Vector3 Multipy(this Vector3 vector, Vector3 other)
    {
        return new Vector3(vector.x * other.x, vector.y * other.y, vector.z * other.z);
    }

    #endregion

    #region Vector2 Overrides

     public static Vector2 To(this Vector2 from, Vector2 to)
    {
        return (to - from).normalized;
    }

    #endregion
}