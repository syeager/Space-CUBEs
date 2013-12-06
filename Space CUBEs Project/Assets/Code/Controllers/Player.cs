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
    private const string AttackingState = "Attacking";

    #endregion

    #region Input Fields

    public float horizontalBounds = 0.05f;
    public float backBounds;
    public float frontBounds;
    public readonly KeyCode[] WeaponKeys = new KeyCode[6]
    {
        KeyCode.J,
        KeyCode.K,
        KeyCode.L,
        KeyCode.M,
        KeyCode.Comma,
        KeyCode.Period
    };

    #endregion


    #region Unity Overrides

    protected override void Awake()
    {
        base.Awake();

        // create states
        CreateState(MovingState, MovingEnter, MovingExit);
        CreateState(AttackingState, AttackingEnter, AttackingExit);
        initialState = MovingState;
    }


    private void Start()
    {
        Debug.Log("0");
        StartInitialState(null);
    }

    #endregion

    #region State Methods

    private void MovingEnter(Dictionary<string, object> info)
    {
        StartCoroutine("MovingUpdate");
    }


    private IEnumerator MovingUpdate()
    {
        while (true)
        {
            // move
            myMotor.MoveHorizontal(HorizontalInput());

            // attack
            var weapons = AttackInput();
            if (weapons.Count > 0)
            {
                SetState(AttackingState, new Dictionary<string, object> { { "weapons", weapons } });
                yield break;
            }

            yield return null;
        }
    }


    private void MovingExit(Dictionary<string, object> info)
    {
        StopCoroutine("MovingUpdate");
    }


    private void AttackingEnter(Dictionary<string, object> info)
    {
        Attack((List<int>)info["weapons"]);
        
        StartCoroutine("AttackingUpdate");
    }


    private IEnumerator AttackingUpdate()
    {

        yield return null;
    }


    private void AttackingExit(Dictionary<string, object> info)
    {
        StopCoroutine("AttackingUpdate");
    }

    #endregion

    #region Input Methods

    private float HorizontalInput()
    {
        #if UNITY_STANDALONE

        float input = Input.GetAxis("Horizontal");
        float ypos = Camera.main.WorldToViewportPoint(myTransform.position).y;
        
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

        #else

        return 0f;

        #endif
    }


    private List<int> AttackInput()
    {
        var weapons = new List<int>();

        #if UNITY_STANDALONE

        for (int i = 0; i < WeaponKeys.Length; i++)
        {
            if (Input.GetKeyDown(WeaponKeys[i]) || (Input.GetKeyUp(WeaponKeys[i])))
            {
                if (myWeapons.CanActivate(i))
                {
                    weapons.Add(i);
                }
            }
        }

        #else
        
        

        #endif

        return weapons;
    }

    #endregion

    #region Combat Methods

    private void Attack(List<int> weapons)
    {
        foreach (var weapon in weapons)
        {
            myWeapons.Activate(weapon);
        }
    }

    #endregion
}