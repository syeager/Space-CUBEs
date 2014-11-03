// Little Byte Games
// Author: Steve Yeager
// Created: 2014.07.27
// Edited: 2014.07.27

using UnityEngine;

public class AudioPlayerVariation : AudioPlayer
{
    #region Public Fields

    public bool randomVolume = true;
    public float volumeVar = 0.2f;

    public bool randomPitch = true;
    public float pitchMin = 0.75f;
    public float pitchMax = 1.5f;

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