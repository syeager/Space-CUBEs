// Little Byte Games
// Author: Steve Yeager
// Created: 2013.11.26
// Edited: 2014.09.20

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Annotations;
using LittleByte.Data;
using UnityEngine;

namespace SpaceCUBEs
{
    public class ConstructionGrid : MonoBase
    {
        #region Public Fields

        /// <summary>Material given to CUBEs.</summary>
        public Material cubeMat;

        /// <summary>Used to show center of the grid.</summary>
        public GameObject Center_Prefab;

        /// <summary>GameObject representations of the grid's cells.</summary>
        public GameObject Cell_Prefab;

        /// <summary>Cell colorIndex when cursor is in cell.</summary>
        public Material CellCursor_Mat;

        /// <summary>Cell colorIndex when cell is included in the cursor bounds.</summary>
        public Material CellCursorBounds_Mat;

        /// <summary>Cell colorIndex when nothing is in cell.</summary>
        public Material CellOpen_Mat;

        /// <summary>Different statuses of the cursor.</summary>
        public enum CursorStatuses
        {
            None,
            Hover,
            Holding,
        }

        public float cellSize = 0.9f;
        public float cursorSize = 0.7f;

        /// <summary>Name of the build. Set in a textfield by player.</summary>
        public string buildName;

        public Color blinkColor;
        public float blinkTime;

        #endregion

        #region Private Fields

        /// <summary>Matrix with cells that hold references to the CUBE that currently occupies them.</summary>
        private CUBE[][][] grid;

        /// <summary>Matrix that holds references to all of the cell gameObjects.</summary>
        private GameObject[][][] cells;

        /// <summary>Center of the grid.</summary>
        private Transform Center;

        /// <summary>All of the CUBEs currently placed.</summary>
        private readonly Dictionary<CUBE, CUBEGridInfo> currentBuild = new Dictionary<CUBE, CUBEGridInfo>();

        /// <summary>Parent of all of the CUBEs.</summary>
        private GameObject ship;

        /// <summary>Offset from cursor to held CUBE's pivotOffset.</summary>
        private Vector3 cursorOffset;

        /// <summary>Current viewAxis being looked through. Affects grid rotation.</summary>
        private Vector3 viewAxis;

        /// <summary>Total core points allowed.</summary>
        private int corePointsMax;

        /// <summary>Core points remaining.</summary>
        public int corePointsAvailable { get; private set; }

        /// <summary>Available weapon slots.</summary>
        private int weaponSlots;

        /// <summary>Available augmentation slots.</summary>
        private int augmentationSlots;

        /// <summary>Level of alpha to set CUBE materials if above current layer.</summary>
        [SerializeField, UsedImplicitly]
        private float nearAlpha = 0.25f;

        #endregion

        #region Static Fields

        private static string selectedBuild;

        public static string SelectedBuild
        {
            get { return selectedBuild; }
            set
            {
                selectedBuild = value;
                SaveData.Save(SelectedBuildKey, selectedBuild);
            }
        }

        #endregion

        #region Const Fields

        public const int BuildSize = 10;

        private static readonly Vector3 StartCursorPosition = new Vector3(4f, 5f, 4f);

        /// <summary>File name for save data that contains all builds.</summary>
        public const string BuildsFolder = @"Builds/";

        public const string SelectedBuildKey = "Selected Build";

        public static readonly string[] DevBuilds =
        {
            "Avenger",
            "Berserker"
        };

        #endregion

        #region Properties

        /// <summary>Size of each dimension of the grid.</summary>
        public int size { get; private set; }

        /// <summary>Position of the center of the grid.</summary>
        public Vector3 center
        {
            get { return Center.position; }
        }

        /// <summary>Cursor's worldposition based on current viewAxis.</summary>
        public Vector3 layer
        {
            get
            {
                if (viewAxis == Vector3.right || viewAxis == Vector3.left)
                {
                    return new Vector3(cursorWorldPosition.x, center.y, center.z);
                }
                else if (viewAxis == Vector3.up || viewAxis == Vector3.down)
                {
                    return new Vector3(center.x, cursorWorldPosition.y, center.z);
                }
                else if (viewAxis == Vector3.forward || viewAxis == Vector3.back)
                {
                    return new Vector3(center.x, center.y, cursorWorldPosition.z);
                }
                return Vector3.zero;
            }
        }

