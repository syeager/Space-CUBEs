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

    #endregion

    #region Input Fields

    public float horizontalBounds = 0.05f;
    public float backBounds;
    public float frontBounds;

    #endregion


    #region Unity Overrides

    protected override void Awake()
    {
        base.Awake();

        // create states
        CreateState(MovingState, MovingEnter, info => { });
        initialState = MovingState;
    }


    private void Start()
    {
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

            yield return null;
        }
    }


    private void MovingExit(Dictionary<string, object> info)
    {
        StopCoroutine("MovingUpdate");
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

    #endregion
}