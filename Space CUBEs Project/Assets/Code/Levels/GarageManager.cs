// Steve Yeager
// 11.26.2013

using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

public class GarageManager : MonoBehaviour
{
    #region References

    public ConstructionGrid Grid;
    public Transform mainCamera;

    #endregion

    #region Public Fields

    public int gridSize;

    public float cameraSpeed;
    public float zoomSpeed;
    public float zoomMin;
    public float zoomMax;

    public float swipeDist;
    public float swipeTime;
    public float pinchModifier;
    public float pinchMin;

    #endregion

    #region Private Fields

    private Transform cameraTarget;
    private float W;
    private float H;

    private bool menuOpen;
    private bool allCUBEs = true;
    private CUBE.Types CUBEFilter;
    public Vector2 CUBEScroll = Vector2.zero;
    private Rect LeftMenuRect;
    private Rect RightMenuRect;
    private Rect DeleteRect;
    private Rect ActionRect;
    private Rect InfoRect;
    private float CUBESize;
    private int weaponIndex = -1;

    private enum Menus
    {
        Menu = 0,
        CUBEs = 1,
        Nav = 2,
        Weapons = 3,
    }
    private Menus menu;

    private int[] inventory;
    private List<CUBEInfo> filteredCUBEs = new List<CUBEInfo>();

    private Vector3 cameraDirection = Vector3.up;
    private float zoom = 15f;

    #endregion

    #region Readonly Fields

    private readonly Vector3[] cameraPositions = new Vector3[6]
    {
        new Vector3(0, 1, 0),
        new Vector3(0, -1, 0),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),
    };
    private readonly Vector3[] cameraRotations = new Vector3[6]
    {
        new Vector3(90, 0, 0),
        new Vector3(270, 180, 0),
        new Vector3(0, 270, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 180, 0),
        new Vector3(0, 0, 0),
    };

    #endregion

    #region Const Fields

    public Rect LeftMenuPer = new Rect(0f, 0.1f, 0.3f, 0.9f);
    public Rect RightMenuPer = new Rect(0.9f, 0f, 0.1f, 0.9f);
    public Rect DeletePer = new Rect(0.5f, 0.9f, 0.5f, 0.1f);
    public Rect ActionPer = new Rect(0.5f, 0.9f, 0.5f, 0.1f);
    public Rect InfoPer = new Rect(0.5f, 0.9f, 0.5f, 0.1f);
    public float CUBEPer = 0.1f;
    private readonly Rect VIEWRECT = new Rect(0f, 0f, 1f, 1f);

    #endregion


    #region Unity Overrides

    private void Awake()
    {
        UpdateScreen();

        Grid.CreateGrid(gridSize);
        cameraTarget = new GameObject("Camera Target").transform;
    }


    private void Start()
    {
        // position camera
        Grid.RotateGrid(Vector3.up);
        cameraTarget.position = CalculateTargetPostion(Vector3.up);
        cameraTarget.rotation = Quaternion.Euler(cameraRotations[0]);

        allCUBEs = true;
        FilterCUBEs();
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
        UpdateCamera();

#if UNITY_STANDALONE

        // move CUBE
        if (Input.GetKeyDown(KeyCode.A))
        {
            Grid.MoveCursor(-cameraTarget.right);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Grid.MoveCursor(cameraTarget.right);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Grid.MoveCursor(cameraTarget.up);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Grid.MoveCursor(-cameraTarget.up);
        }

        // rotate CUBE
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Grid.RotateCursor(cameraTarget.forward);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Grid.RotateCursor(-cameraTarget.forward);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Grid.RotateCursor(cameraTarget.up);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Grid.RotateCursor(-cameraTarget.up);
            }

        }
        // rotate camera
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                RotateCamera(Vector3.right);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                RotateCamera(Vector3.left);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                RotateCamera(Vector3.up);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                RotateCamera(Vector3.down);
            }
        }
        // zoom
        else if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                CameraZoom(-1f);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                CameraZoom(1f);
            }
        }
        // change layer
        else
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Grid.ChangeLayer(-1);
                CameraZoom(0f);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Grid.ChangeLayer(1);
                CameraZoom(0f);
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
            Grid.DeleteCUBE();
        }

        // build
        if (Input.GetKeyUp(KeyCode.Return))
        {
            Grid.SaveBuild();
        }

        // load
        if (Input.GetKeyDown(KeyCode.L))
        {
            Grid.CreateBuild("Test Compact");
        }