        /// <summary>Position of the cursor in the grid.</summary>
        public Vector3 cursor { get; private set; }

        /// <summary>Rotation of the cursor.</summary>
        public Quaternion cursorRotation { get; private set; }

        /// <summary>World position of the cell that is occupied by the cursor</summary>
        private Vector3 cursorWorldPosition
        {
            get { return cells[(int)cursor.y][(int)cursor.z][(int)cursor.x].transform.position; }
        }

        /// <summary>Current status of the cursor.</summary>
        public CursorStatuses cursorStatus { get; private set; }

        /// <summary>Current CUBE being held.</summary>
        public CUBE heldCUBE { get; private set; }

        /// <summary>CUBE being hovered over.</summary>
        public CUBE hoveredCUBE
        {
            get { return grid[(int)cursor.y][(int)cursor.z][(int)cursor.x]; }
        }

        /// <summary>CUBEInfo of currently held CUBE.</summary>
        public CUBEInfo heldInfo { get; private set; }

        /// <summary>Info of currently hovered CUBE if there is one.</summary>
        public CUBEInfo HoverInfo
        {
            get { return hoveredCUBE == null ? null : CUBE.AllCUBES[hoveredCUBE.ID]; }
        }

        /// <summary>Weapon slots for the ship.</summary>
        public List<Weapon> weapons { get; private set; }

        /// <summary>Augmentation slots for the ship.</summary>
        public List<Augmentation> augmentations { get; private set; }

        /// <summary>Current stats for the build.</summary>
        public ShipStats CurrentStats { get; private set; }

        /// <summary>Current weapon count of the ship.</summary>
        public int shipWeapons { get; private set; }

        /// <summary>Player's CUBE inverntory.</summary>
        public int[] inventory { get; private set; }

        #endregion

        #region Events

        public EventHandler<CursorUpdatedArgs> StatusChangedEvent;

        #endregion

        #region Public Methods

