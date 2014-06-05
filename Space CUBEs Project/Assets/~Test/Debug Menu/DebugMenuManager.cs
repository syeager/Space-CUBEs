// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.04.03
// Edited: 2014.06.02

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
    private void Start()
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