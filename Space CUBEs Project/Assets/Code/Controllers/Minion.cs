// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.28
// Edited: 2014.06.28

using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// Created by Medic.
/// </summary>
public class Minion : Enemy
{
    #region References

    public Transform cannonTransform;

    #endregion

    #region State Names

    private const string SpawningState = "Spawning";
    private const string MovingState = "Moving";
    private const string AttackingState = "Attacking";

    #endregion

    #region Public Fields

    /// <summary>Laser weapon.</summary>
    public EnemyCannon cannon;

    /// <summary>Min world position to move to then attack from.</summary>
    public Vector3 attackPositionMin;

    /// <summary>Max world position to move to then attack from.</summary>
    public Vector3 attackPositionMax;

    /// <summary>Angular targeting speed.</summary>
    public float targetingSpeed;

    #endregion

    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // states
        stateMachine = new StateMachine(this, SpawningState);
        stateMachine.CreateState(SpawningState, SpawningEnter, info => { });
        stateMachine.CreateState(MovingState, info => stateMachine.SetUpdate(MovingUpdate()), info => { });
        stateMachine.CreateState(AttackingState, info => stateMachine.SetUpdate(AttackingUpdate()), info => { });
        stateMachine.CreateState(DyingState, DyingEnter, info => { });
    }

    #endregion

    #region State Methods

    private void SpawningEnter(Dictionary<string, object> info)
    {
        MyHealth.maxShield = 0f;
        MyHealth.Initialize();
        cannon.Initialize(this);

        stateMachine.SetState(MovingState);
    }


    private IEnumerator MovingUpdate()
    {
        Transform player = LevelManager.Main.PlayerTransform;
        Vector3 target = Utility.RandomVector3(attackPositionMin, attackPositionMax);
        const float distBuffer = 1f;
        while (Vector3.Distance(myTransform.position, target) > distBuffer)
        {
            myMotor.Move((Vector2)myTransform.position.To(target));
            cannonTransform.rotation = cannonTransform.RotateTowards(player.position, targetingSpeed * deltaTime, Vector3.back);
            yield return null;
        }

        stateMachine.SetState(AttackingState);
    }


    private IEnumerator AttackingUpdate()
    {
        while (true)
        {
            yield return cannon.Activate(true);
            yield return cannon.CoolDown();
        }
    }


    private void DyingEnter(Dictionary<string, object> info)
    {
        poolObject.Disable();
    }

    #endregion
}