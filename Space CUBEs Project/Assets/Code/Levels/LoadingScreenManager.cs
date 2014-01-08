// Steve Yeager
// 1.5.2014

using UnityEngine;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    #region References

    public UILabel levelToLoad;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        levelToLoad.text = GameData.Main.nextScene + "...";
    }


    private void Start()
    {
        Application.LoadLevel(GameData.Main.nextScene);
    }

    #endregion
}