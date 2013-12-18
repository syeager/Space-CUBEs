// Steve Yeager
// 11.26.2013

using System;
using System.Collections.Generic;
using UnityEngine;

public class GarageManager : MonoBehaviour
{
    #region References

    public static GarageManager Main;
    public ConstructionGrid Grid;
    public GameObject mainCamera;

    #endregion

    #region Public Fields

    public int gridSize;

    #endregion

    #region Private Fields

    private float W;
    private float H;

    private bool menuOpen;
    private bool allCUBEs = true;
    private CUBE.CUBETypes CUBEFilter;
    public Vector2 CUBEScroll = Vector2.zero;
    private Rect LeftMenuRect;
    private Rect RightMenuRect;
    private Rect DeleteRect;
    private Rect ActionRect;
    private Rect InfoRect;
    private float CUBESize;
    private int weaponIndex = -1;

    public enum Menus
    {
        Menu = 0,
        CUBEs = 1,
        Nav = 2,
        Weapons = 3,
    }
    public Menus menu;

    #endregion

    #region Const Fields

    public Rect LeftMenuPer = new Rect(0f, 0.1f, 0.3f, 0.9f);
    public Rect RightMenuPer = new Rect(0.9f, 0f, 0.1f, 0.9f);
    public Rect DeletePer = new Rect(0.5f, 0.9f, 0.5f, 0.1f);
    public Rect ActionPer = new Rect(0.5f, 0.9f, 0.5f, 0.1f);
    public Rect InfoPer = new Rect(0.5f, 0.9f, 0.5f, 0.1f);
    public float CUBEPer = 0.1f;

    #endregion


    #region Unity Overrides

    private void Awake()
    {
        Main = this;

        UpdateScreen();

        Grid.CreateGrid(gridSize);
    }


    private void OnGUI()
    {
        Info();
        LeftMenu();
        RightMenu();
        DeleteButton();
        ActionButton();
    }


