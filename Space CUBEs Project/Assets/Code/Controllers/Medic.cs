// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.25
// Edited: 2014.06.25

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// 
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

    #region Staging Fields

    public float stagingTime;
    public float stagingSize;
    public float stagingSpeed;

    #endregion

    #region Public Fields

    public Vector3 startPosition;     

    #endregion

    #region Private Fields

    private List<Health> minions = new List<Health>(); 

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
        InitializeHealth();
        MyHealth.invincible = false;
    }


    private void StagingEnter(Dictionary<string, object> info)
    {
        MyHealth.invincible = true;

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
        stateMachine.SetUpdate(Stage1Update());
    }


    private IEnumerator Stage1Update()
    {
        while (true)
        {

            yield return null;
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

    private IEnumerator ReleaseMinions(int count)
    {
        yield return new WaitForSeconds(count);
    }

    #endregion

    #region Event Handlers

    private void OnStageIncrease(object sender, ValueArgs args)
    {
        stateMachine.SetState(StagingState);
    }

    #endregion
}