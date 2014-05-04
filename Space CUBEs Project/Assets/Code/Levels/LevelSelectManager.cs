// Steve Yeager
// 4.3.2014

using Annotations;
using UnityEngine;

/// <summary>
/// Menu for Level Select screen.
/// </summary>
public class LevelSelectManager : MonoBehaviour
{
    #region Public Fields

    /// <summary>Buttons that load corresponding level.</summary>
    public ActivateButton[] levelButtons;

    #endregion


    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Awake()
    {
        // register button events
        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].ActivateEvent += OnLevelSelected;
            levelButtons[i].isEnabled = !(i > 0);
        }
    }


    [UsedImplicitly]
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameData.LoadLevel("Main Menu");
        }
    }

    #endregion

    #region Event Handlers

    private static void OnLevelSelected(object sender, ActivateButtonArgs args)
    {
        GameData.LoadLevel(args.value, true);
    }

    #endregion
}