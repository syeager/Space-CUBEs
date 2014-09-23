// Little Byte Games
// Author: Steve Yeager
// Created: 2013.11.26
// Edited: 2014.09.22

using System;
using System.Collections;
using System.Collections.Generic;
using Annotations;
using UnityEngine;

namespace SpaceCUBEs
{
    public class GarageManager : Singleton<GarageManager>
    {
        #region State Fields

        public StateMachine States { get; private set; }

        #endregion

        #region Camera Fields

        [Header("Camera")]
        private Transform cameraTarget;

        private Vector3 cameraDirection = Vector3.back;

        [SerializeField, UsedImplicitly]
        private Transform mainCamera;

        [SerializeField, UsedImplicitly]
        private float cameraSpeed;

        [SerializeField, UsedImplicitly]
        private float zoomSpeed;

        [SerializeField, UsedImplicitly]
        private float zoomMin;

        [SerializeField, UsedImplicitly]
        private float zoomMax;

        [SerializeField, UsedImplicitly]
        private float zoomStart;

        private float zoom;

        private const float CameraDist = 15f;

        #endregion

        #region Touch Fields

        [Header("Touch")]
        public float swipeDist;

        [SerializeField, UsedImplicitly]
        private float swipeTime;

        [SerializeField, UsedImplicitly]
        private float pinchModifier;

        [SerializeField, UsedImplicitly]
        private float pinchMin;

        [SerializeField, UsedImplicitly]
        private float menuSwipeDist;

        #endregion

        #region Public Fields

        public enum Menus
        {
            Edit = 0,
            Paint = 1,
            Abilities = 2,
            View = 3,
        }

        #endregion

        #region Private Fields

        [SerializeField, UsedImplicitly]
        private GarageActionButtons actionButtons;

        #endregion

        #region Edit Fields

        [Header("Edit")]
        public ConstructionGrid grid;

        [SerializeField, UsedImplicitly]
        private GameObject previewCube;

        [SerializeField, UsedImplicitly]
        private PreviewShip previewShip;

        #endregion

        #region Paint Fields

        [Header("Paint")]
        [SerializeField, UsedImplicitly]
        private PaintMenu paintMenu;

        #endregion

        #region Const Fields

        public const string BuildKey = "Build";

        private static readonly Vector3[] CameraPositions =
        {
            Vector3.up,
            Vector3.down,
            Vector3.right,
            Vector3.left,
            Vector3.forward,
            Vector3.back
        };

        private static readonly Vector3[] CameraRotations =
        {
            new Vector3(90, 0, 0),
            new Vector3(270, 180, 0),
            new Vector3(0, 270, 0),
            new Vector3(0, 90, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 0, 0)
        };

        #endregion

        #region Properties

        public Menus OpenMenu
        {
            get { return (Menus)Enum.Parse(typeof(Menus), States.currentState); }
        }

        #endregion

        #region Events

        public event Action<Menus, Menus> MenuChangedEvent;

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

        private int weaponIndex = -1;

        #endregion

        #region Save Fields

        public GameObject saveConfirmation;
        public UILabel saveShipName;

        #endregion

        #region Info Panel Fields

        public ActivateButton[] menuNavButtons = new ActivateButton[2];

        /// <summary>Label to display build points remaining.</summary>
        public UILabel corePointsLabel;

        #endregion

        #region MonoBehaviour Overrides

        protected override void Awake()
        {
            base.Awake();

            if (NavigationBar.Main) NavigationBar.Main.gameObject.SetActive(false);

            // states
            States = new StateMachine(this, Menus.Edit.ToString());
            States.CreateState(Menus.Edit.ToString(), EditEnter, EditExit);
            States.CreateState(Menus.Paint.ToString(), PaintEnter, PaintExit);
            //States.CreateState(AbilityState, PaintEnter, PaintExit);
            //States.CreateState(ViewState, PaintEnter, PaintExit);
            EditInit();
            PaintInit();
            States.Start();

            // grid
#if UNITY_EDITOR
            string buildName = ConstructionGrid.SelectedBuild;
            if (string.IsNullOrEmpty(buildName))
            {
                buildName = ConstructionGrid.DevBuilds[0];
                ConstructionGrid.SelectedBuild = buildName;
            }
#else
            string buildName = ConstructionGrid.SelectedBuild;
#endif
            grid.CreateGrid(ConstructionGrid.BuildSize, Player.Weaponlimit, Player.Weaponlimit);
            grid.CreateBuild(buildName);
            previewShip.Initialize(buildName, BuildStats.GetCoreCapacity());
            SetShipInfo();
            corePointsLabel.text = grid.corePointsAvailable.ToString();

            // scene
            cameraTarget = new GameObject("Camera Target").transform;

            return;


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
        }


