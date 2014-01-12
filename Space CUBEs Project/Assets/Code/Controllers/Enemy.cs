// Steve Yeager
// 12.16.2013

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all enemies.
/// </summary>
[RequireComponent(typeof(PoolObject))]
public abstract class Enemy : Ship
{
    #region References

    protected PoolObject poolObject;

    #endregion

    #region Public Fields

    public enum Classes
    {
        Grunt,
    }
    public Classes enemyClass;
    public int score;
    public int money;

    #endregion


    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // references
        poolObject = GetComponent<PoolObject>();
    }


    protected override void Start()
    {
        base.Start();

        // register events
        myHealth.DieEvent += OnDie;

        stateMachine.Start(new Dictionary<string, object>());
    }

    #endregion

    #region Public Methods

    public void Spawn()
    {
        myHealth.Initialize();
        stateMachine.Start(new Dictionary<string, object>());
    }

    #endregion

    #region Private Methods

    private void OnDie(object sender, DieArgs args)
    {
        Player player = sender as Player;
        if (player != null)
        {
            player.RecieveKill(enemyClass, score, money);
        }
    }

    #endregion

    #region Abstract Methods

    public abstract void Spawn(Vector3 stopPosition);

    #endregion
}