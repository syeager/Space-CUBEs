// Steve Yeager
// 1.12.2014

using System.Collections.Generic;
using UnityEngine;

public class FormationLevelManager : LevelManager
{
    #region Public Fields

    public LevelFormationSegment[] formations;

    #endregion

    #region Private Fields

    private int segmentCursor;
    private int enemiesLeft;
    private bool lastSegment;

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
        if (segmentCursor >= formations.Length)
        {
            Log("Level Complete", true, Debugger.LogTypes.LevelEvents);
            return;
        }

        // does the next segment require clearing?
        bool clear = false;
        if (segmentCursor + 1 < formations.Length)
        {
            if (formations[segmentCursor + 1].cleared)
            {
                clear = true;
                Log("Segment " + (segmentCursor + 1) + " requires clearing.", true, Debugger.LogTypes.LevelEvents);
            }
        }

        // last segment
        lastSegment = segmentCursor + 1 == formations.Length;
        
        // create enemies
        enemiesLeft = 0;
        for (int i = 0; i < formations[segmentCursor].formation.positions.Length; i++)
        {
            var enemy = PoolManager.Pop(formations[segmentCursor].enemies[i].ToString());
            enemy.transform.SetPosRot(formations[segmentCursor].formation.positions[i] + SPAWNSTART, SPAWNROTATION);
            enemy.GetComponent<Enemy>().Spawn();
            if (clear || lastSegment)
            {
                enemiesLeft++;
                enemy.GetComponent<ShieldHealth>().DieEvent += OnEnemyDeath;
            }
        }

        // increase segmentCursor
        Log("Segment " + segmentCursor + " spawned.", true, Debugger.LogTypes.LevelEvents);
        segmentCursor++;

        // get next segment ready if time
        if (segmentCursor < formations.Length && !clear)
        {
            Log("Spawn segment " + segmentCursor + " in " + formations[segmentCursor].spawnTime + " seconds.", true, Debugger.LogTypes.LevelEvents);
            InvokeAction(() => SpawnNextSegment(), formations[segmentCursor].spawnTime);
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
            if (lastSegment)
            {
                LevelFinished();
            }
            else
            {
                InvokeAction(() => SpawnNextSegment(), 3f);
            }
        }
    }

    #endregion
}