// Little Byte Games
// Author: Steve Yeager
// Created: 2013.12.17
// Edited: 2014.09.08

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace SpaceCUBEs
{
    /// <summary>
    /// Basic enemy. Flies forward and shoots Plasma Lasers.
    /// </summary>
    public class Grunt : Enemy
    {
        #region Public Fields

        public float minAttackDelay;
        public float maxAttackDelay;

        public Weapon laser;

        #endregion

        #region Private Fields

        private Job attackCycle;

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

            // weapons
            laser.Initialize(this);
        }

        #endregion

        #region State Methods

        private void SpawnEnter(Dictionary<string, object> info)
        {
            path = info["path"] as Path;
            path.Initialize(myTransform);

            if (attackCycle != null)
            {
                attackCycle.Kill();
            }
            attackCycle = new Job(AttackCycle());

            MyHealth.Initialize();

            stateMachine.SetState(MovingState);
        }


        private IEnumerator MovingUpdate()
        {
            while (true)
            {
                // move
                MyMotor.Move(path.Direction(deltaTime));
                yield return null;
            }
        }


        private void DieEnter(Dictionary<string, object> info)
        {
            attackCycle.Kill();
            poolObject.Disable();
        }

        #endregion

        #region Private Methods

        private IEnumerator AttackCycle()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minAttackDelay, maxAttackDelay));
                laser.Activate(true);
                laser.Activate(false);
            }
        }

        #endregion
    }
}