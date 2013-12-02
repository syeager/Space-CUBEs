﻿// Steve Yeager
// 11.26.2013

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
    private bool navigationOpen = true;
    private bool allCUBEs = true;
    private CUBE.CUBETypes CUBEFilter;
    public Vector2 CUBEScroll = Vector2.zero;
    private Rect LeftToolsRect;
    private Rect RightToolsRect;
    private float CUBESize;
    private int weaponIndex = -1;

    #endregion

    #region Const Fields

    public Rect LeftToolsPer = new Rect(0f, 0.1f, 0.3f, 0.9f);
    public Rect RightToolsPer = new Rect(0.7f, 0.1f, 0.3f, 0.9f);
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
        Menu();
        LeftTools();
        RightTools();
        Name();
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
            Grid.CursorAction();
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
            Grid.SaveBuild();
        }

        // load
        if (Input.GetKeyDown(KeyCode.L))
        {
            Grid.LoadBuild("Test Build");
        }
    }

    #endregion

    #region Private Methods

    private void UpdateScreen()
    {
        W = Screen.width;
        H = Screen.height;

        LeftToolsRect = new Rect(W * LeftToolsPer.x, H * LeftToolsPer.y, W * LeftToolsPer.width, H * LeftToolsPer.height);
        RightToolsRect = new Rect(W * RightToolsPer.x, H * RightToolsPer.y, W * RightToolsPer.width, H * RightToolsPer.height);
        CUBESize = H * CUBEPer;
    }


    private void Menu()
    {
        if (GUI.Button(new Rect(W/2-50, H-40, 100, 40), "Menu"))
        {
            menuOpen = !menuOpen;
        }

        if (menuOpen)
        {

        }
        else
        {
            
        }
    }


    private void LeftTools()
    {
        if (GUI.Button(new Rect(0f, 0f, LeftToolsRect.width*0.25f, H*(1-LeftToolsPer.height)), navigationOpen ? "|" : "←"))
        {
            if (!navigationOpen)
            {
                navigationOpen = true;
            }
        }
        GUI.Label(new Rect(LeftToolsRect.width * 0.25f, 0f, LeftToolsRect.width*0.5f, H * (1 - LeftToolsPer.height)), navigationOpen ? "Nav" : "Weapons");
        if (GUI.Button(new Rect(LeftToolsRect.width * 0.75f, 0f, LeftToolsRect.width * 0.25f, H * (1 - LeftToolsPer.height)), navigationOpen ? "→" : "|"))
        {
            if (navigationOpen)
            {
                navigationOpen = false;
            }
        }

        if (navigationOpen)
        {
            Navigation();
        }
        else
        {
            Weapons();
        }
    }


    private void Navigation()
    {
        GUI.BeginGroup(LeftToolsRect);
        {
            GUI.Box(new Rect(0, 0, LeftToolsRect.width, LeftToolsRect.height), "");
            float w = LeftToolsRect.width;
            float h = LeftToolsRect.height;

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

            // delete
            if (GUI.Button(new Rect(0, h*0.36f, w, h*0.1f), "Delete"))
            {
                Grid.DeleteCUBE();
            }

            // cursor info
            GUI.Label(new Rect(0, h - w * 0.8f - h*0.05f, w, h * 0.05f), "Cursor: " + Grid.cursor);

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
        GUI.EndGroup();
    }


    private void Weapons()
    {
        GUI.BeginGroup(LeftToolsRect);
        {
            GUI.Box(new Rect(0, 0, LeftToolsRect.width, LeftToolsRect.height), "");
            float w = LeftToolsRect.width;
            float h = LeftToolsRect.height;

            // weapons
            float _h = h * 2f / 3f / Grid.weapons.Count;
            for (int i = 0; i < Grid.weapons.Count; i++)
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
                weaponIndex = Mathf.Clamp(weaponIndex, 0, Grid.weapons.Count-1);
            }
        }
        GUI.EndGroup();
    }


    private void RightTools()
    {
        if (GUI.Button(new Rect(0f, 0f, LeftToolsRect.width * 0.25f, H * (1 - LeftToolsPer.height)), navigationOpen ? "|" : "←"))
        {
            if (!navigationOpen)
            {
                navigationOpen = true;
            }
        }
        GUI.Label(new Rect(LeftToolsRect.width * 0.25f, 0f, LeftToolsRect.width * 0.5f, H * (1 - LeftToolsPer.height)), navigationOpen ? "Nav" : "Weapons");
        if (GUI.Button(new Rect(LeftToolsRect.width * 0.75f, 0f, LeftToolsRect.width * 0.25f, H * (1 - LeftToolsPer.height)), navigationOpen ? "→" : "|"))
        {
            if (navigationOpen)
            {
                navigationOpen = false;
            }
        }

        if (allCUBEs)
        {
            GUI.BeginGroup(RightToolsRect);
            {
                GUI.Box(new Rect(0, 0, RightToolsRect.width, RightToolsRect.height), "");
                float w = RightToolsRect.width;
                float h = RightToolsRect.height;

                // CUBEs
                CUBEScroll = GUI.BeginScrollView(new Rect(0, 0, w, h * 0.65f), CUBEScroll, new Rect(0, 0, w - 16, CUBESize * GameResources.Main.CUBEs.Count));
                {
                    for (int i = 0; i < GameResources.Main.CUBEs.Count; i++)
                    {
                        if (GUI.Button(new Rect(0, i * CUBESize, w-16f, CUBESize), GameResources.Main.CUBEs[i].name.Substring(5) + " x ∞"))
                        {
                            Grid.CreateCUBE(GameResources.Main.CUBEs[i].ID);
                        }
                    }
                }
                GUI.EndScrollView();

                // CUBE info
                Rect infoRect = new Rect(0, h * 0.65f, w, h * 0.25f);
                GUI.BeginGroup(infoRect);
                {
                    GUI.Box(new Rect(0, 0, infoRect.width, infoRect.height), "");

                    if (Grid.currentCUBE != null)
                    {
                        GUI.Label(new Rect(0, 0, infoRect.width, infoRect.height*0.3f), Grid.currentCUBE.name.Substring(5));
                        GUI.Label(new Rect(0, infoRect.height*0.3f, w, infoRect.height*0.2f), "Health: " + Grid.currentCUBE.health);
                        GUI.Label(new Rect(0, infoRect.height * 0.5f, w, infoRect.height * 0.2f), "Shield: " + Grid.currentCUBE.shield);
                    }
                }
                GUI.EndGroup();

                // cursor action
                string cursorAction = "";
                switch (Grid.cursorStatus)
                {
                    case ConstructionGrid.CursorStatuses.None:
                        cursorAction = "";
                        break;
                    case ConstructionGrid.CursorStatuses.Holding:
                        cursorAction = "Place";
                        break;
                    case ConstructionGrid.CursorStatuses.Hover:
                        cursorAction = "Grab";
                        break;
                }
                if (GUI.Button(new Rect(0, h*0.9f, w, h*0.1f), cursorAction))
                {
                    Grid.CursorAction();
                }
            }
            GUI.EndGroup();
        }
        else
        {

        }
    }


    private void Name()
    {

    }

    #endregion
}