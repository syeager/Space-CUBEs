// Little Byte Games
// Author: Steve Yeager
// Created: 2014.08.31
// Edited: 2014.08.31

using Annotations;
using UnityEngine;

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
    public int ID;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Awake()
    {
        switch (itemType)
        {
            case StoreManager.ItemTypes.CUBE:
                own.text = CUBE.GetInventory()[ID].ToString();
                break;
            case StoreManager.ItemTypes.Core:
                own.text = BuildStats.GetCoreLevel() >= ID ? "1" : "0";
                break;
            case StoreManager.ItemTypes.Weapon:
                own.text = BuildStats.GetWeaponLevel() >= ID ? "1" : "0";
                break;
            case StoreManager.ItemTypes.Augmentation:
                own.text = BuildStats.GetAugmentationLevel() >= ID ? "1" : "0";
                break;
        }
    }

    #endregion

    #region Public Methods

    public void Initialize(StoreManager.ItemTypes itemType, int ID, string itemName, int price, int cp)
    {
        name = itemName;
        this.itemType = itemType;
        this.ID = ID;
        this.itemName.text = itemName;
        buyPrice.text = StoreManager.FormatMoney(price, false);
        sellPrice.text = StoreManager.FormatMoney((int)(price * StoreManager.SellPercent), true);
        this.cp.text = cp.ToString();
    }


    public void Clicked()
    {
        Debug.Log("clicked");
        StoreManager.Main.ItemClicked(this, itemType, ID);
    }


    public void SetOwn(int own)
    {
        this.own.text = own.ToString();
    }

    #endregion
}