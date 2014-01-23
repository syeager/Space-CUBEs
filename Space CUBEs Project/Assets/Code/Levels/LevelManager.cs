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

    public int[] rankLimits = new int[6];

    #endregion

    #region Protected Fields

    protected Player player;

    #endregion

    #region Private Fields

    private string build;

    #endregion

    #region Readonly Fields

    protected readonly Quaternion SPAWNROTATION = Quaternion.Euler(0f, 270f, 90f);

    #endregion

    #region Readonly Fields

    private static readonly char[] ranks = { 'F', 'D', 'C', 'B', 'A', 'S' };
    private static readonly int[] gradeChances = { 50000, 25000, 12500, 6250, 3125 };

    #endregion


    #region Events

    public EventHandler LevelFinishedEvent;

    #endregion


    #region Unity Overrides

    protected virtual void Start()
    {
        Grid = ((GameObject)Instantiate(GameResources.Main.ConstructionGrid_Prefab, Vector3.zero, Quaternion.identity)).GetComponent<ConstructionGrid>();
        #if UNITY_EDITOR

        if (GameData.Main.levelData.ContainsKey("Build"))
        {
            build = (string)GameData.Main.levelData["Build"];
        }
        else
        {
            build = "Test Build";
        }

        #else

        build = (string)GameData.Main.levelData["Build"];

        #endif

        CreatePlayer(build);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameData.LoadLevel("Main Menu", true);
        }
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
        StartCoroutine(Grid.Build(build, 10, new Vector3(-75f, 0, 0), new Vector3(0f, 90f, 270f), 2f));
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
                    grades[i] = j-1;
                    break;
                }
            }
        }

        // get award IDs
        int[] awards = new int[5];
        for (int i = 0; i < 5; i++)
        {
            awards[i] = CUBE.gradedCUBEs[grades[i]][Random.Range(0, CUBE.gradedCUBEs[grades[i]].Length-1)];
        }

        return awards;
    }

    #endregion

    #region Event Handlers

    private void OnBuildFinished(object sender, BuildFinishedArgs args)
    {
        Grid.BuildFinishedEvent -= OnBuildFinished;

        var buildShip = args.ship.AddComponent<ShipCompactor>();
        player = buildShip.Compact(typeof(Player), true) as Player;

        player.GetComponent<ShieldHealth>().Initialize(args.health, args.shield);
        player.GetComponent<ShieldHealth>().rechargeDelay = 1f;
        player.GetComponent<ShieldHealth>().rechargeSpeed = args.shield / 3f;
        player.GetComponent<ShieldHealth>().DieEvent += OnPlayerDeath;
        player.GetComponent<ShipMotor>().speed = args.speed;
        player.GetComponent<ShipMotor>().barrelRollTime = 0.25f;
        player.GetComponent<ShipMotor>().barrelRollMoveSpeed = 2f * args.speed;
    }


    private void OnPlayerDeath(object sender, DieArgs args)
    {
        LevelFinished();
    }

    #endregion
}