// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.16
// Edited: 2014.06.16

using System.Timers;
using Annotations;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[CustomEditor(typeof(AudioPlayer), true)]
public class AudioPlayerEditor : Editor
{
    #region Private Fields

    private GameObject sampler;
    private bool destroySampler;
    private Timer timer;

    #endregion

    #region Editor Overrides

    public override void OnInspectorGUI()
    {
        if (Event.current.isKey && Event.current.keyCode == KeyCode.Space && Event.current.type == EventType.KeyDown)
        {
            if (sampler != null)
            {
                Stop();
            }
            else
            {
                Play();
            }
        }
        else if (destroySampler)
        {
            Stop();
        }

        DrawDefaultInspector();

        if (sampler == null)
        {
            if (GUILayout.Button("Sample"))
            {
                Play();
            }
        }
        else
        {
            if (GUILayout.Button("Stop"))
            {
                DestroyImmediate(sampler);
            }
        }
    }


    [UsedImplicitly]
    private void OnDisable()
    {
        Stop();
    }

    #endregion

    #region Private Methods

    private void Play()
    {
        sampler = Instantiate(((AudioPlayer)target).gameObject) as GameObject;
        AudioPlayer player = sampler.GetComponent<AudioPlayer>();
        player.Play(player.audio.volume, false, player.levelScale);

        if (!player.audio.loop)
        {
            float time = player.audio.clip.length * 1000;
            timer = new Timer(time) {AutoReset = false};
            timer.Elapsed += (sender, args) => destroySampler = true;
            timer.Start();
        }

        Repaint();
    }


    private void Stop()
    {
        if (timer != null) timer.Stop();
        DestroyImmediate(sampler);
        destroySampler = false;

        Repaint();
    }

    #endregion
}