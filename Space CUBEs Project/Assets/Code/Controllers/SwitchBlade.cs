// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.03.25
// Edited: 2014.07.07

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

/// <summary>
/// AI for first boss.
/// </summary>
public class SwitchBlade : Boss
{
    #region References

    private Animation myAnimation;
    public OscillatingLaser[] lasers = new OscillatingLaser[2];
    public SidewinderMissileLauncher[] missileLaunchers = new SidewinderMissileLauncher[2];
    public MovingShield shield;
    public WeaponStacker burstCannon;
    public DeathLaser deathLaser;

    #endregion

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
    private bool doorsOpen;

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
        stateMachine.CreateState(Stage1State, Stage1Enter, i => { });
        stateMachine.CreateState(Stage2State, Stage2Enter, Stage2Exit);
        stateMachine.CreateState(Stage3State, Stage3Enter, i => { });
        stateMachine.CreateState(DyingState, DyingEnter, i => { });

        // stages
        NextStageEvent += OnStageIncrease;

        // weapons
        lasers[0].Initialize(this);
        lasers[1].Initialize(this);
        missileLaunchers[0].Initialize(this);
        missileLaunchers[1].Initialize(this);
        shield.Initialize(this);
        burstCannon.Initialize(this);
        deathLaser.Initialize(this);
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
        moveJob.Pause(true);

        DeactivateWeapons(false);
        if (doorsOpen)
        {
            myAnimation.Play("Doors_Close");
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
            myTransform.localScale += Vector3.one * (float)Math.Sin(timer * stagingSpeed) * stagingSize * Time.deltaTime;
            yield return null;
        }
        myTransform.localScale = Vector3.one;

        stateMachine.SetState(CurrentStage == 2 ? Stage2State : Stage3State);
    }


    private void StagingExit(Dictionary<string, object> info)
    {
        MyHealth.invincible = false;
        moveJob.Pause(false);

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
            yield return StartCoroutine(FireSideWeapons(false));
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
        shieldJob.Pause(true);
    }


    private void Stage3Enter(Dictionary<string, object> info)
    {
        stateMachine.SetUpdate(Stage3Update());
        shieldJob.Pause(false);
    }


    private IEnumerator Stage3Update()
    {
        // death laser
        yield return StartCoroutine(FireDeathLaser());

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

            // death laser
            yield return StartCoroutine(FireDeathLaser());
        }
    }


    private void DyingEnter(Dictionary<string, object> info)
    {
        if (moveJob != null) moveJob.Kill();
        if (shieldJob != null) shieldJob.Kill();


        // clean up
        DeactivateWeapons(true);

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

    private void DeactivateWeapons(bool disable)
    {
        foreach (OscillatingLaser laser in lasers)
        {
            laser.Activate(false, 0f);
            if (disable) laser.gameObject.SetActive(false);
        }

        foreach (SidewinderMissileLauncher missileLauncher in missileLaunchers)
        {
            missileLauncher.Activate(false, 0f);
            if (disable) missileLauncher.gameObject.SetActive(false);
        }

        shield.Activate(false);
        if (disable) shield.gameObject.SetActive(false);

        burstCannon.Activate(false);
        if (disable) burstCannon.gameObject.SetActive(false);

        deathLaser.Activate(false, 0f);
        if (disable) deathLaser.gameObject.SetActive(false);
    }


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
        int leftWeapon = Random.Range(0, 2);
        int rightWeapon = Random.Range(0, 2);

        // open
        if (controlMovement)
        {
            moveJob.Pause(true);
        }

        ActivateSideWeapon(false, leftWeapon, true, stage1SwitchTime);
        ActivateSideWeapon(true, rightWeapon, true, stage1SwitchTime);
        yield return new WaitForSeconds(stage1SwitchTime);

        // fire
        if (controlMovement)
        {
            moveJob.Pause(false);
        }
        yield return new WaitForSeconds(stage1AttackTime);
        ActivateSideWeapon(false, leftWeapon, false, 0f);
        ActivateSideWeapon(true, rightWeapon, false, 0f);

        // close
        if (controlMovement)
        {
            moveJob.Pause(true);
        }
        yield return new WaitForSeconds(stage1SwitchTime);
        if (controlMovement)
        {
            moveJob.Pause(false);
        }
    }


    private IEnumerator FirePattern(bool controlMovement)
    {
        // open
        if (controlMovement)
        {
            moveJob.Pause(true);
        }

        // deploy
        myAnimation.Play("Doors_Open");
        doorsOpen = true;
        burstCannon.Activate(true);
        yield return new WaitForSeconds(bulletEmitterTime);

        // close
        myAnimation.Play("Doors_Close");
        doorsOpen = false;
        burstCannon.Activate(false);
        yield return new WaitForSeconds(stage1SwitchTime);
        if (controlMovement)
        {
            moveJob.Pause(false);
        }
    }


    private IEnumerator Shield()
    {
        WaitForSeconds activated = new WaitForSeconds(shieldTime);
        WaitForSeconds deactivated = new WaitForSeconds(shieldBuffer);

        while (true)
        {
            // activate
            shield.Activate(true);
            yield return activated;

            // close
            shield.Activate(false);
            yield return deactivated;
        }
    }


    private IEnumerator FireDeathLaser()
    {
        // open
        moveJob.Pause(true);
        myAnimation.Play("Doors_Open");
        doorsOpen = true;
        yield return deathLaser.Activate(true);
        yield return new WaitForSeconds(stage1SwitchTime);

        // close
        myAnimation.Play("Doors_Close");
        doorsOpen = false;
        deathLaser.Activate(false);
        yield return new WaitForSeconds(stage1SwitchTime);
        moveJob.Pause(false);
    }


    private void ActivateSideWeapon(bool right, int weapon, bool pressed, float deployTime)
    {
        int side = right ? 1 : 0;
        if (weapon == 0)
        {
            lasers[side].Activate(pressed, deployTime);
        }
        else
        {
            missileLaunchers[side].Activate(pressed, deployTime);
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