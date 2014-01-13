// Steve Yeager
// 1.12.2014

using System.Collections.Generic;
using UnityEngine;

public class PatternLevelManager : LevelManager
{
    #region Public Fields

    public LevelPatternSegment[] segments;

    #endregion

    #region Private Fields

    private int segmentCursor;
    private int enemiesLeft;

    #endregion

    #region Readonly Fields

    private readonly Vector3 SPAWNSTART = Vector3.right * 100f;

    #endregion


    #region Unity Overrides

    protected override void Start()
    {
        base.Start();

        InvokeAction(() => SpawnNextSegment(), 3f);
    }

    #endregion

    #region Private Methods

    private void SpawnNextSegment()
    {
        // completed
        if (segmentCursor >= segments.Length)
        {
            Log("Level Complete", true, Debugger.LogTypes.LevelEvents);
            return;
        }

        // does the next segment require clearing?
        bool clear = false;
        if (segmentCursor + 1 < segments.Length)
        {
            if (segments[segmentCursor + 1].cleared)
            {
                clear = true;
                Log("Segment " + (segmentCursor + 1) + " requires clearing.", true, Debugger.LogTypes.LevelEvents);
            }
        }

        for (int i = 0; i < segments[segmentCursor].pattern.positions.Length; i++)
        {
            var enemy = PoolManager.Pop(segments[segmentCursor].enemies[i].ToString());
            enemy.transform.SetPosRot(segments[segmentCursor].pattern.positions[i] + SPAWNSTART, SPAWNROTATION);
            enemy.GetComponent<Enemy>().Spawn();
            if (clear)
            {
                enemiesLeft++;
                enemy.GetComponent<ShieldHealth>().DieEvent += OnEnemyDeath;
            }
        }

        Log("Segment " + segmentCursor + " spawned.", true, Debugger.LogTypes.LevelEvents);
        segmentCursor++;

        // get next segment ready if time
        if (segmentCursor < segments.Length && !clear)
        {
            Log("Spawn segment " + segmentCursor + " in " + segments[segmentCursor].spawnTime + " seconds.", true, Debugger.LogTypes.LevelEvents);
            InvokeAction(() => SpawnNextSegment(), segments[segmentCursor].spawnTime);
        }
    }

    #endregion

    #region EventHandlers

    private void OnEnemyDeath(object sender, DieArgs args)
    {
        ((ShieldHealth)sender).DieEvent -= OnEnemyDeath;
        enemiesLeft--;
        if (enemiesLeft == 0)
        {
            Log("Segment " + (segmentCursor - 1) + " cleared.", true, Debugger.LogTypes.LevelEvents);
            InvokeAction(() => SpawnNextSegment(), 3f);
        }
    }

    #endregion
}