// Little Byte Games
// Author: Steve Yeager
// Created: 2014.08.31
// Edited: 2014.10.19

using Annotations;
using UnityEngine;

namespace SpaceCUBEs
{
    public class StoreItem : MonoBehaviour
    {
        #region Public Fields

        public UISprite preview;
        public UISprite icon;
        public UILabel itemName;
        public UILabel buyPrice;
        public UILabel sellPrice;
        public UILabel cp;
        public UILabel own;

        public StoreManager.ItemTypes itemType;
        public int id;

        #endregion

        #region Private Fields

        [SerializeField, UsedImplicitly]
        private CubeLibrary library;

        #endregion

        #region MonoBehaviour Overrides

        [UsedImplicitly]
        private void Awake()
        {
            switch (itemType)
            {
                case StoreManager.ItemTypes.CUBE:
                    own.text = CUBE.GetInventory()[id].ToString();
                    break;
                case StoreManager.ItemTypes.Core:
                    own.text = BuildStats.GetCoreLevel() >= id ? "1" : "0";
                    break;
                case StoreManager.ItemTypes.Weapon:
                    own.text = BuildStats.GetWeaponLevel() >= id ? "1" : "0";
                    break;
                case StoreManager.ItemTypes.Augmentation:
                    own.text = BuildStats.GetAugmentationLevel() >= id ? "1" : "0";
                    break;
            }

            if (GameResources.Main.CUBE_Prefabs[id] == null)
            {
                GetComponent<UIButton>().isEnabled = false;
                return;
            }

            GetComponent<ActivateButton>().ActivateEvent += (sender, args) =>
                                                            {
                                                                if (!args.isPressed)
                                                                {
                                                                    library.ItemSelected(this, itemType, id);
                                                                }
                                                            };
        }

        #endregion

        #region Public Methods

        public void Initialize(StoreManager.ItemTypes itemType, int id, string itemName, int price, int cp, CubeLibrary library)
        {
            this.library = library;
            name = itemName;
            this.itemType = itemType;
            this.id = id;
            this.itemName.text = itemName;
            buyPrice.text = StoreManager.FormatMoney(price, false);
            sellPrice.text = StoreManager.FormatMoney((int)(price * StoreManager.SellPercent), true);
            this.cp.text = cp.ToString();
        }

        public void SetOwn(int own)
        {
            this.own.text = own.ToString();
        }

        #endregion
    }
}