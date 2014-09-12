// Little Byte Games
// Author: Steve Yeager
// Created: 2014.03.25
// Edited: 2014.09.08

using UnityEngine;
using System.Collections;

namespace SpaceCUBEs
{
    /// <summary>
    /// Missile with a twisted path.
    /// </summary>
    public class SidewinderMissileLauncher : Weapon
    {
        #region Public Fields

        public PoolObject missilePrefab;
        public Vector3[] missilePositions;
        public float missileDelay;
        public float missileSpeed;
        public float rotationSpeed;
        public float homingTime;
        public float damage;

        public AnimationClip deployClip;
        public AnimationClip fireClip;
        public AnimationClip retractClip;

        /// <summary>Audio clip to play when a missile fires.</summary>
        public AudioPlayer fireAudio;

        public int dummyTargets = 5;

        #endregion

        #region Weapon Overrides

        public Coroutine Activate(bool pressed, float deployTime)
        {
            if (pressed)
            {
                gameObject.SetActive(true);
                StartCoroutine(Fire(deployTime));
            }
            else if (gameObject.activeInHierarchy)
            {
                StopAllCoroutines();
                StartCoroutine(Retract());
            }

            return null;
        }

        #endregion

        #region Private Methods

        private IEnumerator Fire(float deployTime)
        {
            // deploy
            animation.Play(deployClip);
            yield return new WaitForSeconds(deployTime);

            // create missiles
            WaitForSeconds wait = new WaitForSeconds(missileDelay);
            foreach (Vector3 position in missilePositions)
            {
                animation.Stop();
                Prefabs.Pop(missilePrefab, myTransform.position + myTransform.TransformDirection(position), myTransform.rotation).
                        GetComponent<SidewinderMissile>().Initialize(myShip, damage, missileSpeed, rotationSpeed, homingTime, dummyTargets, LevelManager.Main.PlayerTransform);

                AudioManager.Play(fireAudio);
                animation.Play(fireClip);

                yield return wait;
            }
        }


        private IEnumerator Retract()
        {
            yield return new WaitForSeconds(animation.Play(retractClip));
            gameObject.SetActive(false);
        }

        #endregion
    }
}