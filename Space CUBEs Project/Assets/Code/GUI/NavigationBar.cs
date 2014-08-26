// Little Byte Games
// Author: Steve Yeager
// Created: 2014.08.24
// Edited: 2014.08.24

using System;
using System.Linq;
using Annotations;
using UnityEngine;

public class NavigationBar : Singleton<NavigationBar>
{
    #region Public Fields

    public GameObject root;

    public UIButton options;

    public UIButton mainMenu;
    public UIButton garage;
    public UIButton store;
    public UIButton levelSelect;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Start()
    {
        UpdateButtons(Application.loadedLevelName);
    }


    [UsedImplicitly]
    private void OnLevelWasLoaded(int level)
    {
        string levelName = Application.loadedLevelName;
        root.SetActive(Enum.GetNames(typeof(Menus)).Select(l => l.SplitCamelCase()).Contains(levelName));
    }

    #endregion

    #region Public Methods

    public void ToggleOptions()
    {
        SceneManager.LoadScene("Options Menu");
    }

    public void LoadMainMenu()
    {
        string menu = Menus.MainMenu.ToString().SplitCamelCase();
        SceneManager.LoadScene(menu);
        UpdateButtons(menu);
    }


    public void LoadGarage()
    {
        string menu = Menus.Garage.ToString().SplitCamelCase();
        SceneManager.LoadScene(menu);
        UpdateButtons(menu);
    }


    public void LoadStore()
    {
        string menu = Menus.Store.ToString().SplitCamelCase();
        SceneManager.LoadScene(menu);
        UpdateButtons(menu);
    }


    public void LoadLevelSelect()
    {
        string menu = Menus.LevelSelectMenu.ToString().SplitCamelCase();
        SceneManager.LoadScene(menu);
        UpdateButtons(menu);
    }

    #endregion

    #region Private Methods

    private void UpdateButtons(string menu)
    {
        mainMenu.isEnabled = menu != Menus.MainMenu.ToString().SplitCamelCase();
        garage.isEnabled = menu != Menus.Garage.ToString().SplitCamelCase();
        store.isEnabled = menu != Menus.Store.ToString().SplitCamelCase();
        levelSelect.isEnabled = menu != Menus.LevelSelectMenu.ToString().SplitCamelCase();
    }

    #endregion
}