    private void Update()
    {
        UpdateScreen();

        // move CUBE
        if (Input.GetKeyDown(KeyCode.A))
        {
            Grid.MoveCursor(Vector3.left);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Grid.MoveCursor(Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Grid.MoveCursor(Vector3.forward);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Grid.MoveCursor(Vector3.back);
        }

        // rotate CUBE
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Grid.RotateCursor(Vector3.up);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Grid.RotateCursor(Vector3.down);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Grid.RotateCursor(Vector3.forward);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Grid.RotateCursor(Vector3.back);
            }
            
        }
        else
        {
            // change layer
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Grid.ChangeLayer(1);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Grid.ChangeLayer(-1);
            }
        }

        // place/pickup
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Grid.CursorAction(true);
        }

        // delete
        if (Input.GetKeyUp(KeyCode.Delete))
        {
            if (!Grid.DeleteCUBE())
            {
                Debug.LogWarning("Can't delete CUBE.");
            }
        }

        // build
        if (Input.GetKeyUp(KeyCode.Return))
        {
            Grid.SaveToData();
        }

        // load
        if (Input.GetKeyDown(KeyCode.L))
        {
            Grid.Load("Test Build");
        }
    }

    #endregion

    #region Private Methods

    private void UpdateScreen()
    {
        W = Screen.width;
        H = Screen.height;

        LeftMenuRect = new Rect(W * LeftMenuPer.x, H * LeftMenuPer.y, W * LeftMenuPer.width, H * LeftMenuPer.height);
        RightMenuRect = new Rect(W * RightMenuPer.x, H * RightMenuPer.y, W * RightMenuPer.width, H * RightMenuPer.height);
        DeleteRect = new Rect(W * DeletePer.x, H * DeletePer.y, W * DeletePer.width, H * DeletePer.height);
        ActionRect = new Rect(W * ActionPer.x, H * ActionPer.y, W * ActionPer.width, H * ActionPer.height);
        InfoRect = new Rect(W * InfoPer.x, H * InfoPer.y, W * InfoPer.width, H * InfoPer.height);
        CUBESize = H * CUBEPer;
    }


    private void LeftMenu()
    {
        GUI.BeginGroup(LeftMenuRect);
        {
            GUI.Box(new Rect(0, 0, LeftMenuRect.width, LeftMenuRect.height), "");
            switch (menu)
            {
                case Menus.Menu:
                    Menu();
                    break;
                case Menus.CUBEs:
                    CUBEs();
                    break;
                case Menus.Nav:
                    Navigation();
                    break;
                case Menus.Weapons:
                    Weapons();
                    break;
            }
        }
        GUI.EndGroup();
    }


    private void Menu()
    {
        if (GUI.Button(new Rect(0f, 0f, LeftMenuRect.width, LeftMenuRect.height * 0.2f), "Save"))
        {
            Grid.SaveToData();
        }
        if (GUI.Button(new Rect(0f, LeftMenuRect.height * 0.2f, LeftMenuRect.width, LeftMenuRect.height * 0.2f), "Load"))
        {
            Grid.Load(Grid.buildName);
        }
        if (GUI.Button(new Rect(0f, LeftMenuRect.height * 0.4f, LeftMenuRect.width, LeftMenuRect.height * 0.2f), "Test"))
        {
            GameData.Main.LoadScene("Test Room", false, Grid.buildName);
        }
    }


    private void Navigation()
    {
        float w = LeftMenuRect.width;
        float h = LeftMenuRect.height;

        // rotate Y
        GUI.Label(new Rect(0, 0, w, h * 0.05f), "Rotate Y");
        if (GUI.Button(new Rect(0, h * 0.05f, w * 0.5f, h * 0.1f), "←"))
        {
            Grid.RotateCursor(Vector3.up);
        }
        if (GUI.Button(new Rect(w * 0.5f, h * 0.05f, w / 2f, h * 0.1f), "→"))
        {
            Grid.RotateCursor(Vector3.down);
        }

        // rotate X
        GUI.Label(new Rect(0, h*0.16f, w, h * 0.05f), "Rotate Z");
        if (GUI.Button(new Rect(0, h * 0.21f, w * 0.5f, h * 0.1f), "←"))
        {
            Grid.RotateCursor(Vector3.forward);
        }
        if (GUI.Button(new Rect(w * 0.5f, h * 0.21f, w / 2f, h * 0.1f), "→"))
        {
            Grid.RotateCursor(Vector3.back);
        }

        // cursor info
        GUI.Label(new Rect(0, h - w * 0.8f - h * 0.1f, w, h * 0.05f), "Position: " + Grid.cursor);
        GUI.Label(new Rect(0, h - w * 0.8f - h * 0.05f, w, h * 0.05f), "Rotation: " + Grid.cursorRotation);

        // move X/Z
        Rect moveXZ = new Rect(0, h-w*0.8f, w*0.8f, w*0.8f);
        GUI.BeginGroup(moveXZ);
        {
            GUI.Box(new Rect(0, 0, moveXZ.width, moveXZ.height), "");
            float _w = moveXZ.width * 0.25f;
            float _h = moveXZ.height * 0.5f -_w / 2f;

            if (GUI.Button(new Rect(moveXZ.width/2f - _w/2f, 0f, _w, _h), "↑"))
            {
                Grid.MoveCursor(Vector3.forward);
            }
            if (GUI.Button(new Rect(moveXZ.width/2f - _w/2f, _h+_w, _w, _h), "↓"))
            {
                Grid.MoveCursor(Vector3.back);
            }
            if (GUI.Button(new Rect(0f, _h, _h, _w), "←"))
            {
                Grid.MoveCursor(Vector3.left);
            }
            if (GUI.Button(new Rect(_h+_w, _h, _h, _w), "→"))
            {
                Grid.MoveCursor(Vector3.right);
            }
        }
        GUI.EndGroup();

        // move Y
        Rect moveY = new Rect(w * 0.8f, h - w * 0.8f, w * 0.2f, w * 0.8f);
        GUI.BeginGroup(moveY);
        {
            GUI.Box(new Rect(0, 0, moveY.width, moveY.height), "");

            if (GUI.Button(new Rect(0, 0, moveY.width, moveY.height*0.5f), "↑"))
            {
                Grid.ChangeLayer(1);
            }
            if (GUI.Button(new Rect(0, moveY.height * 0.5f, moveY.width, moveY.height * 0.5f), "↓"))
            {
                Grid.ChangeLayer(-1);
            }
        }
        GUI.EndGroup();
    }


    private void Weapons()
    {
        float w = LeftMenuRect.width;
        float h = LeftMenuRect.height;

        // weapons
        float _h = h * 2f / 3f / Grid.weapons.Length;
        for (int i = 0; i < Grid.weapons.Length; i++)
        {
            if (Grid.weapons[i] == null)
            {
                GUI.Label(new Rect(0f, _h*i, w, _h), (i + 1) + ") ");
            }
            else
            {
                if (GUI.Button(new Rect(0f, _h * i, w, _h), (i + 1) + ") " + Grid.weapons[i].GetType().Name + (weaponIndex == i ? "*" : "")))
                {
                    weaponIndex = (weaponIndex == i) ? -1 : i;
                }
            }
        }

        // move
        if (weaponIndex != -1 && Grid.weapons[weaponIndex] != null)
        {
            if (GUI.Button(new Rect(0f, h-h/3f, w, h/6f), "↑"))
            {
                Grid.MoveWeaponMap(weaponIndex, -1);
                weaponIndex--;
            }
            if (GUI.Button(new Rect(0f, h - h / 6f, w, h / 6f), "↓"))
            {
                Grid.MoveWeaponMap(weaponIndex, 1);
                weaponIndex++;
            }
            weaponIndex = Mathf.Clamp(weaponIndex, 0, Grid.weapons.Length-1);
        }
    }


    private void RightMenu()
    {
        GUI.BeginGroup(RightMenuRect);
        {
            GUI.Box(new Rect(0f, 0f, RightMenuRect.width, RightMenuRect.height), "");

            // menus
            string[] menus = Enum.GetNames(typeof(Menus));
            for (int i = 0; i < menus.Length; i++)
            {
                if (GUI.Button(new Rect(0f, i * RightMenuRect.height / menus.Length, RightMenuRect.width, RightMenuRect.height / menus.Length), menus[i]))
                {
                    menu = (Menus)Enum.Parse(typeof(Menus), menus[i]);
                }
            }
        }
        GUI.EndGroup();
    }


    private void DeleteButton()
    {
        if (GUI.Button(DeleteRect, Grid.cursorStatus == ConstructionGrid.CursorStatuses.Holding ? "Delete" : "delete"))
        {
            Grid.DeleteCUBE();
        }
    }


    private void ActionButton()
    {
        string cursorAction = "";
        switch (Grid.cursorStatus)
        {
            case ConstructionGrid.CursorStatuses.None:
                cursorAction = "action";
                break;
            case ConstructionGrid.CursorStatuses.Holding:
                cursorAction = "Place";
                break;
            case ConstructionGrid.CursorStatuses.Hover:
                cursorAction = "Grab";
                break;
        }
        if (GUI.Button(ActionRect, cursorAction))
        {
            Grid.CursorAction(true);
        }
    }


    private void CUBEs()
    {
        float w = LeftMenuRect.width;
        float h = LeftMenuRect.height;

        // top
        if (GUI.Button(new Rect(0f, 0f, w*0.25f, h*0.15f), allCUBEs ? "|" : "←") && !allCUBEs)
        {
            int cursor = (int)CUBEFilter;
            if (cursor == 0)
            {
                allCUBEs = true;
            }
            else
            {
                cursor--;
                CUBEFilter = (CUBE.CUBETypes)cursor;
            }
        }
        GUI.Label(new Rect(w*0.25f, 0f, w*0.5f, h*0.15f), allCUBEs ? "All CUBE_Prefabs" : CUBEFilter.ToString());
        if (GUI.Button(new Rect(w*0.75f, 0f, w*0.25f, h*0.15f), CUBEFilter == CUBE.CUBETypes.Wing ? "|" : "→") && CUBEFilter != CUBE.CUBETypes.Wing)
        {
            allCUBEs = false;
            int cursor = (int)CUBEFilter;
            cursor++;
            CUBEFilter = (CUBE.CUBETypes)cursor;
        }

        // filter CUBEs
        List<CUBE> availableCUBEs;
        if (allCUBEs)
        {
            availableCUBEs = new List<CUBE>(GameResources.Main.CUBE_Prefabs);
        }
        else
        {
            availableCUBEs = new List<CUBE>();
            foreach (var cube in GameResources.Main.CUBE_Prefabs)
            {
                if (cube.CUBEType == CUBEFilter)
                {
                    availableCUBEs.Add(cube);
                }
            }
        }

        // CUBEs
        CUBEScroll = GUI.BeginScrollView(new Rect(0, h * 0.15f, w, h * 0.8f), CUBEScroll, new Rect(0, 0, w - 16, CUBESize * availableCUBEs.Count));
        {
            for (int i = 0; i < availableCUBEs.Count; i++)
            {
                if (GUI.Button(new Rect(0, i * CUBESize, w - 16f, CUBESize), availableCUBEs[i].name.Substring(5) + " x ∞"))
                {
                    Grid.CreateCUBE(availableCUBEs[i].ID);
                }
            }
        }
        GUI.EndScrollView();

        // CUBE info
        Rect infoRect = new Rect(0, h * 0.8f, w, h * 0.25f);
        GUI.BeginGroup(infoRect);
        {
            GUI.Box(new Rect(0, 0, infoRect.width, infoRect.height), "");

            if (Grid.currentCUBE != null)
            {
                GUI.Label(new Rect(0, 0, infoRect.width, infoRect.height * 0.3f), Grid.currentCUBE.name.Substring(5, Grid.currentCUBE.name.Length-12));
                GUI.Label(new Rect(0, infoRect.height * 0.3f, w, infoRect.height * 0.2f), "Health: " + Grid.currentCUBE.health);
                GUI.Label(new Rect(0, infoRect.height * 0.5f, w, infoRect.height * 0.2f), "Shield: " + Grid.currentCUBE.shield);
                GUI.Label(new Rect(0, infoRect.height * 0.7f, w, infoRect.height * 0.2f), "Speed: " + Grid.currentCUBE.speed);
            }
        }
        GUI.EndGroup();
    }


    private void Info()
    {
        GUI.BeginGroup(InfoRect);
        {
            // name
            Grid.buildName = GUI.TextField(new Rect(0f, 0f, InfoRect.width, InfoRect.height*0.4f), Grid.buildName);

            // health
            GUI.Label(new Rect(0f, InfoRect.height * 0.45f, InfoRect.width * 0.25f, InfoRect.height * 0.5f), "Health: " + Grid.shipHealth);
            // shield
            GUI.Label(new Rect(InfoRect.width * 0.25f, InfoRect.height * 0.45f, InfoRect.width * 0.25f, InfoRect.height * 0.5f), "Shield: " + Grid.shipShield);
            // speed
            GUI.Label(new Rect(InfoRect.width * 0.50f, InfoRect.height * 0.45f, InfoRect.width * 0.25f, InfoRect.height * 0.5f), "Speed: " + Grid.shipSpeed);
            // weapons
            GUI.Label(new Rect(InfoRect.width * 0.75f, InfoRect.height * 0.45f, InfoRect.width * 0.25f, InfoRect.height * 0.5f), "Weapons: " + Grid.shipWeapons);
        }
        GUI.EndGroup();

    }

    #endregion
}