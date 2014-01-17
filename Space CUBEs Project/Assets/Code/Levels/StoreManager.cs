// Steve Yeager
// 1.15.2014

using UnityEngine;
using System.Collections;

/// <summary>
/// Manager for the Store.
/// </summary>
public class StoreManager : MonoBehaviour
{
    #region Button Methods
    
    public void LoadMainMenu()
    {
        GameData.LoadLevel("Main Menu");
    }

    #endregion
}