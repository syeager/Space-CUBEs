// Little Byte Games
// Author: Steve Yeager
// Created: 2014.08.14
// Edited: 2014.09.08

using UnityEngine;
using System.Collections;

namespace SpaceCUBEs
{
    public class LaserLock : Weapon
    {
        #region Public Fields

        public PoolObject laserPrefab;
        public Transform barrel;
        public float damage;
        public float speed;
        public float freezeTime;
        public float rotationSpeed;
        public float rotationTime;

        #endregion

        #region Weapon Overrides

        public override Coroutine Activate(bool pressed)
        {
            if (pressed)
            {
                return StartCoroutine(Fire());
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Public Methods

        public void Freeze(bool freeze)
        {
            if (freeze)
            {
                StartCoroutine(FreezePlayer());
            }
            else
            {
                StopAllCoroutines();
                LevelManager.Main.PlayerController.MyMotor.enabled = true;
            }
        }

        #endregion

        #region Private Methods

        private IEnumerator Fire()
        {
            yield return StartCoroutine(Target());
            yield return new WaitForSeconds(1f);
            Prefabs.Pop(laserPrefab, barrel.position, barrel.rotation).GetComponent<LaserLockLaser>().Initialize(myShip, damage, speed * barrel.forward);
            yield return new WaitForSeconds(1f);
        }


        private IEnumerator Target()
        {
            Transform player = LevelManager.Main.PlayerTransform;
            for (float timer = 0f; timer < rotationTime; timer += deltaTime)
            {
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(myTransform.position.To(player.position), myTransform.up), rotationSpeed * deltaTime);
                yield return null;
            }
        }


        private IEnumerator FreezePlayer()
        {
            LevelManager.Main.PlayerController.MyMotor.enabled = false;
            yield return new WaitForSeconds(freezeTime);
            LevelManager.Main.PlayerController.MyMotor.enabled = true;
        }

        #endregion
    }
}