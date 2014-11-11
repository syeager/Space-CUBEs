// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.20
// Edited: 2014.10.06

using Annotations;
using UnityEngine;

namespace SpaceCUBEs
{
    public class PreviewShip : MonoBehaviour
    {
        #region Private Fields

        [SerializeField, UsedImplicitly]
        private UIInput nameInput;

        [SerializeField, UsedImplicitly]
        private UILabel healthLabel;

        [SerializeField, UsedImplicitly]
        private UILabel shieldLabel;

        [SerializeField, UsedImplicitly]
        private UILabel speedLabel;

        [SerializeField, UsedImplicitly]
        private UILabel damageLabel;

        [SerializeField, UsedImplicitly]
        private UILabel costLabel;

        private int costTotal;

        #endregion

        #region Const Fields

        private const string Cost = "{0}/{1}";

        #endregion

        #region Properties

        public string ShipName
        {
            get { return nameInput.value; }
        }

        #endregion

        #region Public Methods

        public void Initialize(string shipName, int costTotal)
        {
            nameInput.value = shipName;
            this.costTotal = costTotal;
        }


        public void SetValues(ShipStats stats, int costUsed)
        {
            healthLabel.text = stats.health.ToString();
            shieldLabel.text = stats.shield.ToString();
            speedLabel.text = stats.speed.ToString();
            damageLabel.text = stats.damage.ToString();
            costLabel.text = string.Format(Cost, costUsed, costTotal);
        }

        #endregion
    }
}