// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.14
// Edited: 2014.09.23

using UnityEngine;

namespace SpaceCUBEs
{
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

        public BuildInfo Info { get; private set; }

        #endregion

        #region ScrollviewButton Overrides

        public void Initialize(BuildInfo buildInfo)
        {
            Info = buildInfo;

            nameLabel.text = buildInfo.name;
            healthLabel.text = buildInfo.stats.health.ToString();
            shieldLabel.text = buildInfo.stats.shield.ToString();
            speedLabel.text = buildInfo.stats.speed.ToString();
            damageLabel.text = buildInfo.stats.damage.ToString();
        }

        #endregion
    }
}