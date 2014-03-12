// Steve Yeager
// 11.26.2013

using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;
using System.Linq;

using Types = CUBE.Types;
using CursorStatuses = ConstructionGrid.CursorStatuses;

public class GarageManager : MonoBase
{
    #region References

    public ConstructionGrid Grid;
    public Transform mainCamera;

    #endregion

    #region Public Fields

    private const int gridSize = 10;

    public float cameraSpeed;
    public float zoomSpeed;
    public float zoomMin;
    public float zoomMax;
    public float zoomStart;

    public float swipeDist;
    public float swipeTime;
    public float pinchModifier;
    public float pinchMin;
    public float menuSwipeDist;

    public GameObject[] menuPanels;
    public GameObject infoPanel;

    public UIScrollView loadScrollView;
    public UIScrollBar loadScrollBar;
    public UIGrid loadGrid;
    public GameObject BuildButton_Prefab;

    public UIScrollView selectionScrollView;
    public UIScrollBar selectionScrollBar;
    public UIGrid selectionGrid;
    public GameObject CUBESelectionButton_Prefab;
    
    public ActivateButton leftFilter;
    public UILabel filterLabel;
    public ActivateButton rightFilter;
    
    public UILabel CUBEName;
    public UILabel CUBEHealth;
    public UILabel CUBEShield;
    public UILabel CUBESpeed;
    public UILabel CUBEDamage;

    public UIInput shipName;
    public UILabel shipHealth;
    public UILabel shipShield;
    public UILabel shipSpeed;
    public UILabel shipDamage;
    public ActivateButton actionButton1;
    public ActivateButton actionButton2;

    #endregion

    #region Private Fields

    private Transform cameraTarget;

    private bool menuOpen;
    private CUBE.Types CUBEFilter;
   
    private int weaponIndex = -1;

    private int[] inventory;

    private Vector3 cameraDirection = Vector3.up;
    private float zoom;

    private bool selectedBuild;
    private string currentBuild = "";

    private ActivateButton[] filteredCUBEButtons;
    private CUBEInfo currentCUBE;

    #endregion

    #region Mobile Fields

    private bool canMenuSwipe = true;
    public float menuSwipeDelay = 0.5f;
    private Rect touchRect;

    #endregion

    #region State Fields

    private StateMachine stateMachine;
    private const string LOADSTATE = "Load";
    private const string SELECTSTATE = "Select";
    private const string NAVSTATE = "Nav";
    private const string WEAPONSTATE = "Weapon";
    private const string PAINTSTATE = "Paint";
    private const string OBSERVESTATE = "Observe";

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

    #region Load Menu Fields

    public GameObject renamePanel;
    public UIInput renameInput;

    #endregion

    #region Nav Menu Fields

    public ActivateButton[] positionButtons;
    public UILabel postionLabel;
    public ActivateButton[] rotationButtons;
    public UILabel rotationLabel;

    #endregion

    #region Paint Fields

    public GameObject paintGrid;
    private Color[] colors;
    private int primaryColor;
    private int secondaryColor;
    private bool primarySelected;
    public ActivateButton[] pieces = new ActivateButton[3];
    private int pieceSelected;
    public GameObject colorSelector;
    public UILabel colorSelectorTitle;
    public ActivateButton[] paintPositionButtons;
    public UILabel paintPostionLabel;
    public ColorButton selectPrimary;
    public ColorButton selectSecondary;
    public ColorButton copyPrimary;
    public ColorButton copySecondary;

    #endregion

    #region Weapon Menu Fields

    public ActivateButton[] weaponButtons;
    public ActivateButton[] weaponNavButtons;

    #endregion

    #region Save Fields

    public float saveConfirmationTime = 0.6f;
    public GameObject saveConfirmation;
    public UILabel saveShipName;

    #endregion


    #region Unity Overrides

