// Steve Yeager
// 11.25.2013

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Player ship controller.
/// </summary>
public class Player : Ship
{
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
    public readonly KeyCode[] WeaponKeys = new KeyCode[]
    {
        KeyCode.J,
        KeyCode.K,
        KeyCode.L,
        KeyCode.Semicolon,
    };
    public float swipeNeeded = 10f;
    private bool barrelRoll;

    #endregion

    #region Const Fields

    public const int WEAPONLIMIT = 4;

    #endregion


    #region Unity Overrides

    protected override void Awake()
    {
        base.Awake();

        // setup
        tag = "Player";
        gameObject.layer = LayerMask.NameToLayer("Player");
        myScore = new ScoreManager();
        myMoney = new MoneyManager();

        // create states
        stateMachine.CreateState(MovingState, MovingEnter, MovingExit);
        stateMachine.CreateState(BarrelRollingState, BarrelRollingEnter, BarrelRollingExit);
        stateMachine.CreateState(DyingState, DyingEnter, info => { });
        stateMachine.initialState = MovingState;
    }


    protected override void Start()
    {
        base.Start();

        HUD.Initialize(this);
        HUD.Main.BarrelRollEvent += OnBarrelRoll;

        stateMachine.Start(null);
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

            // attack
            var weapons = AttackInput();
            if (weapons.Count > 0)
            {
                Attack(weapons);
            }

            // move
            myMotor.Move(MovementInput());

            yield return null;
        }
    }


    private void MovingExit(Dictionary<string, object> info)
    {
        stateMachine.SetUpdate(MovingUpdate());
    }


    private void BarrelRollingEnter(Dictionary<string, object> info)
    {
        barrelRoll = false;
        myHealth.invincible = true;
        collider.enabled = false;

        // release all attacks
        foreach (var weapon in myWeapons.weapons)
        {
            if (weapon != null)
            {
                weapon.Activate(false);
            }
        }

        stateMachine.SetUpdate(BarrelRollingUpdate(MovementInput()));
    }


    private IEnumerator BarrelRollingUpdate(Vector2 direction)
    {
        yield return StartCoroutine(myMotor.BarrelRoll(direction, horizontalBounds));
        stateMachine.SetState(MovingState, new Dictionary<string, object>());
    }


    private void BarrelRollingExit(Dictionary<string, object> info)
    {
        myHealth.invincible = false;
        collider.enabled = true;
        barrelRoll = false;
    }


    private void DyingEnter(Dictionary<string, object> info)
    {
        gameObject.SetActive(false);
        myWeapons.canActivate = false;
    }

    #endregion

    #region Input Methods

    private Vector2 MovementInput()
    {
        Vector2 input;

        #if UNITY_STANDALONE

        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        #else

        input = HUD.Main.joystick.value;

        #endif

        return input;
    }


    private List<KeyValuePair<int, bool>> AttackInput()
    {
        var weapons = new List<KeyValuePair<int, bool>>();

        #if UNITY_STANDALONE

        for (int i = 0; i < WeaponKeys.Length; i++)
        {
            if (myWeapons.CanActivate(i))
            {
                if (Input.GetKeyDown(WeaponKeys[i]))
                {
                    weapons.Add(new KeyValuePair<int, bool>(i, true));
                }
                else if (Input.GetKeyUp(WeaponKeys[i]))
                {
                    weapons.Add(new KeyValuePair<int, bool>(i, false));
                }
            }
        }

        #else



        #endif

        return weapons;
    }


    private bool BarrelRollInput()
    {
        #if UNITY_STANDALONE

        return Input.GetKeyDown(KeyCode.Space);

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

    #endregion

    #region Public Methods

    /// <summary>
    /// Recieve kill information from killed enemy.
    /// </summary>
    /// <param name="enemy">Enemy type killed.</param>
    /// <param name="points">How many base points the enemy is worth.</param>
    public void RecieveKill(Enemy.Classes enemy, int points, int money)
    {
        myScore.RecieveScore(points);
        myScore.IncreaseMultiplier();
        myMoney.Collect(money);
    }


    //
    public void Initialize(float maxHealth, float maxShield, float speed)
    {
        myHealth.Initialize(maxHealth, maxShield);
        myMotor.Initialize(speed);
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
            myWeapons.Activate(weapon.Key, weapon.Value);
        }
    }

    #endregion

    #region Event Handlers

    [Obsolete("Find a better way. Check button status?")]
    private void OnBarrelRoll(object sender, EventArgs args)
    {
        barrelRoll = true;
    }

    #endregion
}