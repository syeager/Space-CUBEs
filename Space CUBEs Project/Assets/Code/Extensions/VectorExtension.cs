// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.01.01
// Edited: 2014.06.28

using UnityEngine;

public static class VectorExtension
{
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
}