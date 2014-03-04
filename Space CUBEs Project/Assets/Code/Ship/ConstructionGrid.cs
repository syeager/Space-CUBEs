// Steve Yeager
// 11.26.2013

using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections;
using System.Text;

public class ConstructionGrid : MonoBase
{
    #region Public Fields

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

    /// <summary>Name of the build. Set in a textfield by player.</summary>
    public string buildName;

    #endregion

    #region Private Fields

    /// <summary>Matrix with cells that hold references to the CUBE that currently occupies them.</summary>
    private CUBE[][][] grid;
    /// <summary>Matrix that holds references to all of the cell gameObjects.</summary>
    private GameObject[][][] cells;
    /// <summary>Center of the grid.</summary>
    private Transform Center;

    /// <summary>All of the CUBEs currently placed.</summary>
    private Dictionary<CUBE, CUBEGridInfo> currentBuild = new Dictionary<CUBE, CUBEGridInfo>();
    /// <summary>Parent of all of the CUBEs.</summary>
    private GameObject ship;

    /// <summary>Offset from cursor to held CUBE's pivotOffset.</summary>
    private Vector3 cursorOffset;
    /// <summary>Current viewAxis being looked through. Affects grid rotation.</summary>
    private Vector3 viewAxis;

    #endregion

    #region Const Fields

    /// <summary>Level of alpha to set CUBE materials if above current layer.</summary>
    private const float NEARALPHA = 0.5f;

    /// <summary>Prefix for path to user created ships in PlayerPrefs.</summary>
    private const string USERBUILDSPATH = "UserBuild: ";
    /// <summary>Path to all user build names.</summary>
    private const string ALLUSERBUILDSPATH = "AllUserBuilds";
    /// <summary>Prefix for path to dev created ships in PlayerPrefs.</summary>
    private const string DEVBUILDSPATH = "DevBuild: ";
    /// <summary>Path to all dev build names.</summary>
    private const string ALLDEVBUILDSPATH = "AllDevBuilds";
    /// <summary>Path to the folder with the enemy combined meshes.</summary>
    private const string ENEMYMESHPATH = "Assets/Ship/Characters/";
    /// <summary>Path to the folder with the enemy prefabs.</summary>
    private const string ENEMYPREFABPATH = "Assets/Ship/Characters/";

    #endregion

    #region Properties

