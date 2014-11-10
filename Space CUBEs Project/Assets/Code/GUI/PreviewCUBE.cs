// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.20
// Edited: 2014.09.20

using Annotations;
using UnityEngine;

namespace SpaceCUBEs
{
    public class PreviewCUBE : MonoBehaviour
    {
        #region Private Fields

        [SerializeField, UsedImplicitly]
        private UILabel nameLabel;

        [SerializeField, UsedImplicitly]
        private UILabel healthLabel;

        [SerializeField, UsedImplicitly]
        private UILabel shieldLabel;

        [SerializeField, UsedImplicitly]
        private UILabel speedLabel;

        [SerializeField, UsedImplicitly]
        private UILabel damageLabel;

        [SerializeField, UsedImplicitly]
        private UILabel stockLabel;

        [SerializeField, UsedImplicitly]
        private UILabel costLabel;

        #endregion

        #region MonoBehaviour Overrides

        [UsedImplicitly]
        private void Start()
        {
            GarageManager.Main.grid.StatusChangedEvent += OnCursorUpdated;
            OnCursorUpdated(GarageManager.Main.grid, new CursorUpdatedArgs(ConstructionGrid.CursorStatuses.None, GarageManager.Main.grid.cursorStatus));
        }

        #endregion

        #region Event Handlers

        private void OnCursorUpdated(object sender, CursorUpdatedArgs args)
        {
            ConstructionGrid grid = (ConstructionGrid)sender;
            CUBEInfo info;
            switch (args.current)
            {
                case ConstructionGrid.CursorStatuses.Holding:
                    info = grid.heldInfo;
                    break;

                case ConstructionGrid.CursorStatuses.Hover:
                    info = grid.HoverInfo;
                    break;

                default:

                    nameLabel.text = string.Empty;
                    healthLabel.text = string.Empty;
                    shieldLabel.text = string.Empty;
                    speedLabel.text = string.Empty;
                    damageLabel.text = string.Empty;
                    stockLabel.text = string.Empty;
                    costLabel.text = string.Empty;
                    return;
            }

            nameLabel.text = info.name;
            healthLabel.text = info.health.ToString();
            shieldLabel.text = info.shield.ToString();
            speedLabel.text = info.speed.ToString();
            damageLabel.text = info.damage.ToString();
            stockLabel.text = CUBE.GetInventory()[info.ID].ToString();
            costLabel.text = info.cost.ToString();
        }

        #endregion
    }
}