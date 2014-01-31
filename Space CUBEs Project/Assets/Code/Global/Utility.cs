﻿// Steve Yeager
// 1.11.2014

using System;
using UnityEngine;

/// <summary>
/// Holds static methods for general jobs.
/// </summary>
public static class Utility
{
    #region Vector Methods

    /// <summary>
    /// Converts string to Vector3.
    /// </summary>
    /// <param name="vectorString">String to convert. Must be in format (#, #, #)</param>
    /// <returns>Vector3 representation of the string.</returns>
    public static Vector3 ParseV3(string vectorString)
    {
        Vector3 vector;
        vectorString = vectorString.Substring(1, vectorString.Length-2).Replace(" ", "");
        string[] split = vectorString.Split(',');
        vector.x = float.Parse(split[0]);
        vector.y = float.Parse(split[1]);
        vector.z = float.Parse(split[2]);

        return vector;
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
        return new Matrix4x4
        {
            m00 = 1,
            m01 = 0,
            m02 = 0,
            m03 = 0,
            m10 = 0,
            m11 = CosZero(angle),
            m12 = -SinZero(angle),
            m13 = 0,
            m20 = 0,
            m21 = SinZero(angle),
            m22 = CosZero(angle),
            m23 = 0,
            m30 = 0,
            m31 = 0,
            m32 = 0,
            m33 = 1
        };
    }


    public static Matrix4x4 RotationMatrixY(float angle)
    {
        return new Matrix4x4
        {
            m00 = CosZero(angle),
            m01 = 0,
            m02 = SinZero(angle),
            m03 = 0,
            m10 = 0,
            m11 = 1,
            m12 = 0,
            m13 = 0,
            m20 = -SinZero(angle),
            m21 = 0,
            m22 = CosZero(angle),
            m23 = 0,
            m30 = 0,
            m31 = 0,
            m32 = 0,
            m33 = 1
        };
    }


    public static Matrix4x4 RotationMatrixZ(float angle)
    {
        return new Matrix4x4
        {
            m00 = CosZero(angle),
            m01 = -SinZero(angle),
            m02 = 0,
            m03 = 0,
            m10 = SinZero(angle),
            m11 = CosZero(angle),
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

    #endregion
}