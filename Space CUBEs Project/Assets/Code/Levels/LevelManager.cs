// Little Byte Games
// Author: Steve Yeager
// Created: 2013.12.03
// Edited: 2014.09.17

using System.Collections.Generic;
using Annotations;
using UnityEngine;
using Random = UnityEngine.Random;
using LittleByte;

namespace SpaceCUBEs
{
    public class LevelManager : Singleton<LevelManager>
    {
        #region References

        private ConstructionGrid grid;

        #endregion

        #region Public Fields

        /// <summary>Pause Menu.</summary>
        public GameObject pauseMenu;

        /// <summary>Playlist name for the level's background music.</summary>
        public string levelMusic;

        public GameObject[] enemyPrefabs;
        public int[] rankLimits = new int[6];

        #endregion

        #region Protected Fields

        protected Dictionary<Enemy.Classes, GameObject> enemies;

        #endregion

        #region Readonly Fields

        protected static readonly Quaternion SpawnRotation = Quaternion.Euler(0f, 270f, 90f);

        #endregion

        #region Static Fields

        private static readonly int[] GradeChances = {50000, 25000, 12500, 6250, 3125};

        #endregion

        #region Properties

        public List<Enemy> ActiveEnemies { get; protected set; }
        public Player PlayerController { get; protected set; }
        public Transform PlayerTransform { get; protected set; }

        #endregion

        #region TESTING

#if UNITY_EDITOR
        public string buildToLoad;
#endif

        #endregion

        #region MonoBehaviour Overrides

        protected override void Awake()
        {
            base.Awake();

            enemies = new Dictionary<Enemy.Classes, GameObject>();
            foreach (GameObject enemy in enemyPrefabs)
            {
                enemies.Add(((Enemy)enemy.GetComponent(typeof(Enemy))).enemyClass, enemy);
            }
        }


        protected virtual void Start()
        {
            // register events
            GameTime.PausedEvent += OnPause;

            // active enemies
            ActiveEnemies = new List<Enemy>();

            grid = ((GameObject)Instantiate(GameResources.Main.ConstructionGrid_Prefab, Vector3.zero, Quaternion.identity)).GetComponent<ConstructionGrid>();

            string build = ConstructionGrid.SelectedBuild;
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(build))
            {
                build = buildToLoad;
            }
#endif
            InvokeAction(() => CreatePlayer(build), 1f);
        }


        protected virtual void Update()
        {
            if (Input.GetButtonDown("Pause"))
            {
                GameTime.TogglePause();
            }

#if UNITY_EDITOR
            // invincible
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (PlayerController != null) PlayerController.GetComponent<ShieldHealth>().invincible = true;
            }
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                if (PlayerController != null) PlayerController.GetComponent<ShieldHealth>().invincible = false;
            }
#endif
        }


        [UsedImplicitly]
        private void OnDestroy()
        {
            GameTime.PausedEvent -= OnPause;
        }

        #endregion

        #region Static Methods

        private bool firstPause = true;

        #endregion

        #region Protected Methods

        // TODO: maybe make abstract
        protected virtual void LevelCompleted(bool won)
        {
            Log("Level Finished.", Debugger.LogTypes.LevelEvents);

            if (won)
            {
                PlayerController.MyHealth.invincible = true;
            }
        }

        #endregion

        #region Private Methods

        private void CreatePlayer(string build)
        {
            grid.Build(build, 10, new Vector3(-75f, 0, 0), new Vector3(0f, 90f, 270f), 0.5f, OnBuildFinished);
        }


        protected static int[] AwardCUBEs()
        {
            // get grades
            var grades = new int[5];
            for (int i = 0; i < 5; i++)
            {
                int rand = Random.Range(0, GradeChances[0]);
                for (int j = 0; j < 5; j++)
                {
                    if (GradeChances[j] <= rand)
                    {
                        grades[i] = j - 1;
                        break;
                    }
                }
            }

            // get award IDs
            var awards = new int[5];
            for (int i = 0; i < 5; i++)
            {
                awards[i] = CUBE.GradedCUBEs[grades[i]][Random.Range(0, CUBE.GradedCUBEs[grades[i]].Length - 1)];
            }

            return awards;
        }

        #endregion

        #region Event Handlers

        private void OnBuildFinished(BuildFinishedArgs args)
        {
            var buildShip = args.ship.AddComponent<ShipCompactor>();
            PlayerController = buildShip.Compact(true) as Player;
            PlayerTransform = PlayerController.transform;
            PlayerController.Initialize(args.health, args.shield, args.speed, args.damage);

#if DEBUG
            if (GameSettings.Main.invincible)
            {
                PlayerController.GetComponent<ShieldHealth>().invincible = true;
            }
#endif

            PlayerController.GetComponent<ShieldHealth>().DieEvent += OnPlayerDeath;
        }


        private void OnPlayerDeath(object sender, DieArgs args)
        {
            LevelCompleted(false);
        }


        protected virtual void OnPause(object sender, PauseArgs args)
        {
            pauseMenu.SetActive(args.paused);
        }

        #endregion

        #region Button Handlers

        /// <summary>
        /// Loads the Main Menu scene.
        /// </summary>
        public void LoadMainMenu()
        {
            GameTime.Pause(false);
            SceneManager.LoadScene(Scenes.Menus.MainMenu.ToString(), true);
        }


        /// <summary>
        /// Restart the current level.
        /// </summary>
        public void RestartLevel()
        {
            GameTime.Pause(false);
            SceneManager.ReloadScene();
        }


        public void TogglePause()
        {
            if (firstPause)
            {
                firstPause = false;
                return;
            }
            GameTime.TogglePause();
        }

        #endregion
    }
}