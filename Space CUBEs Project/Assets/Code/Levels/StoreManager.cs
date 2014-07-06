// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.01.16
// Edited: 2014.05.31

using System;
using System.Collections;
using System.Linq;
using Annotations;
using UnityEngine;

/// <summary>
/// Manager for the Store.
/// </summary>
public class StoreManager : MonoBehaviour
{
    #region Public Fields

    public ActivateButton filterLeft;
    public UILabel filterLabel;
    public ActivateButton filterRight;
    public GameObject Button_Prefab;
    public UILabel bank;
    public UILabel selectedCUBE;
    public UILabel count;
    public UILabel sellPrice;
    public UILabel buyPrice;
    public UIButton sellButton;
    public UIButton buyButton;

    public UILabel health;
    public UILabel shield;
    public UILabel speed;
    public UILabel damage;

    public Transform showcase;

    public GameObject[] selections;

    #endregion

    #region Private Fields

    private int[] inventory;

    /// <summary>Current index of item selected. Changes depending on ItemType.</summary>
    private int itemIndex;

    /// <summary>Current filtered index.</summary>
    private int filter;

    private Transform showcaseItem;

    private ScrollviewButton[][] itemButtons;

    private UIGrid[] grids;
    private UIScrollView[] scrollViews;
    private UIScrollBar[] scrollBars;

