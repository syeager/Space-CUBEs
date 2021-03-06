﻿// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.08
// Edited: 2014.06.08

using System;
using System.Runtime.Serialization;
using UnityClasses;
using UnityEngine;


/// <summary>
/// Holds position, rotation, and weapon mapping for a CUBE in a construction grid build.
/// </summary>
[Serializable]
public class CUBEGridInfo : ISerializable
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


    public CUBEGridInfo(SerializationInfo info, StreamingContext context)
    {
        position = (sVector3)info.GetValue("position", typeof(sVector3));
        rotation = (sVector3)info.GetValue("rotation", typeof(sVector3));
        weaponMap = info.GetInt32("weaponMap");
        augmentationMap = info.GetInt32("augmentationMap");
        colors = (int[])info.GetValue("colors", typeof(int[]));
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("position", (sVector3)position);
        info.AddValue("rotation", (sVector3)rotation);
        info.AddValue("weaponMap", weaponMap);
        info.AddValue("augmentationMap", augmentationMap);
        info.AddValue("colors", colors);
    }
}