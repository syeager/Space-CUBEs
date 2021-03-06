﻿// Little Byte Games

using System;
using System.Collections;
using LittleByte.Audio;
using UnityEngine;

namespace SpaceCUBEs
{
    /// <summary>
    /// 
    /// </summary>
    public class MovingShield : Weapon
    {
        #region Public Fields

        public float amp = 1f;
        public float speed;

        public AnimationClip deployClip;
        public AudioPlayer deployAudio;
        public AnimationClip retractClip;

        #endregion

        #region Weapon Overrides

        public override Coroutine Activate(bool pressed)
        {
            if (pressed)
            {
                gameObject.SetActive(true);
                animation.Play(deployClip);
                AudioManager.Play(deployAudio);
                StartCoroutine(Move());
            }
            else if (gameObject.activeSelf)
            {
                StopAllCoroutines();
                StartCoroutine(Retract());
            }

            return null;
        }

        #endregion

        #region Private Methods

        private IEnumerator Move()
        {
            yield return new WaitForSeconds(deployClip.length);
            float timer = 0f;
            while (true)
            {
                myTransform.localPosition = new Vector3(amp * (float)Math.Sin(speed * timer), 0f, myTransform.localPosition.z);
                timer += deltaTime;
                yield return null;
            }
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