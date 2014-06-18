// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.03.26
// Edited: 2014.06.15

using System;
using System.Collections.Generic;
using Annotations;
using LittleByte.Data;
using UnityEngine;

/// <summary>
/// Singleton to play audio.
/// </summary>
public class AudioManager : Singleton<AudioManager>
{
    #region Public Fields

    /// <summary>Different catagories of audio. Have master controls for each bus.</summary>
    public enum Bus
    {
        SFX,
        Music,
        UI
    };

    /// <summary>All active players grouped under their respective bus.</summary>
    public Dictionary<Bus, HashSet<AudioPlayer>> activePlayers = new Dictionary<Bus, HashSet<AudioPlayer>>();

    /// <summary>Volume that effects all audio.</summary>
    public Volume MasterVolume { get; private set; }

    /// <summary>Buses and their volume levels.</summary>
    public Dictionary<Bus, Volume> busVolumes = new Dictionary<Bus, Volume>();

    #endregion

    #region Private Fields

    /// <summary>Pool Manager for AudioPlayers.</summary>
    public PoolManager poolManager;

    #endregion

    #region Const Fields

    /// <summary>Data folder for all volume settings.</summary>
    private const string VolumeFolder = @"Volume\";

    /// <summary>Data file for saving master volume.</summary>
    private const string MasterFile = "MasterVolume";

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Reset()
    {
        poolManager = ScriptableObject.CreateInstance<PoolManager>();
        poolManager.parent = transform;
        Resources.UnloadUnusedAssets();
    }


