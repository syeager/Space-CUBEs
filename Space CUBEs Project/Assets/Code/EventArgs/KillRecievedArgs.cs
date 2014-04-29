// Steve Yeager
// 4.28.2014

using System;

/// <summary>
/// Holds info passed from killed enemy to player.
/// </summary>
public class KillRecievedArgs : EventArgs
{
    #region Readonly Fields

    public readonly Enemy.Classes enemy;
    public readonly int score;
    public readonly int money;
    public readonly float enemyHealthMax;

    #endregion


    #region Constructors

    public KillRecievedArgs(Enemy.Classes enemy, int score, int money, float enemyHealthMax)
    {
        this.enemy = enemy;
        this.score = score;
        this.money = money;
        this.enemyHealthMax = enemyHealthMax;
    }

    #endregion
}