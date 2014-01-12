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
    #region Public Fields

    public float minAttackDelay;
    public float maxAttackDelay;

    #endregion

    #region Private Fields

    private Job attackCycle1;
    private Job attackCycle2;

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

    #region Enemy Overrides

    public override void Spawn(Vector3 stopPosition)
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region State Methods

    private void MovingEnter(Dictionary<string, object> info)
    {
        stateMachine.update = new Job(MovingUpdate());
    }


    private IEnumerator MovingUpdate()
    {
        if (attackCycle1 != null)
        {
            attackCycle1.Kill();
        }
        attackCycle1 = new Job(AttackCycle(0));
        if (attackCycle2 != null)
        {
            attackCycle2.Kill();
        }
        attackCycle2 = new Job(AttackCycle(1));

        while (true)
        {
            // move
            myMotor.Move(-Vector2.right);
            yield return null;
        }
    }


    private void DieEnter(Dictionary<string, object> info)
    {
        // send hitinfo to player

        attackCycle1.Kill();
        attackCycle2.Kill();
        poolObject.Disable();
    }

    #endregion

    #region Private Methods

    private IEnumerator AttackCycle(int weapon)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minAttackDelay, maxAttackDelay));
            myWeapons.TryActivate(weapon, true);
        }
    }

    #endregion
}