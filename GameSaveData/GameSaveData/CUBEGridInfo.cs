// GameSaveData
// Author: Steve Yeager
// Created: 2014.06.07
// Edited: 2014.06.07

using System;
using ProtoBuf;
using UnityEngine;

namespace GameSaveData
{
    /// <summary>
    /// Holds position, rotation, and weapon mapping for a CUBE in a construction grid build.
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class CUBEGridInfo
    {
        [ProtoMember(1)]
        public SVector3 position;

        [ProtoMember(2)]
        public SVector3 rotation;

        [ProtoMember(3)]
        public int weaponMap;

        [ProtoMember(4)]
        public int augmentationMap;

        [ProtoMember(5)]
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
}