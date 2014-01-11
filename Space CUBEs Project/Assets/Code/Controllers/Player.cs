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

        #if UNITY_ANDROID

        HUD.Initialize(this);
        HUD.Main.BarrelRollEvent += OnBarrelRoll;
        myWeapons.RegisterToHUD();

        #endif

        stateMachine.Start(null);
    }

    #endregion

    #region State Methods

    private void MovingEnter(Dictionary<string, object> info)
    {
        stateMachine.update = new Job(MovingUpdate());
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
        stateMachine.update = new Job(MovingUpdate());
    }


    private void BarrelRollingEnter(Dictionary<string, object> info)
    {
        barrelRoll = false;
        myHealth.invincible = true;
        collider.enabled = false;
        stateMachine.update = new Job(BarrelRollingUpdate(MovementInput()));
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
    }


    private void DyingEnter(Dictionary<string, object> info)
    {
        gameObject.SetActive(false);
    }

    #endregion

    #region Input Methods

    [Obsolete("Use MovementInput")]
    private float HorizontalInput()
    {
        float input = 0f;
        float ypos = Camera.main.WorldToViewportPoint(myTransform.position).y;

        #if UNITY_STANDALONE

        input = Input.GetAxisRaw("Horizontal");

        #else

        input = HUD.NavButton;

        #endif

        // down
        if (input > 0f && ypos <= horizontalBounds)
        {
            return 0f;
        }
        // up
        else if (input < 0f && ypos >= 1 - horizontalBounds)
        {
            return 0f;
        }
        else
        {
            return input;
        }
    }


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

        return barrelRoll;

        #endif
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="points"></param>
    public void RecieveKill(Enemy.Classes enemy, int points, int money)
    {
        myScore.IncreaseMultiplier();
        myScore.RecieveScore(points);
        myMoney.Collect(money);
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

    private void OnBarrelRoll(object sender, EventArgs args)
    {
        barrelRoll = true;
    }

    #endregion
}