// Little Byte Games

using System;
using System.Collections;
using System.Linq;
using Annotations;
using LittleByte;
using LittleByte.Extensions;
using SpaceCUBEs;
using UnityEngine;

public class NavigationBar : Singleton<NavigationBar>
{
    #region Public Fields

    public GameObject root;

    public UIButton options;
    public GameObject optionsMenuPrefab;

    public UIButton mainMenu;
    public UIButton garage;
    public UIButton store;
    public UIButton levelSelect;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Start()
    {
        string levelName = Application.loadedLevelName;

        if (Scenes.MenuNames.ContainsValue(levelName))
        {
            UpdateButtons(Scenes.Menu(levelName));
        }
        else
        {
            UpdateAll(true);
        }
    }

    [UsedImplicitly]
    private void OnLevelWasLoaded(int level)
    {
        string levelName = Application.loadedLevelName;

        if (Scenes.MenuNames.ContainsValue(levelName))
        {
            UpdateButtons(Scenes.Menu(levelName));
        }
        else
        {
            UpdateAll(true);
        }

        bool show = Enum.GetNames(typeof(Scenes.Menus)).Select(l => l.SplitCamelCase()).Contains(levelName) && levelName != Scenes.Menus.Workshop.ToString();
        Show(show);

        if (show && Application.loadedLevelName != Scenes.Scene(Scenes.Menus.MainMenu))
        {
            StartCoroutine(SetSelected());
        }
    }

    #endregion

    #region Public Methods

    public void ToggleOptions()
    {
        Instantiate(optionsMenuPrefab, Vector3.left * 5000f, Quaternion.identity);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(Scenes.Scene(Scenes.Menus.MainMenu), false, true);
        UpdateButtons(Scenes.Menus.MainMenu);
    }

    public void LoadGarage()
    {
        SceneManager.LoadScene(Scenes.Scene(Scenes.Menus.Garage));
        UpdateButtons(Scenes.Menus.Garage);
    }

    public void LoadStore()
    {
        SceneManager.LoadScene(Scenes.Scene(Scenes.Menus.Store), true);
        UpdateButtons(Scenes.Menus.Store);
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene(Scenes.Scene(Scenes.Menus.LevelSelectMenu));
        UpdateButtons(Scenes.Menus.LevelSelectMenu);
    }

    #endregion

    #region Private Methods

    private void UpdateButtons(Scenes.Menus menu)
    {
        mainMenu.isEnabled = menu != Scenes.Menus.MainMenu;
        garage.isEnabled = menu != Scenes.Menus.Garage;
        store.isEnabled = menu != Scenes.Menus.Store;
        levelSelect.isEnabled = menu != Scenes.Menus.LevelSelectMenu;
    }

    private void UpdateAll(bool on)
    {
        mainMenu.isEnabled = on;
        garage.isEnabled = on;
        store.isEnabled = on;
        levelSelect.isEnabled = on;
    }

    private IEnumerator SetSelected()
    {
        yield return new WaitForEndOfFrame();
        UICamera.selectedObject = mainMenu.gameObject;
        mainMenu.SetState(UIButtonColor.State.Hover, true);
    }

    #endregion

    #region Static Methods

    public static void Show(bool on)
    {
        Main.root.SetActive(on);
    }

    #endregion
}