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
        attackCycle = new Job(Firing());

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