// Steve Yeager
// 3.3.2013

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Basic enemy. Flies forward and shoots Plasma Lasers.
/// </summary>
public class Gunner : Enemy
{
    #region Public Fields

    public float minRandomDelay;
    public float maxRandomDelay;
    public float repeatingDelay;
    public int repeatingShots;
    public float repeatingBuffer;

    #endregion

    #region Private Fields

    private Job attackCycle1;
    private Job attackCycle2;

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

        if (attackCycle1 != null)
        {
            attackCycle1.Kill();
        }
        attackCycle1 = new Job(RandomCannon());
        if (attackCycle2 != null)
        {
            attackCycle2.Kill();
        }
        attackCycle2 = new Job(RepeatingCannon());

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

        attackCycle1.Kill();
        attackCycle2.Kill();
        poolObject.Disable();
    }

    #endregion

    #region Private Methods

    private IEnumerator RandomCannon()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minRandomDelay, maxRandomDelay));
            myWeapons.TryActivate(0, true);
            myWeapons.TryActivate(0, false);
        }
    }


    private IEnumerator RepeatingCannon()
    {
        while (true)
        {
            yield return new WaitForSeconds(repeatingDelay);
            for (int i = 0; i < repeatingShots; i++)
            {
                myWeapons.TryActivate(1, true);
                myWeapons.TryActivate(1, false);
                yield return new WaitForSeconds(repeatingBuffer);
            }
        }
    }

    #endregion
}