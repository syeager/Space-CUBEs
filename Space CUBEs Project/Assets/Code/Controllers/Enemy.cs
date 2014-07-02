// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.16
// Edited: 2014.06.25

/// <summary>
/// Base class for all enemies.
/// </summary>
public class Enemy : Ship
{
    #region References

    protected PoolObject poolObject;

    #endregion

    #region Public Fields

    public enum Classes
    {
        None,
        Grunt,
        Sentry,
        Guard,
        Hornet,
        Kamakazee,
        Bomber,
        Elites,
        SwitchBlade,
        Medic,
        Hacker,
        Twins,
        Instagator,
        Minion
    }

    public Classes enemyClass;
    public int score;
    public int money;

    #endregion

    #region Protected Fields

    protected Path path;

    #endregion

    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // references
        poolObject = (PoolObject)GetComponent(typeof(PoolObject));
    }


    protected override void Start()
    {
        base.Start();

        // register events
        MyHealth.DieEvent += OnDie;
    }

    #endregion

    #region Private Methods

    private void OnDie(object sender, DieArgs args)
    {
        Player player = args.killer as Player;
        if (player != null)
        {
            player.RecieveKill(enemyClass, score, money, MyHealth.maxHealth);
        }

        Destroy(path);
    }

    #endregion
}