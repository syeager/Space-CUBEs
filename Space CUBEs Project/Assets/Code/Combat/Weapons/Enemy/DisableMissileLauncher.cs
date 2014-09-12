// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.07.16
// Edited: 2014.07.17

using UnityEngine;
using System.Collections;

namespace SpaceCUBEs
{
    /// <summary>
    /// 
    /// </summary>
    public class DisableMissileLauncher : Weapon
    {
        #region Public Fields

        public DisableMissile missilePrefab;

        /// <summary>The amount of missiles to fire each stage.</summary>
        public int[] missiles;

        /// <summary>Time in seconds to disable a player's weapon.</summary>
        public float disableTime;

        /// <summary>Time in seconds to wait before firing more missiles.</summary>
        public float fireBuffer;

        /// <summary>Amount of damage to do.</summary>
        public float damage;

        /// <summary>Local offset to create missile.</summary>
        public Vector3 offset;

        #endregion

        #region Weapon Overrides

        public Coroutine Activate(bool pressed, int stage)
        {
            if (pressed)
            {
                return StartCoroutine(Fire(stage - 1));
            }
            else if (gameObject.activeInHierarchy)
            {
                StopAllCoroutines();
            }

            return null;
        }

        #endregion

        #region Private Methods

        private IEnumerator Fire(int stage)
        {
            WaitForSeconds wait = new WaitForSeconds(fireBuffer);
            for (int i = 0; i < missiles[stage]; i++)
            {
                Prefabs.Pop(missilePrefab.myPoolObject, myTransform.position + myTransform.TransformDirection(offset), myTransform.rotation).
                        GetComponent<DisableMissile>().Initialize(myShip, damage, disableTime, i);

                yield return wait;
            }
        }

        #endregion
    } 
}