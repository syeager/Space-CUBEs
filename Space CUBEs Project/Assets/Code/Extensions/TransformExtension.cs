// Steve Yeager
// 12.11.2013

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
}