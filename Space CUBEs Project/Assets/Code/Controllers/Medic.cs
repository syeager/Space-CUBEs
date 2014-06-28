// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.25
// Edited: 2014.06.26

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// Boss for Nebula Forest. Summons minions and buffs them. Has a health stealing laser.
/// </summary>
public class Medic : Boss
{
    #region References

    private Animation myAnimation;

    #endregion

    #region State Fields

    private const string EnteringState = "Entering";
    private const string StagingState = "Staging";
    private const string Stage1State = "Stage1";
    private const string Stage2State = "Stage2";
    private const string Stage3State = "Stage3";

    #endregion

    #region Weapon Fields

    public MinionSpawner minionSpawner;
    public PlasmaMachineGun plasmaGun;

    #endregion

    #region Staging Fields

    public float stagingTime;
    public float stagingSize;
    public float stagingSpeed;

    #endregion

    #region Stage 1 Fields

    public float stage1DownTime; 

    #endregion

    #region Public Fields

    public Vector3 startPosition;

    #endregion

    #region Movement Fields

    private Job swayJob;
    public float swaySpeed;
    public float swayDistance;

    #endregion

    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // references
        myAnimation = animation;

        // state machine
        stateMachine = new StateMachine(this, EnteringState);
        stateMachine.CreateState(EnteringState, EnteringEnter, EnteringExit);
        stateMachine.CreateState(StagingState, StagingEnter, StagingExit);
        stateMachine.CreateState(Stage1State, Stage1Enter, info => { });

        stateMachine.CreateState(DyingState, DyingEnter, i => { });

        // stages
        NextStageEvent += OnStageIncrease;

        // weapons
        minionSpawner.Initialize(this);
        plasmaGun.Initialize(this);
    }

    #endregion

    #region State Methods

    private void EnteringEnter(Dictionary<string, object> info)
    {
        MyHealth.invincible = true;

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
        myMotor.Stop();
        InitializeHealth();
        MyHealth.invincible = false;
    }


    private void StagingEnter(Dictionary<string, object> info)
    {
        MyHealth.invincible = true;

        swayJob.Pause(true);
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
            myTransform.localScale += Vector3.one * (float)Math.Sin(timer * stagingSpeed) * stagingSize * Time.deltaTime;
            yield return null;
        }
        myTransform.localScale = Vector3.one;

        stateMachine.SetState(stage == 2 ? Stage2State : Stage3State);
    }


    private void StagingExit(Dictionary<string, object> info)
    {
        MyHealth.invincible = false;

        stateMachine.SetUpdate(StagingUpdate());
    }


    private void Stage1Enter(Dictionary<string, object> info)
    {
        swayJob = new Job(Sway());
        stateMachine.SetUpdate(Stage1Update());
    }


    private IEnumerator Stage1Update()
    {
        WaitForSeconds wait = new WaitForSeconds(stage1DownTime);
        while (true)
        {
            yield return minionSpawner.Activate(true);
            yield return wait;
            yield return wait;
            yield return plasmaGun.Activate(true);
            yield return plasmaGun.Activate(false);
            yield return wait;
        }
    }


    private void DyingEnter(Dictionary<string, object> info)
    {
        GameObject root = new GameObject();
        root.transform.SetPosRot(myTransform.position, myTransform.rotation);
        myTransform.parent = root.transform;
        myAnimation.Play("Medic_Death");

        stateMachine.SetUpdate(DeathUpdate());
    }


    private IEnumerator DeathUpdate()
    {
        yield return new WaitForSeconds(myAnimation["Medic_Death"].length + 1f);
        DeathEvent.Fire();
        Destroy(gameObject);
    }

    #endregion

    #region Private Methods

    private IEnumerator Sway()
    {
        float timer = 0f;
        while (true)
        {
            timer += deltaTime;
            myTransform.position = startPosition + swayDistance * (float)Math.Sin(timer * swaySpeed) * Vector3.up;
            yield return null;
        }
    }

    #endregion

    #region Event Handlers

    private void OnStageIncrease(object sender, ValueArgs args)
    {
        stateMachine.SetState(StagingState);
    }

    #endregion
}