// Little Byte Games
// Author: Steve Yeager
// Created: 2014.01.12
// Edited: 2014.10.12

using System;
using SpaceCUBEs;
using UnityEngine;

[Serializable]
public class FormationGroup
{
    public Formation formation;
    public Vector3 position;
    public float rotation;
    public Enemy.Classes[] enemies = new Enemy.Classes[1];
    public Path[] paths = new Path[1];
    public bool needsClearing;
    public float spawnTime;
}