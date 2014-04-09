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
    public float targetSpeed;

    /// <summary>Speed flying the figure 8.</summary>
    public float pathSpeed;

    #endregion

    #region Private Fields

    /// <summary>Min target distance away from player.</summary>
    public float targetdistance;

    private FigureEight path;

    #endregion

    #region Const Fields

    private const string SpawningState = "Spawning";
    private const string MovingState = "Moving";
    private const string IdlingState = "Idling";

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
        path = new FigureEight { speed = pathSpeed };
        path.Initialize(myTransform);

        myHealth.Initialize();

        // decide target distance
        targetdistance = Random.Range(minTargetDistance, maxTargetDistance);

        stateMachine.SetState(MovingState);
    }


    private IEnumerator MovingUpdate()
    {
        while (true)
        {
            // move
            myMotor.Move((LevelManager.Main.playerTransform.position-myTransform.position).normalized*targetSpeed);
            yield return null;
        }
    }


    private IEnumerator IdlingUpdate()
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

        poolObject.Disable();
    }

    #endregion
}