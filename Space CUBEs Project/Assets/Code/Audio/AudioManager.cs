// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.03.26
// Edited: 2014.06.15

using System;
using System.Collections.Generic;
using System.Linq;
using Annotations;
using LittleByte.Data;
using LittleByte.Pools;
using UnityEngine;

/// <summary>
/// Singleton to play audio.
/// </summary>
[Serializable]
public class AudioManager : Singleton<AudioManager>
{
    #region Public Fields

    /// <summary>Different catagories of audio. Have master controls for each bus.</summary>
    public enum Bus
    {
        Music,
        SFX,
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
    [SerializeField]
    public PoolManager poolManager;

    #endregion

    #region Const Fields

    private const string VolumeFolder = @"Volume\";

    private const string MasterFile = "MasterVolume";

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Reset()
    {
        poolManager = ScriptableObject.CreateInstance<PoolManager>();
        //Initialize();
    }


    protected override void Awake()
    {
        base.Awake();

        if (!enabled) return;

        Initialize();
        poolManager.Initialize();
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// 
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
        MasterVolume = SaveData.Load<Volume>(MasterFile, VolumeFolder, new Volume());

        busVolumes = new Dictionary<Bus, Volume>();
        foreach (var value in Enum.GetValues(typeof(Bus)))
        {
            Bus bus = (Bus)value;
            busVolumes.Add(bus, SaveData.Load<Volume>(bus.ToString(), VolumeFolder, new Volume()));
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
    /// Set the master volume level.
    /// </summary>
    /// <param name="level">New master volume level. 0-1.</param>
    public void SetMasterLevel(float level)
    {
        MasterVolume.level = Mathf.Clamp01(level);
        UpdateActivePlayers();
    }


    /// <summary>
    /// Set the master volume mute status.
    /// </summary>
    /// <param name="muted">Should the master volume be muted?</param>
    public void SetMasterMute(bool muted)
    {
        MasterVolume.muted = muted;
        UpdateActivePlayers();
    }


    /// <summary>
    /// Set the volume level for a bus.
    /// </summary>
    /// <param name="bus">Bus to update.</param>
    /// <param name="level">Volume level to give the bus. 0-1.</param>
    public void SetBusLevel(Bus bus, float level)
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
    public void SetBusMute(Bus bus, bool muted)
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

    #region Play Methods

    public void addPlayer(AudioPlayer audioPlayer)
    {
        poolManager.CreatePool(audioPlayer);
    }


    public AudioPlayer play(AudioPlayer audioPlayer, float volumeScale = 1f)
    {
        Bus playerGroup = audioPlayer.bus;

        AudioPlayer player = poolManager.Pop(audioPlayer).GetComponent(typeof(AudioPlayer)) as AudioPlayer;

        activePlayers[playerGroup].Add(player);
        player.DisableEvent += OnAudioDone;

        player.Play(volumeScale, busVolumes[playerGroup] * MasterVolume, busVolumes[playerGroup] || MasterVolume);
        return player;
    }

    #endregion

    #region Static Methods

    public static void AddPlayer(AudioPlayer audioPlayer)
    {
        Main.addPlayer(audioPlayer);
    }


    public static AudioPlayer Play(AudioPlayer audioPlayer, float volumeScale = 1f)
    {
        return Main.play(audioPlayer, volumeScale);
    }

    #endregion

    #region Event Handlers

    public void OnAudioDone(object sender, EventArgs args)
    {
        AudioPlayer player = (AudioPlayer)sender;
        player.DisableEvent -= OnAudioDone;
        activePlayers[player.bus].Remove(player);
    }

    #endregion
}

[Serializable]
public class Volume
{
    public float level;

    public bool muted;


    public Volume()
    {
        level = 1f;
        muted = false;
    }


    public Volume(float level, bool muted)
    {
        this.level = level;
        this.muted = muted;
    }


    public static float operator *(Volume left, Volume right)
    {
        return left.level * right.level;
    }

    public static float operator *(Volume left, float right)
    {
        return left.level * right;
    }

    public static implicit operator bool(Volume volume)
    {
        return volume.muted;
    }
}