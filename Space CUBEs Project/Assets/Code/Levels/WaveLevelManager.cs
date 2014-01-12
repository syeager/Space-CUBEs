// Steve Yeager
// 12.17.2013

using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Creates random waves based on points.
/// </summary>
public class WaveLevelManager : LevelManager
{
    #region Public Fields

    public int startingPoints;
    public float wavePointMultiplier;

    public float minDelay;
    public float maxDelay;
    public float screenBufferPer;
    public float HUDHeight;
    public float xStart = 50f;

    #endregion

    #region Private Fields

    private int currentWave;
    private float screenTop;
    private float screenBottom;
    private WaveEnemyData[][] waves;
    private int enemiesleft;

    #endregion

    #region Readonly Fields

    private readonly Vector3 SPAWNDISTANCE = new Vector3(50f, 0f, 0f);
    private readonly Quaternion SPAWNROTATION = Quaternion.Euler(0f, 270f, 90f);

    #endregion

    #region Properties

    private int wavePoints { get { return Mathf.CeilToInt(startingPoints * currentWave * wavePointMultiplier); } }

    #endregion

    #region Events

    public EventHandler<WaveUpdateArgs> WaveIncreasedEvent;

    #endregion


    #region MonoBehaviour Overrides

    protected override void Start()
    {
        base.Start();
        
        UpdateScreen();
        GetWaves();
        InvokeAction(() => SpawnWave(), 3f);
    }

    #endregion

    #region Private Methods

    private void UpdateScreen()
    {
        float bottom = Screen.height*screenBufferPer;
        float height = Screen.height * HUDHeight;
        screenBottom = Camera.main.ScreenToWorldPoint(new Vector3(0f, bottom+height, 0f)).y;
        screenTop = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height - bottom, 0f)).y;
    }


    private IEnumerator SpawnCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(minDelay, maxDelay));

            var enemy = PoolManager.Pop("Grunt", 13f);
            enemy.transform.SetPosRot(xStart, UnityEngine.Random.Range(screenBottom, screenTop), 0f, 0f, 270f, 90f);
            enemy.GetComponent<Enemy>().Spawn();
        }
    }

    
    private void SpawnWave()
    {
        currentWave++;
        Log("Spawning Wave: " + currentWave, true, Debugger.LogTypes.LevelEvents);
        if (WaveIncreasedEvent != null)
        {
            WaveIncreasedEvent(this, new WaveUpdateArgs(currentWave));
        }

        enemiesleft = waves[currentWave - 1].Length;
        foreach (var enemyData in waves[currentWave-1])
        {
            var enemy = PoolManager.Pop(enemyData.enemy.ToString());
            enemy.transform.SetPosRot(enemyData.position + SPAWNDISTANCE, SPAWNROTATION);
            enemy.GetComponent<ShieldHealth>().DieEvent += OnEnemyDeath;
        }
    }


    private void GetWaves()
    {
        waves = WaveStream.Read(Application.loadedLevelName);
    }


    private void OnEnemyDeath(object sender, DieArgs args)
    {
        ((ShieldHealth)sender).DieEvent -= OnEnemyDeath;
        enemiesleft--;
        if (enemiesleft == 0)
        {
            InvokeAction(() => SpawnWave(), 3f);
        }
    }

    #endregion
}