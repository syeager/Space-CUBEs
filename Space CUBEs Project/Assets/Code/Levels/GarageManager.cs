// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.11.26
// Edited: 2014.05.31

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Annotations;
using LittleByte.Data;
using UnityEngine;

public class GarageManager : MonoBase
{
    #region References

    public ConstructionGrid Grid;
    public Transform mainCamera;

    #endregion

    #region Public Fields

    private const int GridSize = 10;

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

    public GameObject CUBESelectionButton_Prefab;

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

    private int weaponIndex = -1;

    private int[] inventory;

    private Vector3 cameraDirection = Vector3.up;
    private float zoom;

    private bool selectedBuild;

    private CUBEInfo currentCUBE;

    #endregion

    #region Mobile Fields

    private Rect touchRect;

    #endregion

    #region State Fields

    private StateMachine stateMachine;
    private const string LoadState = "Load";
    private const string SelectState = "Select";
    private const string NavState = "Nav";
    private const string WeaponState = "Weapon";
    private const string PaintState = "Paint";
    private const string ObserveState = "Observe";

    #endregion

    #region Readonly Fields

    private readonly Vector3[] cameraPositions =
    {
        new Vector3(0, 1, 0),
        new Vector3(0, -1, 0),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1)
    };

    private readonly Vector3[] cameraRotations =
    {
        new Vector3(90, 0, 0),
        new Vector3(270, 180, 0),
        new Vector3(0, 270, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 180, 0),
        new Vector3(0, 0, 0)
    };

    private static readonly Rect InfoPanelRect = new Rect(0f, 0f, 1f, 0.125f);
    private static readonly Rect NavMenuButtonsRect = new Rect(0.86f, 0.9f, 0.14f, 0.1f);

    #endregion

    #region Load Menu Fields

    public GameObject renamePanel;
    public UIInput renameInput;

    #endregion

    #region Selection Fields

    private int selectionIndex;
    public ActivateButton leftFilter;
    public UILabel filterLabel;
    public ActivateButton rightFilter;
    public GameObject[] selections;
    private UIGrid[] selectionGrids;
    private UIScrollView[] selectionScrollViews;
    private UIScrollBar[] selectionScrollBars;

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

    /// <summary>Weapon expansions available.</summary>
    private int weaponExpansions;

    #endregion

    #region Save Fields

    public float saveConfirmationTime = 0.6f;
    public GameObject saveConfirmation;
    public UILabel saveShipName;
    public GameObject saveButton;

    #endregion

    #region Info Panel Fields

    public ActivateButton[] menuNavButtons = new ActivateButton[2];

    /// <summary>Label to display build points remaining.</summary>
    public UILabel corePointsLabel;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Awake()
    {
        // create states
        stateMachine = new StateMachine(this, LoadState);
        stateMachine.CreateState(LoadState, LoadEnter, LoadExit);
        stateMachine.CreateState(SelectState, SelectEnter, SelectExit);
        stateMachine.CreateState(NavState, NavEnter, NavExit);
        stateMachine.CreateState(PaintState, PaintEnter, PaintExit);
        stateMachine.CreateState(WeaponState, WeaponEnter, WeaponExit);

        cameraTarget = new GameObject("Camera Target").transform;
        StartCoroutine(ResettingCamera());

        // load colors
        colors = CUBE.LoadColors();

        SelectInit();

        // nav menu
        foreach (ActivateButton button in positionButtons)
        {
            button.ActivateEvent += OnPositionButtonPressed;
        }
        foreach (ActivateButton button in rotationButtons)
        {
            button.ActivateEvent += OnRotationButtonPressed;
        }

        // paint menu
        ActivateButton[] paints = paintGrid.GetComponentsInChildren<ActivateButton>(true);
        foreach (ActivateButton paint in paints)
        {
            paint.ActivateEvent += OnColorSelected;
        }
        foreach (ActivateButton piece in pieces)
        {
            piece.ActivateEvent += OnPieceSelected;
        }
        foreach (ActivateButton button in paintPositionButtons)
        {
            button.ActivateEvent += OnPaintPositionButtonPressed;
        }
        copyPrimary.ActivateEvent += CopyColor;
        copySecondary.ActivateEvent += CopyColor;
        selectPrimary.ActivateEvent += OpenColorSelector;
        selectSecondary.ActivateEvent += OpenColorSelector;
        primaryColor = int.Parse(paints[0].value);
        secondaryColor = int.Parse(paints[1].value);

        // weapon menu
        weaponExpansions = BuildStats.GetWeaponExpansion();
        for (int i = weaponExpansions; i < BuildStats.WeaponExpansions[BuildStats.WeaponExpansions.Length - 1]; i++)
        {
            weaponButtons[i].gameObject.SetActive(false);
        }

        // info panel
        foreach (ActivateButton button in menuNavButtons)
        {
            button.ActivateEvent += ChangeMenu;
        }
    }


    [UsedImplicitly]
    private void Start()
    {
        stateMachine.Start();
    }


    [UsedImplicitly]
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelSave();
            stateMachine.SetState(stateMachine.previousState);
        }
    }

    #endregion

    #region Camera Methods

    /// <summary>
    /// Event handler for menu nav buttons.
    /// </summary>
    public void ChangeMenu(object sender, ActivateButtonArgs args)
    {
        if (args.isPressed) return;

        SwitchMenu(args.value == "right");
    }


    /// <summary>
    /// Switch to the next menu.
    /// </summary>
    /// <param name="moveRight">Should we move to the next right menu?</param>
    private void SwitchMenu(bool moveRight)
    {
        if (moveRight)
        {
            switch (stateMachine.currentState)
            {
                case LoadState:
                    stateMachine.SetState(SelectState);
                    break;
                case SelectState:
                    stateMachine.SetState(NavState);
                    break;
                case NavState:
                    stateMachine.SetState(PaintState);
                    break;
                case PaintState:
                    stateMachine.SetState(WeaponState);
                    break;
            }

            menuNavButtons[0].isEnabled = true;
            menuNavButtons[1].isEnabled = stateMachine.currentState != WeaponState;
        }
        else
        {
            switch (stateMachine.currentState)
            {
                case SelectState:
                    stateMachine.SetState(LoadState);
                    break;
                case NavState:
                    stateMachine.SetState(SelectState);
                    break;
                case PaintState:
                    stateMachine.SetState(NavState);
                    break;
                case WeaponState:
                    stateMachine.SetState(PaintState);
                    break;
            }

            menuNavButtons[1].isEnabled = true;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    private int MenuSwipe()
    {
        int direction = 0;

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
    /// Move and rotate camera to target position and rotation.
    /// </summary>
    private void UpdateCamera()
    {
        mainCamera.position = Vector3.Lerp(mainCamera.position, cameraTarget.position, Time.deltaTime * cameraSpeed);
        mainCamera.rotation = Quaternion.Slerp(mainCamera.rotation, cameraTarget.rotation, Time.deltaTime * cameraSpeed);

        mainCamera.camera.orthographicSize = Mathf.Lerp(mainCamera.camera.orthographicSize, zoom, Time.deltaTime * zoomSpeed);
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
                Grid.MoveCursor(cameraTarget.forward);
                CameraZoom(0f);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Grid.MoveCursor(-cameraTarget.forward);
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
            corePointsLabel.text = Grid.corePointsAvailable.ToString();
        }

        // delete
        if (Input.GetKeyUp(KeyCode.Delete))
        {
            Grid.DeleteCUBE();
            corePointsLabel.text = Grid.corePointsAvailable.ToString();
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
    /// Set zoom and target camera position.
    /// </summary>
    /// <param name="strength"></param>
    private void CameraZoom(float strength)
    {
        zoom = Mathf.Clamp(zoom + (strength * zoomSpeed * Time.deltaTime), zoomMin, zoomMax);
    }


    //
    private void ResetCamera(params Rect[] restricted)
    {
#if UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(ResettingCamera());
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).tapCount == 2)
        {
            Vector3 screenPos = mainCamera.camera.ScreenToViewportPoint(Input.GetTouch(0).position);

            if (screenPos.x < 0f) return;

            for (int i = 0; i < restricted.Length; i++)
            {
                if (restricted[i].Contains(screenPos)) return;
            }

            StartCoroutine(ResettingCamera());
        }
#endif
    }


    /// <summary>
    /// Return camera back to original position and rotation over time.
    /// </summary>
    private IEnumerator ResettingCamera()
    {
        while (cameraDirection != cameraPositions[0])
        {
            RotateCamera(Vector3.up);
        }

        float direction = (zoom - zoomStart > 0 ? -1f : 1f);
        while (Mathf.Abs(zoom - zoomStart) > 0.1f)
        {
            CameraZoom(direction);
            yield return null;
        }
    }

    #endregion

    #region Mobile Methods

    [UsedImplicitly]
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
                if (vector.x >= swipeDist)
                {
                    RotateCamera(Vector3.left);
                    yield break;
                }
                // up
                if (vector.y <= -swipeDist)
                {
                    RotateCamera(Vector3.up);
                    yield break;
                }
                // down
                if (vector.y >= swipeDist)
                {
                    RotateCamera(Vector3.down);
                    yield break;
                }
            }

            yield return null;
        }
    }


    [UsedImplicitly]
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
        mainCamera.camera.rect = new Rect(0f, 0f, 1f, 1f);
        menuPanels[0].SetActive(true);
        StartCoroutine(CreateBuildButtons());

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
                    stateMachine.SetState(SelectState);
                }
            }
            yield return null;
        }
    }


    private void LoadExit(Dictionary<string, object> info)
    {
        // clear
        ActivateButton[] buttons = loadGrid.GetComponentsInChildren<ActivateButton>();
        foreach (ActivateButton button in buttons)
        {
            button.ActivateEvent -= OnBuildChosen;
            Destroy(button.gameObject);
        }

        menuPanels[0].SetActive(false);
    }


    /// <summary>
    /// Create Construction Grid and position camera.
    /// </summary>
    private void CreateGrid()
    {
        Grid.CreateGrid(GridSize, BuildStats.GetWeaponExpansion(), BuildStats.GetAugmentationExpansion());

        // position camera
        Grid.RotateGrid(Vector3.up);
        cameraTarget.position = CalculateTargetPostion(Vector3.up);
        cameraTarget.rotation = Quaternion.Euler(cameraRotations[0]);
    }


    /// <summary>
    /// Create all of the buttons for the builds in the load menu.
    /// </summary>
    private IEnumerator CreateBuildButtons()
    {
        // create
        string[] buildNames = ConstructionGrid.BuildNames().ToArray();
        if (buildNames.Length > 0) SceneManager.Main.currentBuild = buildNames[0];
        for (int i = 0; i < buildNames.Length; i++)
        {
            var button = (Instantiate(BuildButton_Prefab) as GameObject).GetComponent<ScrollviewButton>();
            button.Initialize(i.ToString(), buildNames[i], buildNames[i], loadGrid.transform, loadScrollView);
            button.ActivateEvent += OnBuildChosen;
        }

        yield return StartCoroutine(Utility.UpdateScrollView(loadGrid, loadScrollBar, loadScrollView));

        var centerOnChild = (UICenterOnChild)loadGrid.GetComponent(typeof(UICenterOnChild));
        centerOnChild.Recenter();
        centerOnChild.CenterOn(loadGrid.transform.GetChild(0));
    }


    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }


    public void LoadStore()
    {
        SceneManager.LoadScene("Store");
    }


    private void OnBuildChosen(object sender, ActivateButtonArgs args)
    {
        if (args.isPressed) return;

        SceneManager.Main.currentBuild = args.value;
    }


    /// <summary>
    /// Delete selected build from data.
    /// </summary>
    public void DeleteBuild()
    {
        if (string.IsNullOrEmpty(SceneManager.Main.currentBuild)) return;

        // delete build
        ConstructionGrid.DeleteBuild(SceneManager.Main.currentBuild);

        // remove build button
        ScrollviewButton button = loadGrid.GetComponentsInChildren<ScrollviewButton>().First(b => b.value == SceneManager.Main.currentBuild);
        button.ActivateEvent -= OnBuildChosen;
        Destroy(button.gameObject);

        // reload grid
        StartCoroutine(Utility.UpdateScrollView(loadGrid, loadScrollBar, loadScrollView));

        SceneManager.Main.currentBuild = ConstructionGrid.BuildNames()[0];
    }


    public void ConfirmRename()
    {
        renamePanel.SetActive(true);
        renameInput.value = SceneManager.Main.currentBuild;
    }


    public void RenameBuild()
    {
        ConstructionGrid.RenameBuild(SceneManager.Main.currentBuild, renameInput.value);
        renamePanel.SetActive(false);

        ScrollviewButton button = loadGrid.GetComponentsInChildren<ScrollviewButton>().First(b => b.value == SceneManager.Main.currentBuild);
        SceneManager.Main.currentBuild = renameInput.value;
        button.value = SceneManager.Main.currentBuild;
        button.label.text = SceneManager.Main.currentBuild;
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
        Grid.CreateBuild(SceneManager.Main.currentBuild);
        shipName.value = Grid.buildName;
        corePointsLabel.text = Grid.corePointsAvailable.ToString();
        stateMachine.SetState(SelectState, new Dictionary<string, object>());
        selectedBuild = true;
    }


    public void NewBuild()
    {
        CreateGrid();

        const string custom = "Custom";
        int i = 1;
        while (true)
        {
            if (!SaveData.Contains(custom + i, ConstructionGrid.BuildsFolder))
            {
                shipName.value = custom + i;
                break;
            }
            i++;
        }

        corePointsLabel.text = Grid.corePointsAvailable.ToString();
        stateMachine.SetState(SelectState);
        selectedBuild = true;
    }


    public void Play()
    {
        SceneManager.LoadScene("Level Select Menu", true);
    }

    #endregion

    #region Selection Menu Methods

    private void SelectInit()
    {
        inventory = CUBE.GetInventory();

        foreach (GameObject selection in selections)
        {
            selection.SetActive(true);
        }

        selectionGrids = new UIGrid[selections.Length];
        selectionScrollViews = new UIScrollView[selections.Length];
        selectionScrollBars = new UIScrollBar[selections.Length];
        for (int i = 0; i < selections.Length; i++)
        {
            selectionGrids[i] = selections[i].GetComponentInChildren(typeof(UIGrid)) as UIGrid;
            selectionScrollViews[i] = selections[i].GetComponentInChildren(typeof(UIScrollView)) as UIScrollView;
            selectionScrollBars[i] = selections[i].GetComponentInChildren(typeof(UIScrollBar)) as UIScrollBar;
        }

        leftFilter.ActivateEvent += OnFilterChanged;
        rightFilter.ActivateEvent += OnFilterChanged;

        StartCoroutine(CreateItemButtons());
    }


    private void SelectEnter(Dictionary<string, object> info)
    {
#if UNITY_ANDROID
        touchRect = new Rect(0f, 0.125f, 1f, 1f);
#endif

        mainCamera.camera.rect = new Rect(0.25f, 0f, 1f, 1f);
        menuPanels[1].SetActive(true);
        infoPanel.SetActive(true);

        FilterCUBEs(selectionIndex);

        stateMachine.SetUpdate(SelectUpdate());
        StartCoroutine("SaveConfirmation");
    }


    private IEnumerator SelectUpdate()
    {
        actionButton1.label.text = "---";
        actionButton1.isEnabled = false;
        actionButton2.label.text = "---";
        actionButton2.isEnabled = false;

        while (true)
        {
            // update camera
            UpdateCamera();
            if (UICamera.selectedObject != shipName.gameObject)
            {
                CameraMovementEdit();
            }

            // change menu
            int dir = MenuSwipe();
            if (dir == -1)
            {
                stateMachine.SetState(LoadState);
            }
            else if (dir == 1)
            {
                stateMachine.SetState(NavState);
            }

            // reset camera
            ResetCamera(InfoPanelRect, NavMenuButtonsRect);

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
    private void FilterCUBEs(int index)
    {
        // active selection
        selections[selectionIndex].SetActive(false);
        selectionIndex = Mathf.Clamp(index, 0, selections.Length - 1);
        selections[selectionIndex].SetActive(true);
        selectionScrollViews[selectionIndex].UpdateScrollbars();

        // toggle filter buttons
        leftFilter.isEnabled = selectionIndex > 0;
        rightFilter.isEnabled = selectionIndex < selections.Length - 1;

        // set filter
        filterLabel.text = Enum.GetNames(typeof(CUBE.Types))[selectionIndex];
    }


    /// <summary>
    /// Create buttons in Selection Menu for all CUBEs.
    /// </summary>
    private IEnumerator CreateItemButtons()
    {
        menuPanels[0].SetActive(true);
        menuPanels[1].SetActive(false);
        menuPanels[2].SetActive(false);
        menuPanels[3].SetActive(false);
        menuPanels[4].SetActive(false);
        infoPanel.SetActive(false);
        mainCamera.camera.rect = new Rect(0f, 0f, 1f, 1f);

        string[] names = Enum.GetNames(typeof(CUBE.Types));
        foreach (CUBEInfo info in CUBE.allCUBES)
        {
            int index = Array.IndexOf(names, info.type.ToString());
            var button = (Instantiate(CUBESelectionButton_Prefab) as GameObject).GetComponent(typeof(ScrollviewButton)) as ScrollviewButton;
            button.Initialize(info.ID.ToString(), info.name + "\n" + inventory[info.ID], info.ID.ToString(), selectionGrids[index].transform, selectionScrollViews[index]);
            button.ActivateEvent += OnCUBESelected;
        }

        for (int i = 0; i < selectionScrollViews.Length; i++)
        {
            StartCoroutine(Utility.UpdateScrollView(selectionGrids[i], selectionScrollBars[i], selectionScrollViews[i]));
        }

        yield return new WaitForEndOfFrame();

        foreach (GameObject selection in selections)
        {
            selection.SetActive(false);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void OnCUBESelected(object sender, ActivateButtonArgs args)
    {
        if (!args.isPressed) return;

        if (GameResources.GetCUBE(int.Parse(args.value)) != null)
        {
            SetCurrentCUBE(int.Parse(args.value));
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void OnFilterChanged(object sender, ActivateButtonArgs args)
    {
        if (!args.isPressed) return;

        FilterCUBEs(selectionIndex + int.Parse(args.value));
    }


    private void OnNavMenuPressed(object sender, ActivateButtonArgs args)
    {
        if (args.isPressed) return;

        stateMachine.SetState(NavState);
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
            if (UICamera.selectedObject != shipName.gameObject)
            {
                CameraMovementEdit();
            }

            // detect swipe
            int dir = MenuSwipe();
            if (dir == -1)
            {
                stateMachine.SetState(SelectState);
            }
            else if (dir == 1)
            {
                stateMachine.SetState(PaintState);
            }

            // update position and rotation
            postionLabel.text = "Position " + (Grid.cursor + Vector3.one).ToString("0");
            rotationLabel.text = "Rotation " + Grid.cursorRotation.eulerAngles.ToString("0");

            // update delete button
            if (Grid.cursorStatus == ConstructionGrid.CursorStatuses.Holding && !actionButton1.isEnabled)
            {
                actionButton1.isEnabled = true;
            }
            else if (Grid.cursorStatus != ConstructionGrid.CursorStatuses.Holding && actionButton1.isEnabled)
            {
                actionButton1.isEnabled = false;
            }

            // update action button
            switch (Grid.cursorStatus)
            {
                case ConstructionGrid.CursorStatuses.Holding:
                    actionButton2.label.text = "Place";
                    actionButton2.isEnabled = true;
                    break;
                case ConstructionGrid.CursorStatuses.Hover:
                    actionButton2.label.text = "Pickup";
                    actionButton2.isEnabled = true;
                    break;
                case ConstructionGrid.CursorStatuses.None:
                    actionButton2.label.text = "---";
                    actionButton2.isEnabled = false;
                    break;
            }

            // update ship stats
            UpdateInfoPanel();

            // reset camera
            ResetCamera(InfoPanelRect, NavMenuButtonsRect);

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


    private void OnPositionButtonPressed(object sender, ActivateButtonArgs args)
    {
        if (!args.isPressed) return;

        switch (args.value)
        {
            case "back":
                Grid.MoveCursor(-cameraTarget.forward);
                CameraZoom(0f);
                break;
            case "forward":
                Grid.MoveCursor(cameraTarget.forward);
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
        corePointsLabel.text = Grid.corePointsAvailable.ToString();
    }

    #endregion

    #region Paint Methods

    private void PaintEnter(Dictionary<string, object> info)
    {
        menuPanels[4].SetActive(true);
        colorSelector.SetActive(false);
        infoPanel.SetActive(true);

        actionButton1.ActivateEvent += OnPaint;
        actionButton2.ActivateEvent += OnPaint;

        Grid.DeleteCUBE();

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
            if (UICamera.selectedObject != shipName.gameObject)
            {
                CameraMovementEdit();
            }

            // detect swipe
            int dir = MenuSwipe();
            if (dir == -1)
            {
                stateMachine.SetState(NavState);
            }
            else if (dir == 1)
            {
                stateMachine.SetState(WeaponState);
            }

            // update position and rotation
            paintPostionLabel.text = "Position " + (Grid.cursor + Vector3.one).ToString("0");

            UpdatePieces();
            UpdateInfoPanel();

            // reset camera
            ResetCamera(InfoPanelRect, NavMenuButtonsRect);

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

        Grid.Paint(pieceSelected, args.value == "action1" ? primaryColor : secondaryColor);
    }


    private void UpdatePieces()
    {
        if (Grid.cursorStatus == ConstructionGrid.CursorStatuses.Hover)
        {
            // enable copy to's
            copyPrimary.isEnabled = true;
            copySecondary.isEnabled = true;

            // enable paints
            actionButton1.isEnabled = true;
            SetPrimaryColor(primaryColor);
            actionButton2.isEnabled = true;
            SetSecondaryColor(secondaryColor);

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
            foreach (ActivateButton piece in pieces)
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
                Grid.MoveCursor(-cameraTarget.forward);
                CameraZoom(0f);
                UpdatePieces();
                break;
            case "forward":
                Grid.MoveCursor(cameraTarget.forward);
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
        actionButton1.Activate(CUBE.colors[primaryColor]);
        selectPrimary.SetColor(CUBE.colors[primaryColor]);
    }


    private void SetSecondaryColor(int colorIndex)
    {
        secondaryColor = colorIndex;
        actionButton2.Activate(CUBE.colors[secondaryColor]);
        selectSecondary.SetColor(CUBE.colors[secondaryColor]);
    }

    #endregion

    #region Weapon Menu Methods

    public void WeaponEnter(Dictionary<string, object> info)
    {
#if UNITY_ANDROID
        touchRect = new Rect(0f, 0.125f, 1f, 1f);
#endif

        // gui
        mainCamera.camera.rect = new Rect(0.25f, 0f, 1f, 1f);
        menuPanels[3].SetActive(true);
        infoPanel.SetActive(true);

        // weapon buttons
        for (int i = 0; i < weaponExpansions; i++)
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
        foreach (ActivateButton button in weaponNavButtons)
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
            if (UICamera.selectedObject != shipName.gameObject)
            {
                CameraMovementEdit();
            }

            // change menu
            int dir = MenuSwipe();
            if (dir == -1)
            {
                stateMachine.SetState(PaintState);
            }

            // update ship stats
            UpdateInfoPanel();

            // update weapon buttons
            for (int i = 0; i < weaponExpansions; i++)
            {
                if (Grid.weapons[i] == null)
                {
                    weaponButtons[i].isEnabled = false;
                    weaponButtons[i].label.text = "Weapon " + (i + 1);
                    weaponButtons[i].Activate(false);
                }
                else
                {
                    weaponButtons[i].isEnabled = true;
                    weaponButtons[i].label.text = Grid.weapons[i].name;
                    weaponButtons[i].Activate(i == weaponIndex);
                }
            }

            // reset camera
            ResetCamera(InfoPanelRect, NavMenuButtonsRect);

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
        foreach (ActivateButton button in weaponNavButtons)
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
            foreach (ActivateButton button in weaponNavButtons)
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
            foreach (ActivateButton button in weaponNavButtons)
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
        shipHealth.text = CUBE.HEALTHICON + " " + Grid.CurrentStats.health;
        shipShield.text = CUBE.SHIELDICON + " " + Grid.CurrentStats.shield;
        shipSpeed.text = CUBE.SPEEDICON + " " + Grid.CurrentStats.speed;
        shipDamage.text = CUBE.DAMAGEICON + " " + Grid.CurrentStats.damage;
    }

    #endregion

    #region Save Methods

    [UsedImplicitly]
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
        UICamera.selectedObject = saveButton;
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