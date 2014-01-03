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
    public float swipeNeeded = 15f;

    #endregion


    #region Unity Overrides

    protected override void Awake()
    {
        base.Awake();

        // setup
        tag = "Player";
        gameObject.layer = LayerMask.NameToLayer("Player");

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
        myWeapons.RegisterToHUD();

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
            int direction = BarrelRollInput();
            if (direction != 0)
            {
                stateMachine.SetState(BarrelRollingState, new Dictionary<string, object> { { "direction", direction } });
                yield break;
            }

            // attack
            var weapons = AttackInput();
            if (weapons.Count > 0)
            {
                Attack(weapons);
            }

            // move
            myMotor.Move(HorizontalInput(), Vector3.right);

            yield return null;
        }
    }


    private void MovingExit(Dictionary<string, object> info)
    {
        stateMachine.update = new Job(MovingUpdate());
    }


    private void BarrelRollingEnter(Dictionary<string, object> info)
    {
        myHealth.invincible = true;
        collider.enabled = false;
        stateMachine.update = new Job(BarrelRollingUpdate((int)info["direction"]));
    }


    private IEnumerator BarrelRollingUpdate(int direction)
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
        Destroy(gameObject);
    }

    #endregion

    #region Input Methods

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

        #endif

        return weapons;
    }


    private int BarrelRollInput()
    {
        int roll = 0;

        #if UNITY_STANDALONE

        if (Input.GetKeyDown(KeyCode.Space))
        {
            roll = (int)Input.GetAxisRaw("Horizontal");
        }

        #else

        for (int i = 0; i < Input.touchCount; i++)
        {
            float swipe = Input.GetTouch(i).deltaPosition.y;
            if (swipe > swipeNeeded)
            {
                roll = -1;
                break;
            }
            else if (swipe < -swipeNeeded)
            {
                roll = 1;
                break;
            }
        }

        #endif

            return roll;
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
}