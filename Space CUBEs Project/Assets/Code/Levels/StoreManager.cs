// Steve Yeager
// 1.15.2014

using System.Collections;
using UnityEngine;
using System.Linq;
using System;

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
    private int index;
    private Transform showcaseCUBE;

    /// <summary>Current filtered item index.</summary>
    private int filter;

    private ScrollviewButton[][] itemButtons;

    private UIGrid[] grids;
    private UIScrollView[] scrollViews;
    private UIScrollBar[] scrollBars;

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
        inventory[index]--;
        CUBE.SetInventory(inventory);
        int balance = MoneyManager.Transaction(CUBE.allCUBES[index].price / 2);
        bank.text = String.Format("${0:#,###0}", balance);
        count.text = "You have: " + inventory[index];

        UpdateShopButtons();
    }


    public void Buy()
    {
        inventory[index]++;
        CUBE.SetInventory(inventory);
        int balance = MoneyManager.Transaction(-CUBE.allCUBES[index].price);
        bank.text = String.Format("${0:#,###0}", balance);
        count.text = "You have: " + inventory[index];

        UpdateShopButtons();
    }


    /// <summary>
    /// Load Garage Level.
    /// </summary>
    public void LoadGarage()
    {
        GameData.LoadLevel("Garage");
    }


    /// <summary>
    /// Load Main Menu Level.
    /// </summary>
    public void LoadMainMenu()
    {
        GameData.LoadLevel("Main Menu");
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Set buy and sell buttons to appropriate states for selected item.
    /// </summary>
    private void UpdateShopButtons()
    {
        sellButton.isEnabled = inventory[index] > 0;
        buyButton.isEnabled = MoneyManager.Balance() >= CUBE.allCUBES[index].price;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    private void ChangeFilter(int index)
    {
        selections[this.index].SetActive(false);

        this.index = Mathf.Clamp(index, 0, Items.Length-1);

        selections[this.index].SetActive(true);
        filterLabel.text = Items[this.index];
        filterLeft.isEnabled = this.index != 0;
        filterRight.isEnabled = this.index != Items.Length - 1;
    }


    /// <summary>
    /// Creates all buttons for buyable items and catagorizes them.
    /// </summary>
    private void CreateItemButtons()
    {
        itemButtons = new ScrollviewButton[Items.Length][];
        CUBEInfo[] cubeInfo;

        // system
        cubeInfo = CUBE.allCUBES.Where(c => c.type == CUBE.Types.System).ToArray();
        itemButtons[0] = new ScrollviewButton[cubeInfo.Length];
        for (int i = 0; i < cubeInfo.Length; i++)
        {
            ScrollviewButton button = (Instantiate(Button_Prefab) as GameObject).GetComponent(typeof(ScrollviewButton)) as ScrollviewButton;
            button.GetComponent<ScrollviewButton>().Initialize(cubeInfo[i].name, cubeInfo[i].name + "\nx " + inventory[cubeInfo[i].ID], cubeInfo[i].ID.ToString(), grids[0].transform, scrollViews[0]);
            button.ActivateEvent += OnCUBEButtonSelected;
            itemButtons[0][i] = button;
        }
        // hull
        cubeInfo = CUBE.allCUBES.Where(c => c.type == CUBE.Types.Hull).ToArray();
        itemButtons[1] = new ScrollviewButton[cubeInfo.Length];
        for (int i = 0; i < cubeInfo.Length; i++)
        {
            ScrollviewButton button = (Instantiate(Button_Prefab) as GameObject).GetComponent(typeof(ScrollviewButton)) as ScrollviewButton;
            button.GetComponent<ScrollviewButton>().Initialize(cubeInfo[i].name, cubeInfo[i].name + "\nx " + inventory[cubeInfo[i].ID], cubeInfo[i].ID.ToString(), grids[1].transform, scrollViews[1]);
            button.ActivateEvent += OnCUBEButtonSelected;
            itemButtons[1][i] = button;
        }
        // weapon
        cubeInfo = CUBE.allCUBES.Where(c => c.type == CUBE.Types.Weapon).ToArray();
        itemButtons[2] = new ScrollviewButton[cubeInfo.Length];
        for (int i = 0; i < cubeInfo.Length; i++)
        {
            ScrollviewButton button = (Instantiate(Button_Prefab) as GameObject).GetComponent(typeof(ScrollviewButton)) as ScrollviewButton;
            button.GetComponent<ScrollviewButton>().Initialize(cubeInfo[i].name, cubeInfo[i].name + "\nx " + inventory[cubeInfo[i].ID], cubeInfo[i].ID.ToString(), grids[2].transform, scrollViews[2]);
            button.ActivateEvent += OnCUBEButtonSelected;
            itemButtons[2][i] = button;
        }
        // augmentation
        cubeInfo = CUBE.allCUBES.Where(c => c.type == CUBE.Types.Augmentation).ToArray();
        itemButtons[3] = new ScrollviewButton[cubeInfo.Length];
        for (int i = 0; i < cubeInfo.Length; i++)
        {
            ScrollviewButton button = (Instantiate(Button_Prefab) as GameObject).GetComponent(typeof(ScrollviewButton)) as ScrollviewButton;
            button.GetComponent<ScrollviewButton>().Initialize(cubeInfo[i].name, cubeInfo[i].name + "\nx " + inventory[cubeInfo[i].ID], cubeInfo[i].ID.ToString(), grids[3].transform, scrollViews[3]);
            button.ActivateEvent += OnCUBEButtonSelected;
            itemButtons[3][i] = button;
        }
        // core
        itemButtons[4] = new ScrollviewButton[BuildStats.CoreCapacities.Length];
        for (int i = 0; i < itemButtons[4].Length; i++)
        {
            ScrollviewButton button = (Instantiate(Button_Prefab) as GameObject).GetComponent(typeof(ScrollviewButton)) as ScrollviewButton;
            button.GetComponent<ScrollviewButton>().Initialize("Core " + BuildStats.CoreCapacities[i], "Core " + BuildStats.CoreCapacities[i], BuildStats.CoreCapacities[i].ToString(), grids[4].transform, scrollViews[4]);
            button.ActivateEvent += OnExpansionButtonSelected;
            itemButtons[4][i] = button;
        }
        // weapon expansion
        itemButtons[5] = new ScrollviewButton[BuildStats.WeaponExpansions.Length];
        for (int i = 0; i < itemButtons[5].Length; i++)
        {
            ScrollviewButton button = (Instantiate(Button_Prefab) as GameObject).GetComponent(typeof(ScrollviewButton)) as ScrollviewButton;
            button.GetComponent<ScrollviewButton>().Initialize("Core " + BuildStats.WeaponExpansions[i], "W Expansion " + BuildStats.WeaponExpansions[i], BuildStats.WeaponExpansions[i].ToString(), grids[5].transform, scrollViews[5]);
            button.ActivateEvent += OnExpansionButtonSelected;
            itemButtons[5][i] = button;
        }
        // augmentation expansion
        itemButtons[6] = new ScrollviewButton[BuildStats.AugmentationExpansions.Length];
        for (int i = 0; i < itemButtons[6].Length; i++)
        {
            ScrollviewButton button = (Instantiate(Button_Prefab) as GameObject).GetComponent(typeof(ScrollviewButton)) as ScrollviewButton;
            button.GetComponent<ScrollviewButton>().Initialize("Core " + BuildStats.AugmentationExpansions[i], "A Expansion " + BuildStats.AugmentationExpansions[i], BuildStats.AugmentationExpansions[i].ToString(), grids[6].transform, scrollViews[6]);
            button.ActivateEvent += OnExpansionButtonSelected;
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

    #endregion

    #region Event Handlers

    private void OnFilterMoved(object sender, ActivateButtonArgs args)
    {
        if (!args.isPressed) return;

        ChangeFilter(index + int.Parse(args.value));
    }


    private void OnCUBEButtonSelected(object sender, ActivateButtonArgs args)
    {
        if (!args.isPressed) return;

        // selected CUBE
        int selected = int.Parse(args.value);
        CUBEInfo info = CUBE.allCUBES[selected];
        if (GameResources.GetCUBE(info.ID) == null) return;
        selectedCUBE.text = info.name;
        count.text = "x" + inventory[selected];

        // prices
        sellPrice.text = "+$" + (info.price / 2f);
        buyPrice.text = "-$" + info.price;

        // enable buttons
        UpdateShopButtons();

        // stats
        health.text = info.health.ToString();
        shield.text = info.shield.ToString();
        speed.text = info.speed.ToString();

        // showcase
        if (showcaseCUBE != null) Destroy(showcaseCUBE.gameObject);
        showcaseCUBE = ((GameObject)Instantiate(GameResources.GetCUBE(selected).gameObject)).transform;
        showcaseCUBE.parent = showcase;
        showcaseCUBE.localScale = Vector3.one;
        showcaseCUBE.localRotation = Quaternion.identity;
        showcaseCUBE.localPosition = -showcaseCUBE.GetComponent<MeshFilter>().mesh.bounds.center;
    }


    private void OnExpansionButtonSelected(object sender, ActivateButtonArgs args)
    {

    }

    #endregion
}