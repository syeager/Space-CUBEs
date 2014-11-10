// Little Byte Games
// Author: Steve Yeager
// Created: 2013.11.26
// Edited: 2014.10.06

using System;
using System.Collections;
using System.Collections.Generic;
using Annotations;
using LittleByte;
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

        [SerializeField, UsedImplicitly]
        private CubeLibrary cubeLibrary;

        #endregion

        #region Paint Fields

        [Header("Paint")]
        [SerializeField, UsedImplicitly]
        private PaintMenu paintMenu;

        #endregion

        #region Weapon Menu Fields

        [Header("Ability")]
        [SerializeField, UsedImplicitly]
        private AbilityMenu abilityMenu;

        #endregion

        #region Const Fields

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

        #region Save Fields

        public GameObject saveConfirmation;
        public UILabel saveShipName;

        #endregion

        #region Info Panel Fields

        public ActivateButton[] menuNavButtons = new ActivateButton[2];

        #endregion

        #region MonoBehaviour Overrides

        protected override void Awake()
        {
            base.Awake();

            if (NavigationBar.Main) NavigationBar.Show(false);

            // states
            States = new StateMachine(this, Menus.Edit.ToString());
            States.CreateState(Menus.Edit.ToString(), EditEnter, EditExit);
            States.CreateState(Menus.Paint.ToString(), PaintEnter, PaintExit);
            States.CreateState(Menus.Abilities.ToString(), AbilityEnter, AbilityExit);
            States.CreateState(Menus.View.ToString(), ViewEnter, ViewExit);

            // grid
#if UNITY_EDITOR
            string buildName = ConstructionGrid.SelectedBuild;
            Debug.Log(buildName);
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

            // scene
            cameraTarget = new GameObject("Camera Target").transform;
        }


        [UsedImplicitly]
        private void Start()
        {
            // events
            actionButtons.PickupPlaceEvent += PickupPlaceCUBE;
            actionButtons.DeleteEvent += () => grid.DeleteCUBE();

            StartCoroutine(ResettingCamera());

            EditInit();
            AbilityInit();
            States.Start();
        }


        [UsedImplicitly]
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Exit();
            }
        }

        #endregion

        #region Camera Methods

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
#if UNITY_STANDALONE || UNITY_EDITOR

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

            if (cubeLibrary.IsActive) return;

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
            previewShip.Initialize(ConstructionGrid.SelectedBuild, BuildStats.GetCoreCapacity());
            previewShip.SetValues(grid.CurrentStats, grid.CorePointsAvailable);
            cubeLibrary.ItemSelectedEvent += (sender, args) => SelectCube(args.id);
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

            previewShip.SetValues(grid.CurrentStats, grid.CorePointsAvailable);
        }


        public void ToggleCubeLibrary()
        {
            bool activate = !cubeLibrary.IsActive;
            cubeLibrary.Activate(activate);
        }


        private void SelectCube(int id)
        {
            cubeLibrary.Activate(false);
            grid.CreateCUBE(id);
        }

        #endregion

        #region Paint Methods

        private void PaintEnter(Dictionary<string, object> info)
        {
            paintMenu.gameObject.SetActive(true);

            grid.DeleteCUBE();

            States.SetUpdate(PaintUpdate());
        }


        private IEnumerator PaintUpdate()
        {
            while (true)
            {
                MoveCamera();
                UpdateCamera();
                yield return null;
            }
        }


        private void PaintExit(Dictionary<string, object> info)
        {
            paintMenu.gameObject.SetActive(false);
        }

        #endregion

        #region Ability Methods

        private void AbilityInit()
        {
            abilityMenu.Initialize();
        }


        private void AbilityEnter(Dictionary<string, object> info)
        {
            abilityMenu.Activate(true);
            States.SetUpdate(AbilityUpdate());
        }


        private IEnumerator AbilityUpdate()
        {
            while (true)
            {
                MoveCamera();
                UpdateCamera();
                yield return null;
            }
        }


        private void AbilityExit(Dictionary<string, object> info)
        {
            abilityMenu.Activate(false);
        }

        #endregion

        #region View Methods

        private void ViewEnter(Dictionary<string, object> info)
        {
            grid.ShowShip(true);

            States.SetUpdate(ViewUpdate());
        }


        private IEnumerator ViewUpdate()
        {
            while (true)
            {
                // zoom
                MoveCamera();
                UpdateCamera();

                // reset
#if UNITY_STANDALONE || UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.R))
                {
                    StartCoroutine(ResettingCamera());
                }
#else
                if (Input.touchCount > 0 && Input.GetTouch(0).tapCount == 2)
                {
                    StartCoroutine(ResettingCamera());
                }
#endif

                yield return null;
            }
        }


        private void ViewExit(Dictionary<string, object> info)
        {
            grid.ShowShip(false);
        }

        #endregion

        #region Save Methods

        public void ConfirmSave()
        {
            saveConfirmation.SetActive(true);
            saveShipName.text = grid.buildName;
        }


        public void Save()
        {
            grid.SaveBuild();
            saveConfirmation.SetActive(false);
        }


        public void CancelSave()
        {
            saveConfirmation.SetActive(false);
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


        public void Exit()
        {
            // TODO: save confirmation
            SceneManager.LoadScene(Scenes.Scene(Scenes.Menus.Garage));
        }

        #endregion
    }
}