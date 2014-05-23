// Steve Yeager
// 5.17.2014

using System;
using UnityEngine;
using AudioGroups = AudioManager.AudioGroups;

/// <summary>
/// 
/// </summary>
[Obsolete("Use MasterAudio", true)]
public class AudioPlayer : MonoBehaviour
{
    #region References
    
    public new AudioSource audio;
    public new Transform transform;
    
    #endregion

    #region Private Fields

    private AudioGroups audioGroup;
    private float volumeScale;

    #endregion


    #region Public Methods

    public void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume, float volumeScale)
    {
        transform.position = position;
    }


    public void Play(AudioClip clip, AudioGroups audioGroup, float volumeScale)
    {
        
    }


    public void SetVolume(float volume)
    {
        
    }

    #endregion
}