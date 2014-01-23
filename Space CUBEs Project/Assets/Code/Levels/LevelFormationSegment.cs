// Steve Yeager
// 1.12.2014

using UnityEngine;
using System;

[Serializable]
public class LevelFormationSegment
{
    public Formation formation;
    public Enemy.Classes[] enemies;
    public bool cleared;
    public float spawnTime;
}