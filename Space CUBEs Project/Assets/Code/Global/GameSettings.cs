// Steve Yeager
// 8.17.2013

using UnityEngine;

/// <summary>
/// Singleton to hold all game settings.
/// </summary>
public class GameSettings : Singleton<GameSettings>
{
    #region Volume Fields

    public float volumeSE;
    public float volumeMusic;

    #endregion
}