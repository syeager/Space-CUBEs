// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.22
// Edited: 2014.06.23

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
    }

    #endregion

    #region State Methods

    private void SpawningEnter(Dictionary<string, object> info)
    {
        path = (Path)info["path"];
        myHealth.Initialize();
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
                myMotor.Move(direction);
            }

            yield return null;
        }
    }


    private IEnumerator AttackingUpdate()
    {
        yield return myWeapons.Activate(0, true);
        myWeapons.Activate(0, false);
        stateMachine.SetState(ExitingState);
    }


    private IEnumerator ExitingUpdate()
    {
        while (true)
        {
            myMotor.Move(path.Direction(deltaTime));
            yield return null;
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