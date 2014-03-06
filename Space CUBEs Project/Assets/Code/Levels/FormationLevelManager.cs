// Steve Yeager
// 1.12.2014

using System.Collections.Generic;
using UnityEngine;

public class FormationLevelManager : LevelManager
{
    #region Public Fields

    public FormationGroup[] formationGroups;

    #endregion

    #region Private Fields

    private int segmentCursor;
    private bool lastSegment;

    #endregion


    #region Unity Overrides

    protected override void Start()
    {
        base.Start();

        InvokeAction(() => SpawnNextFormation(), 3f);
    }

    #endregion

    #region Private Methods

    private void SpawnNextFormation()
    {
        // completed
        if (segmentCursor >= formationGroups.Length)
        {
            Log("Level Complete", true, Debugger.LogTypes.LevelEvents);
            return;
        }

        // does the next segment require clearing?
        bool clear = false;
        if (segmentCursor + 1 < formationGroups.Length)
        {
            if (formationGroups[segmentCursor + 1].needsClearing)
            {
                clear = true;
                Log("Formation " + (segmentCursor + 1) + " requires clearing.", true, Debugger.LogTypes.LevelEvents);
            }
        }

        // last segment
        lastSegment = segmentCursor + 1 == formationGroups.Length;
        
        // create enemies
        Vector3 formationCenter = formationGroups[segmentCursor].position;
        for (int i = 0; i < formationGroups[segmentCursor].formation.positions.Length; i++)
        {
            if (formationGroups[segmentCursor].enemies[i] == Enemy.Classes.None) continue;

            GameObject enemyGO = PoolManager.Pop(formationGroups[segmentCursor].enemies[i].ToString());
            enemyGO.transform.SetPosRot(formationGroups[segmentCursor].formation.positions[i] + formationCenter, SPAWNROTATION);
            Enemy enemy = enemyGO.GetComponent<Enemy>();
            enemy.GetComponent<Enemy>().Spawn(formationGroups[segmentCursor].paths[i]);
            enemy.GetComponent<ShieldHealth>().DieEvent += OnEnemyDeath;
            activeEnemies.Add(enemy);
        }

        // increase segmentCursor
        Log("Formation " + segmentCursor + " spawned.", true, Debugger.LogTypes.LevelEvents);
        segmentCursor++;

        // get next segment ready if time
        if (segmentCursor < formationGroups.Length && !clear)
        {
            Log("Spawn formation " + segmentCursor + " in " + formationGroups[segmentCursor].spawnTime + " seconds.", true, Debugger.LogTypes.LevelEvents);
            InvokeAction(() => SpawnNextFormation(), formationGroups[segmentCursor].spawnTime);
        }
    }

    #endregion

    #region EventHandlers

    private void OnEnemyDeath(object sender, DieArgs args)
    {
        ShieldHealth enemyHealth = sender as ShieldHealth;
        enemyHealth.DieEvent -= OnEnemyDeath;
        activeEnemies.Remove(enemyHealth.GetComponent<Enemy>());

        if (activeEnemies.Count == 0)
        {
            Log("Formation " + (segmentCursor - 1) + " cleared.", true, Debugger.LogTypes.LevelEvents);
            if (lastSegment)
            {
                LevelFinished();
            }
            else
            {
                InvokeAction(() => SpawnNextFormation(), 3f);
            }
        }
    }

    #endregion
}