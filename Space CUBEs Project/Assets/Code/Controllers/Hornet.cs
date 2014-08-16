// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.22
// Edited: 2014.07.12

using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// Pops in, takes a couple shot at the player, then leaves.
/// </summary>
public class Hornet : Enemy
{
    #region State Names

    private const string SpawningState = "Spawning";
    private const string EnteringState = "Entering";
    private const string AttackingState = "Attacking";
    private const string ExitingState = "Exiting";

    #endregion

    #region Attacking Fields

    public Weapon laser;

    /// <summary>Time in seconds to sit and attack.</summary>
    public float attackingTime = 3f;

    #endregion

    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // states
        stateMachine = new StateMachine(this, SpawningState);
        stateMachine.CreateState(SpawningState, SpawningEnter, info => { });
        stateMachine.CreateState(EnteringState, info => stateMachine.SetUpdate(EnteringUpdate()), info => { });
        stateMachine.CreateState(AttackingState, info => stateMachine.SetUpdate(AttackingUpdate()), info => { });
        stateMachine.CreateState(ExitingState, info => stateMachine.SetUpdate(ExitingUpdate()), info => { });
        stateMachine.CreateState(DyingState, DyingEnter, info => { });

        // weapons
        laser.Initialize(this);
    }

    #endregion

    #region State Methods

    private void SpawningEnter(Dictionary<string, object> info)
    {
        path = (Path)info["path"];
        MyHealth.Initialize();
        path.Initialize(myTransform);

        stateMachine.SetState(EnteringState);
    }


    private IEnumerator EnteringUpdate()
    {
        while (true)
        {
            Vector2 direction = path.Direction(deltaTime);

            // reached destination
            if (direction == Vector2.zero)
            {
                stateMachine.SetState(AttackingState);
                yield break;
            }
            else
            {
                MyMotor.Move(direction);
            }

            yield return null;
        }
    }


    private IEnumerator AttackingUpdate()
    {
        yield return laser.Activate(true);
        laser.Activate(false);
        stateMachine.SetState(ExitingState);
    }


    private IEnumerator ExitingUpdate()
    {
        while (true)
        {
            MyMotor.Move((Vector2)path.Direction(deltaTime));
            yield return null;
        }
    }


    private void DyingEnter(Dictionary<string, object> info)
    {
        StopAllCoroutines();
        laser.Activate(false);
        poolObject.Disable();
    }

    #endregion
}