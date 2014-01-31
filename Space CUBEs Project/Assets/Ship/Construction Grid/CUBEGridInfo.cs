// Steve Yeager
// 11.30.2013

using UnityEngine;

/// <summary>
/// Holds position, rotation, and weapon mapping for a CUBE in a construction grid build.
/// </summary>
public class CUBEGridInfo
{
    public Vector3 position;
    public Vector3 rotation;
    public int weaponMap;


    public CUBEGridInfo(Vector3 position, Vector3 rotation, int weaponMap)
    {
        this.position = position;
        this.rotation = rotation;
        this.weaponMap = weaponMap;
    }
}