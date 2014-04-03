﻿// Steve Yeager
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
    public float stagingSize;
    public float stagingSpeed;
    public float moveSpeed;
    public float moveHeight;

    #endregion

    #region Private Methods

    private Job moveJob;

    #endregion

    #region Stage 1

    public float stage1AttackTime;
    public float stage1SwitchTime;
    public float bulletEmitterTime;

    #endregion

    #region Stage 2

    public float shieldTime = 5f;
    public float shieldBuffer = 3f;
    private Job shieldJob;

    #endregion

    #region Stage 3

    public float deathLaserTime;

    #endregion


    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // state machine
        stateMachine = new StateMachine(this, EnteringState);
        stateMachine.CreateState(EnteringState, EnteringEnter, EnteringExit);
        stateMachine.CreateState(StagingState, StagingEnter, StagingExit);
        stateMachine.CreateState(Stage1State, Stage1Enter, i => { });
        stateMachine.CreateState(Stage2State, Stage2Enter, Stage2Exit);
        stateMachine.CreateState(Stage3State, Stage3Enter, i => { });
        stateMachine.CreateState(DyingState, DyingEnter, i => { });

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


    private void StagingEnter(Dictionary<string, object> info)
    {
        myHealth.invincible = true;
        moveJob.Pause();

        for (int i = 0; i < myWeapons.weapons.Length; i++)
        {
            myWeapons.Activate(i, false);
            myWeapons.weapons[i].gameObject.SetActive(false);
        }

        StopAllCoroutines();

        stateMachine.SetUpdate(StagingUpdate());
    }


    private IEnumerator StagingUpdate()
    {
        // resize
        float timer = stagingTime;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            myTransform.localScale += Vector3.one * Mathf.Sin(timer * stagingSpeed) * stagingSize * Time.deltaTime;
            yield return null;
        }
        myTransform.localScale = Vector3.one;

        if (stage == 2)
        {
            stateMachine.SetState(Stage2State);
        }
        else
        {
            stateMachine.SetState(Stage3State);
        }
    }


    private void StagingExit(Dictionary<string, object> info)
    {
        myHealth.invincible = false;
        moveJob.UnPause();

        stateMachine.SetUpdate(StagingUpdate());
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
                yield return StartCoroutine(FireSideWeapons(true));
            }

            // top
            yield return StartCoroutine(FirePattern(true));

            // sides
            cycles = Random.Range(1, 3);
            for (int i = 0; i < cycles; i++)
            {
                yield return StartCoroutine(FireSideWeapons(true));
            }

            // all
            StartCoroutine(FirePattern(true));
            StartCoroutine(FireSideWeapons(false));
            yield return new WaitForSeconds(bulletEmitterTime + stage1SwitchTime);
        }
    }


    private void Stage1Exit(Dictionary<string, object> info)
    {
        for (int i = 0; i < 5; i++)
        {
            myWeapons.Activate(i, false);
            myWeapons.weapons[i].gameObject.SetActive(false);
        }
    }


    private void Stage2Enter(Dictionary<string, object> info)
    {
        stateMachine.SetUpdate(Stage2Update());
        shieldJob = new Job(Shield());
    }


    private IEnumerator Stage2Update()
    {
        while (true)
        {
            // sides
            int cycles = Random.Range(1, 3);
            for (int i = 0; i < cycles; i++)
            {
                yield return StartCoroutine(FireSideWeapons(true));
            }

            // top
            yield return StartCoroutine(FirePattern(true));

            // sides
            cycles = Random.Range(1, 3);
            for (int i = 0; i < cycles; i++)
            {
                yield return StartCoroutine(FireSideWeapons(true));
            }

            // all
            StartCoroutine(FirePattern(true));
            StartCoroutine(FireSideWeapons(false));
            yield return new WaitForSeconds(stage1AttackTime + stage1SwitchTime);
        }
    }


    private void Stage2Exit(Dictionary<string, object> info)
    {
        shieldJob.Pause();
    }


    private void Stage3Enter(Dictionary<string, object> info)
    {
        stateMachine.SetUpdate(Stage3Update());
        shieldJob.UnPause();
    }


    private IEnumerator Stage3Update()
    {
        // death laser
        myWeapons.Activate(6, true);
        yield return new WaitForSeconds(deathLaserTime);
        myWeapons.Activate(6, false);

        while (true)
        {
            // sides
            yield return StartCoroutine(FireSideWeapons(true));

            // top
            yield return StartCoroutine(FirePattern(true));

            // sides
            yield return StartCoroutine(FireSideWeapons(true));

            // all
            myWeapons.Activate(6, true);
            StartCoroutine(FireSideWeapons(false));
            yield return new WaitForSeconds(deathLaserTime);

            myWeapons.Activate(6, false);
            yield return new WaitForSeconds(stage1SwitchTime);
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


    private IEnumerator FireSideWeapons(bool controlMovement)
    {
        // get weapons
        int weapon1 = Random.Range(0, 2);
        int weapon2 = Random.Range(2, 4);

        // open
        if (controlMovement)
        {
            moveJob.Pause();
        }
        myWeapons.weapons[weapon1].gameObject.SetActive(true);
        myWeapons.weapons[weapon2].gameObject.SetActive(true);
        yield return new WaitForSeconds(stage1SwitchTime);

        // fire
        if (controlMovement)
        {
            moveJob.UnPause();
        }
        myWeapons.Activate(weapon1, true);
        myWeapons.Activate(weapon2, true);
        yield return new WaitForSeconds(stage1AttackTime);
        myWeapons.Activate(weapon1, false);
        myWeapons.Activate(weapon2, false);

        // close
        if (controlMovement)
        {
            moveJob.Pause();
        }
        myWeapons.weapons[weapon1].gameObject.SetActive(false);
        myWeapons.weapons[weapon2].gameObject.SetActive(false);
        yield return new WaitForSeconds(stage1SwitchTime);
        if (controlMovement)
        {
            moveJob.UnPause();
        }
    }


    private IEnumerator FirePattern(bool controlMovement)
    {
        // open
        if (controlMovement)
        {
            moveJob.Pause();
        }
        myWeapons.weapons[4].gameObject.SetActive(true);
        yield return new WaitForSeconds(stage1SwitchTime);

        // fire
        myWeapons.Activate(4, true);
        yield return new WaitForSeconds(bulletEmitterTime);

        // close
        myWeapons.weapons[4].gameObject.SetActive(false);
        yield return new WaitForSeconds(stage1SwitchTime);
        if (controlMovement)
        {
            moveJob.UnPause();
        }
    }


    private IEnumerator Shield()
    {
        WaitForSeconds activated = new WaitForSeconds(shieldTime);
        WaitForSeconds deactivated = new WaitForSeconds(shieldBuffer);

        while (true)
        {
            // activate
            myWeapons.Activate(5, true);
            yield return activated;
 
            // close
            myWeapons.Activate(5, false);
            yield return deactivated;
        }
    }


    private void DyingEnter(Dictionary<string, object> info)
    {
        moveJob.Kill();
        Destroy(gameObject);
    }

    #endregion

    #region Event Handlers

    private void OnStageIncrease(object sender, ValueArgs args)
    {
        stateMachine.SetState(StagingState);
    }

    #endregion
}