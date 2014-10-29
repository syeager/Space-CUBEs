// Little Byte Games
// Author: Steve Yeager
// Created: 2014.06.22
// Edited: 2014.10.26

using System.Collections;
using System.Collections.Generic;
using Annotations;
using UnityEngine;

namespace SpaceCUBEs
{
    /// <summary>
    /// Pops in, takes a couple shot at the player, then leaves.
    /// </summary>
    public class Hornet : Enemy
    {
        #region State Names

        private const string SpawningState = "Spawning";
        private const string EnteringState = "Entering";
        private const string AttackingState = "Attacking";
        private const string ExitingState = "Exiting";

        #endregion

        #region Attacking Fields

        public Weapon laser;

        /// <summary>Time in seconds to sit and attack.</summary>
        public float attackingTime = 3f;

        [SerializeField, UsedImplicitly]
        private int attacks = 2;

        [SerializeField, UsedImplicitly]
        private float attackDelay;

        #endregion

        #region MonoBehaviour Overrides

        protected override void Awake()
        {
            base.Awake();

            // states
            stateMachine = new StateMachine(this, SpawningState);
            stateMachine.CreateState(SpawningState, SpawningEnter, info => { });
            stateMachine.CreateState(EnteringState, info => stateMachine.SetUpdate(EnteringUpdate()), info => { });
            stateMachine.CreateState(AttackingState, info => stateMachine.SetUpdate(AttackingUpdate()), info => { });
            stateMachine.CreateState(ExitingState, info => stateMachine.SetUpdate(ExitingUpdate()), info => { });
            stateMachine.CreateState(DyingState, DyingEnter, info => { });

            // weapons
            laser.Initialize(this);
        }

        #endregion

        #region State Methods

        private void SpawningEnter(Dictionary<string, object> info)
        {
            path = (Path)info["path"];
            MyHealth.Initialize();
            path.Initialize(myTransform);

            stateMachine.SetState(EnteringState);
        }


        private IEnumerator EnteringUpdate()
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


        private IEnumerator AttackingUpdate()
        {
            for (int i = 0; i < attacks; i++)
            {
                yield return laser.Activate(true);
                laser.Activate(false);
                yield return new WaitForSeconds(attackDelay);
            }

            stateMachine.SetState(ExitingState);
        }


        private IEnumerator ExitingUpdate()
        {
            while (true)
            {
                MyMotor.Move((Vector2)path.Direction(deltaTime));
                yield return null;
            }
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