// Little Byte Games
// Author: Steve Yeager
// Created: 2014.04.03
// Edited: 2014.08.24

using Annotations;
using LittleByte.Data;
using SpaceCUBEs;
using UnityEngine;

/// <summary>
/// Menu for Level Select screen.
/// </summary>
public class LevelSelectManager : MonoBehaviour
{
    #region Public Fields

    /// <summary>Buttons that load corresponding level.</summary>
    public LevelSelectButton[] levelButtons;

    #endregion

    #region Const Fields

    public const string UnlockedLevelsKey = "Unlocked Levels";

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Start()
    {
        // load buttons
        int unlocked = SaveData.Load<int>(UnlockedLevelsKey, FormationLevelManager.LevelsFolder);
        for (int i = 0; i < unlocked; i++)
        {
            string key = FormationLevelManager.HighScoreKey + ((Levels)i).ToString().SplitCamelCase();
            levelButtons[i].highScoreLabel.text = SaveData.Load<int[]>(key)[1].ToString("N");
            levelButtons[i].Toggle(false);
        }
        levelButtons[unlocked].Toggle(true);
        levelButtons[unlocked].highScoreLabel.text = SaveData.Load(FormationLevelManager.HighScoreKey + ((Levels)unlocked).ToString().SplitCamelCase(), "Default/", new[] {0, 0})[1].ToString("N");
        for (int i = unlocked + 1; i < levelButtons.Length; i++)
        {
            string key = FormationLevelManager.HighScoreKey + ((Levels)i).ToString().SplitCamelCase();
            if (SaveData.Contains(key))
            {
                levelButtons[i].highScoreLabel.text = SaveData.Load<int[]>(key, "Default/", new[] {0, 0})[1].ToString("N");
            }
            levelButtons[i].Disable();
        }
    }


    [UsedImplicitly]
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(Menus.MainMenu.ToString().SplitCamelCase());
        }
    }

    #endregion

    #region Public Methods

    public void Play()
    {
        SceneManager.LoadScene(LevelSelectButton.ActiveButton.level.ToString().SplitCamelCase(), true);
    }

    #endregion
}