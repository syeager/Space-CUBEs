// Little Byte Games
// Author: Steve Yeager
// Created: 2014.01.12
// Edited: 2014.10.04

using System;
using System.Collections;
using System.Collections.Generic;
using LittleByte.Data;
using UnityEngine;

namespace SpaceCUBEs
{
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

        public int maxTimeScore;
        public float minutesToBeat = 10f;

        /// <summary>Points given for winning with max health.</summary>
        public float maxHealthScore;

        #endregion

        #region Private Fields

        private int segmentCursor;
        private bool lastSegment;
        private Job spawnJob;

        private readonly LevelTime levelTime = new LevelTime();

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

        #region GA Fields

        private const string GALevelCompletedKey = "Level_Completed:";

        private const string GATime = "Time:";
        private const string GATimeTotal = "Total";
        private const string GATimeLevel = "Level";
        private const string GATimeBoss = "Boss";

        private const string GAPoints = "Points:";
        private const string GAPointsTotal = "Total";
        private const string GAPointsKills = "Kills";
        private const string GAPointsTime = "Time";
        private const string GAPointsHealth = "Health";

        private const string GARank = "Rank";

        private const string GAMoney = "Money";

        #endregion

        #region Classes

        private class LevelTime
        {
            public TimeSpan Level { get; private set; }
            public TimeSpan Boss { get; private set; }

            public TimeSpan Total
            {
                get { return Level + Boss; }
            }


            public void StartLevel()
            {
                Level = DateTime.Now.TimeOfDay;
            }


            public void StartBoss()
            {
                Level = DateTime.Now.TimeOfDay - Level;
                Debugger.Log("Level Time: " + Level, null, Debugger.LogTypes.LevelEvents);
                Boss = DateTime.Now.TimeOfDay;
            }


            public void End()
            {
                Boss = DateTime.Now.TimeOfDay - Boss;
                Debugger.Log("Bos Time: " + Boss, null, Debugger.LogTypes.LevelEvents);
            }
        }

        #endregion

        #region Unity Overrides

        protected override void Start()
        {
            base.Start();

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
                while (ActiveEnemies.Count > 0)
                {
                    if (ActiveEnemies[0] == null) continue;
                    (ActiveEnemies[0].MyHealth).Trash();
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
                if (PlayerController != null)
                {
                    PlayerController.MyHealth.Trash();
                }
            }
        }
#endif

        #endregion

        #region LevelManager Overrides

        protected override void OnBuildFinished(BuildFinishedArgs args)
        {
            base.OnBuildFinished(args);

#if DEBUG
            if (Singleton<GameSettings>.Main.jumpToBoss)
            {
                SpawnBoss();
            }
            else
            {
                SpawnNextFormation();
            }
#else
                SpawnNextFormation();
#endif

            levelTime.StartLevel();
        }


