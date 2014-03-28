// Steve Yeager
// 3.25.2014

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// AI for first boss.
/// </summary>
public class SwitchBlade : Boss
{
    #region State Fields

    private const string EnteringState = "Entering";
    private const string StagingState = "Staging";
    private const string Stage1State = "Stage1";
    private const string Stage2State = "Stage2";
    private const string Stage3State = "Stage3";

    #endregion

    #region Public Fields

    public Vector3 startPosition;
    public float stagingTime;
    public float moveSpeed;
    public float moveHeight;

    #endregion

    #region Private Methods

    private Job moveJob;

    #endregion

    #region Stage 1

    public int stage1AllFireChance;
    public float stage1CycleTime;
    public float stage1CycleDelay;

    #endregion


    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // state machine
        stateMachine = new StateMachine(this, EnteringState);
        stateMachine.CreateState(EnteringState, EnteringEnter, EnteringExit);
        stateMachine.CreateState(StagingState, i => { stateMachine.SetUpdate(StagingUpdate()); }, i => { });
        stateMachine.CreateState(Stage1State, Stage1Enter, i => { });
        stateMachine.CreateState(Stage2State, i => { stateMachine.SetUpdate(Stage2Update());  }, i => { });
        stateMachine.CreateState(Stage3State, i => { }, i => { });
        stateMachine.CreateState(DyingState, i => { Destroy(gameObject); }, i => { });

        // stages
        NextStageEvent += OnStageIncrease;
    }

    #endregion

    #region State Methods

    private void EnteringEnter(Dictionary<string, object> info)
    {
        myHealth.invincible = true;

        stateMachine.SetUpdate(EnteringUpdate());
    }


    private IEnumerator EnteringUpdate()
    {
        while (Vector3.Distance(myTransform.position, startPosition) > 1f)
        {
            myMotor.Move(-Vector2.right);
            yield return null;
        }

        stateMachine.SetState(Stage1State);
    }


    private void EnteringExit(Dictionary<string, object> info)
    {
        InitializeHealth();
        myHealth.invincible = false;
    }


    private IEnumerator StagingUpdate()
    {
        myHealth.invincible = true;
        yield return new WaitForSeconds(stagingTime);
        myHealth.invincible = false;

        if (stage == 2)
        {
            stateMachine.SetState(Stage2State);
        }
        else
        {
            stateMachine.SetState(Stage3State);
        }
    }


    private void Stage1Enter(Dictionary<string, object> info)
    {
        myMotor.speed = moveSpeed;
        moveJob = new Job(Move());
        stateMachine.SetUpdate(Stage1Update());
    }


    private IEnumerator Stage1Update()
    {
        while (true)
        {
            // sides
            int cycles = Random.Range(1, 3);
            for (int i = 0; i < cycles; i++)
            {
                yield return StartCoroutine(FireSideWeapons());
            }

            // top
            Debug.Log("firing top");
            yield return new WaitForSeconds(stage1CycleTime);

            // sides
            cycles = Random.Range(1, 3);
            for (int i = 0; i < cycles; i++)
            {
                yield return StartCoroutine(FireSideWeapons());
            }

            // all
            Debug.Log("firing all");
            yield return new WaitForSeconds(stage1CycleTime);
        }
    }


    private IEnumerator Stage2Update()
    {
        while (true)
        {
            yield return null;
        }
    }

    #endregion

    #region Private Methods

    private IEnumerator Move()
    {
        while (true)
        {
            // up
            while (myTransform.position.y < moveHeight)
            {
                myMotor.Move(Vector2.up);
                yield return null;
            }

            // down
            while (myTransform.position.y > -moveHeight)
            {
                myMotor.Move(-Vector2.up);
                yield return null;
            }

            yield return null;
        }
    }


    private IEnumerator FireSideWeapons()
    {
        // activate
        int weapon1 = Random.Range(0, 2);
        int weapon2 = Random.Range(2, 4);

        myWeapons.weapons[weapon1].gameObject.SetActive(true);
        myWeapons.weapons[weapon2].gameObject.SetActive(true);

        moveJob.Pause();
        yield return new WaitForSeconds(stage1CycleDelay);
        moveJob.UnPause();

        // left
        myWeapons.Activate(weapon1, true);
        // right
        myWeapons.Activate(weapon2, true);

        yield return new WaitForSeconds(stage1CycleTime);
        myWeapons.weapons[weapon1].gameObject.SetActive(false);
        myWeapons.weapons[weapon2].gameObject.SetActive(false);
    }

    #endregion

    #region Event Handlers

    private void OnStageIncrease(object sender, ValueArgs args)
    {
        stage++;
        stateMachine.SetState(StagingState);
    }

    #endregion
}