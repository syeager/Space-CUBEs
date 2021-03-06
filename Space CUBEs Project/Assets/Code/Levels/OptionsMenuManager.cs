﻿// Little Byte Games

using System;
using System.Linq;
using Annotations;
using GooglePlayGames;
using LittleByte;
using LittleByte.Audio;
using LittleByte.Data;
using UnityEngine;

namespace SpaceCUBEs
{
    /// <summary>
    /// Manager for the Options Menu.
    /// </summary>
    public class OptionsMenuManager : MonoBehaviour
    {
        #region Private Fields

        public enum Menus
        {
            Graphics,
            Audio,
            Input,
            Data,
        }

        private Menus menu;

        [SerializeField, UsedImplicitly]
        private UIButton[] menuButtons;

        [SerializeField, UsedImplicitly]
        private GameObject[] menus;

        private static readonly bool[] NeedDescription =
        {
            true,
            false,
            true,
            true,
        };

        [SerializeField, UsedImplicitly]
        private UILabel description;

        #endregion

        #region Quality Fields

        private int qualityLevel;

        [Header("Quality")]
        [SerializeField, UsedImplicitly]
        private UIToggle[] fpsToggles;

        private static readonly int[] FrameRates = {30, 45, 60};

        private const string FrameRateInfo = "The game will attempt to run at this FPS (Frames Per Second).";

        [SerializeField, UsedImplicitly]
        private UIToggle[] qualityToggles;

        private const string QualityLevelInfo = "Quality level.";

        #endregion

        #region Audio Fields

        [Header("Audio")]
        [SerializeField, UsedImplicitly]
        private UISlider masterVolume;

        [SerializeField, UsedImplicitly]
        private UISlider[] busVolumes;

        [SerializeField, UsedImplicitly]
        private UIToggle masterMute;

        [SerializeField, UsedImplicitly]
        private UIToggle[] busMutes;

        #endregion

        #region Input Fields

        [Header("Input")]
        public UISlider[] inputSliders;

        public UIInput[] inputInputs;

        [SerializeField, UsedImplicitly]
        private GameObject[] inputRoots;

        #endregion

        #region MonoBehaviour Overrides

        [UsedImplicitly]
        private void Start()
        {
            LoadQuality();
            LoadAudio();
            LoadInput();

            SetMenu(0);
        }

        #endregion

        #region Public Methods

        public void SetMenuButton(UIButton menuButton)
        {
            SetMenu(Array.IndexOf(menuButtons, menuButton));
        }

        public void ResetOptions()
        {
            switch (menu)
            {
                case Menus.Graphics:
                    fpsToggles[2].value = true;
                    qualityToggles[2].value = true;
                    break;
                case Menus.Audio:
                    const float volumeReset = 0.5f;
                    masterVolume.value = volumeReset;
                    masterMute.value = false;
                    for (int i = 0; i < busVolumes.Length; i++)
                    {
                        busVolumes[i].value = volumeReset;
                        busMutes[i].value = false;
                    }
                    break;
                case Menus.Input:
                    inputSliders[0].value = GameSettings.JoystickSensitivityDefault;
                    inputSliders[1].value = GameSettings.JoystickDeadzoneDefault;
                    inputSliders[2].value = GameSettings.JoystickXBufferDefault;
                    inputSliders[3].value = GameSettings.JoystickYBufferDefault;
                    break;
            }
        }

        public void Exit()
        {
            if (qualityLevel != GameSettings.Main.qualityLevel)
            {
                GameSettings.Main.qualityLevel = qualityLevel;
                QualitySettings.SetQualityLevel(qualityLevel, true);
            }
            Save();
            Destroy(gameObject);
        }

        #endregion

        #region Private Methods

        private void SetMenu(int index)
        {
            int oldIndex = (int)menu;
            menus[oldIndex].SetActive(false);
            menuButtons[oldIndex].isEnabled = true;

            menu = (Menus)index;
            menus[index].SetActive(true);
            menuButtons[index].isEnabled = false;

            description.transform.parent.gameObject.SetActive(NeedDescription[index]);
            description.text = string.Empty;
        }

        private static void Save()
        {
            AudioManager.Main.Save();
            GameSettings.Save();
        }

        #endregion

        #region Quality Methods

        private void LoadQuality()
        {
            int fpsIndex = Array.IndexOf(FrameRates, GameTime.TargetFPS);
            fpsToggles[fpsIndex].value = true;

            qualityLevel = GameSettings.Main.qualityLevel;
            qualityToggles[qualityLevel].value = true;
        }

        public void FrameRateUpdated(UIToggle toggle)
        {
            if (!toggle.value) return;

            description.text = FrameRateInfo;

            int index = Array.IndexOf(fpsToggles, toggle);
            GameTime.CapFPS(FrameRates[index]);
        }

