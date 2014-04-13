// Steve Yeager
// 4.7.2014

using System.Collections.Generic;
using System.Collections;
using Paths;
using UnityEngine;

/// <summary>
/// AI for Sentry. Flies around player. Fires at player.
/// </summary>
public class Sentry : Enemy
{
    #region Public Fields

    /// <summary>Min target distance away from player.</summary>
    public float minTargetDistance;

    /// <summary>Max target distance away from player.</summary>
    public float maxTargetDistance;

    /// <summary>Speed flying to target.</summary>
    public float targetMoveSpeed;

    /// <summary>Distance to Player allowed before Sentry repositions.</summary>
    public float idleDistanceBuffer;

    /// <summary>Speed flying the figure 8.</summary>
    public float idlingSpeed;

    /// <summary>How fast to rotate towards player.</summary>
    public float angularSpeed;

    /// <summary>Seconds between firing.</summary>
    public float attackBuffer;

    #endregion

    #region Private Fields

    /// <summary>Min target distance away from player.</summary>
    private float targetDistance;

    /// <summary>Path to fly when within attacking distance.</summary>
    private FigureEight attackPath;

    /// <summary>Cached Player transform.</summary>
    private Transform player;

    #endregion

    #region Const Fields

    private const string SpawningState = "Spawning";
    private const string MovingState = "Moving";
    private const string AttackingState = "Attacking";

    #endregion


    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // states
        stateMachine = new StateMachine(this, SpawningState);
        stateMachine.CreateState(SpawningState, SpawnEnter, info => { });
        stateMachine.CreateState(MovingState, info => stateMachine.SetUpdate(MovingUpdate()), info => { });
        stateMachine.CreateState(AttackingState, AttackingEnter, info => { });
        stateMachine.CreateState(DyingState, DieEnter, info => { });
    }

    #endregion

    #region State Methods

    private void SpawnEnter(Dictionary<string, object> info)
    {
        player = LevelManager.Main.playerTransform;

        attackPath = ScriptableObject.CreateInstance(typeof(FigureEight)) as FigureEight;
        attackPath.Initialize(myTransform, idlingSpeed);

        myHealth.Initialize();

        // decide target distance
        targetDistance = Random.Range(minTargetDistance, maxTargetDistance);

        stateMachine.SetState(MovingState);
    }


    private IEnumerator MovingUpdate()
    {
        while (true)
        {
            Vector3 targetPosition = player.position;

            // update transform
            Rotate();
            myMotor.Move(myTransform.forward * targetMoveSpeed * Time.deltaTime);

            // enter attacking
            if (Vector3.Distance(targetPosition, myTransform.position) <= targetDistance)
            {
                stateMachine.SetState(AttackingState);
                yield break;
            }

            yield return null;
        }
    }


    private void AttackingEnter(Dictionary<string, object> obj)
    {
        StartCoroutine(Fire());
        stateMachine.SetUpdate(AttackingUpdate());
    }


    private IEnumerator AttackingUpdate()
    {
        while (true)
        {
            if (Vector3.Distance(player.position, myTransform.position) > idleDistanceBuffer + targetDistance)
            {
                stateMachine.SetState(MovingState);
                yield break;
            }

            // idle
            Rotate();
            myMotor.Move(attackPath.Direction(deltaTime));
            yield return null;
        }
    }


    private void DieEnter(Dictionary<string, object> info)
    {
        StopAllCoroutines();
        poolObject.Disable();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Rotate to face player.
    /// </summary>
    private void Rotate()
    {
        myTransform.rotation = Quaternion.Slerp(myTransform.rotation,
                                                Quaternion.LookRotation(player.position - myTransform.position, Vector3.back),
                                                angularSpeed*Time.deltaTime);
    }


    private IEnumerator Fire()
    {
        WaitForSeconds wait = new WaitForSeconds(attackBuffer);
        while (true)
        {
            yield return wait;

            myWeapons.Activate(0, true);
        }
    }

    #endregion
}