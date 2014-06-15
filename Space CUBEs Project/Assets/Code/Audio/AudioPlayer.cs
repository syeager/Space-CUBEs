// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.05.17
// Edited: 2014.06.14

using System;
using UnityEngine;

namespace LittleByte.Pools
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : PoolObject
    {
        #region References

        [HideInInspector]
        public AudioSource myAudio;
        [HideInInspector]
        public Transform myTransform;

        #endregion

        #region Public Fields

        public AudioManager.Bus bus;         

        #endregion

        #region Private Fields

        private float volumeScale;

        #endregion

        #region Public Methods

        public void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume, float volumeScale)
        {
            myTransform.position = position;
        }


        public void Play(float volumeScale, float volume, bool mute)
        {
            this.volumeScale = volumeScale;
            myAudio.mute = mute;
            myAudio.volume = volumeScale * volume;
            myAudio.Play();
            Invoke("Disable", myAudio.clip.length);
        }


        public void SetLevel(float volume)
        {
            myAudio.volume = volumeScale * volume;
        }


        public void SetMuted(bool mute)
        {
            myAudio.mute = mute;
        }


        public void UpdateVolume(float volume, bool muted)
        {
            myAudio.volume = volumeScale * volume;
            myAudio.mute = muted;
        }

        #endregion
        
    }
}