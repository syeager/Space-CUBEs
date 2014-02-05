// Steve Yeager
// 1.1.2014

using UnityEngine;

public static class VectorExtension
{
    public static void SetX(this Vector3 v, float value)
    {
        v = new Vector3(value, v.y, v.z);
    }


    public static void SetY(this Vector3 v, float value)
    {
        v = new Vector3(v.x, value, v.z);
    }


    public static void SetZ(this Vector3 v, float value)
    {
        v = new Vector3(v.x, v.y, value);
    }


    public static Vector3 Round(this Vector3 v)
    {
        return new Vector3(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
    }
   
}