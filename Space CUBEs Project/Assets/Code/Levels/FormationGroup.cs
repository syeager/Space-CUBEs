// Steve Yeager
// 1.12.2014

using UnityEngine;
using System;

[Serializable]
public class FormationGroup
{
    public Formation formation;
    public Enemy.Classes[] enemies = new Enemy.Classes[1];
    public Path[] paths = new Path[1];
    public bool needsClearing;
    public float spawnTime;
}