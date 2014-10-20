// Little Byte Games
// Author: Steve Yeager
// Created: 2014.08.24
// Edited: 2014.10.05

using System;
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

        Show(Enum.GetNames(typeof(Scenes.Menus)).Select(l => l.SplitCamelCase()).Contains(levelName) && levelName != Scenes.Menus.Workshop.ToString());
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

    #endregion

    #region Static Methods

    public static void Show(bool on)
    {
        Main.root.SetActive(on);
    }

    #endregion
}