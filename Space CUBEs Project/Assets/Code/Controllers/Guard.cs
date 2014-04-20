// Steve Yeager
// 4.13.2014

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI for Guard. Stand back and fire straight lasers.
/// </summary>
public class Guard : Enemy
{
    #region State Fields

    private const string SpawningState = "Spawning";
    private const string MovingState = "Moving";
    private const string AttackingState = "Attacking";
    
    #endregion

    #region Attacking Fields

    /// <summary>Seconds in between attacks.</summary>
    public float attackBuffer;

    /// <summary>How long the attack lasts in seconds.</summary>
    public float attackTime;

    #endregion


    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // states
        stateMachine = new StateMachine(this, SpawningState);
        stateMachine.CreateState(SpawningState, SpawnEnter, info => { });
        stateMachine.CreateState(MovingState, info => stateMachine.SetUpdate(MovingUpdate()), info => { });
        stateMachine.CreateState(AttackingState, AttackEnter, info => { });
        stateMachine.CreateState(DyingState, DyingEnter, info => { });
    }

    #endregion

    #region State Methods

    private void SpawnEnter(Dictionary<string, object> info)
    {
        myHealth.Initialize();

        // initialize path from level manager
        path = (Path)info["path"];
        path.Initialize(myTransform);

        stateMachine.SetState(MovingState);
    }


    private IEnumerator MovingUpdate()
    {
        while (true)
        {
            Vector3 direction = path.Direction(deltaTime);

            // reached destination
            if (direction == Vector3.zero)
            {
                stateMachine.SetState(AttackingState);
                yield break;
            }
            else
            {
                myMotor.Move(direction);
            }

            yield return null;
        }
    }


    private void AttackEnter(Dictionary<string, object> info)
    {
        myMotor.Move(Vector3.zero);
        stateMachine.SetUpdate(AttackingUpdate());
    }


    private IEnumerator AttackingUpdate()
    {
        WaitForSeconds buffer = new WaitForSeconds(attackBuffer);
        WaitForSeconds attack = new WaitForSeconds(attackTime);

        while (true)
        {
            yield return buffer;
            myWeapons.Activate(0, true);
            yield return attack;
            myWeapons.Activate(0, false);
        }
    }


    private void DyingEnter(Dictionary<string, object> info)
    {
        StopAllCoroutines();
        myWeapons.Activate(0, false);
        poolObject.Disable();
    }

    #endregion
}