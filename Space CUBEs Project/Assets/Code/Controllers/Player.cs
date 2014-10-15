// Little Byte Games
// Author: Steve Yeager
// Created: 2013.11.25
// Edited: 2014.10.05

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Annotations;
using LittleByte.Extensions;
using UnityEngine;

namespace SpaceCUBEs
{
    /// <summary>
    /// Player ship controller.
    /// </summary>
    public class Player : Ship
    {
        #region References

        public GameObject trailRenderer;
        public AugmentationManager Augmentations { get; private set; }
        public WeaponManager Weapons { get; private set; }

        #endregion

        #region Public Fields

        public ScoreManager myScore;
        public MoneyManager myMoney;

        #endregion

        #region Private Fields

        [SerializeField, UsedImplicitly]
        private ShipStats statMultipliers;

        #endregion

        #region State Fields

        private const string MovingState = "Moving";
        private const string BarrelRollingState = "BarrelRolling";

        #endregion

        #region Input Fields

        private IInputController input;

        public float horizontalBounds = 0.05f;
        public float backBounds;
        public float frontBounds;

        public float swipeNeeded = 10f;

        #endregion

        #region Const Fields

        public const int Weaponlimit = 4;
        public const string Tag = "Player";

        #endregion

        #region Events

        public EventHandler<KillRecievedArgs> KillRecievedEvent;
        public EventHandler<ValueArgs> BarrelRollEvent;

        #endregion

        #region GA Fields

        private const string GAAbilities = "Abilities:";
        private const string GAWeaponCount = "Weapon_Count";
        private const string GAAugCount = "Aug_Count:";

        #endregion

        #region Unity Overrides

        protected override void Awake()
        {
            base.Awake();

            // input
#if UNITY_EDITOR
            input = gameObject.AddComponent<DeviceInput>();
#else
            if (Input.GetJoystickNames().Length > 0 || !Input.multiTouchEnabled)
            {
                input = gameObject.AddComponent<DeviceInput>();
            }
            else
            {
                input = gameObject.AddComponent<TouchInput>();
            }
#endif

            // setup
            myScore = new ScoreManager();
            myMoney = new MoneyManager();
            Augmentations = GetComponent<AugmentationManager>();
            Weapons = GetComponent<WeaponManager>() ?? gameObject.AddComponent<WeaponManager>();

            // effects
            if (GameSettings.Main.qualityLevel == 0)
            {
                Destroy(trailRenderer);
            }

            // create states
            stateMachine = new StateMachine(this, MovingState);
            stateMachine.CreateState(MovingState, MovingEnter, MovingExit);
            stateMachine.CreateState(BarrelRollingState, BarrelRollingEnter, BarrelRollingExit);
            stateMachine.CreateState(DyingState, DyingEnter, info => { });
        }

        #endregion

        #region State Methods

        private void MovingEnter(Dictionary<string, object> info)
        {
            stateMachine.SetUpdate(MovingUpdate());
        }


        private IEnumerator MovingUpdate()
        {
            while (true)
            {
                // roll
                if (MyMotor.CanBarrelRoll && input.BarrelRoll())
                {
                    stateMachine.SetState(BarrelRollingState, new Dictionary<string, object>());
                    break;
                }

                // attack
                Attack();

                // move
                MyMotor.Move(input.Joystick());

                yield return null;
            }
        }


        private void MovingExit(Dictionary<string, object> info)
        {
            stateMachine.SetUpdate(MovingUpdate());
        }


        private void BarrelRollingEnter(Dictionary<string, object> info)
        {
            collider.enabled = false;

            // release all attacks
            Weapons.ActivateAll(false);

            stateMachine.SetUpdate(BarrelRollingUpdate(input.Joystick(), MyHealth.invincible));
            MyHealth.invincible = true;

            BarrelRollEvent.Fire(this, new ValueArgs(true));
        }


        private IEnumerator BarrelRollingUpdate(Vector2 direction, bool invincible)
        {
            yield return StartCoroutine(MyMotor.BarrelRoll(direction, horizontalBounds));
            stateMachine.SetState(MovingState, new Dictionary<string, object> {{"invincible", invincible}});
        }


        private void BarrelRollingExit(Dictionary<string, object> info)
        {
            MyHealth.invincible = (bool)info["invincible"];
            collider.enabled = true;

            BarrelRollEvent.Fire(this, new ValueArgs(false));
        }


        private void DyingEnter(Dictionary<string, object> info)
        {
            gameObject.SetActive(false);
            Weapons.canActivate = false;
        }

        #endregion

        #region Input Methods

        #endregion

        #region Public Methods

        /// <summary>
        /// Recieve kill information from killed enemy.
        /// </summary>
        /// <param name="enemy">Enemy type killed.</param>
        /// <param name="points">How many base points the enemy is worth.</param>
        /// <param name="money">How much money the enemy is worth.</param>
        /// <param name="enemyHealthMax"></param>
        public void RecieveKill(Enemy.Classes enemy, int points, int money, float enemyHealthMax)
        {
            myScore.RecieveScore(points);
            myScore.IncreaseMultiplier();
            myMoney.Collect(money);

            KillRecievedEvent.Fire(this, new KillRecievedArgs(enemy, points, money, enemyHealthMax));
        }


        /// <summary>
        /// Initialize ship components. Health, Motor, and Weapons.
        /// </summary>
        public void Initialize(ShipStats stats)
        {
            // TODO: multiplier for all stats

            MyHealth.Initialize(stats.health * statMultipliers.health, stats.shield * statMultipliers.shield);
            MyMotor.Initialize(stats.speed * statMultipliers.speed);
            Weapons.Initialize(this, stats.damage * statMultipliers.damage);
            //GA.API.Design.NewEvent(GAAbilities + GAWeaponCount, Weapons.weapons.Count(w => w != null));
            GoogleAnalytics.LogEvent(GAAbilities, GAWeaponCount, "", Weapons.weapons.Count(w => w != null));
            Augmentations.Initialize(this);
            //GA.API.Design.NewEvent(GAAbilities + GAAugCount, Augmentations.augmentations.Count(a => a != null));
            GoogleAnalytics.LogEvent(GAAbilities, GAAugCount, "", Augmentations.augmentations.Count(a => a != null));

            HUD.Initialize(this);

            stateMachine.Start();
        }

        #endregion

        #region Private Methods

        private void Attack()
        {
            ButtonStates[] weaponStates = input.Weapons();
            for (int i = 0; i < Weaponlimit; i++)
            {
                if (weaponStates[i] == ButtonStates.Pressed)
                {
                    Weapons.TryActivate(i, true);
                }
                else if (weaponStates[i] == ButtonStates.Released)
                {
                    Weapons.TryActivate(i, false);
                }
            }
        }

        #endregion
    }
}