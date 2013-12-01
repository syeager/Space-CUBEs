// Steve Yeager
// 11.30.2013

using UnityEngine;

public struct CUBEGridInfo
{
    public readonly Vector3 position;
    public readonly Vector3 rotation;


    public CUBEGridInfo(Vector3 position, Vector3 rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}