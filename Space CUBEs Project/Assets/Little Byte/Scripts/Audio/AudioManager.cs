// Little Byte Games
// Author: Steve Yeager
// Created: 2014.03.26
// Edited: 2014.08.25

using System;
using System.Collections;
using System.Collections.Generic;
using Annotations;
using LittleByte;
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
    private const string VolumeFolder = @"Volume/";

    /// <summary>Data file for saving master volume.</summary>
    private const string MasterFile = "Master";

    #endregion

    #region Playlist Fields

    /// <summary>All active playlists.</summary>
    private static readonly Dictionary<string, Playlist> Playlists = new Dictionary<string, Playlist>();

    /// <summary>All current jobs running on playlists.</summary>
    private static readonly Dictionary<Playlist, Job> PlaylistJobs = new Dictionary<Playlist, Job>();

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

        GameTime.PausedEvent += OnPause;
        Initialize();
        poolManager.Initialize();
    }


    [UsedImplicitly]
    private void OnDestroy()
    {
        GameTime.PausedEvent -= OnPause;
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
        foreach (object value in Enum.GetValues(typeof(Bus)))
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
        foreach (object value in Enum.GetValues(typeof(Bus)))
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
        UpdatePlaylists();
    }


    /// <summary>
    /// Set the master volume mute status.
    /// </summary>
    /// <param name="muted">Should the master volume be muted?</param>
    public void setMasterMute(bool muted)
    {
        MasterVolume.muted = muted;
        UpdateActivePlayers();
        UpdatePlaylists();
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
        UpdatePlaylists();
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
        UpdatePlaylists();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Update all active audio players with current level and mute settings.
    /// </summary>
    private void UpdateActivePlayers()
    {
        foreach (object value in Enum.GetValues(typeof(Bus)))
        {
            Bus bus = (Bus)value;
            float level = MasterVolume * busVolumes[bus];
            bool muted = MasterVolume || busVolumes[bus];
            foreach (AudioPlayer activePlayer in activePlayers[bus])
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

    #region Playlist Methods

    /// <summary>
    /// Update all playlists' volumes.
    /// </summary>
    public static void UpdatePlaylists()
    {
        Dictionary<string, Playlist>.KeyCollection names = Playlists.Keys;

        foreach (string name in names)
        {
            UpdatePlaylist(Playlists[name]);
        }
    }


    /// <summary>
    /// Update a playlist's volume settings.
    /// </summary>
    /// <param name="playlist">Playlist to update.</param>
    private static void UpdatePlaylist(Playlist playlist)
    {
        Volume masterVolume = Main.MasterVolume;
        Volume busVolume = Main.busVolumes[playlist.bus];
        playlist.UpdateVolume(new Volume(masterVolume * busVolume, masterVolume || busVolume));
    }


    /// <summary>
    /// Add a new playlist.
    /// </summary>
    /// <param name="playlistName">Name for the playlist.</param>
    /// <param name="playlist">Playlist to add.</param>
    public static void AddPlaylist(string playlistName, Playlist playlist)
    {
        Playlists.Add(playlistName, playlist);
        UpdatePlaylist(playlist);
    }


    /// <summary>
    /// Remove a playlist.
    /// </summary>
    /// <param name="playlistName">Name of the playlist.</param>
    public static void RemovePlaylist(string playlistName)
    {
        Playlists.Remove(playlistName);
    }


    /// <summary>
    /// Get an active playlist.
    /// </summary>
    /// <param name="playlistName">Name of the playlist to search for.</param>
    /// <returns>Playlist if found or null if not.</returns>
    public static Playlist GetPlaylist(string playlistName)
    {
        Playlist playlist;
        return Playlists.TryGetValue(playlistName, out playlist) ? playlist : null;
    }


    /// <summary>
    /// Play a playlist.
    /// </summary>
    /// <param name="playlistName">Name of the playlist to play.</param>
    /// <returns>Playlist that is being played.</returns>
    public static Playlist PlayPlaylist(string playlistName)
    {
        Playlist playlist = Playlists[playlistName];
        playlist.Play();
        return playlist;
    }


    /// <summary>
    /// Pause a playlist.
    /// </summary>
    /// <param name="playlistName">Name of the playlist to pause.</param>
    /// <returns>Playlist that is being paused.</returns>
    public static Playlist PausePlaylist(string playlistName)
    {
        Playlist playlist = Playlists[playlistName];
        playlist.Pause(true);
        return playlist;
    }


    /// <summary>
    /// Stop a playlist.
    /// </summary>
    /// <param name="playlistName">Name of the playlist to stop.</param>
    /// <returns>Playlist that is being stopped.</returns>
    public static Playlist StopPlaylist(string playlistName)
    {
        Playlist playlist = Playlists[playlistName];
        playlist.Stop();
        return playlist;
    }


    /// <summary>
    /// Start a playlist and fade it from 0 to full volume.
    /// </summary>
    /// <param name="playlist">Playlist to fade in.</param>
    /// <param name="time">Time in seconds for fade.</param>
    public static void FadeInPlaylist(Playlist playlist, float time)
    {
        KillPlaylistJob(playlist);

        playlist.gameObject.SetActive(true);
        playlist.levelScale = 0f;
        playlist.Play();

        PlaylistJobs[playlist] = new Job(PlaylistFade(playlist, 1f, time));
    }


    /// <summary>
    /// Start a playlist and fade it from 0 to full volume.
    /// </summary>
    /// <param name="playlistName">Name of the playlist to fade in.</param>
    /// <param name="time">Time in seconds for fade.</param>
    /// <returns>Playlist being faded.</returns>
    public static Playlist FadeInPlaylist(string playlistName, float time)
    {
        Playlist playlist = Playlists[playlistName];
        FadeInPlaylist(playlist, time);
        return playlist;
    }


    /// <summary>
    /// Fade a playlist to 0.
    /// </summary>
    /// <param name="playlist">Playlist to fade out.</param>
    /// <param name="time">Time in seconds for fade.</param>
    /// <param name="stop">Should the playlist be stopped after fading out?</param>
    public static void FadeOutPlaylist(Playlist playlist, float time, bool stop = true)
    {
        KillPlaylistJob(playlist);

        Job fade = new Job(PlaylistFade(playlist, 0f, time, playlist.Stop));
        PlaylistJobs[playlist] = fade;
    }


    /// <summary>
    /// Fade a playlist to 0.
    /// </summary>
    /// <param name="playlistName">Name of the playlist to fade out.</param>
    /// <param name="time">Time in seconds for fade.</param>
    /// <param name="stop">Should the playlist be stopped after fading out?</param>
    /// <returns>Playlist being faded.</returns>
    public static Playlist FadeOutPlaylist(string playlistName, float time, bool stop = true)
    {
        Playlist playlist = Playlists[playlistName];
        FadeOutPlaylist(playlist, time);
        return playlist;
    }


    /// <summary>
    /// Fade a playlist to a set value.
    /// </summary>
    /// <param name="playlist">Playlist to fade.</param>
    /// <param name="value">Value to fade to. 0-1.</param>
    /// <param name="time">Time in seconds for fade.</param>
    public static void FadePlaylist(Playlist playlist, float value, float time)
    {
        KillPlaylistJob(playlist);

        Job fade = new Job(PlaylistFade(playlist, value, time));
        PlaylistJobs[playlist] = fade;
    }


    /// <summary>
    /// Fade a playlist to a set value.
    /// </summary>
    /// <param name="playlistName">Name of the playlist to fade.</param>
    /// <param name="value">Value to fade to. 0-1.</param>
    /// <param name="time">Time in seconds for fade.</param>
    /// <returns>Playlist being faded.</returns>
    public static Playlist FadePlaylist(string playlistName, float value, float time)
    {
        Playlist playlist = Playlists[playlistName];
        FadePlaylist(playlist, value, time);
        return playlist;
    }


    /// <summary>
    /// Fade one playlist out while fading another in.
    /// </summary>
    /// <param name="from">Playlist to fade out.</param>
    /// <param name="to">Playlist to fade in.</param>
    /// <param name="time">Time in seconds for fade.</param>
    public static void CrossFadePlaylist(Playlist from, Playlist to, float time)
    {
        FadeOutPlaylist(from, time);
        FadeInPlaylist(to, time);
    }


    /// <summary>
    /// Fade one playlist out while fading another in.
    /// </summary>
    /// <param name="from">Name of the playlist to fade out.</param>
    /// <param name="to">Name of the playlist to fade in.</param>
    /// <param name="time">Time in seconds for fade.</param>
    /// <returns>Array with both playlists. [0] From, [1] To.</returns>
    public static Playlist[] CrossFadePlaylist(string from, string to, float time)
    {
        Playlist[] playlists = {Playlists[from], Playlists[to]};
        CrossFadePlaylist(playlists[0], playlists[1], time);
        return playlists;
    }


    /// <summary>
    /// Fade a playlist over time to a value.
    /// </summary>
    /// <param name="playlist">Playlist to fade.</param>
    /// <param name="value">Playlist's levelScale to fade to.</param>
    /// <param name="time">Time in seconds to fade.</param>
    /// <param name="onComplete">Action to call once fade is completed.</param>
    private static IEnumerator PlaylistFade(Playlist playlist, float value, float time, Action onComplete = null)
    {
        float originalValue = playlist.levelScale;
        float timer = 0f;
        while (timer < time)
        {
            timer += Time.deltaTime / time;
            playlist.UpdateLevelScale(Mathf.Lerp(originalValue, value, timer));
            yield return null;
        }
        playlist.UpdateLevelScale(value);

        if (onComplete != null)
        {
            onComplete.Invoke();
        }
    }


    /// <summary>
    /// Stop the current job for a playlist. Adds a new job if their is none.
    /// </summary>
    /// <param name="playlist">Playlist whose job is going to be killed.</param>
    private static void KillPlaylistJob(Playlist playlist)
    {
        if (PlaylistJobs.ContainsKey(playlist))
        {
            PlaylistJobs[playlist].Kill();
        }
        else
        {
            PlaylistJobs.Add(playlist, null);
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handler for audio player being disabled/completed.
    /// </summary>
    /// <param name="sender">AudioPlayer.</param>
    /// <param name="args">Not used.</param>
    private void OnAudioDone(object sender, EventArgs args)
    {
        AudioPlayer player = (AudioPlayer)sender;
        player.DisableEvent -= OnAudioDone;
        activePlayers[player.bus].Remove(player);
    }


    /// <summary>
    /// Recieved when the game is paused.
    /// </summary>
    /// <param name="sender">GameTime.</param>
    /// <param name="args">Is the game paused?</param>
    private void OnPause(object sender, PauseArgs args)
    {
        // audio players
        Array buses = Enum.GetValues(typeof(Bus));
        foreach (object bus in buses)
        {
            foreach (AudioPlayer player in activePlayers[(Bus)bus])
            {
                player.Pause(args.paused);
            }
        }

        // playlists
        Dictionary<string, Playlist>.KeyCollection keys = Playlists.Keys;
        Playlist playlist;
        if (args.paused)
        {
            foreach (string key in keys)
            {
                playlist = Playlists[key];
                playlist.Pause(true);
                Job job;
                if (PlaylistJobs.TryGetValue(playlist, out job))
                {
                    job.Pause(true);
                }
            }
        }
        else
        {
            foreach (string key in keys)
            {
                playlist = Playlists[key];
                playlist.Pause(false);
                Job job;
                if (PlaylistJobs.TryGetValue(playlist, out job))
                {
                    job.Pause(false);
                }
            }
        }
    }

    #endregion
}