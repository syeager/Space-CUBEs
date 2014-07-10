// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.01.12
// Edited: 2014.07.09

using System;
using System.Collections.Generic;
using System.Linq;
using LittleByte.Data;
using UnityEngine;

public class FormationLevelManager : LevelManager
{
    #region Public Fields

    public FormationGroup[] formationGroups;
    public int levelIndex;

    /// <summary>Playlist name for boss music.</summary>
    public string bossMusic;

    /// <summary>Time in seconds to fade to boss music.</summary>
    public float bossFadeTime = 3f;

    public CampaignOverview campaignOverview;

    #endregion

    #region Private Fields

    private int segmentCursor;
    private bool lastSegment;

    #endregion

    #region Const Fields

    public static readonly string[] LevelNames =
    {
        "The Abyss",
        "Nebula Forest",
        "Forsaken Colonies",
        "The Capital",
        "Galactic Core",
    };

    /// <summary>Data folder for highscores and unlocked levels.</summary>
    public const string LevelsFolder = @"Levels/";

    /// <summary>Data file prefix for level highscores. 0: rank, 1: score.</summary>
    public const string HighScoreKey = "HighScore ";

    #endregion

    #region Boss Fields

    public GameObject bossPrefab;
    private Boss boss;
    public Vector3 bossSpawnPosition;

    #endregion

    #region Unity Overrides

    protected override void Start()
    {
        base.Start();

#if DEBUG
        if (Singleton<GameSettings>.Main.jumpToBoss)
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

        AudioManager.PlayPlaylist(levelMusic);
    }


#if UNITY_EDITOR
    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            // kill boss
            if (boss != null)
            {
                boss.MyHealth.Trash();
                return;
            }
            // spawn boss
            segmentCursor = 100000;
            while (activeEnemies.Count > 0)
            {
                if (activeEnemies[0] == null) continue;
                (activeEnemies[0].MyHealth).Trash();
            }
            SpawnBoss();
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            // hurt boss
            if (boss != null)
            {
                boss.MyHealth.RecieveHit(null, 1000);
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            // kill player
            if (player != null)
            {
                player.MyHealth.Trash();
            }
        }
    }
#endif

    #endregion

    #region LevelManager Overrides

    protected override void LevelCompleted(bool won)
    {
        base.LevelCompleted(won);

        // stop spawning
        StopAllCoroutines();

        // score
        int score = player.myScore.points;

        // rank
        int rank = rankLimits.Length - 1;
        if (!won)
        {
            rank = 0;
        }
        else
        {
            for (int i = 0; i < rankLimits.Length; i++)
            {
                if (score < rankLimits[i])
                {
                    rank = i - 1;
                    break;
                }
            }
        }

        // save rank and score
        SaveData.Save(HighScoreKey + LevelNames[levelIndex], new int[] {rank, score});

        // money
        int money = player.myMoney.money;
        player.myMoney.Save();

        // awards
        int[] awards = AwardCUBEs();
        int[] inventory = CUBE.GetInventory();
        foreach (int award in awards)
        {
            inventory[award]++;
        }
        CUBE.SetInventory(inventory);

        campaignOverview.Initialize(score, rankLimits, rank, money, awards);
    }

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
        boss.DeathEvent += OnBossDeath;
        boss.stateMachine.Start();

        AudioManager.CrossFadePlaylist(levelMusic, bossMusic, bossFadeTime);
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


    private void OnBossDeath(object sender, EventArgs args)
    {
        if (levelIndex >= SaveData.Load<int>(LevelSelectManager.UnlockedLevelsKey, LevelsFolder))
        {
            SaveData.Save(LevelSelectManager.UnlockedLevelsKey, levelIndex + 1, LevelsFolder);
        }

        LevelCompleted(true);
    }

    #endregion
}