    private void Awake()
    {
        // create states
        stateMachine = new StateMachine(this, LOADSTATE);
        stateMachine.CreateState(LOADSTATE, LoadEnter, LoadExit);
        stateMachine.CreateState(SELECTSTATE, SelectEnter, SelectExit);
        stateMachine.CreateState(NAVSTATE, NavEnter, NavExit);
        stateMachine.CreateState(PAINTSTATE, PaintEnter, PaintExit);
        stateMachine.CreateState(WEAPONSTATE, WeaponEnter, WeaponExit);

        cameraTarget = new GameObject("Camera Target").transform;
        StartCoroutine(ResetCamera());

        // load colors
        colors = CUBE.LoadColors();

        // set to load menu
        menuPanels[0].SetActive(true);
        menuPanels[1].SetActive(false);
        menuPanels[2].SetActive(false);
        menuPanels[3].SetActive(false);
        menuPanels[4].SetActive(false);
        infoPanel.SetActive(false);
        mainCamera.camera.rect = new Rect(0f, 0f, 1f, 1f);

        // load menu
        CreateBuildButtons();

        // selection menu
        inventory = CUBE.GetInventory();
        filteredCUBEButtons = new ActivateButton[inventory.Length];
        leftFilter.ActivateEvent += OnFilterChanged;
        rightFilter.ActivateEvent += OnFilterChanged;

        // nav menu
        foreach (var button in positionButtons)
        {
            button.ActivateEvent += OnPositionButtonPressed;
        }
        foreach (var button in rotationButtons)
        {
            button.ActivateEvent += OnRotationButtonPressed;
        }

        // paint menu
        var paints = paintGrid.GetComponentsInChildren<ActivateButton>(true);
        foreach (var paint in paints)
        {
            paint.ActivateEvent += OnColorSelected;
        }
        foreach (var piece in pieces)
        {
            piece.ActivateEvent += OnPieceSelected;
        }
        foreach (var button in paintPositionButtons)
        {
            button.ActivateEvent += OnPaintPositionButtonPressed;
        }
        copyPrimary.ActivateEvent += CopyColor;
        copySecondary.ActivateEvent += CopyColor;
        selectPrimary.ActivateEvent += OpenColorSelector;
        selectSecondary.ActivateEvent += OpenColorSelector;
        primaryColor = int.Parse(paints[0].value);
        secondaryColor = int.Parse(paints[1].value);
    }


    private void Start()
    {
        stateMachine.Start(new Dictionary<string, object>());
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            stateMachine.SetState(LOADSTATE, null);
        }

#if UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(ResetCamera());
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).tapCount == 2 && touchRect.Contains(mainCamera.camera.ScreenToViewportPoint(Input.GetTouch(0).position)))
        {
            StartCoroutine(ResetCamera());
        }
#endif
    }

    #endregion

    #region Camera Methods

    /// <summary>
    /// 
    /// </summary>
    private int MenuSwipe()
    {
        int direction = 0;
#if UNITY_STANDALONE
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                direction = -1;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                direction = 1;
            }
        }
#else
        if (canMenuSwipe)
        {
            if (Input.touchCount == 2)
            {
                float delta = (Input.GetTouch(0).deltaPosition + Input.GetTouch(1).deltaPosition).x;
                if (Mathf.Abs(delta) >= menuSwipeDist)
                {
                    direction = -(int)Mathf.Sign(delta);
                }
            }
        }

#endif

        return direction;
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
    private void CameraMovementEdit()
    {
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

        // zoom
        if (Input.GetKey(KeyCode.R))
        {
            CameraZoom(-1f);
        }
        if (Input.GetKey(KeyCode.F))
        {
            CameraZoom(1f);
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
                if (touchRect.Contains(mainCamera.camera.ScreenToViewportPoint(touch.position)))
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
                if (touchRect.Contains(mainCamera.camera.ScreenToViewportPoint(touchA.position)) && touchRect.Contains(mainCamera.camera.ScreenToViewportPoint(touchB.position)))
                {
                    StartCoroutine(Pinch());
                }
            }
        }

