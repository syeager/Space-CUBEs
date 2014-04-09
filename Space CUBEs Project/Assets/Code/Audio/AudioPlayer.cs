// Steve Yeager
// 3.26.2014

using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    #region Public Fields
    
    public enum AudioGroups { Music, Game, Menu };
    public AudioGroups audioGroup;
    
    #endregion
}