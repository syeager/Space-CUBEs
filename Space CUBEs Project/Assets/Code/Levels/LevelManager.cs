﻿// Steve Yeager
// 12.3.2013

using System.Diagnostics;
using UnityEngine;

public class LevelManager : MonoBase
{
    #region References

    private ConstructionGrid Grid;

    #endregion

    #region Private Fields

    private string build;

    #endregion


    #region Unity Overrides

    protected virtual void Start()
    {
        Grid = ((GameObject)Instantiate(GameResources.Main.ConstructionGrid_Prefab, Vector3.zero, Quaternion.identity)).GetComponent<ConstructionGrid>();
        build = (string)GameData.Main.levelData;
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
        //Stopwatch watch = new Stopwatch();
        //watch.Start();
        var player = buildShip.Compact(typeof(Player), true);
        //UnityEngine.Debug.Log("Build: " + watch.ElapsedMilliseconds);

        player.GetComponent<ShieldHealth>().Initialize(args.health, args.shield);
        player.GetComponent<ShieldHealth>().rechargeDelay = 1f;
        player.GetComponent<ShieldHealth>().rechargeSpeed = args.shield / 3f;
        player.GetComponent<ShipMotor>().speed = args.speed;
        player.GetComponent<ShipMotor>().barrelRollTime = 0.25f;
        player.GetComponent<ShipMotor>().barrelRollMoveSpeed = 2f * args.speed;
    }


    private void OnDie(object sender, DieArgs args)
    {

    }

    #endregion
}