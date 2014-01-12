// Steve Yeager
// 1.10.2014

using UnityEngine;

public struct WaveEnemyData
{
    public readonly Enemy.Classes enemy;
    public readonly Vector3 position;


    public WaveEnemyData(Enemy.Classes enemy, Vector3 position)
    {
        this.enemy = enemy;
        this.position = position;
    }
}