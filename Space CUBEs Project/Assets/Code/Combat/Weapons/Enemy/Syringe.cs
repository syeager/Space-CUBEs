// Little Byte Games
// Author: Steve Yeager
// Created: 2014.07.01
// Edited: 2014.09.08

using UnityEngine;
using System.Collections;

namespace SpaceCUBEs
{
    /// <summary>
    /// 
    /// </summary>
    public class Syringe : Weapon
    {
        #region Public Fields

        public SyringeLaser laser;

        /// <summary>Time in seconds to fire laser.</summary>
        public float attackTime;

        /// <summary>Damage done per second.</summary>
        public float damage;

        /// <summary>Time in seconds for deploying the gun.</summary>
        public float deployTime;

        /// <summary>Time in sconds for retracting the gun.</summary>
        public float retractTime;

        /// <summary>How fast to rotate to target player.</summary>
        public float angularSpeed;

        #endregion

        #region Weapon Overrides

        public override Coroutine Activate(bool pressed)
        {
            if (pressed)
            {
                gameObject.SetActive(true);
                StartCoroutine(Target());
                return StartCoroutine(Fire());
            }
            else if (gameObject.activeInHierarchy)
            {
                laser.Stop();
                gameObject.SetActive(false);
            }

            return null;
        }

        #endregion

        #region Private Methods

        private IEnumerator Fire()
        {
            yield return new WaitForSeconds(deployTime);
            laser.Initialize(myShip, damage);
            yield return new WaitForSeconds(attackTime);
            laser.Stop();
            yield return new WaitForSeconds(retractTime);
            gameObject.SetActive(false);
        }


        private IEnumerator Target()
        {
            Transform player = LevelManager.Main.PlayerTransform;
            while (true)
            {
                Quaternion targetRotation = Quaternion.LookRotation(player.position - myTransform.position, myTransform.up);
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, angularSpeed * deltaTime);
                yield return null;
            }
        }

        #endregion
    }
}