    /// <summary>Size of each dimension of the grid.</summary>
    public int size { get; private set; }
    /// <summary>Position of the center of the grid.</summary>
    public Vector3 center { get { return Center.position; } }
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
        get
        {
            return cells[(int)cursor.y][(int)cursor.z][(int)cursor.x].transform.position;
        }
    }
    /// <summary>Current status of the cursor.</summary>
    public CursorStatuses cursorStatus { get; private set; }
    /// <summary>Current CUBE being held.</summary>
    public CUBE heldCUBE { get; private set; }
    /// <summary>CUBE being hovered over.</summary>
    public CUBE hoveredCUBE
    {
        get
        {
            return grid[(int)cursor.y][(int)cursor.z][(int)cursor.x];
        }
    }
    /// <summary>CUBEInfo of currently held CUBE.</summary>
    public CUBEInfo heldInfo { get; private set; }
    /// <summary>Weapon slots for the ship.</summary>
    public Weapon[] weapons { get; private set; }
    /// <summary>Returns the next free weapon slot. -1 if all are taken.</summary>
    private int nextWeaponSlot
    {
        get
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }
    }

    /// <summary>Current health of the ship.</summary>
    public float shipHealth { get; private set; }
    /// <summary>Current shield of the ship.</summary>
    public float shipShield { get; private set; }
    /// <summary>Current speed of the ship.</summary>
    public float shipSpeed { get; private set; }
    /// <summary>Current damage of the ship.</summary>
    public float shipDamage { get; private set; }
    /// <summary>Current weapon count of the ship.</summary>
    public int shipWeapons { get; private set; }

    /// <summary>Player's CUBE inverntory.</summary>
    public int[] inventory { get; private set; }

    #endregion

    #region Events

    public EventHandler<BuildFinishedArgs> BuildFinishedEvent;

    #endregion


    #region MonoBehaviour Overrides

    private void Update()
    {
        if (grid != null)
        {
            Vector3 cursorLayer = new Vector3(viewAxis.x * cursor.x, viewAxis.y * cursor.y, viewAxis.z * cursor.z).Round();

            for (int y = 0; y < grid.Length; y++)
            {
                for (int z = 0; z < grid.Length; z++)
                {
                    for (int x = 0; x < grid.Length; x++)
                    {
                        Vector3 c = new Vector3(x, y, z);
                        Vector3 cLayer = new Vector3(viewAxis.x * c.x, viewAxis.y * c.y, viewAxis.z * c.z).Round();
                        if (cLayer == cursorLayer && (grid[y][z][x] == null || cursor == c))
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
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Create and initialize grid, cells, ship, and cursor.
    /// </summary>
    /// <param name="size">Size of the grid's dimensions.</param>
    public void CreateGrid(int size)
    {
        // get player inventory
        inventory = CUBE.GetInventory();

        // set start cursor rotation
        cursorRotation = new Quaternion(0, 0, 0, 1);

        // initialize
        this.size = size;
        grid = new CUBE[size][][];
        cells = new GameObject[size][][];
        Vector3 cursor = new Vector3(-size / 2f + 0.5f, 0f, -size / 2f + 0.5f);

        // create grid and cells
        for (int i = 0; i < size; i++)
        {
            grid[i] = new CUBE[size][];
            cells[i] = new GameObject[size][];
            cursor.z = -size / 2f + 0.5f;

            for (int j = 0; j < size; j++)
            {
                grid[i][j] = new CUBE[size];
                cells[i][j] = new GameObject[size];
                cursor.x = -size / 2f + 0.5f;

                for (int k = 0; k < size; k++)
                {
                    cells[i][j][k] = (GameObject)GameObject.Instantiate(Cell_Prefab);
                    cells[i][j][k].transform.parent = transform;
                    cells[i][j][k].transform.localPosition = cursor;
                    cursor.x++;
                }
                cursor.z++;
            }
            cursor.y++;
        }

        // set current layer and cursor
        this.cursor = new Vector3(4, 5, 4);
        ChangeLayer(0);

        // create ship
        if (ship == null)
        {
            ship = new GameObject("Ship");
        }
        ship.transform.position = transform.position + Vector3.up * (size / 2f - 0.5f);

        // set up weapons
        weapons = new Weapon[Player.WEAPONLIMIT];

        // create grid center
        Center = (Instantiate(Center_Prefab, ship.transform.position, Quaternion.identity) as GameObject).transform;
    }


    /// <summary>
    /// Rotate the grid.
    /// </summary>
    /// <param name="viewAxis"></param>
    public void RotateGrid(Vector3 plane)
    {
        this.viewAxis = plane.Round();
        ChangeLayer(0);
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

        // contain new position within grid
        if (cursor.x + vector.x < 0 || cursor.x + vector.x > size - 1) vector.x = 0;
        if (cursor.y + vector.y < 0 || cursor.y + vector.y > size - 1) vector.y = 0;
        if (cursor.z + vector.z < 0 || cursor.z + vector.z > size - 1) vector.z = 0;
        // move cursor
        cursor += vector;

        // set new cursor cell to cursor colorIndex
        cells[(int)cursor.y][(int)cursor.z][(int)cursor.x].renderer.material = CellCursor_Mat;

        // move currently held CUBE
        if (heldCUBE != null)
        {
            heldCUBE.transform.position += vector;
            cursorStatus = CursorStatuses.Holding;
        }
        // set status to hover
        else if (grid[(int)cursor.y][(int)cursor.z][(int)cursor.x] != null)
        {
            cursorStatus = CursorStatuses.Hover;
        }
        // set status to none
        else
        {
            cursorStatus = CursorStatuses.None;
        }
    }


    /// <summary>
    /// Move the cursor up or down.
    /// </summary>
    /// <param name="amount">Direction and distance to move.</param>
    [Obsolete("Use new method of calculating layer.")]
    public void ChangeLayer(int amount)
    {
        // turn all cells off
        //for (int i = 0; i < size; i++)
        //{
        //    for (int j = 0; j < size; j++)
        //    {
        //        for (int k = 0; k < size; k++)
        //        {
        //            cells[i][j][k].SetActive(false);
        //        }
        //    }
        //}

        // up
        if (viewAxis == Vector3.up || viewAxis == Vector3.down)
        {
            int y = (int)(cursor + viewAxis * amount).y;
            int newLayer = Mathf.Clamp(y, 0, size - 1);

            // toggle cell colors in the previous and new layers
            //for (int i = 0; i < size; i++)
            //{
            //    for (int j = 0; j < size; j++)
            //    {
            //        cells[newLayer][i][j].SetActive(true);
            //    }
            //}

            // toggle CUBE colors; alpha for CUBEs in a higher layer than the new layer; full opacity for CUBEs on the same layer
            if (viewAxis == Vector3.up)
            {
                foreach (var cube in currentBuild)
                {
                    if (cube.Value.position.y > newLayer) foreach (var mat in cube.Key.renderer.materials) mat.SetFloat("_Alpha", NEARALPHA);
                    else foreach (var mat in cube.Key.renderer.materials) mat.SetFloat("_Alpha", 1f);
                }
            }
            else
            {
                foreach (var cube in currentBuild)
                {
                    if (cube.Value.position.y < newLayer) foreach (var mat in cube.Key.renderer.materials) mat.SetFloat("_Alpha", NEARALPHA);
                    else foreach (var mat in cube.Key.renderer.materials) mat.SetFloat("_Alpha", 1f);
                }
            }
        }
        // right
        else if (viewAxis == Vector3.right || viewAxis == Vector3.left)
        {
            int x = (int)(cursor + viewAxis * amount).x;
            int newLayer = Mathf.Clamp(x, 0, size - 1);

            // toggle cell colors in the previous and new layers
            //for (int i = 0; i < size; i++)
            //{
            //    for (int j = 0; j < size; j++)
            //    {
            //        cells[i][j][newLayer].SetActive(true);
            //    }
            //}

            // toggle CUBE colors; alpha for CUBEs in a higher layer than the new layer; full opacity for CUBEs on the same layer
            if (viewAxis == Vector3.right)
            {
                foreach (var cube in currentBuild)
                {
                    if (cube.Value.position.x > newLayer) foreach (var mat in cube.Key.renderer.materials) mat.SetFloat("_Alpha", NEARALPHA);
                    else foreach (var mat in cube.Key.renderer.materials) mat.SetFloat("_Alpha", 1f);
                }
            }
            else
            {
                foreach (var cube in currentBuild)
                {
                    if (cube.Value.position.x < newLayer) foreach (var mat in cube.Key.renderer.materials) mat.SetFloat("_Alpha", NEARALPHA);
                    else foreach (var mat in cube.Key.renderer.materials) mat.SetFloat("_Alpha", 1f);
                }
            }
        }
        // forward
        else if (viewAxis == Vector3.forward || viewAxis == Vector3.back)
        {
            int z = (int)(cursor + viewAxis * amount).z;
            int newLayer = Mathf.Clamp(z, 0, size - 1);

            // toggle cell colors in the previous and new layers
            //for (int i = 0; i < size; i++)
            //{
            //    for (int j = 0; j < size; j++)
            //    {
            //        cells[i][newLayer][j].SetActive(true);
            //    }
            //}

            // toggle CUBE colors; alpha for CUBEs in a higher layer than the new layer; full opacity for CUBEs on the same layer
            if (viewAxis == Vector3.forward)
            {
                foreach (var cube in currentBuild)
                {
                    if (cube.Value.position.z > newLayer) foreach (var mat in cube.Key.renderer.materials) mat.SetFloat("_Alpha", NEARALPHA);
                    else foreach (var mat in cube.Key.renderer.materials) mat.SetFloat("_Alpha", 1f);
                }
            }
            else
            {
                foreach (var cube in currentBuild)
                {
                    if (cube.Value.position.z < newLayer) foreach (var mat in cube.Key.renderer.materials) mat.SetFloat("_Alpha", NEARALPHA);
                    else foreach (var mat in cube.Key.renderer.materials) mat.SetFloat("_Alpha", 1f);
                }
            }
        }

        // move the cursor
        MoveCursor(viewAxis * amount);
    }


    /// <summary>
    /// Creates CUBE and added to cursor.
    /// </summary>
    /// <param name="CUBEID">ID of the CUBE to create.</param>
    public void CreateCUBE(int CUBEID)
    {
        // destroy currently held CUBE
        if (heldCUBE != null)
        {
            Destroy(heldCUBE.gameObject);
        }

        // create new CUBE
        heldCUBE = (CUBE)GameObject.Instantiate(GameResources.GetCUBE(CUBEID));
        // set materials
        int materialCount = heldCUBE.renderer.materials.Length;
        Material[] alphaMats = new Material[materialCount];
        for (int i = 0; i < materialCount; i++)
        {
            alphaMats[i] = GameResources.Main.VertexOverlay_Mat;
        }
        heldCUBE.renderer.sharedMaterials = alphaMats;
        heldCUBE.GetComponent<ColorVertices>().Bake();
        
        // reset cursorOffset when creating new type of CUBE
        if (heldInfo.ID != CUBEID)
        {
            cursorOffset = Vector3.zero;
        }

        // get data
        heldInfo = CUBE.allCUBES[heldCUBE.ID];

        // set rotation and position
        PositionCUBE(); 

        // update status
        cursorStatus = CursorStatuses.Holding;
    }


    /// <summary>
    /// If cursor is empty, pick up CUBE if hovering. If cursor is not empty, place CUBE.
    /// </summary>
    /// <param name="loadAnother">Should another CUBE be loaded of the same type that was placed?</param>
    public void CursorAction(bool loadAnother)
    {
        // place currenlty held CUBE
        if (heldCUBE != null)
        {
            PlaceCUBE(loadAnother);
        }
        // pick up CUBE if possible
        else if (grid[(int)cursor.y][(int)cursor.z][(int)cursor.x] != null)
        {
            PickupCUBE(grid[(int)cursor.y][(int)cursor.z][(int)cursor.x]);
        }
    }


    /// <summary>
    /// If there is a CUBE being held, delete it.
    /// </summary>
    public void DeleteCUBE()
    {
        if (heldCUBE == null) return;

        Destroy(heldCUBE.gameObject);
        heldCUBE = null;
        cursorStatus = hoveredCUBE == null ? CursorStatuses.None : CursorStatuses.Hover;
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
        Vector3 position = cursor;
        foreach (var piece in buildInfo.partList)
        {
            // position
            cursor = piece.Value.position;
            // rotation
            cursorRotation = Quaternion.Euler(piece.Value.rotation);
            // create CUBE
            CreateCUBE(piece.Key);
            // colors
            heldCUBE.GetComponent<ColorVertices>().Bake(piece.Value.colors);
            // place
            PlaceCUBE(piece.Value.weaponMap);
        }
        cursor = position;
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
        if (newSlot >= weapons.Length || newSlot < 0) return index;

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


    [Obsolete("Move to a new class.")]
    public IEnumerator Build(string build, int buildSize, Vector3 startPosition, Vector3 startRotation, float maxTime)
    {
        var buildInfo = LoadBuild(build);
        if (buildInfo == null)
        {
            if (BuildFinishedEvent != null)
            {
                BuildFinishedEvent(this, new BuildFinishedArgs(null, 0f, 0f, 0f, 0f));
            }
            yield break;
        }

        List<BuildCUBE> pieces = new List<BuildCUBE>();
        GameObject finishedShip = new GameObject("Player");

        const float minDist = 100f;
        const float maxDist = 250f;
        Vector3 halfGrid = Vector3.one * (buildSize / 2f - 1f);
        Vector3 pivotOffset = new Vector3(-0.5f, -0.5f, -0.5f);
        finishedShip.transform.position = startPosition;
        finishedShip.transform.eulerAngles = startRotation;
        float speed = maxDist / maxTime;

        foreach (var piece in buildInfo.partList)
        {
            var cube = (CUBE)GameObject.Instantiate(GameResources.GetCUBE(piece.Key));
            cube.transform.parent = finishedShip.transform;

            cube.transform.localPosition = piece.Value.position.normalized * UnityEngine.Random.Range(minDist, maxDist);
            cube.transform.localPosition = Quaternion.Euler(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)) * cube.transform.localPosition;
            cube.transform.localEulerAngles = piece.Value.rotation;

            if (piece.Value.weaponMap != -1)
            {
                cube.GetComponent<Weapon>().index = piece.Value.weaponMap;
            }

            cube.GetComponent<ColorVertices>().Bake(piece.Value.colors);

            pieces.Add(new BuildCUBE(cube.transform, piece.Value.position - halfGrid + Utility.RotateVector(pivotOffset, Quaternion.Euler(piece.Value.rotation)), speed));
        }

        float time = maxTime;
        while (time > 0f)
        {
            foreach (var piece in pieces)
            {
                piece.Update(deltaTime);
            }
            time -= deltaTime;
            yield return null;
        }

        if (BuildFinishedEvent != null)
        {
            BuildFinishedEvent(this, new BuildFinishedArgs(finishedShip, buildInfo.health, buildInfo.shield, buildInfo.speed, buildInfo.damage));
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Clear all data.
    /// </summary>
    private void Clear()
    {
        if (grid == null) return;

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

        // new weapons array according to player.WEAPONLIMIT
        weapons = new Weapon[Player.WEAPONLIMIT];
    }


    private void PositionCUBE()
    {
        Vector3 pivotOffset = RotateVector(new Vector3(-0.5f, -0.5f, -0.5f));
        heldCUBE.transform.rotation = cursorRotation;
        Debugger.Log("cursorWorldPosition: " + cursorWorldPosition, this, true, Debugger.LogTypes.Construction);
        Debugger.Log("RotateVector(cursorOffset).Round(): " + RotateVector(cursorOffset), this, true, Debugger.LogTypes.Construction);
        Debugger.Log("pivotOffset: " + pivotOffset, this, true, Debugger.LogTypes.Construction);
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
                        Debugger.Log("Doesn't Fit Bounds: " + point, this, true, Debugger.LogTypes.Construction);
                        return false;
                    }
                    if (point.y < 0 || point.y > size - 1)
                    {
                        Debugger.Log("Doesn't Fit Bounds: " + point, this, true, Debugger.LogTypes.Construction);
                        return false;
                    }
                    if (point.z < 0 || point.z > size - 1)
                    {
                        Debugger.Log("Doesn't Fit Bounds: " + point, this, true, Debugger.LogTypes.Construction);
                        return false;
                    }
                    if (grid[(int)point.y][(int)point.z][(int)point.x])
                    {
                        Debugger.Log("Doesn't Fit Taken: " + point, this, true, Debugger.LogTypes.Construction);
                        return false;
                    }
                    Debugger.Log("Fits: " + point, this, true, Debugger.LogTypes.Construction);
                }
            }
        }

        return true;
    }


    /// <summary>
    /// Rotates a CUBE piece by the cursorRotation.
    /// </summary>
    /// <param name="localPostion">Piece.</param>
    /// <returns>New piece position.</returns>
    private Vector3 RotateVector(Vector3 localVector, bool reverse = false)
    {
        Matrix4x4 rot = new Matrix4x4();
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
        heldInfo = CUBE.allCUBES[heldCUBE.ID];
        // stock inventory
        inventory[heldCUBE.ID]++;
        // remove CUBE from build
        RemoveCUBE(cube);
        // set status
        cursorStatus = CursorStatuses.Holding;
    }


    /// <summary>
    /// Place held CUBE into the build.
    /// </summary>
    /// <param name="weaponIndex">Index of the weapon.</param>
    /// <returns>True, if the CUBE was successfully placed.</returns>
    private bool PlaceCUBE(int weaponIndex = -1)
    {
        if (heldCUBE == null) return false;
        if (!Fits()) return false;

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
        if (weaponIndex == -1 && heldInfo.type == CUBE.Types.Weapon)
        {
            // find next open weapon slot if there is one
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] == null)
                {
                    weaponIndex = i;
                    break;
                }
            }
        }
        if (weaponIndex != -1)
        {
            weapons[weaponIndex] = heldCUBE.GetComponent<Weapon>();
        }

        Vector3 pivot = cursor + RotateVector(cursorOffset).Round();

        // add to build
        currentBuild.Add(heldCUBE, new CUBEGridInfo(pivot, cursorRotation.eulerAngles, weaponIndex, heldCUBE.GetComponent<ColorVertices>().colors));

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
                    Debugger.Log("Placing: " + point, this, true, Debugger.LogTypes.Construction);
                    grid[(int)point.y][(int)point.z][(int)point.x] = heldCUBE;
                }
            }
        }

        // add CUBE's stats
        shipHealth += heldInfo.health;
        shipShield += heldInfo.shield;
        shipSpeed += heldInfo.speed;
        shipDamage += heldInfo.damage;

        // set parent to ship
        heldCUBE.transform.parent = ship.transform;
        // empty cursor
        heldCUBE = null;
        // reset
        cursorStatus = CursorStatuses.Hover;

        return true;
    }


    /// <summary>
    /// Place held CUBE into the build and create another if applicable.
    /// </summary>
    /// <param name="loadAnother">Should another of the same CUBE be created after the current one is placed?</param>
    /// <param name="weaponIndex">Index of the weapon.</param>
    /// <returns>True, if the CUBE was successfully placed.</returns>
    private bool PlaceCUBE(bool loadAnother, int weaponIndex = -1)
    {
        if (heldCUBE == null) return false;

        int id = heldCUBE.ID;
        if (PlaceCUBE(weaponIndex) && loadAnother)
        {
            CreateCUBE(id);
            return true;
        }
        return false;
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
                    Debugger.Log("Removing: " + point, this, true, Debugger.LogTypes.Construction);
                    grid[(int)point.y][(int)point.z][(int)point.x] = null;
                    cells[(int)point.y][(int)point.z][(int)point.x].renderer.material = CellOpen_Mat;
                }
            }
        }

        // remove weapon if applicable
        if (heldInfo.type == CUBE.Types.Weapon && weapons.Contains(heldCUBE.GetComponent<Weapon>()))
        {
            weapons[Array.IndexOf(weapons, heldCUBE.GetComponent<Weapon>())] = null;
        }

        // remove CUBE from current build
        currentBuild.Remove(cube);

        // remove stats
        CUBEInfo cubeInfo = CUBE.allCUBES[cube.ID];
        shipHealth -= cubeInfo.health;
        shipShield -= cubeInfo.shield;
        shipSpeed -= cubeInfo.speed;
        shipDamage -= cubeInfo.damage;

        cells[(int)cursor.y][(int)cursor.z][(int)cursor.x].renderer.material = CellCursor_Mat;
    }


    /// <summary>
    /// Get build data from data.
    /// </summary>
    /// <param name="buildName">Name of the ship.</param>
    /// <returns>BuildInfo for the ship.</returns>
    private BuildInfo LoadBuild(string buildName)
    {
        this.buildName = buildName;
        string path;
#if DEVMODE
        path = DEVBUILDSPATH;
#else
        path = USERBUILDSPATH;
#endif
        // get buildInfo string from data
        string build = PlayerPrefs.GetString(path + buildName, "NA");
        if (build == "NA")
        {
            Log(buildName + " is not in data.", false, Debugger.LogTypes.Data);
            return null;
        }
        BuildInfo buildInfo = new BuildInfo();
        buildInfo.partList = new List<KeyValuePair<int, CUBEGridInfo>>();

        string[] data = build.Split(BuildInfo.DATASEP[0]);

        // stats
        buildInfo.name = data[0];
        buildInfo.health = float.Parse(data[1]);
        buildInfo.shield = float.Parse(data[2]);
        buildInfo.speed = float.Parse(data[3]);
        buildInfo.damage = float.Parse(data[4]);

        // pieces
        for (int i = 5; i < data.Length-1; i++)
        {
            string[] info = data[i].Split(BuildInfo.PIECESEP[0]);
            int[] colors = info[4].Substring(0, info[4].Length-1).Split(BuildInfo.COLORSEP[0]).Select(c => int.Parse(c)).ToArray();
            buildInfo.partList.Add(new KeyValuePair<int,CUBEGridInfo>(int.Parse(info[0]),
                                                                      new CUBEGridInfo(
                                                                          Utility.ParseV3(info[1]),
                                                                          Utility.ParseV3(info[2]),
                                                                          int.Parse(info[3]),
                                                                          colors)));
        }

        return buildInfo;
    }


    /// <summary>
    /// Compress the ship and save it to a prefab.
    /// </summary>
    private IEnumerator SavePrefab()
    {
        // save weapons
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].index = i;
            }
        }

        // bake colors
        //var colorVertices = ship.GetComponentsInChildren<ColorVertices>();
        //foreach (var color in colorVertices)
        //{
        //    color.Bake();
        //}

        // create one mesh
        var compactor = ship.AddComponent<ShipCompactor>();
        var compShip = compactor.Compact(typeof(PoolObject));

        yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.CreateAsset(compShip.GetComponent<MeshFilter>().mesh, ENEMYMESHPATH + buildName + "_Mesh.asset");
        UnityEditor.PrefabUtility.CreatePrefab(ENEMYPREFABPATH + buildName + ".prefab", compShip.gameObject); // keeping children
