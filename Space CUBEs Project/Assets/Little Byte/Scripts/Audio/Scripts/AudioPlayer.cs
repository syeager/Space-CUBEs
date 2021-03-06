﻿// Little Byte Games

using UnityEngine;

namespace LittleByte.Audio
{
    /// <summary>
    /// Plays an audio clip and responds
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : PoolObject
    {
        #region References

        public AudioSource myAudio;
        public Transform myTransform;

        #endregion

        #region Public Fields

        /// <summary>Bus that this clip belongs to.</summary>
        public AudioManager.Bus bus;

        /// <summary>Volume level scale for this clip.</summary>
        public float levelScale = 1f;

        #endregion

        #region Properties

        /// <summary>Audio clip's length in seconds.</summary>
        public float Length
        {
            get { return myAudio.clip.length; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Play audio clip in world position.
        /// </summary>
        /// <param name="position">World position to play clip.</param>
        /// <param name="levelScale">Local level scale to cache.</param>
        /// <param name="level">Volume level from Audio Manager.</param>
        /// <param name="muted">Mute from Audio Manager.</param>
        public virtual void PlayClipAtPoint(Vector3 position, float level, bool muted, float? levelScale = null)
        {
            myTransform.position = position;
            Play(level, muted, levelScale);
        }

        /// <summary>
        /// Play the audio clip.
        /// </summary>
        /// <param name="level">Volume level from Audio Manager.</param>
        /// <param name="muted">Mute from Audio Manager.</param>
        /// <param name="levelScale">Local level scale to cache.</param>
        public virtual void Play(float level, bool muted, float? levelScale = null)
        {
            if (levelScale != null)
            {
                this.levelScale = levelScale.Value;
            }
            myAudio.mute = muted;
            myAudio.volume = this.levelScale * level;
            myAudio.Play();

            if (!myAudio.loop)
            {
                Stop(myAudio.clip.length);
            }
        }

        /// <summary>
        /// Pause/unpause the player.
        /// </summary>
        /// <param name="pause">Should the player be paused?</param>
        public void Pause(bool pause)
        {
            if (pause)
            {
                myAudio.Pause();
            }
            else
            {
                myAudio.Play();
            }
        }

        /// <summary>
        /// Stop playing audio.
        /// </summary>
        public void Stop()
        {
            CancelInvoke();
            myAudio.Stop();
            Disable();
        }

        /// <summary>
        /// Stop playing audio.
        /// </summary>
        /// <param name="delay">Time in seconds to delay the stop call.</param>
        public void Stop(float delay)
        {
            Invoke("Stop", delay);
        }

        /// <summary>
        /// Set the audio's volume level.
        /// </summary>
        /// <param name="level">Volume level from Audio Manager.</param>
        public void SetLevel(float level)
        {
            myAudio.volume = levelScale * level;
        }

        /// <summary>
        /// Set the audio's mute status.
        /// </summary>
        /// <param name="muted">Mute from Audio Manager.</param>
        public void SetMuted(bool muted)
        {
            myAudio.mute = muted;
        }

        /// <summary>
        /// Update volume.
        /// </summary>
        /// <param name="level">Volume level from Audio Manager.</param>
        /// <param name="muted">Mute from Audio Manager.</param>
        public void UpdateVolume(float level, bool muted)
        {
            myAudio.volume = levelScale * level;
            myAudio.mute = muted;
        }

        #endregion
    }
}