        public void QualityUpdated(UIToggle toggle)
        {
            if (!toggle.value) return;

            description.text = QualityLevelInfo;

            int index = Array.IndexOf(qualityToggles, toggle);
            qualityLevel = index;
        }

        #endregion

        #region Audio Methods

        private void LoadAudio()
        {
            Volume volume = AudioManager.Main.MasterVolume;
            masterVolume.value = volume.level;
            masterMute.value = volume.muted;

            for (int i = 0; i < busVolumes.Length; i++)
            {
                volume = AudioManager.Main.busVolumes[(AudioManager.Bus)i];
                busVolumes[i].value = volume.level;
                busMutes[i].value = volume.muted;
            }
        }

        public void UpdateVolume(UISlider slider)
        {
            if (slider == masterVolume)
            {
                AudioManager.SetMasterLevel(slider.value);
                return;
            }

            int index = Array.IndexOf(busVolumes, slider);
            AudioManager.SetBusLevel((AudioManager.Bus)index, slider.value);
        }

        public void UpdateMute(UIToggle toggle)
        {
            if (toggle == masterMute)
            {
                AudioManager.SetMasterMute(toggle.value);
                return;
            }

            int index = Array.IndexOf(busMutes, toggle);
            AudioManager.SetBusMute((AudioManager.Bus)index, toggle.value);
        }

        #endregion

        #region Input Methods

        private void LoadInput()
        {
            float input = GameSettings.Main.joystickSensitivity;
            inputSliders[0].value = input;
            inputInputs[0].value = FormatInput(input * 100f);

            input = GameSettings.Main.joystickDeadzone;
            inputSliders[1].value = input;
            inputInputs[1].value = FormatInput(input * 100f);

            input = GameSettings.Main.joystickXBuffer;
            inputSliders[2].value = input;
            inputInputs[2].value = FormatInput(input * 100f);

            input = GameSettings.Main.joystickYBuffer;
            inputSliders[3].value = input;
            inputInputs[3].value = FormatInput(input * 100f);

            switch (InputManager.ActiveInput)
            {
                case InputManager.Inputs.Keyboard:
                    for (int i = 0; i < inputRoots.Length; i++)
                    {
                        inputRoots[i].SetActive(false);
                    }
                    break;

                case InputManager.Inputs.Gamepad:
                    for (int i = 2; i < inputRoots.Length; i++)
                    {
                        inputRoots[i].SetActive(false);
                    }
                    break;
            }
        }

        public void UpdatedInputSlider(UISlider slider)
        {
            int index = Array.IndexOf(inputSliders, slider);
            inputInputs[index].value = FormatInput(slider.value * 100f);

            switch (index)
            {
                case 0:
                    GameSettings.Main.joystickSensitivity = slider.value;
                    break;
                case 1:
                    GameSettings.Main.joystickDeadzone = slider.value;
                    break;
                case 2:
                    GameSettings.Main.joystickXBuffer = slider.value;
                    break;
                case 3:
                    GameSettings.Main.joystickYBuffer = slider.value;
                    break;
            }
        }

        public void UpdatedInputField(UIInput input)
        {
            if (string.IsNullOrEmpty(input.value))
            {
                input.value = "0";
            }
            float value = Mathf.Clamp(float.Parse(input.value), 0f, 100f);
            input.value = FormatInput(value);

            int index = Array.IndexOf(inputInputs, input);
            value /= 100f;
            inputSliders[index].value = value;

            switch (index)
            {
                case 0:
                    GameSettings.Main.joystickSensitivity = value;
                    break;
                case 1:
                    GameSettings.Main.joystickDeadzone = value;
                    break;
                case 2:
                    GameSettings.Main.joystickXBuffer = value;
                    break;
                case 3:
                    GameSettings.Main.joystickYBuffer = value;
                    break;
            }
        }

        private static string FormatInput(float value)
        {
            return Mathf.RoundToInt(value).ToString();
        }

        #endregion

        #region Data Methods

        public void GameReset()
        {
            Debugger.Log("Resetting game.", null, Debugger.LogTypes.Data);
            PlayerPrefs.DeleteAll();
            SaveData.DeleteAll();
            if (Social.localUser.authenticated)
            {
                ((PlayGamesPlatform)Social.Active).UpdateState(0, null, GoogleCloud.Main);
            }
            GameStart.Main.UpdateVersions(true);
            SceneManager.LoadScene(Scenes.Scene(Scenes.Menus.MainMenu), true, true);
        }

        public void ReloadStarterBuilds()
        {
            string[] builds = ConstructionGrid.BuildNames().ToArray();
            foreach (string build in ConstructionGrid.DevBuilds.Where(build => !builds.Contains(build)))
            {
                ConstructionGrid.SaveBuild(build, SaveData.LoadFromResources<BuildInfo>(build));
            }
        }

        #endregion
    }
}