// Little Byte Games

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
        private GameObject stats;

        [SerializeField, UsedImplicitly]
        private UILabel healthLabel;

        [SerializeField, UsedImplicitly]
        private UILabel shieldLabel;

        [SerializeField, UsedImplicitly]
        private UILabel speedLabel;

        [SerializeField, UsedImplicitly]
        private UILabel damageLabel;

        [SerializeField, UsedImplicitly]
        private GameObject description;

        [SerializeField, UsedImplicitly]
        private UILabel descriptionLabel;

        [SerializeField, UsedImplicitly]
        private UILabel stockLabel;

        [SerializeField, UsedImplicitly]
        private UILabel costLabel;

        #endregion

        #region Public Methods

        public void RegisterToGrid()
        {
            GarageManager.Main.grid.StatusChangedEvent += OnCursorUpdated;
            OnCursorUpdated(GarageManager.Main.grid, new CursorUpdatedArgs(ConstructionGrid.CursorStatuses.None, GarageManager.Main.grid.cursorStatus));
        }

        public void Blank()
        {
            description.SetActive(false);
            stats.SetActive(false);

            nameLabel.text = string.Empty;
            healthLabel.text = string.Empty;
            shieldLabel.text = string.Empty;
            speedLabel.text = string.Empty;
            damageLabel.text = string.Empty;
            stockLabel.text = string.Empty;
            costLabel.text = string.Empty;
        }

        public void SetStats(CUBEInfo info)
        {
            stats.SetActive(true);
            description.SetActive(false);

            nameLabel.text = info.name;
            healthLabel.text = info.health.ToString();
            shieldLabel.text = info.shield.ToString();
            speedLabel.text = info.speed.ToString();
            damageLabel.text = info.damage.ToString();
            costLabel.text = info.cost.ToString();
        }

        public void SetStock(int stock)
        {
            stockLabel.text = stock.ToString();
        }

        public void SetDescription(CUBEInfo info)
        {
            SetDescription(info.name, "description for " + info.name);
        }

        public void SetDescription(string itemName, string description)
        {
            stats.SetActive(false);
            this.description.SetActive(true);

            nameLabel.text = itemName;
            descriptionLabel.text = description;
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
                    description.SetActive(info.type != CUBE.Types.System);
                    stats.SetActive(info.type == CUBE.Types.System);
                    break;

                case ConstructionGrid.CursorStatuses.Hover:
                    info = grid.HoverInfo;
                    description.SetActive(info.type != CUBE.Types.System);
                    stats.SetActive(info.type == CUBE.Types.System);
                    break;

                default:
                    Blank();
                    return;
            }

            SetStats(info);
        }

        #endregion
    }
}