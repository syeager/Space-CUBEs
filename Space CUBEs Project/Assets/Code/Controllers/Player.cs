// Steve Yeager
// 11.25.2013

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
    public AugmentationManager myAugmentations { get; private set; }

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

#if UNITY_STANDALONE
    public static readonly KeyCode[] WeaponKeys =
    {
        KeyCode.J,
        KeyCode.K,
        KeyCode.L,
        KeyCode.Semicolon
    };
#else
    private bool barrelRoll;
    public float swipeNeeded = 10f;
#endif

    #endregion

    #region Const Fields

    public const int Weaponlimit = 4;

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
        myAugmentations = GetComponent<AugmentationManager>();

        // effects
        if (!GameSettings.Main.trailRenderer)
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
            var weapons = AttackInput();
            if (weapons.Count > 0)
            {
                Attack(weapons);
            }
#endif

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
#if !UNITY_STANDALONE
        barrelRoll = false;
#endif
        collider.enabled = false;

        // release all attacks
        myWeapons.ActivateAll(false);

        stateMachine.SetUpdate(BarrelRollingUpdate(MovementInput(), myHealth.invincible));
        myHealth.invincible = true;
    }


    private IEnumerator BarrelRollingUpdate(Vector2 direction, bool invincible)
    {
        yield return StartCoroutine(myMotor.BarrelRoll(direction, horizontalBounds));
        stateMachine.SetState(MovingState, new Dictionary<string, object>{{"invincible", invincible}});
    }


    private void BarrelRollingExit(Dictionary<string, object> info)
    {
        myHealth.invincible = (bool)info["invincible"];
        collider.enabled = true;
#if !UNITY_STANDALONE
        barrelRoll = false;
#endif
    }


    private void DyingEnter(Dictionary<string, object> info)
    {
        gameObject.SetActive(false);
        myWeapons.canActivate = false;
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

        return weapons;
    }
#endif


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
    /// <param name="money">How much money the enemy is worth.</param>
    public void RecieveKill(Enemy.Classes enemy, int points, int money)
    {
        myScore.RecieveScore(points);
        myScore.IncreaseMultiplier();
        myMoney.Collect(money);
    }


    /// <summary>
    /// Initialize ship components. Health, Motor, and Weapons.
    /// </summary>
    public void Initialize(float maxHealth, float maxShield, float speed, float damage)
    {
        myHealth.Initialize(maxHealth, maxShield);
        myMotor.Initialize(speed);
        myWeapons.Initialize(this, damage);
        myAugmentations.Initialize(this);

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
            myWeapons.Activate(weapon.Key, weapon.Value);
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