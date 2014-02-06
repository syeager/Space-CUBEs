// Steve Yeager
// 1.15.2014

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

/// <summary>
/// Manager for the Store.
/// </summary>
public class StoreManager : MonoBehaviour
{
    #region Public Fields

    public ActivateButton filterLeft;
    public UILabel filter;
    public ActivateButton filterRight;
    public Transform Grid;
    public UIScrollBar scrollBar;
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
    public Transform showcase;

    #endregion

    #region Private Fields

    private int[] inventory;
    private int index;
    private CUBE.Types filterType;
    private bool allTypes = true;
    private Transform showcaseCUBE;
    private Dictionary<CUBE.Types, List<CUBEInfo>> filterLists;
    private ActivateButton[] activeButtons;
    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        inventory = CUBE.GetInventory();
        activeButtons = new ActivateButton[inventory.Length];
        filterLists = new Dictionary<CUBE.Types,List<CUBEInfo>>();
        for (int i = 0; i < Enum.GetNames(typeof(CUBE.Types)).Length; i++)
        {
            filterLists.Add((CUBE.Types)i, CUBE.allCUBES.Where(c => c.type == (CUBE.Types)i).ToList());
        }

        // reset GUI
        bank.text = String.Format("${0:#,###0}", MoneyManager.Balance());
        selectedCUBE.text = "";
        sellPrice.text = "";
        buyPrice.text = "";
        sellButton.isEnabled = false;
        buyButton.isEnabled = false;

        AllFilters();

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
        int balance = MoneyManager.Transaction(CUBE.allCUBES[index].price/2);
        bank.text = String.Format("${0:#,###0}", balance);
        count.text = "You have: " + inventory[index];
        activeButtons[index].GetComponent<CUBEButton>().label.text = CUBE.allCUBES[index].name + " x " + inventory[index];

        UpdateShopButtons();
    }


    public void Buy()
    {
        inventory[index]++;
        CUBE.SetInventory(inventory);
        int balance = MoneyManager.Transaction(-CUBE.allCUBES[index].price);
        bank.text = String.Format("${0:#,###0}", balance);
        count.text = "You have: " + inventory[index];
        activeButtons[index].GetComponent<CUBEButton>().label.text = CUBE.allCUBES[index].name + " x " + inventory[index];

        UpdateShopButtons();
    }


    public void LoadGarage()
    {
        GameData.LoadLevel("Garage");
    }


    public void LoadMainMenu()
    {
        GameData.LoadLevel("Main Menu");
    }

    #endregion

    #region Private Methods

    private void UpdateShopButtons()
    {
        sellButton.isEnabled = inventory[index] > 0;
        buyButton.isEnabled = MoneyManager.Balance() >= CUBE.allCUBES[index].price;
    }


    private void ChangeFilter(CUBE.Types type)
    {
        allTypes = false;
        ClearActiveButtons();
        foreach (var cube in filterLists[type])
        {
            CreateCUBEButton(cube);
        }
        filter.text = type.ToString();
        StartCoroutine(UpdateScrollView());

        if ((int)filterType == Enum.GetNames(typeof(CUBE.Types)).Length - 1)
        {
            filterRight.isEnabled = false;
        }
    }


    private void AllFilters()
    {
        allTypes = true;
        filterLeft.isEnabled = false;
        filterRight.isEnabled = true;

        ClearActiveButtons();
        foreach (var cube in CUBE.allCUBES)
        {
            CreateCUBEButton(cube);
        }
        filter.text = "All CUBEs";
        StartCoroutine(UpdateScrollView());
    }


    private IEnumerator UpdateScrollView()
    {
        yield return new WaitForEndOfFrame();
        Grid.GetComponent<UIGrid>().Reposition();
        scrollBar.value = 0f;
        scrollBar.ForceUpdate();
    }


    private void CreateCUBEButton(CUBEInfo info)
    {
        ActivateButton button = (Instantiate(Button_Prefab) as GameObject).GetComponent <ActivateButton>();
        button.transform.parent = Grid;
        button.transform.localScale = Vector3.one;
        button.name = info.name;
        button.GetComponent<CUBEButton>().label.text = button.name + " x " + inventory[info.ID];
        button.value = info.ID.ToString();
        button.ActivateEvent += OnIndexChanged;
        activeButtons[info.ID] = button;
    }


    private void DeleteCUBEButton(int i)
    {
        activeButtons[i].ActivateEvent -= OnIndexChanged;
        Destroy(activeButtons[i].gameObject);
    }


    private void ClearActiveButtons()
    {
        foreach (var button in activeButtons)
        {
            if (button == null) continue;
            button.ActivateEvent -= OnIndexChanged;
            Destroy(button.gameObject);
        }
    }

    #endregion

    #region Event Handlers

    private void OnFilterMoved(object sender, ActivateButtonArgs args)
    {
        if (!args.isPressed) return;

        // left
        if (args.value == "left")
        {
            filterRight.isEnabled = true;
            // all
            if ((int)filterType == 0)
            {
                AllFilters();
            }
            else
            {
                filterType = (CUBE.Types)((int)filterType - 1);
                ChangeFilter(filterType);
            }
        }
        // right
        else
        {
            filterLeft.isEnabled = true;
            // all
            if (allTypes)
            {
                ChangeFilter(filterType);
            }
            else
            {
                filterType = (CUBE.Types)((int)filterType + 1);
                ChangeFilter(filterType);
            }
        }
    }


    private void OnIndexChanged(object sender, ActivateButtonArgs args)
    {
        if (!args.isPressed) return;

        // selected CUBE
        index = int.Parse(args.value);
        CUBEInfo info = CUBE.allCUBES[index];
        selectedCUBE.text = info.name;
        count.text = "You have: " + inventory[index];

        // prices
        sellPrice.text = "+$" + (info.price / 2f).ToString();
        buyPrice.text = "-$" + info.price.ToString();

        // enable buttons
        UpdateShopButtons();

        // stats
        health.text = info.health.ToString();
        shield.text = info.shield.ToString();
        speed.text = info.speed.ToString();

        // showcase
        if (showcaseCUBE != null) Destroy(showcaseCUBE.gameObject);
        showcaseCUBE = ((GameObject)Instantiate(GameResources.GetCUBE(index).gameObject)).transform;
        showcaseCUBE.parent = showcase;
        showcaseCUBE.localScale = Vector3.one;
        showcaseCUBE.localRotation = Quaternion.identity;
        showcaseCUBE.localPosition = -showcaseCUBE.GetComponent<MeshFilter>().mesh.bounds.center;
    }

    #endregion
}