#else

        int touchCount = Input.touchCount;
        // rotate camera
        if (touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (VIEWRECT.Contains(mainCamera.camera.ScreenToViewportPoint(touch.position)))
                {
                    StartCoroutine(Swipe(touch));
                }
            }
        }
        // zoom camera
        else if (touchCount == 2)
        {
            Touch touchA = Input.GetTouch(0);
            Touch touchB = Input.GetTouch(1);
            if (touchA.phase == TouchPhase.Began || touchB.phase == TouchPhase.Began)
            {
                if (VIEWRECT.Contains(mainCamera.camera.ScreenToViewportPoint(touchA.position)) && VIEWRECT.Contains(mainCamera.camera.ScreenToViewportPoint(touchB.position)))
                {
                    StartCoroutine(Pinch());
                }
            }
        }

#endif

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
            Grid.SaveBuild();
        }
        if (GUI.Button(new Rect(0f, LeftMenuRect.height * 0.2f, LeftMenuRect.width, LeftMenuRect.height * 0.2f), "Load"))
        {
            Grid.CreateBuild(Grid.buildName);
        }
        if (GUI.Button(new Rect(0f, LeftMenuRect.height * 0.4f, LeftMenuRect.width, LeftMenuRect.height * 0.2f), "Test"))
        {
            GameData.LoadLevel("Deep Space", false, new Dictionary<string, object> { { "Build", Grid.buildName } });
        }
        if (GUI.Button(new Rect(0f, LeftMenuRect.height * 0.6f, LeftMenuRect.width, LeftMenuRect.height * 0.2f), "Main Menu"))
        {
            GameData.LoadLevel("Main Menu", false, new Dictionary<string, object>());
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
        GUI.Label(new Rect(0, h * 0.16f, w, h * 0.05f), "Rotate Z");
        if (GUI.Button(new Rect(0, h * 0.21f, w * 0.5f, h * 0.1f), "←"))
        {
            Grid.RotateCursor(Vector3.forward);
        }
        if (GUI.Button(new Rect(w * 0.5f, h * 0.21f, w / 2f, h * 0.1f), "→"))
        {
            Grid.RotateCursor(Vector3.back);
        }

        // cursor info
        GUI.Label(new Rect(0, h - w * 0.8f - h * 0.1f, w, h * 0.05f), "Position: " + (Grid.cursor + Vector3.one));
        GUI.Label(new Rect(0, h - w * 0.8f - h * 0.05f, w, h * 0.05f), "Rotation: " + Grid.cursorRotation.eulerAngles);

        // move X/Z
        Rect moveXZ = new Rect(0, h - w * 0.8f, w * 0.8f, w * 0.8f);
        GUI.BeginGroup(moveXZ);
        {
            GUI.Box(new Rect(0, 0, moveXZ.width, moveXZ.height), "");
            float _w = moveXZ.width * 0.25f;
            float _h = moveXZ.height * 0.5f - _w / 2f;

            if (GUI.Button(new Rect(moveXZ.width / 2f - _w / 2f, 0f, _w, _h), "↑"))
            {
                Grid.MoveCursor(cameraTarget.up);
            }
            if (GUI.Button(new Rect(moveXZ.width / 2f - _w / 2f, _h + _w, _w, _h), "↓"))
            {
                Grid.MoveCursor(-cameraTarget.up);
            }
            if (GUI.Button(new Rect(0f, _h, _h, _w), "←"))
            {
                Grid.MoveCursor(-cameraTarget.right);
            }
            if (GUI.Button(new Rect(_h + _w, _h, _h, _w), "→"))
            {
                Grid.MoveCursor(cameraTarget.right);
            }
        }
        GUI.EndGroup();

        // move Y
        Rect moveY = new Rect(w * 0.8f, h - w * 0.8f, w * 0.2f, w * 0.8f);
        GUI.BeginGroup(moveY);
        {
            GUI.Box(new Rect(0, 0, moveY.width, moveY.height), "");

            if (GUI.Button(new Rect(0, 0, moveY.width, moveY.height * 0.5f), "↑"))
            {
                Grid.ChangeLayer(1);
                CameraZoom(0f);
            }
            if (GUI.Button(new Rect(0, moveY.height * 0.5f, moveY.width, moveY.height * 0.5f), "↓"))
            {
                Grid.ChangeLayer(-1);
                CameraZoom(0f);
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
                GUI.Label(new Rect(0f, _h * i, w, _h), (i + 1) + ") ");
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
            if (GUI.Button(new Rect(0f, h - h / 3f, w, h / 6f), "↑"))
            {
                Grid.MoveWeaponMap(weaponIndex, -1);
                weaponIndex--;
            }
            if (GUI.Button(new Rect(0f, h - h / 6f, w, h / 6f), "↓"))
            {
                Grid.MoveWeaponMap(weaponIndex, 1);
                weaponIndex++;
            }
            weaponIndex = Mathf.Clamp(weaponIndex, 0, Grid.weapons.Length - 1);
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

        // filter left
        if (GUI.Button(new Rect(0f, 0f, w * 0.25f, h * 0.15f), allCUBEs ? "|" : "←") && !allCUBEs)
        {
            int cursor = (int)CUBEFilter;
            if (cursor == 0)
            {
                allCUBEs = true;
            }
            else
            {
                cursor--;
                CUBEFilter = (CUBE.Types)cursor;
            }
            FilterCUBEs();
        }
        // filter
        GUI.Label(new Rect(w * 0.25f, 0f, w * 0.5f, h * 0.15f), allCUBEs ? "All CUBE_Prefabs" : CUBEFilter.ToString());
        // filter right
        if (GUI.Button(new Rect(w * 0.75f, 0f, w * 0.25f, h * 0.15f), CUBEFilter == CUBE.Types.Weapon ? "|" : "→") && CUBEFilter != CUBE.Types.Weapon)
        {
            allCUBEs = false;
            int cursor = (int)CUBEFilter;
            cursor++;
            CUBEFilter = (CUBE.Types)cursor;
            FilterCUBEs();
        }

        // CUBEs
        CUBEScroll = GUI.BeginScrollView(new Rect(0, h * 0.15f, w, h * 0.8f), CUBEScroll, new Rect(0, 0, w - 16, CUBESize * filteredCUBEs.Count));
        {
            for (int i = 0; i < filteredCUBEs.Count; i++)
            {
                if (GUI.Button(new Rect(0, i * CUBESize, w - 16f, CUBESize), filteredCUBEs[i].name + " x " + Grid.inventory[filteredCUBEs[i].ID]))
                {
                    Grid.CreateCUBE(filteredCUBEs[i].ID);
                }
            }
        }
        GUI.EndScrollView();

        // CUBE info
        Rect infoRect = new Rect(0, h * 0.8f, w, h * 0.25f);
        GUI.BeginGroup(infoRect);
        {
            GUI.Box(new Rect(0, 0, infoRect.width, infoRect.height), "");

            if (Grid.heldCUBE != null)
            {
                GUI.Label(new Rect(0, 0, infoRect.width, infoRect.height * 0.3f), Grid.heldInfo.name);
                GUI.Label(new Rect(0, infoRect.height * 0.3f, w, infoRect.height * 0.2f), "Health: " + Grid.heldInfo.health);
                GUI.Label(new Rect(0, infoRect.height * 0.5f, w, infoRect.height * 0.2f), "Shield: " + Grid.heldInfo.shield);
                GUI.Label(new Rect(0, infoRect.height * 0.7f, w, infoRect.height * 0.2f), "Speed: " + Grid.heldInfo.speed);
            }
        }
        GUI.EndGroup();
    }


    private void FilterCUBEs()
    {
        filteredCUBEs.Clear();

        // filter CUBEs
        if (allCUBEs)
        {
            filteredCUBEs = new List<CUBEInfo>(CUBE.allCUBES);
        }
        else
        {
            foreach (var cube in CUBE.allCUBES)
            {
                if (cube.type == CUBEFilter)
                {
                    filteredCUBEs.Add(cube);
                }
            }
        }
    }


    private void Info()
    {
        GUI.BeginGroup(InfoRect);
        {
            // name
            Grid.buildName = GUI.TextField(new Rect(0f, 0f, InfoRect.width, InfoRect.height * 0.4f), Grid.buildName);

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


    private IEnumerator Swipe(Touch startTouch)
    {
        float timer = swipeTime;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            // retrieve touch
            if (Input.touchCount != 1) yield break;
            Touch touch = Input.GetTouch(0);

            // determine if swipe
            Vector2 vector = touch.position - startTouch.position;
            if (vector.magnitude >= swipeDist)
            {
                // right
                if (vector.x <= -swipeDist)
                {
                    RotateCamera(Vector3.right);
                    yield break;
                }
                // left
                else if (vector.x >= swipeDist)
                {
                    RotateCamera(Vector3.left);
                    yield break;
                }
                // up
                else if (vector.y <= -swipeDist)
                {
                    RotateCamera(Vector3.up);
                    yield break;
                }
                // down
                else if (vector.y >= swipeDist)
                {
                    RotateCamera(Vector3.down);
                    yield break;
                }
            }

            yield return null;
        }
    }


    private IEnumerator Pinch()
    {
        Touch touchA = Input.GetTouch(0);
        Touch touchB = Input.GetTouch(1);
        float prevDist = Vector3.Distance(touchA.position, touchB.position);
        while (Input.touchCount == 2)
        {
            touchA = Input.GetTouch(0);
            touchB = Input.GetTouch(1);
            float newDist = Vector3.Distance(touchA.position, touchB.position);
            float deltaDist = newDist - prevDist;

            if (Mathf.Abs(deltaDist) >= pinchMin)
            {
                CameraZoom(-deltaDist / pinchModifier);
            }

            prevDist = newDist;
            yield return null;
        }
    }


    /// <summary>
    /// Sets position and rotation target of camera. Starts movement.
    /// </summary>
    /// <param name="direction"></param>
    private void RotateCamera(Vector3 direction)
    {
        // convert camera position
        cameraDirection = cameraTarget.TransformDirection(direction).Round();

        int index = -1;
        for (int i = 0; i < cameraPositions.Length; i++)
        {
            if (cameraDirection == cameraPositions[i])
            {
                index = i;
                break;
            }
        }

        // rotate grid
        Grid.RotateGrid(cameraPositions[index]);

        // set target position
        cameraTarget.position = CalculateTargetPostion(cameraDirection);
        // get target rotation
        cameraTarget.rotation = Quaternion.Euler(cameraRotations[index]);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="strength"></param>
    private void CameraZoom(float strength)
    {
        zoom = Mathf.Clamp(zoom + (strength * zoomSpeed * Time.deltaTime), zoomMin, zoomMax);
        cameraTarget.position = CalculateTargetPostion(cameraDirection);
    }


    /// <summary>
    /// 
    /// </summary>
    private void UpdateCamera()
    {
        mainCamera.position = Vector3.Lerp(mainCamera.position, cameraTarget.position, Time.deltaTime * cameraSpeed);
        mainCamera.rotation = Quaternion.Slerp(mainCamera.rotation, cameraTarget.rotation, Time.deltaTime * cameraSpeed);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private Vector3 CalculateTargetPostion(Vector3 direction)
    {
        return (Grid.layer + direction * zoom).Round();
    }

    #endregion
}