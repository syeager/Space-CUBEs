// Little Byte Games
// Author: Steve Yeager
// Created: 2014.06.30
// Edited: 2014.09.08

using System.Collections;
using UnityEngine;

namespace SpaceCUBEs
{
    /// <summary>
    /// 
    /// </summary>
    public class BossHUD : Singleton<BossHUD>
    {
        #region Public Fields

        /// <summary>Boss title.</summary>
        public GameObject title;

        /// <summary>Time in seconds to show title.</summary>
        public float titleShowTime;

        /// <summary>Health bar.</summary>
        public UISprite healthBar;

        /// <summary>Notches in the health bar indicating stages.</summary>
        public GameObject[] notches = new GameObject[2];

        /// <summary>Time in seconds for the health bar to grow to max size.</summary>
        public float growTime;

        /// <summary>Time in seconds to wait before pausing time for the title.</summary>
        public float pauseDelay = 1f;

        #endregion

        #region Private Fields

        /// <summary>Cached reference to boss this HUD belongs to.</summary>
        private Boss boss;

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boss"></param>
        public Coroutine Initialize(Boss boss)
        {
            this.boss = boss;
            boss.MyHealth.HealthUpdateEvent += OnBossHealthUpdate;

            return StartCoroutine(Initializing());
        }

        #endregion

        #region Private Methods

        private IEnumerator Initializing()
        {
            yield return new WaitForSeconds(pauseDelay);
            yield return StartCoroutine(ShowTitle());
            yield return StartCoroutine(ShowHealth());
        }


        private IEnumerator ShowTitle()
        {
            title.SetActive(true);
            Time.timeScale = 0f;
            float target = Time.realtimeSinceStartup + titleShowTime;
            while (Time.realtimeSinceStartup < target)
            {
                yield return null;
            }
            Time.timeScale = 1f;
            title.SetActive(false);
        }


        private IEnumerator ShowHealth()
        {
            healthBar.gameObject.SetActive(true);
            float maxHealth = boss.MyHealth.maxHealth;
            float health = 0f;
            float growSpeed = maxHealth / growTime;
            while (health < maxHealth)
            {
                health += growSpeed * deltaTime;
                OnBossHealthUpdate(null, new HealthUpdateArgs(maxHealth, 0f, health));
                yield return null;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnBossHealthUpdate(object sender, HealthUpdateArgs args)
        {
            healthBar.fillAmount = args.health / args.max;
            for (int i = 0; i < notches.Length; i++)
            {
                notches[i].SetActive(boss.stages[i] <= args.health);
            }
        }

        #endregion
    }
}