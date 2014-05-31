// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.11.30
// Edited: 2014.05.31

using UnityEngine;

/// <summary>
/// Holds position, rotation, and weapon mapping for a CUBE in a construction grid build.
/// </summary>
public class CUBEGridInfo
{
    public Vector3 position;
    public Vector3 rotation;
    public int weaponMap;
    public int augmentationMap;
    public int[] colors;


    public CUBEGridInfo(Vector3 position, Vector3 rotation, int weaponMap, int augmentationMap, int[] colors = null)
    {
        this.position = position;
        this.rotation = rotation;
        this.weaponMap = weaponMap;
        this.augmentationMap = augmentationMap;
        this.colors = colors;
    }
}