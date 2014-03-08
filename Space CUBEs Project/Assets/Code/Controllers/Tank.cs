// Steve Yeager
// 3.7.2013

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Tank : Enemy
{
    #region Public Fields

    public float shotDelay;

    #endregion

    #region Private Fields

    private Job attackCycle;

    #endregion

    #region Const Fields

    private const string MovingState = "Moving";

    #endregion


    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // states
        stateMachine.CreateState(MovingState, MovingEnter, info => { });
        stateMachine.CreateState(DyingState, DieEnter, info => { });
        stateMachine.initialState = MovingState;
    }

    #endregion

    #region State Methods

    private void MovingEnter(Dictionary<string, object> info)
    {
        if (attackCycle != null)
        {
            attackCycle.Kill();
        }
        attackCycle = new Job(Firing());

        stateMachine.SetUpdate(MovingUpdate());
    }


    private IEnumerator MovingUpdate()
    {
        while (true)
        {
            // move
            myMotor.Move(path.Direction(deltaTime));
            yield return null;
        }
    }


    private void DieEnter(Dictionary<string, object> info)
    {
        // send hitinfo to player

        attackCycle.Kill();
        poolObject.Disable();
    }

    #endregion

    #region Private Methods

    private IEnumerator Firing()
    {
        while (true)
        {
            yield return new WaitForSeconds(shotDelay);
            myWeapons.TryActivate(0, true);
            myWeapons.TryActivate(0, false);
            myWeapons.TryActivate(1, true);
            myWeapons.TryActivate(1, false);
        }
    }

    #endregion
}