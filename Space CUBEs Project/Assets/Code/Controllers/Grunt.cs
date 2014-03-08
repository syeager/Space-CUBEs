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

    private Job attackCycle;

    #endregion

    #region Const Fields

    private const string SpawningState = "Spawning";
    private const string MovingState = "Moving";

    #endregion


    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // states
        stateMachine = new StateMachine(this, SpawningState);
        stateMachine.CreateState(SpawningState, SpawnEnter, info => { });
        stateMachine.CreateState(MovingState, info => stateMachine.SetUpdate(MovingUpdate()), info => { });
        stateMachine.CreateState(DyingState, DieEnter, info => { });
    }

    #endregion

    #region State Methods

    private void SpawnEnter(Dictionary<string, object> info)
    {
        path = info["path"] as Path;
        path.Initialize(myTransform);
        myMotor.Initialize(path.speed, false);

        if (attackCycle != null)
        {
            attackCycle.Kill();
        }
        attackCycle = new Job(AttackCycle(0));

        myHealth.Initialize();

        stateMachine.SetState(MovingState);
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

    private IEnumerator AttackCycle(int weapon)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minAttackDelay, maxAttackDelay));
            myWeapons.TryActivate(weapon, true);
            myWeapons.TryActivate(weapon, false);
        }
    }

    #endregion
}