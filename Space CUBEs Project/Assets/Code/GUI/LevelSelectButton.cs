// Little Byte Games
// Author: Steve Yeager
// Created: 2014.08.24
// Edited: 2014.08.24

using System.Text;
using UnityEngine;
using SpaceCUBEs;

public class LevelSelectButton : UIButton
{
    #region Public Fields

    public Scenes.Levels level;
    public UILabel nameLabel;
    public GameObject infoBackground;
    public UILabel highScoreLabel;
    public UILabel bestTimeLabel;
    public UISprite medal;

    #endregion

    #region Static Fields

    public static LevelSelectButton ActiveButton { get; private set; }

    #endregion

    #region UIButton Overrides

    protected override void OnClick()
    {
        if (ActiveButton == this) return;

        base.OnClick();

        ActiveButton.Toggle(false);
        Toggle(true);
    }

    #endregion

    #region Public Methods

    public void Toggle(bool on)
    {
        ActiveButton = this;
        infoBackground.SetActive(on);
    }


    public void Disable()
    {
        infoBackground.SetActive(false);
        isEnabled = false;

        const int randomSize = 20;
        StringBuilder builder = new StringBuilder(randomSize);
        for (int i = 0; i < randomSize /2; i++)
        {
            builder.Append((char)Random.Range(32, 127));
        }
        builder.Append('\n');
        for (int i = 0; i < randomSize /2; i++)
        {
            builder.Append((char)Random.Range(32, 127));
        }
        nameLabel.text = builder.ToString();
    }

    #endregion
}