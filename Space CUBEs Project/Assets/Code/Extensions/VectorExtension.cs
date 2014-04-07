// Steve Yeager
// 1.1.2014

using UnityEngine;

public static class VectorExtension
{
    public static Vector3 Round(this Vector3 v)
    {
        return new Vector3(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
    }


    public static float Area(this Vector3 v)
    {
        return v.x*v.y*v.z;
    }
}