        [UsedImplicitly]
        private void Start()
        {
            // events
            grid.StatusChangedEvent += OnCursorStatusChanged;
            actionButtons.PickupPlaceEvent += PickupPlaceCUBE;
            actionButtons.DeleteEvent += () => grid.DeleteCUBE();

            StartCoroutine(ResettingCamera());
        }


        [UsedImplicitly]
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelSave();
                States.SetState(States.previousState);
            }
        }

        #endregion

        #region Camera Methods

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
            return (grid.layer + direction * CameraDist).Round();
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
        private void MoveCamera()
        {
#if UNITY_STANDALONE

            // move CUBE
            if (Input.GetKeyDown(KeyCode.A))
            {
                grid.MoveCursor(-cameraTarget.right);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                grid.MoveCursor(cameraTarget.right);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                grid.MoveCursor(cameraTarget.up);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                grid.MoveCursor(-cameraTarget.up);
            }

            // rotate CUBE
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    grid.RotateCursor(cameraTarget.forward);
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    grid.RotateCursor(-cameraTarget.forward);
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    grid.RotateCursor(cameraTarget.up);
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    grid.RotateCursor(-cameraTarget.up);
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
                    grid.MoveCursor(cameraTarget.forward);
                    CameraZoom(0f);
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    grid.MoveCursor(-cameraTarget.forward);
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
            }

            // delete
            if (Input.GetKeyUp(KeyCode.Delete))
            {
                grid.DeleteCUBE();
                corePointsLabel.text = grid.corePointsAvailable.ToString();
            }

            // build
            if (Input.GetKeyUp(KeyCode.Return))
            {
                grid.SaveBuild();
            }

            // load
            if (Input.GetKeyDown(KeyCode.L))
            {
                grid.CreateBuild("Test Compact");
            }

