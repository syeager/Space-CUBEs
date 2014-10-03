// Little Byte Games
// Author: Steve Yeager
// Created: 2014.01.16
// Edited: 2014.08.31

using System;
using Annotations;
using UnityEngine;

/// <summary>
/// Manager for the Store.
/// </summary>
public class StoreManager : Singleton<StoreManager>
{
    #region Public Fields

    public StoreItem buttonPrefab;

    [Header("Filter")]
    public ActivateButton filterLeft;

    [SerializeField, UsedImplicitly]
    private UILabel filterLabel;

    [SerializeField, UsedImplicitly]
    private ActivateButton filterRight;

    public GameObject[] itemGrids;

    [Header("HUD")]
    [SerializeField, UsedImplicitly]
    private UILabel selectedCUBE;

    [SerializeField, UsedImplicitly]
    private UILabel health;

    [SerializeField, UsedImplicitly]
    private UILabel shield;

    [SerializeField, UsedImplicitly]
    private UILabel speed;

    [SerializeField, UsedImplicitly]
    private UILabel damage;

    [SerializeField, UsedImplicitly]
    private UILabel count;

    [SerializeField, UsedImplicitly]
    private UILabel cpLabel;

    [Header("Bank")]
    [SerializeField, UsedImplicitly]
    private UILabel bank;

    [SerializeField, UsedImplicitly]
    private UILabel sellPrice;

    [SerializeField, UsedImplicitly]
    private UILabel buyPrice;

    [SerializeField, UsedImplicitly]
    private UIButton sellButton;

    [SerializeField, UsedImplicitly]
    private UIButton buyButton;

    [Header("Showcase")]
    [SerializeField, UsedImplicitly]
    private Transform showcase;

    #endregion

    #region Private Fields

    private int[] inventory;

    /// <summary>Current index of item selected. Changes depending on ItemType.</summary>
    private int itemIndex;

    private StoreItem currentItem;

    /// <summary>Current filtered index.</summary>
    private int filter;

    private Transform showcaseItem;

    /// <summary>Different types of items available for purchasing.</summary>
    public enum ItemTypes
    {
        CUBE,
        Core,
        Weapon,
        Augmentation
    }

    /// <summary>Current item type being looked at.</summary>
    private ItemTypes currentItemType;

    #endregion

    #region Static Fields

    private static readonly string[] Items =
    {
        "System",
        "Hull",
        "Weapon",
        "Aug",
        "Upgrade",
    };

    #endregion

    #region Const Fields

    public const float SellPercent = 0.5f;
    private const string OwnSuffix = " Own";

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Start()
    {
        inventory = CUBE.GetInventory();

        // reset GUI
        bank.text = FormatMoney(MoneyManager.Balance());
        selectedCUBE.text = "";
        sellPrice.text = "";
        buyPrice.text = "";
        sellButton.isEnabled = false;
        buyButton.isEnabled = false;

        ChangeFilter("-1");
    }

    #endregion

    #region Public Methods

    public void Sell()
    {
        if (currentItemType != ItemTypes.CUBE) return;

        inventory[itemIndex]--;
        CUBE.SetInventory(inventory);
        currentItem.SetOwn(inventory[itemIndex]);
        int balance = MoneyManager.Transaction((int)(CUBE.AllCUBES[itemIndex].price * SellPercent));
        bank.text = FormatMoney(balance);
        count.text = inventory[itemIndex] + OwnSuffix;
        UpdateShopButtons(itemIndex);
    }


    public void Buy()
    {
        int balance;
        switch (currentItemType)
        {
            case ItemTypes.CUBE:
                inventory[itemIndex]++;
                CUBE.SetInventory(inventory);
                currentItem.SetOwn(inventory[itemIndex]);
                balance = MoneyManager.Transaction(-CUBE.AllCUBES[itemIndex].price);
                bank.text = FormatMoney(balance);
                count.text = inventory[itemIndex] + OwnSuffix;
                UpdateShopButtons(itemIndex);
                break;
            case ItemTypes.Core:
                balance = MoneyManager.Transaction(-BuildStats.CorePrices[BuildStats.GetCoreLevel()]);
                BuildStats.SetCoreLevel(BuildStats.GetCoreLevel() + 1);
                currentItem.SetOwn(1);
                bank.text = FormatMoney(balance);
                count.text = 1 + OwnSuffix;
                UpdateShopButtons(BuildStats.GetCoreLevel());
                break;
            case ItemTypes.Weapon:
                balance = MoneyManager.Transaction(-BuildStats.WeaponPrices[BuildStats.GetWeaponLevel()]);
                BuildStats.SetWeaponLevel(BuildStats.GetWeaponLevel() + 1);
                currentItem.SetOwn(1);
                bank.text = FormatMoney(balance);
                count.text = 1 + OwnSuffix;
                UpdateShopButtons(BuildStats.GetWeaponLevel());
                break;
            case ItemTypes.Augmentation:
                balance = MoneyManager.Transaction(-BuildStats.AugmentationPrices[BuildStats.GetAugmentationLevel()]);
                BuildStats.SetAugmentationLevel(BuildStats.GetAugmentationLevel() + 1);
                currentItem.SetOwn(1);
                bank.text = FormatMoney(balance);
                count.text = 1 + OwnSuffix;
                UpdateShopButtons(BuildStats.GetAugmentationLevel());
                break;
        }
    }


