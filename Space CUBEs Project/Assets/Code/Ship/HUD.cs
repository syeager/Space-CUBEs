// Steve Yeager
// 12.8.2013

using UnityEngine;

/// <summary>
/// Player HUD in level.
/// </summary>
public class HUD : MonoBehaviour
{
    #region Private Fields

    private float H;
    private float W;

    #endregion

    #region Static Fields

    public static float NavButton { get; private set; }

    #endregion

    #region Const Fields

    public Rect NAVPER;
    private Rect NavRect;

    #endregion

    


    #region MonoBehaviour Overrides

    private void Update()
    {
        UpdateScreen();
    }


    private void OnGUI()
    {
        Nav();
    }

    #endregion

    #region Private Methods

    private void UpdateScreen()
    {
        H = Screen.height;
        W = Screen.width;

        NavRect = new Rect(NAVPER.x * W, NAVPER.y * H, NAVPER.width * W, NAVPER.height * H);
    }


    private void Nav()
    {
        NavButton = 0f;

        GUI.BeginGroup(NavRect);
        {
            // up
            if (GUI.RepeatButton(new Rect(0f, 0f, NavRect.width / 2f, NavRect.height), "↑"))
            {
                NavButton = -1f;
            }
            // down
            if (GUI.RepeatButton(new Rect(NavRect.width / 2f, 0f, NavRect.width / 2f, NavRect.height), "↓"))
            {
                NavButton = 1f;
            }
        }
        GUI.EndGroup();
    }

    #endregion
}