#else

            int touchCount = Input.touchCount;
            // rotate camera
            if (touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    StartCoroutine(Swipe(touch));
                }
            }
                // zoom camera
            else if (touchCount == 2)
            {
                Touch touchA = Input.GetTouch(0);
                Touch touchB = Input.GetTouch(1);
                if (touchA.phase == TouchPhase.Began || touchB.phase == TouchPhase.Began)
                {
                    StartCoroutine(Pinch());
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
            for (int i = 0; i < CameraPositions.Length; i++)
            {
                if (cameraDirection == CameraPositions[i])
                {
                    index = i;
                    break;
                }
            }

            // rotate grid
            grid.RotateGrid(CameraPositions[index]);

            // set target position
            cameraTarget.position = CalculateTargetPostion(cameraDirection);
            // get target rotation
            cameraTarget.rotation = Quaternion.Euler(CameraRotations[index]);
        }


        /// <summary>
        /// Set zoom and target camera position.
        /// </summary>
        /// <param name="strength"></param>
        private void CameraZoom(float strength)
        {
            zoom = Mathf.Clamp(zoom + (strength * zoomSpeed * Time.deltaTime), zoomMin, zoomMax);
        }


        /// <summary>
        /// Return camera back to original position and rotation over time.
        /// </summary>
        private IEnumerator ResettingCamera()
        {
            while (cameraDirection != CameraPositions[0])
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

        #region Touch Methods

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

        #region Edit Methods

        private void EditInit()
        {
        }


        private void EditEnter(Dictionary<string, object> info)
        {
            previewCube.SetActive(true);

            States.SetUpdate(EditUpdate());
        }


        private IEnumerator EditUpdate()
        {
            while (true)
            {
                MoveCamera();
                UpdateCamera();
                yield return null;
            }
        }


        private void EditExit(Dictionary<string, object> info)
        {
            previewCube.SetActive(false);
        }


        private void PickupPlaceCUBE()
        {
            if (grid.cursorStatus == ConstructionGrid.CursorStatuses.Holding)
            {
                grid.PlaceCUBE(true);
            }
            else
            {
                grid.PickupCUBE();
            }
        }

        #endregion

        #region Paint Methods

        private void PaintInit()
        {
            colors = CUBE.LoadColors();
        }


        private void PaintEnter(Dictionary<string, object> info)
        {
            paintMenu.gameObject.SetActive(true);
            colorSelector.SetActive(false);

            grid.DeleteCUBE();
        }


        private void PaintExit(Dictionary<string, object> info)
        {
            paintMenu.gameObject.SetActive(false);
            colorSelector.SetActive(false);
        }

        #endregion

        #region Paint Methods

        private IEnumerator PaintUpdate()
        {
            UpdatePieces();

            while (true)
            {
                // update camera
                UpdateCamera();
                {
                    MoveCamera();
                }

                // detect swipe
                int dir = MenuSwipe();
                if (dir == -1)
                {
                }
                else if (dir == 1)
                {
                }

                // update position and rotation
                paintPostionLabel.text = "Position " + (grid.cursor + Vector3.one).ToString("0");

                UpdatePieces();
                SetShipInfo();

                yield return null;
            }
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

            grid.Paint(pieceSelected, args.value == "action1" ? primaryColor : secondaryColor);
        }


        private void UpdatePieces()
        {
            if (grid.cursorStatus == ConstructionGrid.CursorStatuses.Hover)
            {
                // enable copy to's
                copyPrimary.isEnabled = true;
                copySecondary.isEnabled = true;

                // enable paints
                SetPrimaryColor(primaryColor);
                SetSecondaryColor(secondaryColor);

                // enable pieces
                pieces[pieceSelected].Activate(false);
                int count = grid.hoveredCUBE.renderer.sharedMaterials.Length;
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
                Color copyColor = colors[grid.hoveredCUBE.GetComponent<ColorVertices>().colors[pieceSelected]];
                copyPrimary.SetColor(copyColor);
                copySecondary.SetColor(copyColor);
            }
            else
            {
                // disable copy to's
                copyPrimary.isEnabled = false;
                copySecondary.isEnabled = false;

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
            Color copyColor = colors[grid.hoveredCUBE.GetComponent<ColorVertices>().colors[pieceSelected]];
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
                Color color = colors[grid.hoveredCUBE.GetComponent<ColorVertices>().colors[pieceSelected]];
                SetPrimaryColor(Array.IndexOf(CUBE.Colors, color));
            }
            else
            {
                Color color = colors[grid.hoveredCUBE.GetComponent<ColorVertices>().colors[pieceSelected]];
                SetSecondaryColor(Array.IndexOf(CUBE.Colors, color));
            }
        }


        private void OnPaintPositionButtonPressed(object sender, ActivateButtonArgs args)
        {
            if (!args.isPressed) return;

            switch (args.value)
            {
                case "back":
                    grid.MoveCursor(-cameraTarget.forward);
                    CameraZoom(0f);
                    UpdatePieces();
                    break;
                case "forward":
                    grid.MoveCursor(cameraTarget.forward);
                    CameraZoom(0f);
                    UpdatePieces();
                    break;
                case "left":
                    grid.MoveCursor(-cameraTarget.right);
                    UpdatePieces();
                    break;
                case "right":
                    grid.MoveCursor(cameraTarget.right);
                    UpdatePieces();
                    break;
                case "down":
                    grid.MoveCursor(-cameraTarget.up);
                    UpdatePieces();
                    break;
                case "up":
                    grid.MoveCursor(cameraTarget.up);
                    UpdatePieces();
                    break;
            }
        }


        private void SetPrimaryColor(int colorIndex)
        {
            primaryColor = colorIndex;
            selectPrimary.SetColor(CUBE.Colors[primaryColor]);
        }


        private void SetSecondaryColor(int colorIndex)
        {
            secondaryColor = colorIndex;
            selectSecondary.SetColor(CUBE.Colors[secondaryColor]);
        }

        #endregion

        #region Weapon Menu Methods

        public void WeaponEnter(Dictionary<string, object> info)
        {
#if UNITY_ANDROID
    //touchRect = new Rect(0f, 0.125f, 1f, 1f);
#endif

            // gui
            mainCamera.camera.rect = new Rect(0.25f, 0f, 1f, 1f);

            // weapon buttons
            for (int i = 0; i < weaponExpansions; i++)
            {
                if (grid.weapons[i] == null)
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

            States.SetUpdate(WeaponUpdate());
            StartCoroutine("SaveConfirmation");
        }


        public IEnumerator WeaponUpdate()
        {
            while (true)
            {
                // update camera
                UpdateCamera();
                {
                    MoveCamera();
                }

                // change menu
                int dir = MenuSwipe();
                if (dir == -1)
                {
                    States.SetState(Menus.Paint.ToString());
                }

                // update ship stats
                SetShipInfo();

                // update weapon buttons
                for (int i = 0; i < weaponExpansions; i++)
                {
                    if (grid.weapons[i] == null)
                    {
                        weaponButtons[i].isEnabled = false;
                        weaponButtons[i].label.text = "Weapon " + (i + 1);
                        weaponButtons[i].Activate(false);
                    }
                    else
                    {
                        weaponButtons[i].isEnabled = true;
                        weaponButtons[i].label.text = grid.weapons[i].name;
                        weaponButtons[i].Activate(i == weaponIndex);
                    }
                }

                yield return null;
            }
        }


        public void WeaponExit(Dictionary<string, object> info)
        {
            StopWeaponBlink();
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
                StartWeaponBlink(index);

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

            weaponIndex = grid.MoveWeaponMap(weaponIndex, dir);
        }


        private void StartWeaponBlink(int index)
        {
            StopWeaponBlink();
            grid.StartBlink(grid.weapons[index].renderer);
        }


        private void StopWeaponBlink()
        {
            if (weaponIndex >= 0)
            {
                grid.StopBlink(grid.weapons[weaponIndex].renderer);
            }
        }

        #endregion

        #region Observation Methods

        private void ObservationEnter(Dictionary<string, object> info)
        {
        }


        private IEnumerator ObservationUpdate()
        {
            while (true)
            {
                yield return null;
            }
        }

        #endregion

        #region Info Panel Methods

        private void SetShipInfo()
        {
            // name
            //grid.buildName = shipName.value;

            // stats
            previewShip.SetValues(grid.CurrentStats, grid.corePointsAvailable);
        }

        #endregion

        #region Save Methods

        [UsedImplicitly]
        private IEnumerator SaveConfirmation()
        {
            yield return null;
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
    //while (true)
    //{
    //    // one finger
    //    if (Input.touchCount == 1 && touchRect.Contains(mainCamera.camera.ScreenToViewportPoint(Input.GetTouch(0).position)))
    //    {
    //        float heldTime = 0f;
    //        while (Input.touchCount == 1)
    //        {
    //            heldTime += Time.deltaTime;
    //            if (heldTime >= saveConfirmationTime)
    //            {
    //                ConfirmSave();
    //                yield break;
    //            }

    //            yield return null;
    //        }
    //    }

    //    yield return null;
    //}
#endif
        }


        private void ConfirmSave()
        {
            saveConfirmation.SetActive(true);
            saveShipName.text = grid.buildName;
            StopCoroutine("SaveConfirmation");
        }


        public void Save()
        {
            grid.SaveBuild();
            saveConfirmation.SetActive(false);
            StartCoroutine("SaveConfirmation");
        }


        public void CancelSave()
        {
            saveConfirmation.SetActive(false);
            StartCoroutine("SaveConfirmation");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Change the currently open menu.
        /// </summary>
        /// <param name="menu">Menu to switch to.</param>
        public void ChangeMenu(Menus menu)
        {
            if (menu == OpenMenu) return;

            if (MenuChangedEvent != null)
            {
                MenuChangedEvent.Invoke(OpenMenu, menu);
            }

            States.SetState(menu.ToString());
        }


        /// <summary>
        /// Move the cursor inside the grid.
        /// </summary>
        /// <param name="direction">Local unit vector.</param>
        public void MoveCursor(Vector3 direction)
        {
            grid.MoveCursor(cameraTarget.TransformDirection(direction));
        }


        /// <summary>
        /// Rotate the cursor inside the grid.
        /// </summary>
        /// <param name="direction">Local rotation axis.</param>
        public void RotateCursor(Vector3 direction)
        {
            grid.RotateCursor(direction);
        }

        #endregion

        #region Event Handlers

        private void OnCursorStatusChanged(object sender, CursorUpdatedArgs args)
        {
            switch (args.current)
            {
                case ConstructionGrid.CursorStatuses.Holding:
                    SetShipInfo();
                    break;

                case ConstructionGrid.CursorStatuses.Hover:
                    break;

                case ConstructionGrid.CursorStatuses.None:
                    SetShipInfo();
                    break;
            }
        }

        #endregion
    }
}