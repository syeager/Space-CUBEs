// Little Byte Games

using Annotations;
using UnityEngine;

namespace LittleByte.Audio
{
    public class AudioPlayerVariation : AudioPlayer
    {
        #region Private Fields

        [SerializeField, UsedImplicitly]
        private bool randomVolume = true;

        [SerializeField, UsedImplicitly]
        [Range(0, 1)]
        private float volumeVar = 0.2f;

        [SerializeField, UsedImplicitly]
        private bool randomPitch = true;

        [SerializeField, UsedImplicitly]
        [Range(0, 3)]
        private float pitchMin = 0.75f;

        [SerializeField, UsedImplicitly]
        [Range(0, 3)]
        private float pitchMax = 1.5f;

        #endregion

        #region AudioPlayer Overrides

        public override void PlayClipAtPoint(Vector3 position, float level, bool muted, float? levelScale = null)
        {
            if (randomVolume && levelScale != null) levelScale = RandomVolume(levelScale.Value);
            if (randomPitch) RandomPitch();

            base.PlayClipAtPoint(position, level, muted, levelScale);
        }

        public override void Play(float level, bool muted, float? levelScale = null)
        {
            if (randomVolume && levelScale != null) levelScale = RandomVolume(levelScale.Value);
            if (randomPitch) RandomPitch();

            base.Play(level, muted, levelScale);
        }

        #endregion

        #region Private Methods

        private float RandomVolume(float passedLevelScale)
        {
            return levelScale + Random.Range(-volumeVar, volumeVar);
        }

        private void RandomPitch()
        {
            myAudio.pitch = Random.Range(pitchMin, pitchMax);
        }

        #endregion
    }
}