    public void ItemClicked(StoreItem storeItem, ItemTypes itemType, int ID)
    {
        currentItem = storeItem;

        switch (itemType)
        {
            case ItemTypes.CUBE:
                currentItemType = ItemTypes.CUBE;

                // selected CUBE
                itemIndex = ID;
                CUBEInfo info = CUBE.AllCUBES[itemIndex];
                if (GameResources.Main.CUBE_Prefabs[info.ID] == null) return;
                selectedCUBE.text = info.name;
                count.text = inventory[itemIndex] + OwnSuffix;

                // prices
                sellPrice.text = FormatMoney((int)(info.price * SellPercent), true);
                buyPrice.text = FormatMoney(info.price, false);

                // enable buttons
                UpdateShopButtons(itemIndex);

                // stats
                health.text = info.health.ToString();
                shield.text = info.shield.ToString();
                speed.text = info.speed.ToString();
                damage.text = info.damage.ToString();

                // cp
                const string cp = " CP";
                cpLabel.text = info.cost + cp;

                // showcase
                UpdateShowCase(GameResources.CreateCUBE(itemIndex).gameObject);
                break;
            case ItemTypes.Core:
                currentItemType = ItemTypes.Core;

                // showcase
                selectedCUBE.text = "Ship Core Lv " + (ID + 1);
                count.text = (BuildStats.GetCoreLevel() >= ID ? "1" : "0") + OwnSuffix;
                UpdateShowCase(GameObject.CreatePrimitive(PrimitiveType.Cube));

                // prices
                sellPrice.text = string.Empty;
                buyPrice.text = FormatMoney(BuildStats.CorePrices[ID], false);

                // buttons
                UpdateShopButtons(ID);
                break;
            case ItemTypes.Weapon:
                currentItemType = ItemTypes.Weapon;

                // showcase
                selectedCUBE.text = "Weapon Exp Lv " + (ID + 1);
                count.text = (BuildStats.GetWeaponLevel() >= ID ? "1" : "0") + OwnSuffix;
                UpdateShowCase(GameObject.CreatePrimitive(PrimitiveType.Capsule));

                // prices
                sellPrice.text = string.Empty;
                buyPrice.text = FormatMoney(BuildStats.WeaponPrices[ID], false);

                // buttons
                UpdateShopButtons(ID);
                break;
            case ItemTypes.Augmentation:
                currentItemType = ItemTypes.Augmentation;
                // showcase
                selectedCUBE.text = "Aug Exp Lv " + (ID + 1);
                count.text = (BuildStats.GetAugmentationLevel() >= ID ? "1" : "0") + OwnSuffix;
                UpdateShowCase(GameObject.CreatePrimitive(PrimitiveType.Cylinder));

                // prices
                sellPrice.text = string.Empty;
                buyPrice.text = FormatMoney(BuildStats.AugmentationPrices[ID], false);

                // buttons
                UpdateShopButtons(ID);
                break;
        }
    }


    /// <summary>
    /// Change current filter for items.
    /// </summary>
    /// <param name="index">Moving left or right.</param>
    public void ChangeFilter(string index)
    {
        itemGrids[filter].SetActive(false);

        filter = Mathf.Clamp(filter + int.Parse(index), 0, Items.Length - 1);

        itemGrids[filter].SetActive(true);
        filterLabel.text = Items[filter];
        filterLeft.isEnabled = filter != 0;
        filterRight.isEnabled = filter != Items.Length - 1;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Set buy and sell buttons to appropriate states for selected item.
    /// </summary>
    private void UpdateShopButtons(int index)
    {
        switch (currentItemType)
        {
            case ItemTypes.CUBE:
                sellButton.isEnabled = inventory[index] > 0;
                buyButton.isEnabled = MoneyManager.Balance() >= CUBE.AllCUBES[index].price;
                break;
            case ItemTypes.Core:
                sellButton.isEnabled = false;
                buyButton.isEnabled = index == BuildStats.GetCoreLevel() + 1 &&
                                      MoneyManager.Balance() >= BuildStats.CorePrices[BuildStats.GetCoreLevel()];
                break;
            case ItemTypes.Weapon:
                sellButton.isEnabled = false;
                buyButton.isEnabled = index == BuildStats.GetWeaponLevel() + 1 &&
                                      MoneyManager.Balance() >= BuildStats.WeaponPrices[BuildStats.GetWeaponLevel()];
                break;
            case ItemTypes.Augmentation:
                sellButton.isEnabled = false;
                buyButton.isEnabled = index == BuildStats.GetAugmentationLevel() + 1 &&
                                      MoneyManager.Balance() >= BuildStats.AugmentationPrices[BuildStats.GetAugmentationLevel()];
                break;
        }
    }


    private void UpdateShowCase(GameObject newShowcase)
    {
        if (showcaseItem != null) Destroy(showcaseItem.gameObject);
        showcaseItem = newShowcase.transform;
        showcaseItem.parent = showcase;
        showcaseItem.localScale = Vector3.one;
        showcaseItem.localRotation = Quaternion.identity;
        showcaseItem.localPosition = -showcaseItem.GetComponent<MeshFilter>().mesh.bounds.center;
    }

    #endregion

    #region Static Methods

    public static string FormatMoney(int amount, bool? positive = null)
    {
        const string pos = "+";
        const string neg = "-";
        return (positive != null ? (positive.Value ? pos : neg) : string.Empty)+ String.Format("{0:#,###0}", amount);
    } 

    #endregion
}