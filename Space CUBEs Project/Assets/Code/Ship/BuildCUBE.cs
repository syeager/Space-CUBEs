﻿// Little Byte Games
// Author: Steve Yeager
// Created: 2013.12.04
// Edited: 2014.10.02

using UnityEngine;

/// <summary>
/// 
/// </summary>
public class BuildCUBE
{
    private readonly Transform transform;
    private readonly Vector3 localTarget;
    private float time;
    private readonly float speed;
    public Vector3 vector;
    private bool done;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="localTarget"></param>
    /// <param name="speed"></param>
    public BuildCUBE(Transform transform, Vector3 localTarget, float speed)
    {
        this.transform = transform;
        this.localTarget = localTarget;
        this.speed = speed;

        vector = localTarget - transform.localPosition;
        time = vector.magnitude / speed;
        vector.Normalize();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="deltaTime"></param>
    public void Update(float deltaTime)
    {
        if (done) return;

        time -= deltaTime;
        if (time <= 0f)
        {
            done = true;
            transform.localPosition = localTarget;
        }
        else
        {
            transform.localPosition += vector * speed * deltaTime;
        }
    }
}