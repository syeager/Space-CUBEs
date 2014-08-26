// Little Byte Games
// Author: Steve Yeager
// Created: 2013.11.25
// Edited: 2014.08.25

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    #region State Fields

    private const string MovingState = "Moving";
    private const string BarrelRollingState = "BarrelRolling";

    #endregion

    #region Input Fields

    public float horizontalBounds = 0.05f;
    public float backBounds;
    public float frontBounds;

    private const string WeaponInput = "Weapon";
    private bool barrelRoll;
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

    #region Unity Overrides

    protected override void Awake()
    {
        base.Awake();

        // setup
        myScore = new ScoreManager();
        myMoney = new MoneyManager();
        Augmentations = GetComponent<AugmentationManager>();
        Weapons = GetComponent<WeaponManager>() ?? gameObject.AddComponent<WeaponManager>();

        // PC
#if UNITY_STANDALONE
        barrelRoll = true;
#endif

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
            if (BarrelRollInput())
            {
                stateMachine.SetState(BarrelRollingState, new Dictionary<string, object>());
                break;
            }

#if UNITY_STANDALONE
            // attack
            List<KeyValuePair<int, bool>> weapons = AttackInput();
            if (weapons.Count > 0)
            {
                Attack(weapons);
            }
#endif

            // move
            MyMotor.Move(MovementInput());

            yield return null;
        }
    }


    private void MovingExit(Dictionary<string, object> info)
    {
        stateMachine.SetUpdate(MovingUpdate());
    }


    private void BarrelRollingEnter(Dictionary<string, object> info)
    {
#if !UNITY_STANDALONE
        barrelRoll = false;
#endif
        collider.enabled = false;

        // release all attacks
        Weapons.ActivateAll(false);

        stateMachine.SetUpdate(BarrelRollingUpdate(MovementInput(), MyHealth.invincible));
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
#if !UNITY_STANDALONE
        barrelRoll = false;
#endif

        BarrelRollEvent.Fire(this, new ValueArgs(false));
    }


    private void DyingEnter(Dictionary<string, object> info)
    {
        gameObject.SetActive(false);
        Weapons.canActivate = false;
    }

    #endregion

    #region Input Methods

    private static Vector2 MovementInput()
    {
#if UNITY_STANDALONE
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
#else
        return HUD.Main.joystick.value;
#endif
    }


#if UNITY_STANDALONE

    /// <summary>
    /// Get activated weapons.
    /// </summary>
    /// <returns>List of weapons and if they were pressed or released.</returns>
    private List<KeyValuePair<int, bool>> AttackInput()
    {
        var weapons = new List<KeyValuePair<int, bool>>();

        for (int i = 0; i < Weaponlimit; i++)
        {
            if (Weapons.CanActivate(i))
            {
                string input = WeaponInput + i;

                if (Input.GetButtonDown(input))
                {
                    weapons.Add(new KeyValuePair<int, bool>(i, true));
                }
                else if (Input.GetButtonUp(input))
                {
                    weapons.Add(new KeyValuePair<int, bool>(i, false));
                }
            }
        }

        return weapons;
    }
#endif


    private bool BarrelRollInput()
    {
#if UNITY_STANDALONE

        if (!barrelRoll) return false;
        bool result = Input.GetAxis("BarrelRoll") >= 0.5f;
        if (result)
        {
            barrelRoll = false;
            StartCoroutine(BarrelRollReleased());
        }
        return result;

#else

        if (barrelRoll)
        {
            return true;
        }
        else
        {
            // get swipe input
            foreach (var touch in Input.touches)
            {
                if (touch.position.x > Screen.width / 2f)
                {
                    if (touch.deltaPosition.y >= 15f)
                    {
                        return true;
                    }
                }
            }
        }
        return false;

        #endif
    }


    private IEnumerator BarrelRollReleased()
    {
        while (Input.GetAxis("BarrelRoll") >= 0.5f) yield return null;

        barrelRoll = true;
        CancelInvoke("BarrelRollReleased");
    }

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
    public void Initialize(float maxHealth, float maxShield, float speed, float damage)
    {
        MyHealth.Initialize(maxHealth, maxShield);
        MyMotor.Initialize(speed);
        Weapons.Initialize(this, damage);
        Augmentations.Initialize(this);

        HUD.Initialize(this);
#if !UNITY_STANDALONE
        HUD.Main.BarrelRollEvent += OnBarrelRoll;
#endif

        stateMachine.Start();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="weapons"></param>
    private void Attack(List<KeyValuePair<int, bool>> weapons)
    {
        foreach (var weapon in weapons)
        {
            Weapons.Activate(weapon.Key, weapon.Value);
        }
    }

    #endregion

    #region Event Handlers

#if !UNITY_STANDALONE
    private void OnBarrelRoll(object sender, System.EventArgs args)
    {
        barrelRoll = true;
    }
#endif

    #endregion
}