        protected override void LevelCompleted(bool won)
        {
            base.LevelCompleted(won);
            //GA.API.Design.NewEvent(GALevelCompletedKey + LevelNames[levelIndex], won ? 1 : 0);
            GoogleAnalytics.LogEvent(GALevelCompletedKey, LevelNames[levelIndex], "", won ? 1 : 0);

            // time
            levelTime.End();
            float levelSeconds = (float)levelTime.Total.TotalSeconds;
            //GA.API.Design.NewEvent(GATime + GATimeTotal, levelSeconds);
            GoogleAnalytics.LogEvent(GATime, GATimeTotal, "", (long)levelSeconds);
            //GA.API.Design.NewEvent(GATime + GATimeLevel, (float)levelTime.Level.TotalSeconds);
            GoogleAnalytics.LogEvent(GATime, GATimeLevel, "", (long)levelTime.Level.TotalSeconds);
            //GA.API.Design.NewEvent(GATime + GATimeBoss, (float)levelTime.Boss.TotalSeconds);
            GoogleAnalytics.LogEvent(GATime, GATimeBoss, "", (long)levelTime.Boss.TotalSeconds);

            // stop spawning
            StopAllCoroutines();

            // score
            int score = PlayerController.myScore.points;
            Debugger.Log("Kill Score: " + score, this, Debugger.LogTypes.Data);
            GA.API.Design.NewEvent(GAPoints + GAPointsKills, score);

            // time score
            int timeScore = TimeScore(levelSeconds);
            Debugger.Log("Time Score: " + timeScore, this, Debugger.LogTypes.Data);
            score += timeScore;
            //GA.API.Design.NewEvent(GAPoints + GAPointsTime, timeScore);
            GoogleAnalytics.LogEvent(GAPoints, GAPointsTime, "", timeScore);
            Debugger.Log("Completion Time: " + Time.timeSinceLevelLoad, this, Debugger.LogTypes.LevelEvents);
            Debugger.Log("Time Score: " + timeScore, this, Debugger.LogTypes.LevelEvents);

            // health score
            int healthScore = Mathf.RoundToInt((PlayerController.MyHealth.health / PlayerController.MyHealth.maxHealth) * maxHealthScore);
            Debugger.Log("Health Score: " + healthScore, this, Debugger.LogTypes.Data);
            score += healthScore;
            //GA.API.Design.NewEvent(GAPoints + GAPointsHealth, healthScore);
            GoogleAnalytics.LogEvent(GAPoints, GAPointsHealth, "", healthScore);

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
            //GA.API.Design.NewEvent(GARank, rank);
            GoogleAnalytics.LogEvent(GARank, "", "", rank);

            // save rank and score
            Highscore playerScore = new Highscore(rank, score, levelTime.Total);

            Highscore currentHighscore = SaveData.Load<Highscore>(HighScoreKey + LevelNames[levelIndex], LevelsFolder);
            if (score > currentHighscore.score || (score == currentHighscore.score && levelTime.Total < currentHighscore.time))
            {
                SaveData.Save(HighScoreKey + LevelNames[levelIndex], playerScore, LevelsFolder);
            }

            // money
            int money = PlayerController.myMoney.money;
            PlayerController.myMoney.Save();
            //GA.API.Design.NewEvent(GAMoney, money);
            GoogleAnalytics.LogEvent(GAMoney, "", "", money);

            // awards
            int[] awards = AwardCUBEs();
            int[] inventory = CUBE.GetInventory();
            foreach (int award in awards)
            {
                inventory[award]++;
            }
            CUBE.SetInventory(inventory);

            campaignOverview.Initialize(playerScore, rankLimits, money, awards);
            //GA.API.Design.NewEvent(GAPoints + GAPointsTotal, score);
            GoogleAnalytics.LogEvent(GAPoints, GAPointsTotal, "", score);
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

                GameObject enemyObject = Prefabs.Pop((PoolObject)enemies[formationGroups[segmentCursor].enemies[i]].GetComponent(typeof(PoolObject)));
                enemyObject.transform.position = Utility.RotateVector(formationGroups[segmentCursor].formation.positions[i], Quaternion.AngleAxis(formationGroups[segmentCursor].rotation, Vector3.back)) + formationCenter;
                enemyObject.transform.rotation = SpawnRotation;

                Enemy enemy = (Enemy)enemyObject.GetComponent(typeof(Enemy));
                enemy.stateMachine.Start(new Dictionary<string, object> {{"path", (formationGroups[segmentCursor].paths[i])}});
                ((ShieldHealth)enemy.GetComponent(typeof(ShieldHealth))).DieEvent += OnEnemyDeath;
                ActiveEnemies.Add(enemy);
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
                    spawnJob = new Job(Spawning(formationGroups[segmentCursor].spawnTime));
                }
            }
        }


        private void SpawnBoss()
        {
            boss = (Instantiate(bossPrefab, bossSpawnPosition, SpawnRotation) as GameObject).GetComponent<Boss>();
            ActiveEnemies.Add(boss);
            boss.DeathEvent += OnBossDeath;
            boss.stateMachine.Start();

            levelTime.StartBoss();

            AudioManager.CrossFadePlaylist(levelMusic, bossMusic, bossFadeTime);
        }


        private IEnumerator Spawning(float time)
        {
            yield return new WaitForSeconds(time);
            SpawnNextFormation();
        }

        #endregion

        #region EventHandlers

        private void OnEnemyDeath(object sender, DieArgs args)
        {
            ShieldHealth enemyHealth = (ShieldHealth)sender;
            enemyHealth.DieEvent -= OnEnemyDeath;
            ActiveEnemies.Remove((Enemy)enemyHealth.GetComponent(typeof(Enemy)));
            if (ActiveEnemies.Count == 0)
            {
                Log("Formation " + (segmentCursor - 1) + " cleared.", Debugger.LogTypes.LevelEvents);
                if (lastSegment)
                {
                    SpawnBoss();
                }
                else
                {
                    if (spawnJob != null)
                    {
                        spawnJob.Kill();
                        spawnJob = null;
                    }
                    SpawnNextFormation();
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


        private int TimeScore(float time)
        {
            return Mathf.RoundToInt(maxTimeScore * Mathf.Pow(Mathf.Clamp01(time / (minutesToBeat * 60f)) - 1, 2f));
        }

        #endregion
    }
}