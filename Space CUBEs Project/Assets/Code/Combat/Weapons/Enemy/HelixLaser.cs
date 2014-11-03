// Little Byte Games

using System;
using System.Collections;
using UnityEngine;

namespace SpaceCUBEs
{
    // Space CUBEs Project-csharp
    // Author: Steve Yeager
    // Created: 2014.07.19
    // Edited: 2014.07.19
    
    /// <summary>
    /// 
    /// </summary>
    public class HelixLaser : Weapon
    {
        #region Public Fields

        public PoolObject laserPrefab;

        public float damage;

        public float attackTime;

        public float frequency;

        public float amplitude;

        public float speed;

        public float fireRate;

        #endregion

        #region Weapon Overrides

        public override Coroutine Activate(bool pressed)
        {
            if (pressed)
            {
                return StartCoroutine(Fire());
            }

            StopAllCoroutines();
            return null;
        }

        #endregion

        #region Private Methods

        private IEnumerator Fire()
        {
            float cannon = 0f;
            float attackTimer = 1f / fireRate;
            for (float timer = 0; timer < attackTime; timer += deltaTime)
            {
                // attack
                attackTimer -= deltaTime;
                if (attackTimer <= 0f)
                {
                    attackTimer = 1f / fireRate;

                    // fire 1
                    Prefabs.Pop(laserPrefab, myTransform.position + myTransform.right * cannon, myTransform.rotation).GetComponent<Hitbox>().Initialize(myShip, damage, myTransform.forward * speed);
                    // fire 2
                    Prefabs.Pop(laserPrefab, myTransform.position + myTransform.right * -cannon, myTransform.rotation).GetComponent<Hitbox>().Initialize(myShip, damage, myTransform.forward * speed);
                }

                // move
                cannon = amplitude * (float)Math.Sin(timer * frequency);

                yield return null;
            }
        }

        #endregion
    }
}