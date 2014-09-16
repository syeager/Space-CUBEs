// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.14
// Edited: 2014.09.14

using UnityEngine;
public class BuildPreview : MonoBehaviour
{
    #region Public Fields

    public UILabel nameLabel;
    public UILabel healthLabel;
    public UILabel shieldLabel;
    public UILabel speedLabel;
    public UILabel damageLabel;

    #endregion

    #region Private Fields

    public BuildInfo buildInfo { get; private set; }

    #endregion

    #region ScrollviewButton Overrides

    public void Initialize(BuildInfo buildInfo)
    {
        this.buildInfo = buildInfo;

        nameLabel.text = buildInfo.name;
        healthLabel.text = buildInfo.stats.health.ToString();
        shieldLabel.text = buildInfo.stats.shield.ToString();
        speedLabel.text = buildInfo.stats.speed.ToString();
        damageLabel.text = buildInfo.stats.damage.ToString();
    }

    #endregion
}