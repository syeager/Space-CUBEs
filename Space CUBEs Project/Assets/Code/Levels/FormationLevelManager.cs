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
            }
        }

        // last segment
        lastSegment = segmentCursor + 1 == formationGroups.Length;
        
        // create enemies
        Vector3 formationCenter = formationGroups[segmentCursor].position;
        for (int i = 0; i < formationGroups[segmentCursor].formation.positions.Length; i++)
        {
            if (formationGroups[segmentCursor].enemies[i] == Enemy.Classes.None) continue;

            GameObject enemyObject = PoolManager.Pop(enemies[formationGroups[segmentCursor].enemies[i]]);
            enemyObject.transform.SetPosRot(Utility.RotateVector(formationGroups[segmentCursor].formation.positions[i], Quaternion.AngleAxis(formationGroups[segmentCursor].rotation, Vector3.back)) + formationCenter, SpawnRotation);
            Enemy enemy = (Enemy)enemyObject.GetComponent(typeof(Enemy));
            enemy.stateMachine.Start(new Dictionary<string, object> { { "path", (formationGroups[segmentCursor].paths[i]) } });
            ((ShieldHealth)enemy.GetComponent(typeof(ShieldHealth))).DieEvent += OnEnemyDeath;
            activeEnemies.Add(enemy);
        }

        // increase segmentCursor
        Log("Formation " + segmentCursor + " spawned.", true, Debugger.LogTypes.LevelEvents);
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
        boss.GetComponent<ShieldHealth>().DieEvent += (s, a) => LevelFinished();
        boss.stateMachine.Start();

        audio.clip = bossMusic;
        audio.Play();
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
            Log("Formation " + (segmentCursor - 1) + " cleared.", true, Debugger.LogTypes.LevelEvents);
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

    #endregion
}