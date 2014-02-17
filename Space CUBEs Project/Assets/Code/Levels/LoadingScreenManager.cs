// Steve Yeager
// 1.5.2014

using UnityEngine;
using System.Collections;
using System;

public class LoadingScreenManager : MonoBehaviour
{
    #region References

    public UILabel levelToLoad;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        // set up GUI
        levelToLoad.text = GameData.Main.nextScene + "...";

        // clean
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }


    private void Start()
    {
        Application.LoadLevel(GameData.Main.nextScene);
    }

    #endregion
}