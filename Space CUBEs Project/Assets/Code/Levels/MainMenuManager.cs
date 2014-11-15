// Little Byte Games

using Annotations;
using LittleByte;
using LittleByte.Data;
using LittleByte.Extensions;
using SpaceCUBEs;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

    #region Private Fields

    [SerializeField, UsedImplicitly]
    private UIButton playButton;

    #endregion

    #region MonoBehaviour Overrides

    private void Start()
    {
        NGUIUtility.SetSelected(playButton);
    }

#if DEBUG
    [UsedImplicitly]
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Debug Menu");
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).tapCount == 2 && debugTouchRect.Contains(Input.GetTouch(0).position))
        {
            SceneManager.LoadScene("Debug Menu");
        }
#endif
    }
#endif

    #endregion

    #region Button Methods

    public void Play()
    {
        ConstructionGrid.SelectedBuild = ConstructionGrid.DevBuilds[0];
        int unlocked = SaveData.Load<int>(LevelSelectManager.UnlockedLevelsKey, FormationLevelManager.LevelsFolder);
        SceneManager.LoadScene(((Scenes.Levels)unlocked).ToString().SplitCamelCase(), true, true);
    }

    /// <summary>
    /// Exit game.
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion
}