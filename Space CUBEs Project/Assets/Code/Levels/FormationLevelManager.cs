// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.01.12
// Edited: 2014.06.13

using System.Collections.Generic;
using LittleByte.Data;
using LittleByte.Pools;
using UnityEngine;

public class FormationLevelManager : LevelManager
{
    #region Public Fields

    public FormationGroup[] formationGroups;
    public int levelIndex;

    #endregion

    #region Private Fields

    private int segmentCursor;
    private bool lastSegment;

    #endregion

    #region Boss Fields

    public GameObject bossPrefab;
    private Boss boss;
    public Vector3 bossSpawnPosition;

    /// <summary>Music for boss battle.</summary>
    public AudioClip bossMusic;

    #endregion

    #region Unity Overrides

    protected override void Start()
    {
        base.Start();

#if DEBUG
        if (GameSettings.Main.jumpToBoss)
        {
            SpawnBoss();
        }
        else
        {
            InvokeAction(SpawnNextFormation, 3f);
        }
#else
        InvokeAction(() => SpawnNextFormation(), 3f);
#endif
    }


#if UNITY_EDITOR
    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            if (boss != null)
            {
                boss.GetComponent<Health>().Trash();
                return;
            }
            segmentCursor = 100000;
            while (activeEnemies.Count > 0)
            {
                if (activeEnemies[0] == null) continue;
                ((Health)activeEnemies[0].GetComponent(typeof(Health))).Trash();
            }
            SpawnBoss();
        }
    }
#endif

    #endregion

    #region Private Methods

    private void SpawnNextFormation()
    {
        // completed
        if (segmentCursor >= formationGroups.Length)
        {
            Log("Level Complete", Debugger.LogTypes.LevelEvents);
            return;
        }

        // does the next segment require clearing?
        bool clear = false;
        if (segmentCursor + 1 < formationGroups.Length)
        {
            if (formationGroups[segmentCursor + 1].needsClearing)
            {
                clear = true;
            }
        }

        // last segment
        lastSegment = segmentCursor + 1 == formationGroups.Length;

        // create enemies
        Vector3 formationCenter = formationGroups[segmentCursor].position;
        for (int i = 0; i < formationGroups[segmentCursor].formation.positions.Length; i++)
        {
            if (formationGroups[segmentCursor].enemies[i] == Enemy.Classes.None) continue;

            GameObject enemyObject = Prefabs.Pop(enemies[formationGroups[segmentCursor].enemies[i]].GetComponent<PoolObject>());
            enemyObject.transform.SetPosRot(Utility.RotateVector(formationGroups[segmentCursor].formation.positions[i], Quaternion.AngleAxis(formationGroups[segmentCursor].rotation, Vector3.back)) + formationCenter, SpawnRotation);
            Enemy enemy = (Enemy)enemyObject.GetComponent(typeof(Enemy));
            enemy.stateMachine.Start(new Dictionary<string, object> {{"path", (formationGroups[segmentCursor].paths[i])}});
            ((ShieldHealth)enemy.GetComponent(typeof(ShieldHealth))).DieEvent += OnEnemyDeath;
            activeEnemies.Add(enemy);
        }

        // increase segmentCursor
        Log("Formation " + segmentCursor + " spawned.", Debugger.LogTypes.LevelEvents);
        segmentCursor++;

        // get next segment ready if time
        if (segmentCursor < formationGroups.Length && !clear)
        {
            if (formationGroups[segmentCursor].spawnTime == 0)
            {
                SpawnNextFormation();
            }
            else
            {
                InvokeAction(SpawnNextFormation, formationGroups[segmentCursor].spawnTime);
            }
        }
    }


    private void SpawnBoss()
    {
        boss = (Instantiate(bossPrefab, bossSpawnPosition, SpawnRotation) as GameObject).GetComponent<Boss>();
        activeEnemies.Add(boss);
        boss.GetComponent<ShieldHealth>().DieEvent += OnBossDeath;
        boss.stateMachine.Start();

        //AudioManager.Pl
    }

    #endregion

    #region EventHandlers

    private void OnEnemyDeath(object sender, DieArgs args)
    {
        ShieldHealth enemyHealth = sender as ShieldHealth;
        enemyHealth.DieEvent -= OnEnemyDeath;
        activeEnemies.Remove((Enemy)enemyHealth.GetComponent(typeof(Enemy)));
        if (activeEnemies.Count == 0)
        {
            Log("Formation " + (segmentCursor - 1) + " cleared.", Debugger.LogTypes.LevelEvents);
            if (lastSegment)
            {
                SpawnBoss();
            }
            else
            {
                InvokeAction(SpawnNextFormation, 3f);
            }
        }
    }


    private void OnBossDeath(object sender, DieArgs args)
    {
        if (levelIndex >= SaveData.Load<int>(LevelSelectManager.UnlockedLevelsKey, LevelSelectManager.LevelsFolder))
        {
            SaveData.Save(LevelSelectManager.UnlockedLevelsKey, levelIndex + 1, LevelSelectManager.LevelsFolder);
        }

        LevelFinished();
    }

    #endregion
}