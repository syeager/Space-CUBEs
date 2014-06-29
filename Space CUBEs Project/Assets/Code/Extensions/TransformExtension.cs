// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.12
// Edited: 2014.05.28

using UnityEngine;

public static class TransformExtension
{
    public static void SetPosRot(this Transform transform, Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }


    public static void SetPosRot(this Transform transform, float x, float y, float z, Quaternion rotation)
    {
        transform.position = new Vector3(x, y, z);
        transform.rotation = rotation;
    }


    public static void SetPosRot(this Transform transform, float x, float y, float z, float rotX, float rotY, float rotZ)
    {
        transform.position = new Vector3(x, y, z);
        transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
    }


    public static void Reset(this Transform transform, bool resetScale = true)
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        if (resetScale)
        {
            transform.localScale = Vector3.one;
        }
    }

    public static void ResetLocal(this Transform transform, bool resetScale = true)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        if (resetScale)
        {
            transform.localScale = Vector3.one;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="target"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    public static Quaternion RotateTowards(this Transform transform, Vector3 target, float speed, Vector3 up)
    {
        return Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target - transform.position, up), speed);
    }
}