// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.07.11
// Edited: 2014.07.11

using System.Diagnostics;
using Annotations;
using LittleByte.Debug.Attributes;
using UnityEngine;

namespace LittleByte.Debug
{
    [InstanceCount]
    public class TimeControl : Singleton<TimeControl>
    {
        #region Public Fields

        public float aboveOne = 1f;

        public float belowOne = 0.1f;

        public GUIText timeDisplay;

        public float displayTime = 1f;

        #endregion

        #region Const Fields

        private const float MaxTime = 10f;

        private const float MinTime = 0f;

        #endregion

        #region MonoBehaviour Overrides

        [UsedImplicitly]
        [Conditional("DEBUG")]
        private void Update()
        {
            // fast forward
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                float time = Time.timeScale >= 1f ? aboveOne : belowOne;
                Time.timeScale = Mathf.Clamp(Time.timeScale + time, MinTime, MaxTime);
                DisplayTime();
            }

            // slow down
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                float time = Time.timeScale > 1f ? aboveOne : belowOne;
                Time.timeScale = Mathf.Clamp(Time.timeScale - time, MinTime, MaxTime);
                DisplayTime();
            }

            // reset
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Time.timeScale = 1f;
                DisplayTime();
            }

            // pause
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Time.timeScale = 0f;
                DisplayTime();
            }
        }

        #endregion

        #region Private Methods

        private void DisplayTime()
        {
            if (timeDisplay == null) return;

            CancelInvoke();
            timeDisplay.text = Time.timeScale.ToString("0.0");
            timeDisplay.gameObject.SetActive(true);
            Invoke("HideTime", displayTime);
        }


        [UsedImplicitly]
        private void HideTime()
        {
            timeDisplay.gameObject.SetActive(false);
        }

        #endregion
    }
}