        /// <summary>
        /// Create and initialize grid, cells, ship, and cursor.
        /// </summary>
        /// <param name="size">Size of the grid's dimensions.</param>
        /// <param name="weaponCount">How many weapons allowed.</param>
        /// <param name="augmentationCount">How many augmentations alloed.</param>
        /// <param name="cubeMaterial">Material to apply to new CUBEs.</param>
        public void CreateGrid(int size, int weaponCount, int augmentationCount, Material cubeMaterial = null)
        {
            // cache material
            if (cubeMaterial != null)
            {
                cubeMat = cubeMaterial;
            }
            else if (cubeMat == null)
            {
                cubeMat = Singleton<GameResources>.Main.VertexColorLerp_Mat;
            }

            // clear any previous data
            Clear();
            if (cells != null)
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        for (int k = 0; k < size; k++)
                        {
                            Destroy(cells[i][j][k]);
                        }
                    }
                }
            }

            // get player inventory
            inventory = CUBE.GetInventory();

            // set start cursor rotation
            cursorRotation = new Quaternion(0, 0, 0, 1);

            // initialize
            this.size = size;
            grid = new CUBE[size][][];
            cells = new GameObject[size][][];
            var cursorPosition = new Vector3(-size / 2f + 0.5f, 0f, -size / 2f + 0.5f);

            // create grid and cells
            for (int i = 0; i < size; i++)
            {
                grid[i] = new CUBE[size][];
                cells[i] = new GameObject[size][];
                cursorPosition.z = -size / 2f + 0.5f;

                for (int j = 0; j < size; j++)
                {
                    grid[i][j] = new CUBE[size];
                    cells[i][j] = new GameObject[size];
                    cursorPosition.x = -size / 2f + 0.5f;

                    for (int k = 0; k < size; k++)
                    {
                        cells[i][j][k] = (GameObject)Instantiate(Cell_Prefab);
                        cells[i][j][k].transform.parent = transform;
                        cells[i][j][k].transform.localPosition = cursorPosition;
                        cursorPosition.x++;
                    }
                    cursorPosition.z++;
                }
                cursorPosition.y++;
            }

            // set current layer and cursor
            cursor = StartCursorPosition;

            // create ship
            if (ship == null)
            {
                ship = new GameObject("Ship");
            }
            ship.transform.position = transform.position + Vector3.up * (size / 2f - 0.5f);
            CurrentStats = new ShipStats();

            // build points
            corePointsMax = BuildStats.GetCoreCapacity();
            corePointsAvailable = corePointsMax;

            // set up weapons
            weaponSlots = weaponCount;
            weapons = new List<Weapon>(weaponCount);
            weapons.Initialize(null, weaponCount);

            // set up augmentations
            augmentationSlots = augmentationCount;
            augmentations = new List<Augmentation>(augmentationCount);
            augmentations.Initialize(null, augmentationCount);

            // create grid center
            Center = ((GameObject)Instantiate(Center_Prefab, ship.transform.position, Quaternion.identity)).transform;
        }


        /// <summary>
        /// Rotate the grid.
        /// </summary>
        /// <param name="plane"></param>
        public void RotateGrid(Vector3 plane)
        {
            viewAxis = plane.Round();
            MoveCursor(Vector3.zero);
        }


        /// <summary>
        /// Rotate the cursor around an axis.
        /// </summary>
        /// <remarks>Only use unit vectors. Usually Y and Z.</remarks>
        /// <param name="axis">Unit vector to rotate around.</param>
        public void RotateCursor(Vector3 axis)
        {
            // quaternion math to get new angle
            cursorRotation = Quaternion.Euler((axis * -90f).Round()) * cursorRotation;

            // rotate and position currently held CUBE
            if (heldCUBE != null)
            {
                PositionCUBE();
            }
        }


        /// <summary>
        /// Move the cursor to another cell.
        /// </summary>
        /// <param name="vector">Vector to move cursor.</param>
        public void MoveCursor(Vector3 vector)
        {
            // get rid of floating points
            vector = vector.Round();

            // reset last cell to open colorIndex
            cells[(int)cursor.y][(int)cursor.z][(int)cursor.x].renderer.material = CellOpen_Mat;
            cells[(int)cursor.y][(int)cursor.z][(int)cursor.x].transform.localScale = Vector3.one * cellSize;

            // contain new position within grid
            if (cursor.x + vector.x < 0 || cursor.x + vector.x > size - 1) vector.x = 0;
            if (cursor.y + vector.y < 0 || cursor.y + vector.y > size - 1) vector.y = 0;
            if (cursor.z + vector.z < 0 || cursor.z + vector.z > size - 1) vector.z = 0;
            // move cursor
            cursor += vector;

            // set new cursor cell to cursor colorIndex
            cells[(int)cursor.y][(int)cursor.z][(int)cursor.x].renderer.material = CellCursor_Mat;
            cells[(int)cursor.y][(int)cursor.z][(int)cursor.x].transform.localScale = Vector3.one * cursorSize;

            // move currently held CUBE
            if (heldCUBE != null)
            {
                heldCUBE.transform.position += vector;
                SetStatus(CursorStatuses.Holding);
            }
                // set status to hover
            else if (grid[(int)cursor.y][(int)cursor.z][(int)cursor.x] != null)
            {
                SetStatus(CursorStatuses.Hover);
            }
                // set status to none
            else
            {
                SetStatus(CursorStatuses.None);
            }

            UpdateGrid();
        }


        /// <summary>
        /// Creates CUBE and added to cursor.
        /// </summary>
        /// <param name="CUBEID">ID of the CUBE to create.</param>
        public void CreateCUBE(int CUBEID, int[] colors = null)
        {
            // destroy currently held CUBE
            DeleteCUBE();

            // create new CUBE
            heldCUBE = (CUBE)Instantiate(GameResources.GetCUBE(CUBEID));
            heldCUBE.name = CUBE.AllCUBES[CUBEID].name;
            // set materials
            int materialCount = heldCUBE.renderer.materials.Length;
            var alphaMats = new Material[materialCount];
            for (int i = 0; i < materialCount; i++)
            {
                alphaMats[i] = new Material(cubeMat);
            }
            heldCUBE.renderer.materials = alphaMats;
            heldCUBE.GetComponent<ColorVertices>().Bake(colors);
            // blink
            StartBlink(heldCUBE.renderer);

            // reset cursorOffset when creating new type of CUBE
            if (heldInfo == null || heldInfo.ID != CUBEID)
            {
                cursorOffset = Vector3.zero;
            }

            // get data
            heldInfo = CUBE.AllCUBES[heldCUBE.ID];

            // set rotation and position
            PositionCUBE();

            // update status
            SetStatus(CursorStatuses.Holding);
        }


        /// <summary>
        /// If there is a CUBE being held, delete it.
        /// </summary>
        public void DeleteCUBE()
        {
            if (heldCUBE == null) return;

            StopBlink(heldCUBE.renderer);
            Destroy(heldCUBE.gameObject);
            heldCUBE = null;
            SetStatus(hoveredCUBE == null ? CursorStatuses.None : CursorStatuses.Hover);
        }


        /// <summary>
        /// Loads build from data and creates it in grid.
        /// </summary>
        /// <param name="build">Name of the build.</param>
        public void CreateBuild(string build)
        {
            // load build's data
            BuildInfo buildInfo = LoadBuild(build);
            if (buildInfo == null) return;

            // clear current values
            Clear();

            // run through build list and create build (this adds stats)
            foreach (var piece in buildInfo.partList)
            {
                // position
                cursor = piece.Value.position;
                // rotation
                cursorRotation = Quaternion.Euler(piece.Value.rotation);
                // create CUBE
                CreateCUBE(piece.Key, piece.Value.colors);
                // place
                PlaceCUBE(piece.Value.weaponMap, piece.Value.augmentationMap);
            }
            cursor = StartCursorPosition;

            UpdateGrid();
        }


        /// <summary>
        /// Swap weapon slots.
        /// </summary>
        /// <param name="index">Weapon slot to move.</param>
        /// <param name="direction">Direction and distance to move.</param>
        public int MoveWeaponMap(int index, int direction)
        {
            if (weapons[index] == null) return index;

            // get new slot index and return if out of bounds
            int newSlot = index + direction;
            if (newSlot >= weapons.Count || newSlot < 0) return index;

            // cache weapon
            Weapon saved = weapons[index];
            // update weapon's weaponmap
            currentBuild[saved.GetComponent<CUBE>()].weaponMap += direction;
            // swap
            weapons[index] = weapons[newSlot];
            if (weapons[index] != null)
            {
                currentBuild[weapons[index].GetComponent<CUBE>()].weaponMap -= direction;
            }
            weapons[newSlot] = saved;

            return newSlot;
        }


        //
        public void Paint(int pieceSelected, int colorIndex)
        {
            hoveredCUBE.GetComponent<ColorVertices>().SetandBake(pieceSelected, colorIndex);
            currentBuild[hoveredCUBE].colors[pieceSelected] = colorIndex;
        }


        public void Build(string build, int buildSize, Vector3 startPosition, Vector3 startRotation, float maxTime, Action<BuildFinishedArgs> finshedAction)
        {
            var showShip = (GameObject)Instantiate(Singleton<GameResources>.Main.player_Prefab);
            showShip.name = "Player";
            showShip.transform.SetPosRot(startPosition, Quaternion.Euler(startRotation));
            StartCoroutine(ShowBuild.Join(LoadBuild(build), buildSize, showShip.transform, maxTime, finshedAction));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Clear all data.
        /// </summary>
        private void Clear()
        {
            if (grid == null) return;

            // held CUBE
            if (heldCUBE)
            {
                StopBlink(heldCUBE.renderer);
                Destroy(heldCUBE.gameObject);
                heldCUBE = null;
            }

            // create grid and set all cells to null
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int k = 0; k < size; k++)
                    {
                        grid[i][j][k] = null;
                    }
                }
            }

            // destroy any placed CUBEs
            foreach (var cube in currentBuild)
            {
                Destroy(cube.Key.gameObject);
            }
            currentBuild.Clear();

            weapons.Initialize(null, weaponSlots);
            augmentations.Initialize(null, augmentationSlots);
        }


        private void PositionCUBE()
        {
            Vector3 pivotOffset = RotateVector(new Vector3(-0.5f, -0.5f, -0.5f));
            heldCUBE.transform.rotation = cursorRotation;
            Debugger.Log("cursorWorldPosition: " + cursorWorldPosition, this, Debugger.LogTypes.Construction);
            Debugger.Log("RotateVector(cursorOffset).Round(): " + RotateVector(cursorOffset), this, Debugger.LogTypes.Construction);
            Debugger.Log("pivotOffset: " + pivotOffset, this, Debugger.LogTypes.Construction);
            heldCUBE.transform.position = cursorWorldPosition + RotateVector(cursorOffset).Round() + pivotOffset;
        }


        /// <summary>
        /// Checks to see if the currently held CUBE fits in the grid and is not in other CUBEs.
        /// </summary>
        /// <returns></returns>
        private bool Fits()
        {
            if (heldCUBE == null) return false;

            // test pivotOffset against grid bounds and other placed CUBEs
            Vector3 pivot = cursor + RotateVector(cursorOffset).Round();
            pivot = pivot.Round();
            // test CUBE bounds against grid bounds
            Vector3 bounds = heldInfo.size;
            for (int x = 0; x < bounds.x; x++)
            {
                for (int y = 0; y < bounds.y; y++)
                {
                    for (int z = 0; z < bounds.z; z++)
                    {
                        Vector3 point = pivot + RotateVector(new Vector3(x, y, z)).Round();
                        point = point.Round();
                        if (point.x < 0 || point.x > size - 1)
                        {
                            Debugger.Log("Doesn't Fit Bounds: " + point, this, Debugger.LogTypes.Construction);
                            return false;
                        }
                        if (point.y < 0 || point.y > size - 1)
                        {
                            Debugger.Log("Doesn't Fit Bounds: " + point, this, Debugger.LogTypes.Construction);
                            return false;
                        }
                        if (point.z < 0 || point.z > size - 1)
                        {
                            Debugger.Log("Doesn't Fit Bounds: " + point, this, Debugger.LogTypes.Construction);
                            return false;
                        }
                        if (grid[(int)point.y][(int)point.z][(int)point.x])
                        {
                            Debugger.Log("Doesn't Fit Taken: " + point, this, Debugger.LogTypes.Construction);
                            return false;
                        }
                        Debugger.Log("Fits: " + point, this, Debugger.LogTypes.Construction);
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Rotates a CUBE piece by the cursorRotation.
        /// </summary>
        /// <param name="localVector">Piece.</param>
        /// <param name="reverse">Should the vector be inverted?</param>
        /// <returns>New piece position.</returns>
        private Vector3 RotateVector(Vector3 localVector, bool reverse = false)
        {
            var rot = new Matrix4x4();
            rot.SetTRS(Vector3.zero, reverse ? Quaternion.Inverse(cursorRotation) : cursorRotation, Vector3.one);
            return rot.MultiplyVector(localVector);
        }


        /// <summary>
        /// Pick up a CUBE from the grid.
        /// </summary>
        /// <param name="cube">CUBE to pick up.</param>
        private void PickupCUBE(CUBE cube)
        {
            // grab CUBE
            heldCUBE = cube;
            heldInfo = CUBE.AllCUBES[heldCUBE.ID];
            StartBlink(heldCUBE.renderer);
            // stock inventory
            inventory[heldCUBE.ID]++;
            // remove CUBE from build
            RemoveCUBE(cube);
            // set status
            SetStatus(CursorStatuses.Holding);
        }


        public void PickupCUBE()
        {
            PickupCUBE(hoveredCUBE);
        }


        /// <summary>
        /// Place held CUBE into the build.
        /// </summary>
        /// <param name="weaponIndex">Index of the weapon.</param>
        /// <param name="augmentationIndex">Index of the augmentation.</param>
        /// <returns>True, if the CUBE was successfully placed.</returns>
        private bool PlaceCUBE(int weaponIndex = -1, int augmentationIndex = -1)
        {
            if (heldCUBE == null) return false;
            if (!Fits()) return false;

            // build points
            if (heldInfo.cost <= corePointsAvailable)
            {
                corePointsAvailable -= heldInfo.cost;
            }
            else
            {
                return false;
            }

            // update inventory
            if (inventory[heldCUBE.ID] > 0)
            {
                inventory[heldCUBE.ID]--;
            }
            else
            {
                return false;
            }

            // add to weapons if applicable
            if (heldInfo.type == CUBE.Types.Weapon)
            {
                if (weaponIndex == -1)
                {
                    // is there an open slot?
                    for (int i = 0; i < weapons.Count; i++)
                    {
                        if (weapons[i] != null) continue;
                        weapons[i] = (Weapon)heldCUBE.GetComponent(typeof(Weapon));
                        weaponIndex = i;
                        break;
                    }

                    // add as extra
                    if (weaponIndex == -1)
                    {
                        weapons.Add((Weapon)heldCUBE.GetComponent(typeof(Weapon)));
                        weaponIndex = weapons.Count - 1;
                    }
                }
                else
                {
                    if (weaponIndex >= weapons.Count)
                    {
                        weapons.Add((Weapon)heldCUBE.GetComponent(typeof(Weapon)));
                    }
                    else
                    {
                        weapons[weaponIndex] = (Weapon)heldCUBE.GetComponent(typeof(Weapon));
                    }
                }
            }

            // add to augmentations if applicable
            if (heldInfo.type == CUBE.Types.Augmentation)
            {
                if (augmentationIndex == -1)
                {
                    // is there an open slot?
                    for (int i = 0; i < augmentations.Count; i++)
                    {
                        if (augmentations[i] != null) continue;

                        augmentations[i] = (Augmentation)heldCUBE.GetComponent(typeof(Augmentation));
                        augmentationIndex = i;
                        break;
                    }

                    // add as extra
                    if (augmentationIndex == -1)
                    {
                        augmentations.Add((Augmentation)heldCUBE.GetComponent(typeof(Augmentation)));
                        augmentationIndex = augmentations.Count - 1;
                    }
                }
                else
                {
                    if (augmentationIndex >= augmentations.Count)
                    {
                        augmentations.Add((Augmentation)heldCUBE.GetComponent(typeof(Augmentation)));
                    }
                    else
                    {
                        augmentations[augmentationIndex] = (Augmentation)heldCUBE.GetComponent(typeof(Augmentation));
                    }
                }
            }

            Vector3 pivot = cursor + RotateVector(cursorOffset).Round();

            // add to build
            currentBuild.Add(heldCUBE, new CUBEGridInfo(pivot, cursorRotation.eulerAngles, weaponIndex, augmentationIndex, ((ColorVertices)heldCUBE.GetComponent(typeof(ColorVertices))).colors));

            // add all of the CUBE's bounds to the grid
            Vector3 bounds = heldInfo.size;
            for (int x = 0; x < bounds.x; x++)
            {
                for (int y = 0; y < bounds.y; y++)
                {
                    for (int z = 0; z < bounds.z; z++)
                    {
                        Vector3 point = pivot + RotateVector(new Vector3(x, y, z)).Round();
                        point = point.Round();
                        Debugger.Log("Placing: " + point, this, Debugger.LogTypes.Construction);
                        grid[(int)point.y][(int)point.z][(int)point.x] = heldCUBE;
                    }
                }
            }

            // add CUBE's stats
            CurrentStats.health += heldInfo.health;
            CurrentStats.shield += heldInfo.shield;
            CurrentStats.speed += heldInfo.speed;
            CurrentStats.damage += heldInfo.damage;

            // set parent to ship
            heldCUBE.transform.parent = ship.transform;
            // empty cursor
            StopBlink(heldCUBE.renderer);

            heldCUBE = null;
            // reset
            SetStatus(CursorStatuses.Hover);

            return true;
        }


        /// <summary>
        /// Place held CUBE into the build and create another if applicable.
        /// </summary>
        /// <param name="loadAnother">Should another of the same CUBE be created after the current one is placed?</param>
        /// <param name="weaponIndex">Index of the weapon.</param>
        /// <param name="augmentationIndex">Index of the augmentation.</param>
        /// <returns>True, if the CUBE was successfully placed.</returns>
        public void PlaceCUBE(bool loadAnother, int weaponIndex = -1, int augmentationIndex = -1, int[] colors = null)
        {
            if (heldCUBE == null) return;

            int id = heldCUBE.ID;
            if (PlaceCUBE(weaponIndex, augmentationIndex) && loadAnother)
            {
                CreateCUBE(id, colors);
            }
        }


        /// <summary>
        /// Remove CUBE from grid and all stats.
        /// </summary>
        /// <param name="cube"></param>
        private void RemoveCUBE(CUBE cube)
        {
            // get pivotOffset and rotation
            cursorRotation = Quaternion.Euler(currentBuild[cube].rotation);
            cursorOffset = currentBuild[cube].position - cursor;
            Vector3 pivot = cursor + cursorOffset.Round();
            pivot = pivot.Round();
            cursorOffset = RotateVector(cursorOffset.Round(), true).Round();

            // remove all of CUBE's pieces
            Vector3 bounds = heldInfo.size;
            for (int x = 0; x < bounds.x; x++)
            {
                for (int y = 0; y < bounds.y; y++)
                {
                    for (int z = 0; z < bounds.z; z++)
                    {
                        Vector3 point = pivot + RotateVector(new Vector3(x, y, z)).Round();
                        point = point.Round();
                        grid[(int)point.y][(int)point.z][(int)point.x] = null;
                        cells[(int)point.y][(int)point.z][(int)point.x].SetActive(true);
                        cells[(int)point.y][(int)point.z][(int)point.x].renderer.material = CellOpen_Mat;
                    }
                }
            }

            // remove weapon if applicable
            if (heldInfo.type == CUBE.Types.Weapon && weapons.Contains(((Weapon)heldCUBE.GetComponent(typeof(Weapon)))))
            {
                int index = currentBuild[heldCUBE].weaponMap;
                if (index < weaponSlots)
                {
                    // replace with extra
                    if (weapons.Count > weaponSlots)
                    {
                        weapons[index] = weapons[weaponSlots];
                        weapons.RemoveAt(weaponSlots);
                    }
                    else
                    {
                        weapons[index] = null;
                    }
                }
                else
                {
                    weapons.RemoveAt(index);
                }
            }

            // remove augmentation if applicable
            if (heldInfo.type == CUBE.Types.Augmentation && augmentations.Contains((Augmentation)heldCUBE.GetComponent(typeof(Augmentation))))
            {
                int index = currentBuild[heldCUBE].augmentationMap;
                if (index < augmentationSlots)
                {
                    // replace with extra
                    if (augmentations.Count > augmentationSlots)
                    {
                        augmentations[index] = augmentations[augmentationSlots];
                        augmentations.RemoveAt(augmentationSlots);
                    }
                    else
                    {
                        augmentations[index] = null;
                    }
                }
                else
                {
                    augmentations.RemoveAt(index);
                }
            }

            // remove CUBE from current build
            currentBuild.Remove(cube);

            // remove stats
            CUBEInfo cubeInfo = CUBE.AllCUBES[cube.ID];
            CurrentStats.health -= cubeInfo.health;
            CurrentStats.shield -= cubeInfo.shield;
            CurrentStats.speed -= cubeInfo.speed;
            CurrentStats.damage -= cubeInfo.damage;
            corePointsAvailable += cubeInfo.cost;

            cells[(int)cursor.y][(int)cursor.z][(int)cursor.x].renderer.material = CellCursor_Mat;
        }


        /// <summary>
        /// Get build data from data.
        /// </summary>
        /// <param name="buildName">Name of the ship.</param>
        /// <returns>BuildInfo for the ship.</returns>
        public static BuildInfo LoadBuild(string buildName)
        {
            // get buildInfo string from data
            BuildInfo build = SaveData.Load<BuildInfo>(buildName, BuildsFolder);
            if (build == null)
            {
                Debugger.Log(buildName + " is not in data.", null, Debugger.LogTypes.Data, false);
                return null;
            }
            Debugger.Log("Loading: " + build, null, Debugger.LogTypes.Data);

            return build;
        }


        /// <summary>
        /// Save current build to data.
        /// </summary>
        public void SaveBuild()
        {
            SaveBuild(buildName, new BuildInfo(buildName, CurrentStats, currentBuild));
        }


        /// <summary>
        /// Update all cell's visibility and CUBE's alpha.
        /// </summary>
        private void UpdateGrid()
        {
            Vector3 cursorLayer = new Vector3(viewAxis.x * cursor.x, viewAxis.y * cursor.y, viewAxis.z * cursor.z).Round();

            // update cells
            for (int y = 0; y < grid.Length; y++)
            {
                for (int z = 0; z < grid.Length; z++)
                {
                    for (int x = 0; x < grid.Length; x++)
                    {
                        var index = new Vector3(x, y, z);
                        Vector3 currentLayer = new Vector3(viewAxis.x * index.x, viewAxis.y * index.y, viewAxis.z * index.z).Round();
                        if (currentLayer == cursorLayer && (grid[y][z][x] == null || cursor == index))
                        {
                            cells[y][z][x].SetActive(true);
                        }
                        else
                        {
                            cells[y][z][x].SetActive(false);
                        }
                    }
                }
            }

            float direction = (viewAxis == Vector3.right || viewAxis == Vector3.up || viewAxis == Vector3.forward ? 1f : -1f);
            Vector3 viewAxisNorm = viewAxis.normalized;

            // update CUBEs
            foreach (var cube in currentBuild)
            {
                Vector3 cPosition = new Vector3(cube.Value.position.x * viewAxis.x, cube.Value.position.y * viewAxis.y, cube.Value.position.z * viewAxis.z).Round();
                Vector3 difference = cPosition - cursorLayer;

                if (difference.normalized * direction == viewAxisNorm)
                {
                    foreach (Material mat in cube.Key.renderer.materials)
                    {
                        mat.SetFloat("_Alpha", nearAlpha);
                    }
                }
                else
                {
                    foreach (Material mat in cube.Key.renderer.materials) mat.SetFloat("_Alpha", 1f);
                }
            }
        }


        private void SetStatus(CursorStatuses nextStatus)
        {
            StatusChangedEvent.Fire(this, new CursorUpdatedArgs(cursorStatus, nextStatus));
            cursorStatus = nextStatus;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Get all build names.
        /// </summary>
        /// <returns>All build names.</returns>
        public static List<string> BuildNames()
        {
            return SaveData.GetFiles(BuildsFolder).ToList();
        }


        /// <summary>
        /// Save build to data.
        /// </summary>
        /// <param name="buildName">Name of the build.</param>
        /// <param name="build">Build info.</param>
        public static void SaveBuild(string buildName, BuildInfo build)
        {
            Debugger.Log("Saving " + buildName + ": " + build, null, Debugger.LogTypes.Data);
            SaveData.Save(buildName, build, BuildsFolder);
        }


        /// <summary>
        /// Delete build from data.
        /// </summary>
        /// <param name="buildName">Name of build.</param>
        public static void DeleteBuild(string buildName)
        {
            SaveData.Delete(buildName, BuildsFolder);
        }


        /// <summary>
        /// Rename a build in data.
        /// </summary>
        /// <param name="oldName">Current name of the build.</param>
        /// <param name="newName">New name of the build.</param>
        public static void RenameBuild(string oldName, string newName)
        {
            // update build data
            string buildInfo = SaveData.Load<string>(oldName, BuildsFolder);
            SaveData.Delete(oldName);
            SaveData.Save(newName, buildInfo, BuildsFolder);
        }


        public static void Load()
        {
            selectedBuild = SaveData.Load(SelectedBuildKey, SaveData.DefaultPath, DevBuilds[0]);
        }

        #endregion

        #region Blink Methods

        public void StartBlink(Renderer target)
        {
            foreach (Material material in target.sharedMaterials)
            {
                material.color = blinkColor;
            }
            StartCoroutine("Blinking", target);
        }


        [UsedImplicitly]
        private IEnumerator Blinking(Renderer target)
        {
            float timer = 0f;
            while (timer < 1f)
            {
                timer += Time.deltaTime / blinkTime;

                foreach (Material material in target.sharedMaterials)
                {
                    material.SetFloat("_Mix", timer);
                }

                yield return null;
            }

            timer = 0f;
            while (timer < 1f)
            {
                timer += Time.deltaTime / blinkTime;

                foreach (Material material in target.sharedMaterials)
                {
                    material.SetFloat("_Mix", 1 - timer);
                }

                yield return null;
            }

            StartCoroutine("Blinking", target);
        }


        public void StopBlink(Renderer target)
        {
            StopCoroutine("Blinking");

            foreach (Material material in target.sharedMaterials)
            {
                material.SetFloat("_Mix", 0f);
            }
        }

        #endregion
    }

    #region Classes

    public class CursorUpdatedArgs : EventArgs
    {
        public readonly ConstructionGrid.CursorStatuses previous;
        public readonly ConstructionGrid.CursorStatuses current;


        public CursorUpdatedArgs(ConstructionGrid.CursorStatuses previous, ConstructionGrid.CursorStatuses current)
        {
            this.previous = previous;
            this.current = current;
        }
    }

    #endregion
}