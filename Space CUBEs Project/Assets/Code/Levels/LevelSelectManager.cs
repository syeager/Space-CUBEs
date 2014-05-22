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

    #region Const Fields

    public const string UnlockedLevelsPath = "Unlocked Levels";

    #endregion


    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Start()
    {
        // register button events
        int unlocked = PlayerPrefs.GetInt(UnlockedLevelsPath);
        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].ActivateEvent += OnLevelSelected;
            levelButtons[i].Toggle(!(i > unlocked));
        }
    }


    [UsedImplicitly]
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Main Menu");
        }
    }

    #endregion

    #region Event Handlers

    private static void OnLevelSelected(object sender, ActivateButtonArgs args)
    {
        SceneManager.LoadScene(args.value, true);
    }

    #endregion
}