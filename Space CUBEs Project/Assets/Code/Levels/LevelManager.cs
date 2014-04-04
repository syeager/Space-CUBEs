// Steve Yeager
// 12.3.2013

using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class LevelManager : Singleton<LevelManager>
{
    #region References

    private ConstructionGrid Grid;

    #endregion

    #region Public Fields

    public GameObject[] enemyPrefabs;
    public int[] rankLimits = new int[6];

    #endregion

    #region Protected Fields

    protected Dictionary<Enemy.Classes, GameObject> enemies;

    #endregion

    #region Private Fields

    private string build;

    #endregion

    #region Readonly Fields

    protected readonly Quaternion SPAWNROTATION = Quaternion.Euler(0f, 270f, 90f);

    #endregion

    #region Static Fields

    private static readonly char[] ranks = { 'F', 'D', 'C', 'B', 'A', 'S' };
    private static readonly int[] gradeChances = { 50000, 25000, 12500, 6250, 3125 };

    #endregion

    #region Test Fields

    public string testBuild;

    #endregion

    #region Properties

    public List<Enemy> activeEnemies { get; protected set; }
    public Player player { get; protected set; }
    public Transform playerTransform { get; protected set; }

    #endregion

    #region Events

    public EventHandler LevelFinishedEvent;

    #endregion


    #region Unity Overrides

    protected override void Awake()
    {
        base.Awake();

        enemies = new Dictionary<Enemy.Classes, GameObject>();
        foreach (var enemy in enemyPrefabs)
        {
            enemies.Add(enemy.GetComponent<Enemy>().enemyClass, enemy);
        }
    }


    protected virtual void Start()
    {
        // active enemies
        activeEnemies = new List<Enemy>();

        Grid = ((GameObject)Instantiate(GameResources.Main.ConstructionGrid_Prefab, Vector3.zero, Quaternion.identity)).GetComponent<ConstructionGrid>();
#if UNITY_EDITOR
        if (GameData.Main.levelData.ContainsKey("Build"))
        {
            build = (string)GameData.Main.levelData["Build"];
        }
        else
        {
            build = testBuild;
        }
#else
        build = (string)GameData.Main.levelData["Build"];
#endif

        InvokeAction(() => CreatePlayer(build), 1f);
    }


    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameData.LoadLevel("Main Menu", true);
        }

#if UNITY_EDITOR
        // invincible
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            player.GetComponent<ShieldHealth>().invincible = true;
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            player.GetComponent<ShieldHealth>().invincible = false;
        }

        // time controls
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Time.timeScale += 1f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Time.timeScale = Mathf.Clamp(Time.timeScale - 0.5f * Time.deltaTime, 0f, 5f);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Time.timeScale = 1f;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            UnityEditor.EditorApplication.isPaused = true;
        }
#endif
    }

    #endregion

    #region Protected Methods

    protected void LevelFinished()
    {
        Log("Level Finished.", true, Debugger.LogTypes.LevelEvents);

        var data = new Dictionary<string, object>();
        // score
        data.Add("Score", player.myScore.points);
        // save score
        // money
        data.Add("Money", player.myMoney.money);
        MoneyManager.Transaction(player.myMoney.money);
        // awards
        int[] awards = AwardCUBEs();
        data.Add("Awards", awards);
        int[] inventory = CUBE.GetInventory();
        foreach (var award in awards)
        {
            inventory[award]++;
        }
        CUBE.SetInventory(inventory);
        // level rank
        char rank = ranks[ranks.Length - 1];

        for (int i = 0; i < rankLimits.Length; i++)
        {
            if (player.myScore.points <= rankLimits[i])
            {
                rank = ranks[i - 1];
                break;
            }
        }
        data.Add("Rank", rank);
        InvokeAction(() => GameData.LoadLevel("Level Overview", true, data), 2f);

        if (LevelFinishedEvent != null)
        {
            LevelFinishedEvent(this, EventArgs.Empty);
        }
    }

    #endregion

    #region Private Methods

    private void CreatePlayer(string build)
    {
        Grid.BuildFinishedEvent += OnBuildFinished;
        StartCoroutine(Grid.Build(build, 10, new Vector3(-75f, 0, 0), new Vector3(0f, 90f, 270f), 0.5f));
    }


    private int[] AwardCUBEs()
    {
        // get grades
        int[] grades = new int[5];
        for (int i = 0; i < 5; i++)
        {
            int rand = Random.Range(0, gradeChances[0]);
            for (int j = 0; j < 5; j++)
            {
                if (gradeChances[j] <= rand)
                {
                    grades[i] = j - 1;
                    break;
                }
            }
        }

        // get award IDs
        int[] awards = new int[5];
        for (int i = 0; i < 5; i++)
        {
            awards[i] = CUBE.gradedCUBEs[grades[i]][Random.Range(0, CUBE.gradedCUBEs[grades[i]].Length - 1)];
        }

        return awards;
    }

    #endregion

    #region Event Handlers

    private void OnBuildFinished(object sender, BuildFinishedArgs args)
    {
        Grid.BuildFinishedEvent -= OnBuildFinished;

        var buildShip = args.ship.AddComponent<ShipCompactor>();
        player = buildShip.Compact(typeof(Player), true, true) as Player;
        playerTransform = player.transform;
        player.Initialize(args.health, args.shield, args.speed, args.damage);

        if (GameSettings.Main.invincible)
        {
            player.GetComponent<ShieldHealth>().invincible = true;
        }
        else
        {
            player.GetComponent<ShieldHealth>().DieEvent += OnPlayerDeath;
        }
    }


    private void OnPlayerDeath(object sender, DieArgs args)
    {
        LevelFinished();
    }

    #endregion
}