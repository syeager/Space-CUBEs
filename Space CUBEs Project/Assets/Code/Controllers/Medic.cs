// Little Byte Games
// Author: Steve Yeager
// Created: 2014.06.25
// Edited: 2014.10.05

using System;
using System.Collections;
using System.Collections.Generic;
using LittleByte.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpaceCUBEs
{
    /// <summary>
    /// Boss for Nebula Forest. Summons minions and buffs them. Has a health stealing laser.
    /// </summary>
    public class Medic : Boss
    {
        #region State Fields

        private const string StagingState = "Staging";
        private const string Stage1State = "Stage1";
        private const string Stage2State = "Stage2";
        private const string Stage3State = "Stage3";

        #endregion

        #region Weapon Fields

        public MinionSpawner minionSpawner;
        public PlasmaMachineGun plasmaGun;
        public WeaponStacker burstCannon;
        public Syringe syringeLaser;

        #endregion

        #region Staging Fields

        public float stagingTime;
        public float stagingSize;
        public float stagingSpeed;

        #endregion

        #region Public Fields

        public float attackBuffer;
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
            minionSpawner.Initialize(this);
            plasmaGun.Initialize(this);
            burstCannon.Initialize(this);
            syringeLaser.Initialize(this);
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
            MyMotor.Stop();
            MyHealth.invincible = false;
        }


        private void StagingEnter(Dictionary<string, object> info)
        {
            MyHealth.invincible = true;

            swayJob.Pause(true);
            StopAllCoroutines();
            plasmaGun.Activate(false);

            minionSpawner.BuffHealth();

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
            swayJob = new Job(Sway());
            stateMachine.SetUpdate(Stage1Update());
        }


        private IEnumerator Stage1Update()
        {
            WaitForSeconds wait = new WaitForSeconds(attackBuffer);
            while (true)
            {
                yield return minionSpawner.Spawn(0);
                yield return wait;
                yield return minionSpawner.BuffHealth();
                yield return wait;
                yield return plasmaGun.Activate(true);
                yield return plasmaGun.Activate(false);
                yield return wait;
                yield return minionSpawner.BuffHealth();
                yield return wait;
            }
        }


        private void Stage2Enter(Dictionary<string, object> info)
        {
            swayJob.Pause(false);
            stateMachine.SetUpdate(Stage2Update());
        }


        private IEnumerator Stage2Update()
        {
            WaitForSeconds wait = new WaitForSeconds(attackBuffer);
            while (true)
            {
                yield return burstCannon.Activate(true);
                yield return wait;
                yield return minionSpawner.Spawn(1);
                yield return wait;
                if (Random.Range(0, 2) == 0)
                {
                    yield return minionSpawner.BuffHealth();
                }
                else
                {
                    yield return minionSpawner.BuffShield();
                }
                yield return plasmaGun.Activate(true);
                yield return plasmaGun.Activate(false);
                yield return wait;
                if (Random.Range(0, 2) == 0)
                {
                    yield return minionSpawner.BuffHealth();
                }
                else
                {
                    yield return minionSpawner.BuffShield();
                }
                yield return wait;
            }
        }


        private void Stage3Enter(Dictionary<string, object> info)
        {
            swayJob.Pause(false);
            stateMachine.SetUpdate(Stage3Update());
        }


        private IEnumerator Stage3Update()
        {
            WaitForSeconds wait = new WaitForSeconds(attackBuffer);
            while (true)
            {
                yield return syringeLaser.Activate(true);
                yield return syringeLaser.Activate(false);
                yield return wait;
                yield return minionSpawner.Spawn(2);
                yield return minionSpawner.BuffShield();
                yield return wait;
                yield return burstCannon.Activate(true);
                yield return minionSpawner.BuffHealth();
                yield return wait;
                yield return plasmaGun.Activate(true);
                yield return plasmaGun.Activate(false);
                yield return wait;
                if (Random.Range(0, 2) == 0)
                {
                    yield return minionSpawner.BuffHealth();
                }
                else
                {
                    yield return minionSpawner.BuffShield();
                }
                yield return wait;
            }
        }


        private void DyingEnter(Dictionary<string, object> info)
        {
            MyHealth.invincible = true;

            GameObject root = new GameObject();
            root.transform.SetPosRot(myTransform.position, myTransform.rotation);
            myTransform.parent = root.transform;
            if (swayJob != null)
            {
                swayJob.Kill();
            }
            myAnimation.Play("Switchblade_Death");

            // kill minions
            minionSpawner.KillAll();

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
}