// Little Byte Games

using System.Collections;
using Annotations;
using LittleByte.Audio;
using UnityEngine;

namespace SpaceCUBEs
{
    // Little Byte Games

    /// <summary>
    /// 
    /// </summary>
    public class DeathLaser : Weapon
    {
        #region Public Fields

        public GameObject laser;
        public float damage;

        /// <summary>Time in seconds to delay firing after deploying.</summary>
        public float fireDelay;

        /// <summary>Deploying animation.</summary>
        public AnimationClip deployClip;

        /// <summary>Audio to play when deploying.</summary>
        public AudioPlayer deployAudio;

        /// <summary>Audio to play while charging.</summary>
        public AudioPlayer chargeAudio;

        /// <summary>Audio to play when firing.</summary>
        public AudioPlayer fireAudio;

        /// <summary>Retracting animation.</summary>
        public AnimationClip retractClip;

        /// <summary>Laser time in seconds.</summary>
        public float fireTime;

        #endregion

        #region Private Fields

        [SerializeField, UsedImplicitly]
        private float retractDelay;

        #endregion

        #region Weapon Overrides

        public override Coroutine Activate(bool pressed)
        {
            if (pressed)
            {
                gameObject.SetActive(true);
                return StartCoroutine(Fire());
            }
            else if (gameObject.activeInHierarchy)
            {
                laser.SetActive(false);
                return StartCoroutine(Retract());
            }

            return null;
        }

        #endregion

        #region Private Methods

        private IEnumerator Fire()
        {
            // deploy
            animation.Play(deployClip);
            yield return new WaitForSeconds(deployClip.length);

            // charge
            AudioManager.Play(chargeAudio);
            yield return new WaitForSeconds(chargeAudio.Length);

            // fire
            laser.GetComponent<Hitbox>().Initialize(myShip, damage);
            laser.SetActive(true);
            AudioPlayer player = AudioManager.Play(fireAudio);
            yield return new WaitForSeconds(fireTime);
            player.Stop();
            laser.SetActive(false);
            yield return new WaitForSeconds(retractDelay);
        }

        private IEnumerator Retract()
        {
            animation.Play(retractClip);
            yield return new WaitForSeconds(retractClip.length);
            gameObject.SetActive(false);
        }

        #endregion
    }
}