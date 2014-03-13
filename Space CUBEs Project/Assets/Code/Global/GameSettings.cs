// Steve Yeager
// 8.17.2013

using UnityEngine;

/// <summary>
/// Singleton to hold all game settings.
/// </summary>
public class GameSettings : Singleton<GameSettings>
{
    #region Testing Fields

    public bool invincible;

    #endregion

    #region Volume Fields

    public float volumeSE;
    public float volumeMusic;

    #endregion
}