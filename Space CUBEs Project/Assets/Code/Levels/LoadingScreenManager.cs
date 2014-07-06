// Steve Yeager
// 1.5.2014

using Annotations;
using UnityEngine;
using System;

public class LoadingScreenManager : MonoBehaviour
{
    #region References

    public UILabel levelToLoad;

    #endregion


    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Awake()
    {
        // set up GUI
        levelToLoad.text = SceneManager.Main.NextScene + "...";

        // clean
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }


    [UsedImplicitly]
    private void Start()
    {
        Application.LoadLevel(SceneManager.Main.NextScene);
    }

    #endregion
}