// Steve Yeager
// 4.3.2014

using Annotations;
using UnityEngine;

/// <summary>
/// Manager for the Debug Menu.
/// </summary>
public class DebugMenuManager : MonoBehaviour
{
    #region Public Fields

    public UIToggle invincibleToggle;
    public UIToggle jumpToBossToggle;
    
    #endregion


    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Awake()
    {
        invincibleToggle.value = GameSettings.Main.invincible;
        jumpToBossToggle.value = GameSettings.Main.jumpToBoss;
    }

    #endregion

    #region Button Methods

    public void ToggleInvincibility()
    {
        GameSettings.Main.invincible = invincibleToggle.value;
    }


    public void ToggleJumpToBoss()
    {
        GameSettings.Main.jumpToBoss = jumpToBossToggle.value;
    }


    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    #endregion
}