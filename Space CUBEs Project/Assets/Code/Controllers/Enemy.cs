// Little Byte Games
// Author: Steve Yeager
// Created: 2013.12.16
// Edited: 2014.09.08

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCUBEs
{
    /// <summary>
    /// Base class for all enemies.
    /// </summary>
    public class Enemy : Ship
    {
        #region References

        protected PoolObject poolObject;

        #endregion

        #region Public Fields

        public enum Classes
        {
            None,
            Grunt,
            Sentry,
            Guard,
            Hornet,
            Kamikaze,
            Bomber,
            Elites,
            SwitchBlade,
            Medic,
            Hacker,
            Twins,
            Instagator,
            Minion
        }

        public Classes enemyClass;
        public int score;
        public int money;

        #endregion

        #region Protected Fields

        protected Path path;
        protected const string LeavingState = "Leaving";

        #endregion

        #region Const Fields

        protected const string LeavingVectorKey = "Leaving Vector";

        #endregion

        #region MonoBehaviour Overrides

        protected override void Awake()
        {
            base.Awake();

            // references
            poolObject = (PoolObject)GetComponent(typeof(PoolObject));
        }


        protected override void Start()
        {
            base.Start();

            // register events
            MyHealth.DieEvent += OnDie;
        }

        #endregion

        #region State Methods

        protected void LeavingEnter(Dictionary<string, object> info)
        {
            StopAllCoroutines();
            stateMachine.SetUpdate(LeavingUpdate((Vector3)info[LeavingVectorKey]));
        }


        private IEnumerator LeavingUpdate(Vector3 exitVector)
        {
            Vector2 move = exitVector;
            while (true)
            {
                MyMotor.Move(move);
                yield return null;
            }
        }

        #endregion

        #region Event Handlers

        private void OnDie(object sender, DieArgs args)
        {
            Player player = args.killer as Player;
            if (player != null)
            {
                player.RecieveKill(enemyClass, score, money, MyHealth.maxHealth);
            }

            Destroy(path);
        }

        #endregion
    }
}