﻿// Little Byte Games
// Author: Steve Yeager
// Created: 2014.04.13
// Edited: 2014.09.08

using System;
using System.Collections;
using System.Collections.Generic;
using Annotations;
using LittleByte.Debug.Attributes;
using UnityEngine;

namespace SpaceCUBEs
{
    /// <summary>
    /// AI for Guard. Stand back and fire straight lasers.
    /// </summary>
    public class Guard : Enemy
    {
        #region Public Fields

        public Weapon laser;

        #endregion

        #region State Fields

        private const string SpawningState = "Spawning";
        private const string MovingState = "Moving";
        private const string AttackingState = "Attacking";
        private const string IdlingState = "Idling";

        #endregion

        #region Attacking Fields

        /// <summary>Seconds in between attacks.</summary>
        public float attackBuffer;

        /// <summary>How long the attack lasts in seconds.</summary>
        public float attackTime;

        #endregion

        #region Idling Fields

        /// <summary>Time in seconds to idle.</summary>
        [GreaterThanZero]
        public float idleCycles = 1f;

        /// <summary>How fast the guard moves while idling. m/s</summary>
        public float idleSpeed;

        /// <summary>Height of the idle curve.</summary>
        public float idleAmp;

        #endregion

        #region Leaving Fields

        [SerializeField, UsedImplicitly]
        private int attacksAllowed;

        private int attacksRemaining;

        #endregion

        #region MonoBehaviour Overrides

        protected override void Awake()
        {
            base.Awake();

            // states
            stateMachine = new StateMachine(this, SpawningState);
            stateMachine.CreateState(SpawningState, SpawnEnter, info => { });
            stateMachine.CreateState(MovingState, info => stateMachine.SetUpdate(MovingUpdate()), info => { });
            stateMachine.CreateState(AttackingState, AttackEnter, info => { });
            stateMachine.CreateState(IdlingState, info => stateMachine.SetUpdate(IdlingUpdate()), info => { });
            stateMachine.CreateState(LeavingState, LeavingEnter, info => { });
            stateMachine.CreateState(DyingState, DyingEnter, info => { });

            // weapons
            laser.Initialize(this);
        }

        #endregion

        #region State Methods

        private void SpawnEnter(Dictionary<string, object> info)
        {
            MyHealth.Initialize();

            attacksRemaining = attacksAllowed;

            // initialize path from level manager
            path = (Path)info["path"];
            path.Initialize(myTransform);

            stateMachine.SetState(MovingState);
        }


        private IEnumerator MovingUpdate()
        {
            while (true)
            {
                Vector2 direction = path.Direction(deltaTime);

                // reached destination
                if (direction == Vector2.zero)
                {
                    stateMachine.SetState(AttackingState);
                    yield break;
                }
                else
                {
                    MyMotor.Move(direction);
                }

                yield return null;
            }
        }


        private void AttackEnter(Dictionary<string, object> info)
        {
            MyMotor.Move(Vector3.zero);
            stateMachine.SetUpdate(AttackingUpdate());
        }


        private IEnumerator AttackingUpdate()
        {
            attacksRemaining--;
            yield return new WaitForSeconds(attackBuffer);
            laser.Activate(true);
            yield return new WaitForSeconds(attackTime);
            laser.Activate(false);
            yield return new WaitForSeconds(attackBuffer / 2f);
            if (attacksRemaining <= 0)
            {
                stateMachine.SetState(LeavingState, new Dictionary<string, object>{{LeavingVectorKey, myTransform.position.y >= 0f ? myTransform.right : -myTransform.right}});
            }
            else
            {
                stateMachine.SetState(IdlingState);
            }
        }


        private IEnumerator IdlingUpdate()
        {
            float period = 2f * Mathf.PI / idleSpeed * idleCycles;
            float timer = 0;
            while (timer < period)
            {
                float time = deltaTime;
                timer += time;
                myTransform.position += new Vector3(0f, idleAmp * (float)Math.Cos(timer * idleSpeed) * time, 0f);
                if (timer > period)
                {
                    float extra = timer - period;
                    timer = period;
                    myTransform.position -= new Vector3(0f, idleAmp * (float)Math.Cos(timer * idleSpeed) * extra, 0f);
                }
                yield return null;
            }

            stateMachine.SetState(AttackingState);
        }


        private void DyingEnter(Dictionary<string, object> info)
        {
            StopAllCoroutines();
            laser.Activate(false);
            poolObject.Disable();
        }

        #endregion
    }
}