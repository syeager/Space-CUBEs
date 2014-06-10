// GameSaveData
// Author: Steve Yeager
// Created: 2014.06.07
// Edited: 2014.06.09

using System;
using ProtoBuf;
using UnityEngine;

namespace GameSaveData
{
    public interface IUnity
    {
        object Cast();
    }

    [ProtoContract]
    [Serializable]
    public class SVector3 : IUnity
    {
        [ProtoMember(1)]
        public float x;

        [ProtoMember(2)]
        public float y;

        [ProtoMember(3)]
        public float z;


        public SVector3()
        {
            x = y = z = 0f;
        }


        public SVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }


        public static implicit operator Vector3(SVector3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }


        public static implicit operator SVector3(Vector3 v)
        {
            return new SVector3(v.x, v.y, v.z);
        }


        public object Cast()
        {
            return (Vector3)this;
        }
    }
}