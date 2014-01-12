// Steve Yeager
// 12.3.2013

using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;

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

    #region Const Fields

    private char[] ranks = { 'F', 'D', 'C', 'B', 'A', 'S' };

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
            GameData.Main.LoadScene("Garage");
        }
    }

    #endregion

    #region Private Methods

    private void CreatePlayer(string build)
    {
        Grid.BuildFinishedEvent += OnBuildFinished;
        StartCoroutine(Grid.Build(build, 10, new Vector3(-75f, 0, 0), new Vector3(0f, 90f, 270f), 2f));
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
        var data = new Dictionary<string, object>();
        data.Add("Score", player.myScore.points);
        data.Add("Money", player.myMoney.money);
        char rank = ranks[ranks.Length-1];
        
        for (int i = 0; i < rankLimits.Length; i++)
        {
            if (player.myScore.points <= rankLimits[i])
            {
                rank = ranks[i-1];
                break;
            }
        }
        data.Add("Rank", rank);
        InvokeAction(() => GameData.Main.LoadScene("Level Overview", true, data), 2f);
    }

    #endregion
}