#endif
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


    private IEnumerator ResetCamera()
    {
        while (cameraDirection != cameraPositions[0])
        {
            RotateCamera(Vector3.up);
        }

        float direction = (zoom-zoomStart > 0 ? -1f : 1f);
        while (Mathf.Abs(zoom - zoomStart) > 0.1f)
        {
            CameraZoom(direction);
            yield return null;
        }

    }

    #endregion

    #region Mobile Methods

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

    #endregion

    #region Load Menu Methods

    private void LoadEnter(Dictionary<string, object> info)
    {
        canMenuSwipe = false;
        InvokeAction(() => canMenuSwipe = true, menuSwipeDelay);

#if UNITY_ANDROID
        touchRect = new Rect(0f, 0f, 1f, 1f);
#endif

        mainCamera.camera.rect = new Rect(0f, 0f, 1f, 1f);
        menuPanels[0].SetActive(true);
        stateMachine.SetUpdate(LoadUpdate());
    }


    private IEnumerator LoadUpdate()
    {
        while (true)
        {
            if (selectedBuild)
            {
                if (MenuSwipe() == 1)
                {
                    stateMachine.SetState(SELECTSTATE, new Dictionary<string, object>());
                }
            }
            yield return null;
        }
    }


    private void LoadExit(Dictionary<string, object> info)
    {
        menuPanels[0].SetActive(false);
    }


    /// <summary>
    /// Create Construction Grid and position camera.
    /// </summary>
    private void CreateGrid()
    {
        Grid.CreateGrid(gridSize);

        // position camera
        Grid.RotateGrid(Vector3.up);
        cameraTarget.position = CalculateTargetPostion(Vector3.up);
        cameraTarget.rotation = Quaternion.Euler(cameraRotations[0]);
    }


    /// <summary>
    /// Create all of the buttons for the builds in the load menu.
    /// </summary>
    private void CreateBuildButtons()
    {
        string[] buildNames = ConstructionGrid.BuildNames().ToArray();
        if (buildNames.Length > 0) currentBuild = buildNames[0];
        for (int i = 0; i < buildNames.Length; i++)
        {
            ScrollviewButton button = (Instantiate(BuildButton_Prefab) as GameObject).GetComponent<ScrollviewButton>();
            button.Initialize(i.ToString(), buildNames[i], buildNames[i], loadGrid.transform, loadScrollView);
            button.ActivateEvent += OnBuildChosen;
        }

        StartCoroutine(Utility.UpdateScrollView(loadGrid, loadScrollBar));
    }


    public void LoadMainMenu()
    {
        GameData.LoadLevel("Main Menu");
    }


    public void LoadStore()
    {
        GameData.LoadLevel("Store");
    }


    private void OnBuildChosen(object sender, ActivateButtonArgs args)
    {
        if (args.isPressed) return;

        currentBuild = args.value;
    }


    /// <summary>
    /// Delete selected build from data.
    /// </summary>
    public void DeleteBuild()
    {
        if (string.IsNullOrEmpty(currentBuild)) return;

        // delete build
        ConstructionGrid.DeleteBuild(currentBuild);

        // remove build button
        ScrollviewButton button = loadGrid.GetComponentsInChildren<ScrollviewButton>().First(b => b.value == currentBuild);
        button.ActivateEvent -= OnBuildChosen;
        Destroy(button.gameObject);

        // reload grid
        StartCoroutine(Utility.UpdateScrollView(loadGrid, loadScrollBar, false));

        currentBuild = "";
    }


    public void ConfirmRename()
    {
        renamePanel.SetActive(true);
        renameInput.value = currentBuild;
    }


    public void RenameBuild()
    {
        ConstructionGrid.RenameBuild(currentBuild, renameInput.value);
        renamePanel.SetActive(false);

        ScrollviewButton button = loadGrid.GetComponentsInChildren<ScrollviewButton>().First(b => b.value == currentBuild);
        currentBuild = renameInput.value;
        button.value = currentBuild;
        button.label.text = currentBuild;
    }


    public void CancelRename()
    {
        renamePanel.SetActive(false);
    }


    public void LoadBuild()
    {
        if (!selectedBuild)
        {
            CreateGrid();
        }
        Grid.CreateBuild(currentBuild);
        shipName.value = Grid.buildName;
        stateMachine.SetState(SELECTSTATE, new Dictionary<string,object>());
        selectedBuild = true;
    }


    public void NewBuild()
    {
        CreateGrid();
        stateMachine.SetState(SELECTSTATE, new Dictionary<string, object>());
        selectedBuild = true;
    }


    public void Play()
    {
        GameData.LoadLevel("Deep Space", true, new Dictionary<string, object> { { "Build", currentBuild } });
    }

    #endregion

    #region Selection Menu Methods

    private void SelectEnter(Dictionary<string, object> info)
    {
        canMenuSwipe = false;
        InvokeAction(() => canMenuSwipe = true, menuSwipeDelay);

#if UNITY_ANDROID
        touchRect = new Rect(0f, 0.125f, 1f, 1f);
#endif

        mainCamera.camera.rect = new Rect(0.25f, 0f, 1f, 1f);
        menuPanels[1].SetActive(true);
        infoPanel.SetActive(true);
        FilterCUBEs(CUBEFilter, true);
        
        actionButton2.ActivateEvent += OnNavMenuPressed;
        stateMachine.SetUpdate(SelectUpdate());
        StartCoroutine("SaveConfirmation");
    }


    private IEnumerator SelectUpdate()
    {
        actionButton1.label.text = "---";
        actionButton1.isEnabled = false;
        actionButton2.label.text = "Nav";
        actionButton2.isEnabled = true;

        while (true)
        {
            // update camera
            UpdateCamera();
            CameraMovementEdit();

            // change menu
            int dir = MenuSwipe();
            if (dir == -1)
            {
                stateMachine.SetState(LOADSTATE, new Dictionary<string, object>());
            }
            else if (dir == 1)
            {
                stateMachine.SetState(NAVSTATE, new Dictionary<string, object>());
            }

            // update ship stats
            UpdateInfoPanel();
            yield return null;
        }
    }


    private void SelectExit(Dictionary<string, object> info)
    {
        menuPanels[1].SetActive(false);
        infoPanel.SetActive(false);
        actionButton2.ActivateEvent -= OnNavMenuPressed;
        StopCoroutine("SaveConfirmation");
    }


    /// <summary>
    /// 
    /// </summary>
    private void FilterCUBEs(Types cubeType, bool force = false)
    {
        if (CUBEFilter == cubeType && !force) return;

        // cache
        CUBEFilter = cubeType;

        // delete current buttons
        foreach (var button in filteredCUBEButtons)
        {
            if (button == null) continue;
            button.ActivateEvent -= OnCUBESelected;
            Destroy(button.gameObject);
        }

        // create buttons
        foreach (var cube in CUBE.allCUBES)
        {
            if (cube.type == CUBEFilter)
            {
                CreateCUBEButton(cube.ID);
            }
        }
        StartCoroutine(Utility.UpdateScrollView(selectionGrid, selectionScrollBar));

        // set filters
        leftFilter.isEnabled = (int)CUBEFilter > 0;
        rightFilter.isEnabled = (int)CUBEFilter < Enum.GetNames(typeof(Types)).Length - 1;
        filterLabel.text = CUBEFilter.ToString();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="ID"></param>
    private void CreateCUBEButton(int ID)
    {
        CUBEInfo info = CUBE.allCUBES[ID];
        ScrollviewButton button = (Instantiate(CUBESelectionButton_Prefab) as GameObject).GetComponent<ScrollviewButton>();
        button.Initialize(ID.ToString(), info.name + "\n" + inventory[ID], ID.ToString(), selectionGrid.transform);
        button.ActivateEvent += OnCUBESelected;
        button.isEnabled = GameResources.GetCUBE(ID) != null;
        filteredCUBEButtons[info.ID] = button;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void OnCUBESelected(object sender, ActivateButtonArgs args)
    {
        if (!args.isPressed) return;

        SetCurrentCUBE(int.Parse(args.value));
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void OnFilterChanged(object sender, ActivateButtonArgs args)
    {
        if (!args.isPressed) return;

        int direction = int.Parse(args.value);
        int filter = Mathf.Clamp((int)CUBEFilter+direction, 0, Enum.GetNames(typeof(CUBE.Types)).Length);
        FilterCUBEs((Types)filter);
    }


    private void OnNavMenuPressed(object sender, ActivateButtonArgs args)
    {
        if (args.isPressed) return;

        stateMachine.SetState(NAVSTATE, new Dictionary<string,object>());
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="ID"></param>
    private void SetCurrentCUBE(int ID)
    {
        currentCUBE = CUBE.allCUBES[ID];
        CUBEName.text = currentCUBE.name;
        CUBEHealth.text = CUBE.HEALTHICON + " " + currentCUBE.health;
        CUBEShield.text = CUBE.SHIELDICON + " " + currentCUBE.shield;
        CUBESpeed.text = CUBE.SPEEDICON + " " + currentCUBE.speed;
        CUBEDamage.text = CUBE.DAMAGEICON + " " + currentCUBE.damage;

        Grid.CreateCUBE(ID);
    }

    #endregion

    #region Nav Menu Methods

    private void NavEnter(Dictionary<string, object> info)
    {
        canMenuSwipe = false;
        InvokeAction(() => canMenuSwipe = true, menuSwipeDelay);

#if UNITY_ANDROID
        touchRect = new Rect(0f, 0.125f, 1f, 1f);
#endif

        mainCamera.camera.rect = new Rect(0.25f, 0f, 1f, 1f);
        menuPanels[2].SetActive(true);
        infoPanel.SetActive(true);
        actionButton1.ActivateEvent += OnDeleteButtonPressed;
        actionButton2.ActivateEvent += OnActionButtonPressed;
        stateMachine.SetUpdate(NavUpdate());
        StartCoroutine("SaveConfirmation");
    }


    private IEnumerator NavUpdate()
    {
        actionButton1.isEnabled = true;
        actionButton1.label.text = "Delete";
        actionButton2.isEnabled = true;
        actionButton2.label.text = "Place";

        while (true)
        {
            // update camera
            UpdateCamera();
            CameraMovementEdit();

            // detect swipe
            int dir = MenuSwipe();
            if (dir == -1)
            {
                stateMachine.SetState(SELECTSTATE, new Dictionary<string, object>());
            }
            else if (dir == 1)
            {
                stateMachine.SetState(PAINTSTATE, new Dictionary<string, object>());
            }

            // update position and rotation
            postionLabel.text = "Position " + (Grid.cursor + Vector3.one).ToString("0");
            rotationLabel.text = "Rotation " + Grid.cursorRotation.eulerAngles.ToString("0");

            // update delete button
            if (Grid.cursorStatus == CursorStatuses.Holding && !actionButton1.isEnabled)
            {
                actionButton1.isEnabled = true;
            }
            else if (Grid.cursorStatus != CursorStatuses.Holding && actionButton1.isEnabled)
            {
                actionButton1.isEnabled = false;
            }

            // update action button
            switch (Grid.cursorStatus)
            {
                case CursorStatuses.Holding:
                    actionButton2.label.text = "Place";
                    actionButton2.isEnabled = true;
                    break;
                case CursorStatuses.Hover:
                    actionButton2.label.text = "Pickup";
                    actionButton2.isEnabled = true;
                    break;
                case CursorStatuses.None:
                    actionButton2.label.text = "---";
                    actionButton2.isEnabled = false;
                    break;
            }

            // update ship stats
            UpdateInfoPanel();

            yield return null;
        }
    }


    private void NavExit(Dictionary<string, object> info)
    {
        menuPanels[2].SetActive(false);
        infoPanel.SetActive(false);
        actionButton1.ActivateEvent -= OnDeleteButtonPressed;
        actionButton2.ActivateEvent -= OnActionButtonPressed;
        StopCoroutine("SaveConfirmation");
    }


    public void SetSelectMenu()
    {
        stateMachine.SetState(SELECTSTATE, new Dictionary<string,object>());
    }


    private void OnPositionButtonPressed(object sender, ActivateButtonArgs args)
    {
        if (!args.isPressed) return;

        switch (args.value)
        {
            case "back":
                Grid.ChangeLayer(-1);
                CameraZoom(0f);
                break;
            case "forward":
                Grid.ChangeLayer(1);
                CameraZoom(0f);
                break;
            case "left":
                Grid.MoveCursor(-cameraTarget.right);
                break;
            case "right":
                Grid.MoveCursor(cameraTarget.right);
                break;
            case "down":
                Grid.MoveCursor(-cameraTarget.up);
                break;
            case "up":
                Grid.MoveCursor(cameraTarget.up);
                break;
        }
    }


    private void OnRotationButtonPressed(object sender, ActivateButtonArgs args)
    {
        if (!args.isPressed) return;

        switch (args.value)
        {
            case "y left":
                Grid.RotateCursor(Vector3.up);
                break;
            case "y right":
                Grid.RotateCursor(Vector3.down);
                break;
            case "z left":
                Grid.RotateCursor(Vector3.forward);
                break;
            case "z right":
                Grid.RotateCursor(Vector3.back);
                break;
        }
    }


    private void OnDeleteButtonPressed(object sender, ActivateButtonArgs args)
    {
        if (!args.isPressed) return;
        Grid.DeleteCUBE();
    }


    private void OnActionButtonPressed(object sender, ActivateButtonArgs args)
    {
        if (!args.isPressed) return;
        Grid.CursorAction(true);
    }

    #endregion

    #region Paint Methods

    private void PaintEnter(Dictionary<string, object> info)
    {
        canMenuSwipe = false;
        InvokeAction(() => canMenuSwipe = true, menuSwipeDelay);

        menuPanels[4].SetActive(true);
        colorSelector.SetActive(false);
        infoPanel.SetActive(true);

        actionButton1.ActivateEvent += OnPaint;
        actionButton2.ActivateEvent += OnPaint;

        stateMachine.SetUpdate(PaintUpdate());
        StartCoroutine("SaveConfirmation");
    }


    private IEnumerator PaintUpdate()
    {
        actionButton1.label.text = "";
        actionButton2.label.text = "";
        UpdatePieces();

        while (true)
        {
            // update camera
            UpdateCamera();
            CameraMovementEdit();

            // detect swipe
            int dir = MenuSwipe();
            if (dir == -1)
            {
                stateMachine.SetState(NAVSTATE, new Dictionary<string, object>());
            }
            else if (dir == 1)
            {
                stateMachine.SetState(WEAPONSTATE, new Dictionary<string, object>());
            }

            // update position and rotation
            paintPostionLabel.text = "Position " + (Grid.cursor + Vector3.one).ToString("0");

            UpdateInfoPanel();
            yield return null;
        }
    }


    private void PaintExit(Dictionary<string, object> info)
    {
        menuPanels[4].SetActive(false);
        colorSelector.SetActive(false);
        infoPanel.SetActive(false);

        actionButton1.ActivateEvent -= OnPaint;
        actionButton1.defaultColor = Color.white;
        actionButton2.ActivateEvent -= OnPaint;
        actionButton2.defaultColor = Color.white;

        StopCoroutine("SaveConfirmation");
    }


    private void OnColorSelected(object sender, ActivateButtonArgs args)
    {
        if (args.isPressed) return;

        if (primarySelected)
        {
            SetPrimaryColor(int.Parse(args.value));
        }
        else
        {
            SetSecondaryColor(int.Parse(args.value));
        }

        colorSelector.SetActive(false);
    }


    private void OnPaint(object sender, ActivateButtonArgs args)
    {
        if (args.isPressed) return;

        Grid.Paint(pieceSelected, args.value == "action2" ? primaryColor : secondaryColor);
    }


    private void UpdatePieces()
    {
        if (Grid.cursorStatus == CursorStatuses.Hover)
        {
            // enable copy to's
            copyPrimary.isEnabled = true;
            copySecondary.isEnabled = true;

            // enable paints
            actionButton1.isEnabled = true;
            SetSecondaryColor(secondaryColor);
            actionButton2.isEnabled = true;
            SetPrimaryColor(primaryColor);

            // enable pieces
            pieces[pieceSelected].Activate(false);
            int count = Grid.hoveredCUBE.renderer.sharedMaterials.Length;
            int cursor = 0;
            while (cursor < count)
            {
                pieces[cursor].isEnabled = true;
                cursor++;
            }
            while (cursor < 3)
            {
                pieces[cursor].isEnabled = false;
                cursor++;
            }

            while (pieceSelected > count - 1)
            {
                pieceSelected--;
            }
            pieces[pieceSelected].Activate(true);

            // copy colors
            Color copyColor = colors[Grid.hoveredCUBE.GetComponent<ColorVertices>().colors[pieceSelected]];
            copyPrimary.SetColor(copyColor);
            copySecondary.SetColor(copyColor);
        }
        else
        {
            // disable copy to's
            copyPrimary.isEnabled = false;
            copySecondary.isEnabled = false;

            // disable paints
            actionButton1.isEnabled = false;
            actionButton2.isEnabled = false;

            // disable pieces
            foreach (var piece in pieces)
            {
                piece.Activate(false);
                piece.isEnabled = false;
            }

            // copy colors
            copyPrimary.SetColor(Color.gray);
            copySecondary.SetColor(Color.gray);
        }
    }


    private void OnPieceSelected(object sender, ActivateButtonArgs args)
    {
        if (args.isPressed) return;
        pieces[pieceSelected].Activate(false);
        pieceSelected = int.Parse(args.value);
        pieces[pieceSelected].Activate(true);

        // copy colors
        Color copyColor = colors[Grid.hoveredCUBE.GetComponent<ColorVertices>().colors[pieceSelected]];
        copyPrimary.SetColor(copyColor);
        copySecondary.SetColor(copyColor);
    }


    private void OpenColorSelector(object sender, ActivateButtonArgs args)
    {
        primarySelected = args.value == "Primary";
        colorSelectorTitle.text = args.value;
        colorSelector.SetActive(true);
    }


    public void CopyColor(object sender, ActivateButtonArgs args)
    {
        if (args.value == "Primary")
        {
            Color color = colors[Grid.hoveredCUBE.GetComponent<ColorVertices>().colors[pieceSelected]];
            SetPrimaryColor(Array.IndexOf(CUBE.colors, color));
        }
        else
        {
            Color color = colors[Grid.hoveredCUBE.GetComponent<ColorVertices>().colors[pieceSelected]];
            SetSecondaryColor(Array.IndexOf(CUBE.colors, color));
        }
    }


    private void OnPaintPositionButtonPressed(object sender, ActivateButtonArgs args)
    {
        if (!args.isPressed) return;

        switch (args.value)
        {
            case "back":
                Grid.ChangeLayer(-1);
                CameraZoom(0f);
                UpdatePieces();
                break;
            case "forward":
                Grid.ChangeLayer(1);
                CameraZoom(0f);
                UpdatePieces();
                break;
            case "left":
                Grid.MoveCursor(-cameraTarget.right);
                UpdatePieces();
                break;
            case "right":
                Grid.MoveCursor(cameraTarget.right);
                UpdatePieces();
                break;
            case "down":
                Grid.MoveCursor(-cameraTarget.up);
                UpdatePieces();
                break;
            case "up":
                Grid.MoveCursor(cameraTarget.up);
                UpdatePieces();
                break;
        }
    }


    private void SetPrimaryColor(int colorIndex)
    {
        primaryColor = colorIndex;
        actionButton2.Activate(CUBE.colors[primaryColor]);
        selectPrimary.SetColor(CUBE.colors[primaryColor]);
    }


    private void SetSecondaryColor(int colorIndex)
    {
        secondaryColor = colorIndex;
        actionButton1.Activate(CUBE.colors[secondaryColor]);
        selectSecondary.SetColor(CUBE.colors[secondaryColor]);
    }


    #endregion

    #region Weapon Menu Methods

    public void WeaponEnter(Dictionary<string, object> info)
    {
        canMenuSwipe = false;
        InvokeAction(() => canMenuSwipe = true, menuSwipeDelay);

#if UNITY_ANDROID
        touchRect = new Rect(0f, 0.125f, 1f, 1f);
#endif

        // gui
        mainCamera.camera.rect = new Rect(0.25f, 0f, 1f, 1f);
        menuPanels[3].SetActive(true);
        infoPanel.SetActive(true);

        // weapon buttons
        for (int i = 0; i < 4; i++)
        {
            if (Grid.weapons[i] == null)
            {
                weaponButtons[i].isEnabled = false;
            }
            else
            {
                weaponButtons[i].isEnabled = true;
                weaponButtons[i].ActivateEvent += OnWeaponButtonPressed;
            }
        }

        // nav buttons
        foreach (var button in weaponNavButtons)
        {
            button.isEnabled = false;
            button.ActivateEvent += OnWeaponNavButton;
        }

        stateMachine.SetUpdate(WeaponUpdate());
        StartCoroutine("SaveConfirmation");
    }


    public IEnumerator WeaponUpdate()
    {
        // action buttons
        actionButton1.label.text = "---";
        actionButton1.isEnabled = false;
        actionButton2.label.text = "---";
        actionButton2.isEnabled = false;

        while (true)
        {
            // update camera
            UpdateCamera();
            CameraMovementEdit();

            // change menu
            int dir = MenuSwipe();
            if (dir == -1)
            {
                stateMachine.SetState(PAINTSTATE, new Dictionary<string, object>());
            }
            else if (dir == 1)
            {
                //stateMachine.SetState(PAINTSTATE, new Dictionary<string, object>());
            }

            // update ship stats
            UpdateInfoPanel();
            
            // update weapon buttons
            for (int i = 0; i < 4; i++)
            {
                if (Grid.weapons[i] == null)
                {
                    weaponButtons[i].isEnabled = false;
                    weaponButtons[i].label.text = "Weapon " + i;
                }
                else
                {
                    weaponButtons[i].isEnabled = true;
                    if (i == weaponIndex)
                    {
                        weaponButtons[i].label.text = "★ " + Grid.weapons[i].name;
                    }
                    else
                    {
                        weaponButtons[i].label.text = Grid.weapons[i].name;
                    }
                }
            }

            yield return null;
        }
    }


    public void WeaponExit(Dictionary<string, object> info)
    {
        menuPanels[3].SetActive(false);
        infoPanel.SetActive(false);
        weaponIndex = -1;

        // weapon buttons
        for (int i = 0; i < 4; i++)
        {
            weaponButtons[i].ActivateEvent -= OnWeaponButtonPressed;
        }

        // nav buttons
        foreach (var button in weaponNavButtons)
        {
            button.ActivateEvent -= OnWeaponNavButton;
        }

        StopCoroutine("SaveConfirmation");
    }


    private void OnWeaponButtonPressed(object sender, ActivateButtonArgs args)
    {
        if (args.isPressed) return;

        int index = int.Parse(args.value);
        if (weaponIndex == index)
        {
            weaponButtons[index].Activate(false);
            foreach (var button in weaponNavButtons)
            {
                button.isEnabled = false;
            }

            weaponIndex = -1;
        }
        else
        {
            weaponButtons[index].Activate(true);
            if (weaponIndex != -1)
            {
                weaponButtons[weaponIndex].Activate(false);
            }
            foreach (var button in weaponNavButtons)
            {
                button.isEnabled = true;
            }

            weaponIndex = index;
        }
    }


    private void OnWeaponNavButton(object sender, ActivateButtonArgs args)
    {
        if (!args.isPressed) return;

        int dir = int.Parse(args.value);

        weaponIndex = Grid.MoveWeaponMap(weaponIndex, dir);
    }

    #endregion

    #region Info Panel Methods

    private void UpdateInfoPanel()
    {
        // name
        Grid.buildName = shipName.value;

        // stats
        shipHealth.text = CUBE.HEALTHICON + " " + Grid.shipHealth;
        shipShield.text = CUBE.SHIELDICON + " " + Grid.shipShield;
        shipSpeed.text = CUBE.SPEEDICON + " " + Grid.shipSpeed;
        shipDamage.text = CUBE.DAMAGEICON + " " + Grid.shipDamage;
    }

    #endregion

    #region Save Methods

    private IEnumerator SaveConfirmation()
    {
#if UNITY_STANDALONE
        while (true)
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.S))
            {
                ConfirmSave();
            }

            yield return null;
        }
#else
        while (true)
        {
            // one finger
            if (Input.touchCount == 1 && touchRect.Contains(mainCamera.camera.ScreenToViewportPoint(Input.GetTouch(0).position)))
            {
                float heldTime = 0f;
                while (Input.touchCount == 1)
                {
                    heldTime += Time.deltaTime;
                    if (heldTime >= saveConfirmationTime)
                    {
                        ConfirmSave();
                        yield break;
                    }

                    yield return null;
                }
            }

            yield return null;
        }
#endif
    }


    private void ConfirmSave()
    {
        saveConfirmation.SetActive(true);
        saveShipName.text = Grid.buildName;
        StopCoroutine("SaveConfirmation");
    }


    public void Save()
    {
        Grid.SaveBuild();
        saveConfirmation.SetActive(false);
        StartCoroutine("SaveConfirmation");
    }


    public void CancelSave()
    {
        saveConfirmation.SetActive(false);
        StartCoroutine("SaveConfirmation");
    }

    #endregion
}