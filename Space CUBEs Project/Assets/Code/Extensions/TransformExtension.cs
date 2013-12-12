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
}