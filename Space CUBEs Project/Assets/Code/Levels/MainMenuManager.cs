// Steve Yeager
// 1.15.2014

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manager for the Main Menu.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    #region Button Methods

    public void LoadPlay()
    {
        GameData.LoadLevel("Deep Space", true, new Dictionary<string,object>{{"Build", "Test Build"}});
    }


    public void LoadGarage()
    {
        GameData.LoadLevel("Garage");
    }


    public void LoadStore()
    {
        GameData.LoadLevel("Store");
    }


    public void LoadOptions()
    {
        GameData.LoadLevel("Options Menu");
    }

    #endregion
}