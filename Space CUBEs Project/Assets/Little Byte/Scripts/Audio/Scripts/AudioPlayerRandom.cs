// Little Byte Games

using Annotations;
using UnityEngine;

namespace LittleByte.Audio
{
    public class AudioPlayerRandom : AudioPlayerVariation
    {
        #region Public Fields

        public AudioClip[] clips;

        #endregion

        #region AudioPlayerVariation Overrides

        public override void Play(float level, bool muted, float? levelScale = null)
        {
            myAudio.clip = clips[Random.Range(0, clips.Length)];
            base.Play(level, muted, levelScale);
        }

        #endregion
    }
}