// Steve Yeager
// 12.17.2013

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Basic enemy. Flies forward and shoots Plasma Lasers.
/// </summary>
public class Grunt : Enemy
{
    #region State Fields

    private const string MovingState = "Moving";

    #endregion


    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // states
        stateMachine.CreateState(MovingState, MovingEnter, info => { });
        stateMachine.initialState = MovingState;
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
            myMotor.Move(1f, Vector3.forward);
            yield return null;
        }
    }

    #endregion
}