#endif
    }


    /// <summary>
    /// Save build to data.
    /// </summary>
    private string CreateBuildInfoString()
    {
        StringBuilder build = new StringBuilder();

        // stats
        build.Append(buildName);
        build.Append(BuildInfo.DATASEP);
        build.Append(shipHealth);
        build.Append(BuildInfo.DATASEP);
        build.Append(shipShield);
        build.Append(BuildInfo.DATASEP);
        build.Append(shipSpeed);
        build.Append(BuildInfo.DATASEP);
        build.Append(shipDamage);
        build.Append(BuildInfo.DATASEP);

        // pieces
        foreach (var piece in currentBuild)
        {
            // ID
            build.Append(piece.Key.ID);
            build.Append(BuildInfo.PIECESEP);
            // positon
            build.Append(piece.Value.position);
            build.Append(BuildInfo.PIECESEP);
            // rotation
            build.Append(piece.Value.rotation);
            build.Append(BuildInfo.PIECESEP);
            // weapon map
            build.Append(piece.Value.weaponMap);
            build.Append(BuildInfo.PIECESEP);
            // colors
            for (int i = 0; i < piece.Value.colors.Length; i++)
            {
                build.Append(piece.Value.colors[i]);
                build.Append(BuildInfo.COLORSEP);
            }
            build.Append(BuildInfo.DATASEP);
        }

        return build.ToString();
    }


    /// <summary>
    /// Save current build to data.
    /// </summary>
    public void SaveBuild()
    {
        SaveBuild(buildName, CreateBuildInfoString());

#if DEVMODE
        // compact and prefab ship
        StartCoroutine(SavePrefab());
#endif
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Get all build names. User vs Dev.
    /// </summary>
    /// <returns>All build names.</returns>
    public static List<string> BuildNames()
    {
        string namePath;
#if DEVMODE
        namePath = ALLDEVBUILDSPATH;
#else
        namePath = ALLUSERBUILDSPATH;
#endif

        List<string> builds = PlayerPrefs.GetString(namePath).Split(BuildInfo.PIECESEP[0]).ToList();
        if (builds[0] == "")
        {
            builds.RemoveAt(0);
        }
        return builds;
    }


    /// <summary>
    /// Save build to data.
    /// </summary>
    /// <param name="buildName">Name of ship to save.</param>
    /// <param name="build">BuildInfo string to save.</param>
    public static void SaveBuild(string buildName, string build)
    {
        if (Application.isPlaying)
        {
            Debugger.Log("Saving " + buildName + ": " + build, null, false, Debugger.LogTypes.Data);
        }

        // get paths
        string dataPath;
        string namePath;
#if DEVMODE
        dataPath = DEVBUILDSPATH;
        namePath = ALLDEVBUILDSPATH;
#else
        dataPath = USERBUILDSPATH;
        namePath = ALLUSERBUILDSPATH;
#endif
        // save data
        PlayerPrefs.SetString(dataPath + buildName, build);
        // save build name
        List<string> buildNames = BuildNames();
        if (!buildNames.Contains(buildName))
        {
            buildNames.Add(buildName);
            PlayerPrefs.SetString(namePath, string.Join(BuildInfo.PIECESEP, buildNames.ToArray()));
        }
        
    }


    /// <summary>
    /// Delete build from data.
    /// </summary>
    /// <param name="buildName">Name of build.</param>
    public static void DeleteBuild(string buildName)
    {
        // get path
        string dataPath;
        string namePath;
#if DEVMODE
        dataPath = DEVBUILDSPATH + buildName;
        namePath = ALLDEVBUILDSPATH;
#else
        dataPath = USERBUILDSPATH + buildName;
        namePath = ALLUSERBUILDSPATH;
#endif

        // remove from list of all builds
        string[] builds = PlayerPrefs.GetString(namePath).Split(BuildInfo.PIECESEP[0]).Where(b => b != buildName && !string.IsNullOrEmpty(b)).ToArray();
        PlayerPrefs.SetString(namePath, string.Join(BuildInfo.PIECESEP, builds));

        // remove build data
        PlayerPrefs.DeleteKey(dataPath);
    }


    /// <summary>
    /// Rename a build in data.
    /// </summary>
    /// <param name="oldName">Current name of the build.</param>
    /// <param name="newName">New name of the build.</param>
    public static void RenameBuild(string oldName, string newName)
    {
        // get path
        string dataPath;
        string namePath;
#if DEVMODE
        dataPath = DEVBUILDSPATH;
        namePath = ALLDEVBUILDSPATH;
#else
        dataPath = USERBUILDSPATH;
        namePath = ALLUSERBUILDSPATH;
#endif

        // update list of all builds
        string[] builds = PlayerPrefs.GetString(namePath).Split(BuildInfo.PIECESEP[0]);
        int index = Array.IndexOf(builds, oldName);
        builds[index] = newName;
        PlayerPrefs.SetString(namePath, string.Join(BuildInfo.PIECESEP, builds));

        // update build data
        string buildInfo = PlayerPrefs.GetString(dataPath + oldName);
        PlayerPrefs.DeleteKey(oldName);
        PlayerPrefs.SetString(dataPath + newName, buildInfo);
    }

    #endregion
}