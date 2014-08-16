// Little Byte Games
// Author: Steve Yeager
// Created: 2014.07.15
// Edited: 2014.08.14

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class Hacker : Boss
{
    #region References

    public DisableMissileLauncher[] disableMissileLaunchers;
    public WeaponStacker burstCannon;
    public HelixLaser helixLaser;
    public EMPBlaster empBlaster;
    public LaserLock laserLock;

    #endregion

    #region State Fields

    private const string EnteringState = "Entering";
    private const string StagingState = "Staging";
    private const string Stage1State = "Stage1";
    private const string Stage2State = "Stage2";
    private const string Stage3State = "Stage3";

    #endregion

    #region Enter Fields

    public Vector3 startPosition;

    #endregion

    #region Staging Fields

    public float stagingTime;
    public float stagingSize;
    public float stagingSpeed;

    #endregion

    #region Public Fields

    /// <summary>Bottom-left possible movement position.</summary>
    public Vector3 minBarrier;

    /// <summary>Top-right possible movement position.</summary>
    public Vector3 maxBarrier;

    /// <summary>Time in seconds to delay after each attack each stage.</summary>
    public float[] attackCooldowns = new float[3];

    /// <summary>Target angular speed.</summary>
    public float targetingSpeed;

    #endregion

    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // state machine
        stateMachine = new StateMachine(this, EnteringState);
        stateMachine.CreateState(EnteringState, EnteringEnter, EnteringExit);
        stateMachine.CreateState(StagingState, StagingEnter, StagingExit);
        stateMachine.CreateState(Stage1State, Stage1Enter, info => { });
        stateMachine.CreateState(Stage2State, Stage2Enter, info => { });
        stateMachine.CreateState(Stage3State, Stage3Enter, info => { });
        stateMachine.CreateState(DyingState, DyingEnter, i => { });

        // stages
        NextStageEvent += OnStageIncrease;

        // weapons
        disableMissileLaunchers[0].Initialize(this);
        disableMissileLaunchers[1].Initialize(this);
        burstCannon.Initialize(this);
        helixLaser.Initialize(this);
        empBlaster.Initialize(this);
        laserLock.Initialize(this);
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
            MyMotor.Move(-Vector2.right);
            yield return null;
        }

        yield return BossHUD.Main.Initialize(this);

        stateMachine.SetState(Stage1State);
    }


    private void EnteringExit(Dictionary<string, object> info)
    {
        MyHealth.invincible = false;
    }


    private void StagingEnter(Dictionary<string, object> info)
    {
        MyHealth.invincible = true;

        DeactivateWeapons();
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

        stateMachine.SetState(CurrentStage == 2 ? Stage2State : Stage3State);
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
        WaitForSeconds attackCooldown = new WaitForSeconds(attackCooldowns[0]);

        while (true)
        {
            // missiles, targeted laser
            disableMissileLaunchers[0].Activate(true, CurrentStage);
            disableMissileLaunchers[1].Activate(true, CurrentStage);
            yield return StartCoroutine(Target(LevelManager.Main.PlayerTransform.position));
            yield return helixLaser.Activate(true);
            yield return attackCooldown;

            // move
            yield return StartCoroutine(Move());

            // missiles, straight laser, burst
            disableMissileLaunchers[0].Activate(true, CurrentStage);
            disableMissileLaunchers[1].Activate(true, CurrentStage);
            yield return StartCoroutine(Target(myTransform.position + Vector3.left));
            burstCannon.Activate(true);
            yield return helixLaser.Activate(true);
            yield return attackCooldown;

            // move
            yield return StartCoroutine(Move());
        }
    }


    private void Stage2Enter(Dictionary<string, object> info)
    {
        stateMachine.SetUpdate(Stage2Update());
    }


    private IEnumerator Stage2Update()
    {
        WaitForSeconds attackCooldown = new WaitForSeconds(attackCooldowns[1]);

        yield return empBlaster.Activate(true);

        while (true)
        {
            // missiles, targeted laser
            disableMissileLaunchers[0].Activate(true, CurrentStage);
            disableMissileLaunchers[1].Activate(true, CurrentStage);
            yield return StartCoroutine(Target(LevelManager.Main.PlayerTransform.position));
            yield return helixLaser.Activate(true);
            yield return attackCooldown;

            // move
            yield return StartCoroutine(Move());

            // emp blast
            yield return empBlaster.Activate(true); // TODO: fix emp blast

            // missiles, straight laser, burst
            disableMissileLaunchers[0].Activate(true, CurrentStage);
            disableMissileLaunchers[1].Activate(true, CurrentStage);
            yield return StartCoroutine(Target(myTransform.position + Vector3.left));
            burstCannon.Activate(true);
            yield return helixLaser.Activate(true);
            yield return attackCooldown;

            // emp blast
            yield return empBlaster.Activate(true);

            // move
            yield return StartCoroutine(Move());
        }
    }


    private void Stage3Enter(Dictionary<string, object> info)
    {
        stateMachine.SetUpdate(Stage3Update());
    }


    private IEnumerator Stage3Update()
    {
        WaitForSeconds attackCooldown = new WaitForSeconds(attackCooldowns[1]);

        yield return laserLock.Activate(true);

        while (true)
        {
            // missiles, targeted laser
            disableMissileLaunchers[0].Activate(true, CurrentStage);
            disableMissileLaunchers[1].Activate(true, CurrentStage);
            yield return StartCoroutine(Target(LevelManager.Main.PlayerTransform.position));
            yield return helixLaser.Activate(true);
            yield return attackCooldown;

            // emp blast
            yield return empBlaster.Activate(true);

            // move
            yield return StartCoroutine(Move());

            // laser lock
            yield return laserLock.Activate(true); 

            // missiles, straight laser, burst
            disableMissileLaunchers[0].Activate(true, CurrentStage);
            disableMissileLaunchers[1].Activate(true, CurrentStage);
            yield return StartCoroutine(Target(myTransform.position + Vector3.left));
            burstCannon.Activate(true);
            yield return helixLaser.Activate(true);
            yield return attackCooldown;

            // move
            yield return StartCoroutine(Move());

            // laser lock
            yield return laserLock.Activate(true);

            // emp blast
            yield return empBlaster.Activate(true);

            // missiles, straight laser, burst
            disableMissileLaunchers[0].Activate(true, CurrentStage);
            disableMissileLaunchers[1].Activate(true, CurrentStage);
            yield return StartCoroutine(Target(myTransform.position + Vector3.left));
            burstCannon.Activate(true);
            yield return helixLaser.Activate(true);
            yield return attackCooldown;

            // move
            yield return StartCoroutine(Move());
        }
    }


    private void DyingEnter(Dictionary<string, object> info)
    {
        DeactivateWeapons();
        StopAllCoroutines();

        GameObject root = new GameObject();
        root.transform.SetPosRot(myTransform.position, myTransform.rotation);
        myTransform.parent = root.transform;
        myAnimation.Play("Switchblade_Death");

        stateMachine.SetUpdate(DeathUpdate());
    }


    private IEnumerator DeathUpdate()
    {
        yield return new WaitForSeconds(myAnimation["Switchblade_Death"].length + 1f);
        DeathEvent.Fire();
        Destroy(gameObject);
    }

    #endregion

    #region Private Methods

    private IEnumerator Move()
    {
        const float distBuffer = 2f;
        Vector3 targetPosition = Utility.RandomVector3(minBarrier, maxBarrier);
        while (Vector3.Distance(myTransform.position, targetPosition) >= distBuffer)
        {
            MyMotor.Move((Vector2)myTransform.position.To(targetPosition));
            yield return null;
        }
    }


    private IEnumerator Target(Vector3 targetPosition)
    {
        Quaternion targetRotation = Quaternion.LookRotation(myTransform.position.To(targetPosition), Vector3.back);
        while (Mathf.Abs(Quaternion.Angle(myTransform.rotation, targetRotation)) > 1f)
        {
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, targetingSpeed * deltaTime);
            yield return null;
        }
    }


    private void DeactivateWeapons()
    {
        disableMissileLaunchers[0].Activate(false);
        disableMissileLaunchers[1].Activate(false);
        burstCannon.Activate(false);
        helixLaser.Activate(false);
        laserLock.Activate(false);
    }

    #endregion

    #region Event Handlers

    private void OnStageIncrease(object sender, ValueArgs args)
    {
        stateMachine.SetState(StagingState);
    }

    #endregion
}