    protected override void Awake()
    {
        base.Awake();

        if (!enabled) return;

        Initialize();
        poolManager.Initialize();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Load data and create activePlayers.
    /// </summary>
    public void Initialize()
    {
        Load();

        activePlayers = new Dictionary<Bus, HashSet<AudioPlayer>>();
        foreach (int audioGroup in Enum.GetValues(typeof(Bus)))
        {
            activePlayers[(Bus)audioGroup] = new HashSet<AudioPlayer>();
        }
    }


    /// <summary>
    /// Load all volumes and mute statuses from data.
    /// </summary>
    public void Load()
    {
        MasterVolume = SaveData.Load(MasterFile, VolumeFolder, new Volume(0.5f, false));

        busVolumes = new Dictionary<Bus, Volume>();
        foreach (var value in Enum.GetValues(typeof(Bus)))
        {
            Bus bus = (Bus)value;
            busVolumes.Add(bus, SaveData.Load(bus.ToString(), VolumeFolder, new Volume(0.5f, false)));
        }
    }


    /// <summary>
    /// Save master volume and bus volumes.
    /// </summary>
    public void Save()
    {
        SaveData.Save(MasterFile, MasterVolume, VolumeFolder);
        foreach (var value in Enum.GetValues(typeof(Bus)))
        {
            Bus bus = (Bus)value;
            SaveData.Save(bus.ToString(), busVolumes[bus], VolumeFolder);
        }
    }


    /// <summary>
    /// Play audio clip.
    /// </summary>
    /// <param name="audioPlayer">AudioPlayer to play.</param>
    /// <param name="levelScale">Level scale to give to AudioPlayer.</param>
    /// <returns>AudioPlayer being played.</returns>
    public AudioPlayer play(AudioPlayer audioPlayer, float? levelScale = null)
    {
        Bus playerGroup = audioPlayer.bus;

        AudioPlayer player = poolManager.Pop(audioPlayer).GetComponent(typeof(AudioPlayer)) as AudioPlayer;

        activePlayers[playerGroup].Add(player);
        player.DisableEvent += OnAudioDone;

        player.Play(busVolumes[playerGroup] * MasterVolume, busVolumes[playerGroup] || MasterVolume, levelScale);
        return player;
    }


    /// <summary>
    /// Set the master volume level.
    /// </summary>
    /// <param name="level">New master volume level. 0-1.</param>
    public void setMasterLevel(float level)
    {
        MasterVolume.level = Mathf.Clamp01(level);
        UpdateActivePlayers();
    }


    /// <summary>
    /// Set the master volume mute status.
    /// </summary>
    /// <param name="muted">Should the master volume be muted?</param>
    public void setMasterMute(bool muted)
    {
        MasterVolume.muted = muted;
        UpdateActivePlayers();
    }


    /// <summary>
    /// Set the volume level for a bus.
    /// </summary>
    /// <param name="bus">Bus to update.</param>
    /// <param name="level">Volume level to give the bus. 0-1.</param>
    public void setBusLevel(Bus bus, float level)
    {
        level = Mathf.Clamp01(level);

        busVolumes[bus].level = level;

        float newVolume = level * MasterVolume.level;
        foreach (AudioPlayer activePlayer in activePlayers[bus])
        {
            activePlayer.SetLevel(newVolume);
        }
    }


    /// <summary>
    /// Set the mute status for a bus.
    /// </summary>
    /// <param name="bus">Bus to update.</param>
    /// <param name="muted">Should the bus be muted?</param>
    public void setBusMute(Bus bus, bool muted)
    {
        busVolumes[bus].muted = muted;

        bool newMuted = muted || MasterVolume;
        foreach (AudioPlayer activePlayer in activePlayers[bus])
        {
            activePlayer.SetMuted(newMuted);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Update all active audio players with current level and mute settings.
    /// </summary>
    private void UpdateActivePlayers()
    {
        foreach (var value in Enum.GetValues(typeof(Bus)))
        {
            Bus bus = (Bus)value;
            float level = MasterVolume * busVolumes[bus];
            bool muted = MasterVolume || busVolumes[bus];
            foreach (var activePlayer in activePlayers[bus])
            {
                activePlayer.UpdateVolume(level, muted);
            }
        }
    } 

    #endregion

    #region Static Methods

    /// <summary>
    /// Play audio clip. Calls on Main.
    /// </summary>
    /// <param name="audioPlayer">AudioPlayer to play.</param>
    /// <param name="levelScale">Level scale to give to AudioPlayer.</param>
    /// <returns>AudioPlayer being played.</returns>
    public static AudioPlayer Play(AudioPlayer audioPlayer, float? levelScale = null)
    {
        return audioPlayer == null ? null : Main.play(audioPlayer, levelScale);
    }


    /// <summary>
    /// Set the master volume level. Calls on Main.
    /// </summary>
    /// <param name="level">New master volume level. 0-1.</param>
    public static void SetMasterLevel(float level)
    {
        Main.setMasterLevel(level);
    }


    /// <summary>
    /// Set the master volume mute status. Calls on Main.
    /// </summary>
    /// <param name="muted">Should the master volume be muted?</param>
    public static void SetMasterMute(bool muted)
    {
        Main.setMasterMute(muted);
    }


    /// <summary>
    /// Set the volume level for a bus. Calls on Main.
    /// </summary>
    /// <param name="bus">Bus to update.</param>
    /// <param name="level">Volume level to give the bus. 0-1.</param>
    public static void SetBusLevel(Bus bus, float level)
    {
        Main.setBusLevel(bus, level);
    }


    /// <summary>
    /// Set the mute status for a bus. Calls on Main.
    /// </summary>
    /// <param name="bus">Bus to update.</param>
    /// <param name="muted">Should the bus be muted?</param>
    public static void SetBusMute(Bus bus, bool muted)
    {
        Main.setBusMute(bus, muted);
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handler for audio player being disabled/completed.
    /// </summary>
    /// <param name="sender">AudioPlayer.</param>
    /// <param name="args">Not used.</param>
    public void OnAudioDone(object sender, EventArgs args)
    {
        AudioPlayer player = (AudioPlayer)sender;
        player.DisableEvent -= OnAudioDone;
        activePlayers[player.bus].Remove(player);
    }

    #endregion
}