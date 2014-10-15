// Little Byte Games
// Author: Steve Yeager
// Created: 2014.04.03
// Edited: 2014.09.17

using Annotations;
using LittleByte;
using LittleByte.Data;
using LittleByte.Extensions;
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
            string key = FormationLevelManager.HighScoreKey + ((Scenes.Levels)i).ToString().SplitCamelCase();
            Highscore highscore = SaveData.Load<Highscore>(key, FormationLevelManager.LevelsFolder);

            levelButtons[i].Initialize(false, highscore);
            levelButtons[i].Toggle(false);
        }
        levelButtons[unlocked].Initialize(true, SaveData.Load<Highscore>(FormationLevelManager.HighScoreKey + ((Scenes.Levels)unlocked).ToString().SplitCamelCase(), FormationLevelManager.LevelsFolder));
        for (int i = unlocked + 1; i < levelButtons.Length; i++)
        {
            string key = FormationLevelManager.HighScoreKey + ((Scenes.Levels)i).ToString().SplitCamelCase();
            levelButtons[i].Initialize(false, SaveData.Load<Highscore>(key, FormationLevelManager.LevelsFolder));
            levelButtons[i].Disable();
        }
    }


    [UsedImplicitly]
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(Scenes.Scene(Scenes.Menus.MainMenu));
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