// Steve Yeager
// 12.16.2013

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
        Instagator
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
        poolObject = GetComponent<PoolObject>();
    }


    protected override void Start()
    {
        base.Start();

        myWeapons.Initialize(this, 1f);

        // register events
        myHealth.DieEvent += OnDie;
    }

    #endregion

    #region Private Methods

    private void OnDie(object sender, DieArgs args)
    {
        Player player = args.killer as Player;
        if (player != null)
        {
            player.RecieveKill(enemyClass, score, money);
        }

        Destroy(path);
    }

    #endregion
}