    /// <summary>Different types of items available for purchasing.</summary>
    private enum ItemTypes
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
        "Augmentation",
        "Core",
        "W Expansion",
        "A Expansion"
    };

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Start()
    {
        inventory = CUBE.GetInventory();

        // get references
        grids = new UIGrid[selections.Length];
        scrollViews = new UIScrollView[selections.Length];
        scrollBars = new UIScrollBar[selections.Length];
        for (int i = 0; i < selections.Length; i++)
        {
            grids[i] = selections[i].GetComponentInChildren(typeof(UIGrid)) as UIGrid;
            scrollViews[i] = selections[i].GetComponentInChildren(typeof(UIScrollView)) as UIScrollView;
            scrollBars[i] = selections[i].GetComponentInChildren(typeof(UIScrollBar)) as UIScrollBar;
        }

        // reset GUI
        bank.text = String.Format("${0:#,###0}", MoneyManager.Balance());
        selectedCUBE.text = "";
        sellPrice.text = "";
        buyPrice.text = "";
        sellButton.isEnabled = false;
        buyButton.isEnabled = false;

        CreateItemButtons();
        ChangeFilter(0);
        StartCoroutine(InitScrollViews());

        // register events
        filterLeft.ActivateEvent += OnFilterMoved;
        filterRight.ActivateEvent += OnFilterMoved;
    }

    #endregion

    #region Button Methods

    public void Sell()
    {
        if (currentItemType != ItemTypes.CUBE) return;

        inventory[itemIndex]--;
        CUBE.SetInventory(inventory);
        int balance = MoneyManager.Transaction(CUBE.AllCUBES[itemIndex].price / 2);
        bank.text = String.Format("${0:#,###0}", balance);
        count.text = "You have: " + inventory[itemIndex];
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
                balance = MoneyManager.Transaction(-CUBE.AllCUBES[itemIndex].price);
                bank.text = String.Format("${0:#,###0}", balance);
                count.text = "You have: " + inventory[itemIndex];
                UpdateShopButtons(itemIndex);
                break;
            case ItemTypes.Core:
                balance = MoneyManager.Transaction(-BuildStats.CorePrices[BuildStats.GetCoreLevel()]);
                BuildStats.SetCoreLevel(BuildStats.GetCoreLevel() + 1);
                bank.text = String.Format("${0:#,###0}", balance);
                count.text = "Own";
                UpdateShopButtons(BuildStats.GetCoreLevel());
                break;
            case ItemTypes.Weapon:
                balance = MoneyManager.Transaction(-BuildStats.WeaponPrices[BuildStats.GetWeaponLevel()]);
                BuildStats.SetWeaponLevel(BuildStats.GetWeaponLevel() + 1);
                bank.text = String.Format("${0:#,###0}", balance);
                count.text = "Own";
                UpdateShopButtons(BuildStats.GetWeaponLevel());
                break;
            case ItemTypes.Augmentation:
                balance = MoneyManager.Transaction(-BuildStats.AugmentationPrices[BuildStats.GetAugmentationLevel()]);
                BuildStats.SetAugmentationLevel(BuildStats.GetAugmentationLevel() + 1);
                bank.text = String.Format("${0:#,###0}", balance);
                count.text = "Own";
                UpdateShopButtons(BuildStats.GetAugmentationLevel());
                break;
        }
    }


    /// <summary>
    /// Load Garage Level.
    /// </summary>
    public void LoadGarage()
    {
        SceneManager.LoadScene("Garage");
    }


    /// <summary>
    /// Load Main Menu Level.
    /// </summary>
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
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


    /// <summary>
    /// Change current filter for items.
    /// </summary>
    /// <param name="index">Index to change filter to.</param>
    private void ChangeFilter(int index)
    {
        selections[filter].SetActive(false);

        filter = Mathf.Clamp(index, 0, Items.Length - 1);

        selections[filter].SetActive(true);
        filterLabel.text = Items[filter];
        filterLeft.isEnabled = filter != 0;
        filterRight.isEnabled = filter != Items.Length - 1;
    }


    /// <summary>
    /// Creates all buttons for buyable items and catagorizes them.
    /// </summary>
    private void CreateItemButtons()
    {
        itemButtons = new ScrollviewButton[Items.Length][];
        CUBEInfo[] cubeInfo;

        // system
        cubeInfo = CUBE.AllCUBES.Where(c => c.type == CUBE.Types.System).ToArray();
        itemButtons[0] = new ScrollviewButton[cubeInfo.Length];
        for (int i = 0; i < cubeInfo.Length; i++)
        {
            var button = (Instantiate(Button_Prefab) as GameObject).GetComponent(typeof(ScrollviewButton)) as ScrollviewButton;
            button.GetComponent<ScrollviewButton>().Initialize(cubeInfo[i].name, cubeInfo[i].name + "\nx " + inventory[cubeInfo[i].ID], cubeInfo[i].ID.ToString(), grids[0].transform, scrollViews[0]);
            button.ActivateEvent += OnCUBEButtonSelected;
            itemButtons[0][i] = button;
        }
        // hull
        cubeInfo = CUBE.AllCUBES.Where(c => c.type == CUBE.Types.Hull).ToArray();
        itemButtons[1] = new ScrollviewButton[cubeInfo.Length];
        for (int i = 0; i < cubeInfo.Length; i++)
        {
            var button = (Instantiate(Button_Prefab) as GameObject).GetComponent(typeof(ScrollviewButton)) as ScrollviewButton;
            button.GetComponent<ScrollviewButton>().Initialize(cubeInfo[i].name, cubeInfo[i].name + "\nx " + inventory[cubeInfo[i].ID], cubeInfo[i].ID.ToString(), grids[1].transform, scrollViews[1]);
            button.ActivateEvent += OnCUBEButtonSelected;
            itemButtons[1][i] = button;
        }
        // weapon
        cubeInfo = CUBE.AllCUBES.Where(c => c.type == CUBE.Types.Weapon).ToArray();
        itemButtons[2] = new ScrollviewButton[cubeInfo.Length];
        for (int i = 0; i < cubeInfo.Length; i++)
        {
            var button = (Instantiate(Button_Prefab) as GameObject).GetComponent(typeof(ScrollviewButton)) as ScrollviewButton;
            ((ScrollviewButton)button.GetComponent(typeof(ScrollviewButton))).Initialize(cubeInfo[i].name, cubeInfo[i].name + "\nx " + inventory[cubeInfo[i].ID], cubeInfo[i].ID.ToString(), grids[2].transform, scrollViews[2]);
            button.ActivateEvent += OnCUBEButtonSelected;
            itemButtons[2][i] = button;
        }
        // augmentation
        cubeInfo = CUBE.AllCUBES.Where(c => c.type == CUBE.Types.Augmentation).ToArray();
        itemButtons[3] = new ScrollviewButton[cubeInfo.Length];
        for (int i = 0; i < cubeInfo.Length; i++)
        {
            var button = (Instantiate(Button_Prefab) as GameObject).GetComponent(typeof(ScrollviewButton)) as ScrollviewButton;
            ((ScrollviewButton)button.GetComponent(typeof(ScrollviewButton))).Initialize(cubeInfo[i].name, cubeInfo[i].name + "\nx " + inventory[cubeInfo[i].ID], cubeInfo[i].ID.ToString(), grids[3].transform, scrollViews[3]);
            button.ActivateEvent += OnCUBEButtonSelected;
            itemButtons[3][i] = button;
        }

        // core
        itemButtons[4] = new ScrollviewButton[BuildStats.CoreCapacities.Length];
        for (int i = 0; i < itemButtons[4].Length; i++)
        {
            var button = (Instantiate(Button_Prefab) as GameObject).GetComponent(typeof(ScrollviewButton)) as ScrollviewButton;
            ((ScrollviewButton)button.GetComponent(typeof(ScrollviewButton)))
                .Initialize("Core " + i, "Core " + (i + 1), i.ToString(), grids[4].transform, scrollViews[4]);
            button.ActivateEvent += OnCoreButtonSelected;
            itemButtons[4][i] = button;
        }
        // weapon expansion
        itemButtons[5] = new ScrollviewButton[BuildStats.WeaponExpansions.Length];
        for (int i = 0; i < itemButtons[5].Length; i++)
        {
            var button = (Instantiate(Button_Prefab) as GameObject).GetComponent(typeof(ScrollviewButton)) as ScrollviewButton;
            ((ScrollviewButton)button.GetComponent(typeof(ScrollviewButton)))
                .Initialize("Weapon Ext " + i, "Weapon Ext " + (i + 1), i.ToString(), grids[5].transform, scrollViews[5]);
            button.ActivateEvent += OnWeaponButtonSelected;
            itemButtons[5][i] = button;
        }
        // augmentation expansion
        itemButtons[6] = new ScrollviewButton[BuildStats.AugmentationExpansions.Length];
        for (int i = 0; i < itemButtons[6].Length; i++)
        {
            var button = (Instantiate(Button_Prefab) as GameObject).GetComponent(typeof(ScrollviewButton)) as ScrollviewButton;
            ((ScrollviewButton)button.GetComponent(typeof(ScrollviewButton)))
                .Initialize("Augmentation Ext " + i, "Augmentation Ext " + (i + 1), i.ToString(), grids[6].transform, scrollViews[6]);
            button.ActivateEvent += OnAugmentationButtonSelected;
            itemButtons[6][i] = button;
        }
    }


    private IEnumerator InitScrollViews()
    {
        for (int i = 0; i < selections.Length; i++)
        {
            StartCoroutine(Utility.UpdateScrollView((UIGrid)grids[i].GetComponent(typeof(UIGrid)), scrollBars[i], scrollViews[i]));
        }

        yield return new WaitForEndOfFrame();

        for (int i = 1; i < selections.Length; i++)
        {
            selections[i].SetActive(false);
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

    #region Event Handlers

    /// <summary>
    /// Left and right filter buttons pressed.
    /// </summary>
    private void OnFilterMoved(object sender, ActivateButtonArgs args)
    {
        if (!args.isPressed) return;

        ChangeFilter(filter + int.Parse(args.value));
    }


    private void OnCUBEButtonSelected(object sender, ActivateButtonArgs args)
    {
        if (!args.isPressed) return;

        currentItemType = ItemTypes.CUBE;

        // selected CUBE
        itemIndex = int.Parse(args.value);
        CUBEInfo info = CUBE.AllCUBES[itemIndex];
        if (GameResources.GetCUBE(info.ID) == null) return;
        selectedCUBE.text = info.name;
        count.text = "x" + inventory[itemIndex];

        // prices
        sellPrice.text = "+$" + (info.price / 2f);
        buyPrice.text = "-$" + info.price;

        // enable buttons
        UpdateShopButtons(itemIndex);

        // stats
        health.text = info.health.ToString();
        shield.text = info.shield.ToString();
        speed.text = info.speed.ToString();
        damage.text = info.damage.ToString();

        // showcase
        UpdateShowCase((GameObject)Instantiate(GameResources.GetCUBE(itemIndex).gameObject));
    }


    /// <summary>
    /// Core button selected for viewing.
    /// </summary>
    /// <param name="sender">Button pressed.</param>
    /// <param name="args">Button's 0 indexed core level.</param>
    private void OnCoreButtonSelected(object sender, ActivateButtonArgs args)
    {
        currentItemType = ItemTypes.Core;
        int level = int.Parse(args.value);

        // showcase
        selectedCUBE.text = "Ship Core Lv " + (level + 1);
        count.text = BuildStats.GetCoreLevel() >= level ? "Own" : "Don't Own";
        UpdateShowCase(GameObject.CreatePrimitive(PrimitiveType.Cube));

        // prices
        int price = BuildStats.CorePrices[level];
        sellPrice.text = string.Empty;
        buyPrice.text = "-$" + price;

        // buttons
        UpdateShopButtons(level);
    }


    private void OnWeaponButtonSelected(object sender, ActivateButtonArgs args)
    {
        currentItemType = ItemTypes.Weapon;
        int level = int.Parse(args.value);

        // showcase
        selectedCUBE.text = "Weapon Expansion Lv " + (level + 1);
        count.text = BuildStats.GetWeaponLevel() >= level ? "Own" : "Don't Own";
        UpdateShowCase(GameObject.CreatePrimitive(PrimitiveType.Capsule));

        // prices
        int price = BuildStats.WeaponPrices[level];
        sellPrice.text = string.Empty;
        buyPrice.text = "-$" + price;

        // buttons
        UpdateShopButtons(level);
    }


    private void OnAugmentationButtonSelected(object sender, ActivateButtonArgs args)
    {
        currentItemType = ItemTypes.Augmentation;
        int level = int.Parse(args.value);

        // showcase
        selectedCUBE.text = "Augmentation Expansion Lv " + (level + 1);
        count.text = BuildStats.GetAugmentationLevel() >= level ? "Own" : "Don't Own";
        UpdateShowCase(GameObject.CreatePrimitive(PrimitiveType.Cylinder));

        // prices
        int price = BuildStats.AugmentationPrices[level];
        sellPrice.text = string.Empty;
        buyPrice.text = "-$" + price;

        // buttons
        UpdateShopButtons(level);
    }

    #endregion
}