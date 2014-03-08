// Steve Yeager
// 1.26.2014

using UnityEngine;
using System;

//
public class Path : ScriptableObject
{
    protected Transform myTransform;


    public virtual Vector3 Direction(float deltaTime) { return Vector3.zero; }
    
    public virtual void Initialize(Transform transform)
    {
        myTransform = transform;
    }
}