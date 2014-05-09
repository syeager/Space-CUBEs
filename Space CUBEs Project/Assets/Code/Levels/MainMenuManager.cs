// Steve Yeager
// 1.15.2014

using Annotations;
using UnityEngine;

/// <summary>
/// Manager for the Main Menu.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    #region Public Fields
#if DEBUG
    public Rect debugTouchRect;
#endif
    #endregion


    #region MonoBehaviour Overrides

#if DEBUG
    [UsedImplicitly]
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameData.LoadLevel("Debug Menu");
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).tapCount == 2 && debugTouchRect.Contains(Input.GetTouch(0).position))
        {
            GameData.LoadLevel("Debug Menu");
        }
#endif
    }
#endif

    #endregion

    #region Button Methods

    public void LoadPlay()
    {
        GameData.LoadLevel("Level Select Menu");
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