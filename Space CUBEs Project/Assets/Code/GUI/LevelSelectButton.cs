// Little Byte Games
// Author: Steve Yeager
// Created: 2014.08.24
// Edited: 2014.10.14

using System.Text;
using Annotations;
using SpaceCUBEs;
using UnityEngine;

public class LevelSelectButton : UIButton
{
    #region Public Fields

    public Scenes.Levels level;
    public UILabel nameLabel;
    public GameObject infoBackground;
    public UILabel highScoreLabel;
    public UILabel bestTimeLabel;
    public UISprite medal;
    public Color disabledTextColor;

    #endregion

    #region Private Fields

    [SerializeField, UsedImplicitly]
    private UISprite medalSprite;

    [SerializeField, UsedImplicitly]
    private string[] medalNames;

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

    public void Initialize(bool on, Highscore highscore)
    {
        Toggle(on);
        highScoreLabel.text = highscore.score.ToString("N0");
        bestTimeLabel.text = highscore.TimeString;
        if (highscore.rank > 0)
        {
            medalSprite.spriteName = medalNames[highscore.rank];
        }
        else
        {
            medalSprite.enabled = false;
        }
    }


    public void Toggle(bool on)
    {
        if (on)
        {
            ActiveButton = this;
        }
        infoBackground.SetActive(on);
        medalSprite.gameObject.SetActive(on);
    }


    public void Disable()
    {
        nameLabel.color = disabledTextColor;
        infoBackground.SetActive(false);
        isEnabled = false;

        const int randomSize = 14;
        StringBuilder builder = new StringBuilder(randomSize);
        for (int i = 0; i < randomSize / 2; i++)
        {
            builder.Append((char)Random.Range(32, 127));
        }
        builder.Append('\n');
        for (int i = 0; i < randomSize / 2; i++)
        {
            builder.Append((char)Random.Range(32, 127));
        }
        nameLabel.text = builder.ToString();
    }

    #endregion
}