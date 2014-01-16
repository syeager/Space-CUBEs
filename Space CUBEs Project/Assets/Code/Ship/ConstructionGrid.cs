﻿// Steve Yeager
// 11.26.2013

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;
using System.Collections;

public class ConstructionGrid : MonoBase
{
    #region Public Fields

    public GameObject Center_Prefab;
    public GameObject Cell_Prefab;
    public Material CellCursor_Mat;
    public Material CellOpen_Mat;
    public Material CellClosed_Mat;
    public Material CellHover_Mat;
    public enum CursorStatuses
    {
        None,
        Hover,
        Holding,
    }

    public bool showWholeGrid = true;

    public string buildName;

    #endregion

    #region Private Fields

    private CUBE[][][] grid;
    private GameObject[][][] cells;
    private Vector3 cursorOffset;
    private Dictionary<CUBE, CUBEGridInfo> currentBuild = new Dictionary<CUBE, CUBEGridInfo>();
    private GameObject ship;
    private Quaternion rotation = new Quaternion(0, 0, 0, 1);

    #endregion

    #region Const Fields

    private const string USERBUILDSPATH = "UserBuilds: ";
    private const string DEVBUILDSPATH = "DevBuilds";
    private const string MESHPATH = "Assets/Ship/Characters/";
    private const string PREFABPATH = "Assets/Ship/Characters/";

    #endregion

    #region Properties

    public int size { get; private set; }
    public Vector3 cursor { get; private set; }
    public Vector3 cursorRotation { get; private set; }
    public int currentLayer { get; private set; }
    public CursorStatuses cursorStatus { get; private set; }
    public CUBE currentCUBE { get; private set; }
    private Vector3 cursorPosition
    {
        get
        {
            return cells[(int)cursor.y][(int)cursor.z][(int)cursor.x].transform.position;
        }
    }
    private Weapon[] _weapons = new Weapon[6];
    public Weapon[] weapons { get { return _weapons; } private set { _weapons = value; } }
    private int weaponsIndex
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

    public float shipHealth { get; private set; }
    public float shipShield { get; private set; }
    public float shipSpeed { get; private set; }
    public int shipWeapons { get; private set; }

    public int[] inventory { get; private set; }

    #endregion

    #region Events

    public EventHandler<BuildFinishedArgs> BuildFinishedEvent;

    #endregion


    #region Public Methods

    public void CreateGrid()
    {
        CreateGrid(size);
    }


    public void CreateGrid(int size)
    {
        inventory = CUBE.GetInventory();
        ClearCells();
        this.size = size;
        grid = new CUBE[size][][];
        cells = new GameObject[size][][];
        Vector3 cursor = new Vector3(-size / 2f +  0.5f, 0f, -size / 2f + 0.5f);
        
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
                    if (!showWholeGrid)
                    {
                        if (i != 0)
                        {
                            cells[i][j][k].SetActive(false);
                        }
                    }
                    cursor.x++;
                }
                cursor.z++;
            }
            cursor.y++;
        }

        currentLayer = 0;
        ChangeLayer(Mathf.FloorToInt(size / 2f));
        cursor = Vector3.one*Mathf.FloorToInt(size/2f);
        if (ship == null)
        {
            ship = new GameObject("Ship");
        }
        ship.transform.position = transform.position + Vector3.up * (size / 2f + 0.5f);
        GameObject.Instantiate(Center_Prefab, ship.transform.position, Quaternion.identity);
        weapons = new Weapon[6];
    }


    public void RotateCursor(Vector3 axis)
    {
        rotation = Quaternion.Euler(axis * 90f) * rotation;
        cursorRotation = rotation.eulerAngles;

        if (currentCUBE != null)
        {
            currentCUBE.transform.rotation = rotation;
        }
    }


    public void MoveCursor(Vector3 vector)
    {
        //if (grid[(int)cursor.y][(int)cursor.z][(int)cursor.x])
        //{
        //    cells[(int)cursor.y][(int)cursor.z][(int)cursor.x].renderer.material = CellClosed_Mat;
        //}
        //else
        {
            cells[(int)cursor.y][(int)cursor.z][(int)cursor.x].renderer.material = CellOpen_Mat;
        }

        if (cursor.x + vector.x < 0 || cursor.x + vector.x > size - 1) vector.x = 0f;
        if (cursor.y + vector.y < 0 || cursor.y + vector.y > size - 1) vector.y = 0f;
        if (cursor.z + vector.z < 0 || cursor.z + vector.z > size - 1) vector.z = 0f;
        cursor += vector;

        cells[(int)cursor.y][(int)cursor.z][(int)cursor.x].renderer.material = CellCursor_Mat;

        if (currentCUBE != null)
        {
            currentCUBE.transform.position += vector;
        }

        if (currentCUBE != null)
        {
            cursorStatus = CursorStatuses.Holding;
        }
        else if (grid[(int)cursor.y][(int)cursor.z][(int)cursor.x] != null)
        {
            cursorStatus = CursorStatuses.Hover;
        }
        else
        {
            cursorStatus = CursorStatuses.None;
        }
    }


    public void ChangeLayer(int amount)
    {
        int newLayer = Mathf.Clamp(currentLayer + amount, 0, size-1);
        if (newLayer == currentLayer) return;

        if (!showWholeGrid)
        // toggle cells
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    cells[currentLayer][i][j].SetActive(false);
                    cells[newLayer][i][j].SetActive(true);
                }
            }

            // toggle CUBEs
            foreach (var cube in currentBuild)
            {
                if (cube.Value.position.y > newLayer) foreach (var mat in cube.Key.renderer.materials) mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0.25f);
                if (cube.Value.position.y == newLayer) foreach (var mat in cube.Key.renderer.materials) mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 1f);
            }
        }

        // move camera
        Camera.main.transform.position += Vector3.up * amount;

        currentLayer = newLayer;
        MoveCursor(Vector3.up * amount);
    }


    public void CreateCUBE(int CUBEID)
    {
        if (currentCUBE != null)
        {
            Destroy(currentCUBE.gameObject);
        }

        cursorOffset = Vector3.zero;

        currentCUBE = (CUBE)GameObject.Instantiate(GameResources.GetCUBE(CUBEID));
        currentCUBE.transform.position = cursorPosition;
        currentCUBE.transform.eulerAngles = cursorRotation;

        cursorStatus = CursorStatuses.Holding;
    }


    /// <summary>
    /// If cursor is empty, pick up CUBE if hovering. If cursor is not empty, place CUBE.
    /// </summary>
    /// <param name="loadAnother">Should another CUBE be loaded of the same type that was placed?</param>
    public void CursorAction(bool loadAnother)
    {
        if (currentCUBE != null)
        {
            PlaceCUBE(loadAnother);
        }
        else if (grid[(int)cursor.y][(int)cursor.z][(int)cursor.x] != null)
        {
            PickupCUBE(grid[(int)cursor.y][(int)cursor.z][(int)cursor.x]);
        }
    }


    public int DeleteCUBE()
    {
        if (currentCUBE == null) return -1;

        int ID = currentCUBE.ID;
        RemoveCUBE(currentCUBE);
        Destroy(currentCUBE.gameObject);
        currentCUBE = null;
        cursorStatus = CursorStatuses.None;

        return ID;
    }


    public string SaveToData()
    {
        string build;
        using (StringWriter str = new StringWriter())
        using (XmlTextWriter xml = new XmlTextWriter(str))
        {
            // root
            xml.WriteStartDocument();
            xml.WriteStartElement("Ship");

            // main
            xml.WriteElementString("Info", buildName);
            xml.WriteElementString("Health", shipHealth.ToString());
            xml.WriteElementString("Shield", shipShield.ToString());
            xml.WriteElementString("Speed", shipSpeed.ToString());

            // pieces
            foreach (var piece in currentBuild)
            {
                xml.WriteStartElement("Piece");
                {
                    xml.WriteElementString("ID", piece.Key.ID.ToString());
                    xml.WriteElementString("Position", piece.Value.position.ToString());
                    xml.WriteElementString("Rotation", piece.Value.rotation.ToString());
                    xml.WriteElementString("WeaponMap", piece.Value.weaponMap.ToString());
                }
                xml.WriteEndElement();
            }

            // end
            xml.WriteEndElement();
            xml.WriteEndDocument();
            
            build = str.ToString();
        }

        // save
        string path;
        #if DEVMODE
        path = DEVBUILDSPATH;
        #else  
        path = USERBUILDSPATH;
        #endif
        PlayerPrefs.SetString(path + buildName, build);
        // add to list of all buildNames

        // compact and prefab ship
        #if DEVMODE
        StartCoroutine(SavePrefab());
        #endif


        Log(build, true, Debugger.LogTypes.Data); // causes crash in mobile
        return build;
    }


    public void Load(string build)
    {
        var buildInfo = LoadFromData(build);
        if (buildInfo == null) return;

        var position = cursor;
        foreach (var piece in buildInfo.partList)
        {
            cursor = piece.Value.position;
            cursorRotation = piece.Value.rotation;
            CreateCUBE(piece.Key);
            PlaceCUBE(piece.Value.weaponMap);
        }
        cursor = position;
    }


    public void MoveWeaponMap(int index, int direction)
    {
        if (index == -1) return;
        if (index + direction >= weapons.Length || index + direction < 0) return;
        if (weapons[index] == null) return;

        Weapon saved = weapons[index];
        currentBuild[saved.GetComponent<CUBE>()].weaponMap += direction;
        weapons[index] = weapons[index + direction];
        if (weapons[index] != null)
        {
            currentBuild[weapons[index].GetComponent<CUBE>()].weaponMap -= direction;
        }
        weapons[index + direction] = saved;
    }


    public IEnumerator Build(string build, int buildSize, Vector3 startPosition, Vector3 startRotation, float maxTime)
    {
        var buildInfo = LoadFromData(build);
        if (buildInfo == null)
        {
            if (BuildFinishedEvent != null)
            {
                BuildFinishedEvent(this, new BuildFinishedArgs(null, 0f, 0f, 0f));
            }
            yield break;
        }

        List<BuildCUBE> pieces = new List<BuildCUBE>();
        GameObject finishedShip = new GameObject("Player");

        const float minDist = 10f;
        const float maxDist = 25f;
        Vector3 halfGrid = Vector3.one * (buildSize/2f - 0.5f);
        finishedShip.transform.position = startPosition;
        finishedShip.transform.eulerAngles = startRotation;
        float speed = (finishedShip.transform.position * maxDist).magnitude / maxTime;

        foreach (var piece in buildInfo.partList)
        {
            var cube = (CUBE)GameObject.Instantiate(GameResources.GetCUBE(piece.Key));
            cube.transform.parent = finishedShip.transform;

            cube.transform.localPosition = piece.Value.position*UnityEngine.Random.Range(minDist, maxDist);
            cube.transform.localPosition = Quaternion.Euler(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)) * cube.transform.localPosition;
            cube.transform.localEulerAngles = piece.Value.rotation;

            if (piece.Value.weaponMap != -1)
            {
                cube.GetComponent<Weapon>().index = piece.Value.weaponMap;
            }

            cube.GetComponent<ColorVertices>().Bake();

            pieces.Add(new BuildCUBE(cube.transform, piece.Value.position-halfGrid, speed));
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
            BuildFinishedEvent(this, new BuildFinishedArgs(finishedShip, buildInfo.health, buildInfo.shield, buildInfo.speed));
        }
    }

    #endregion

    #region Private Methods

    private void ClearCells()
    {
        if (cells == null) return;

        for (int i = 0; i < cells.Length; i++)
        {
            for (int j = 0; j < cells[i].Length; j++)
            {
                for (int k = 0; k < cells[i][j].Length; k++)
                {
                    Destroy(cells[i][j][k]);
                }
            }
        }
    }


    private bool Fits()
    {
        if (currentCUBE == null) return false;

        // pivot
        Vector3 pivot = cursor;
        if (pivot.x < 0 || pivot.x > size - 1) return false;
        if (pivot.y < 0 || pivot.y > size - 1) return false;
        if (pivot.z < 0 || pivot.z > size - 1) return false;
        if (grid[(int)pivot.y][(int)pivot.z][(int)pivot.x]) return false;

        // empty for pieces
        foreach (var piece in currentCUBE.pieces)
        {
            Vector3 position = pivot - cursorOffset + Rotate(piece);
            if (position.x < 0 || position.x > size - 1) return false;
            if (position.y < 0 || position.y > size - 1) return false;
            if (position.z < 0 || position.z > size - 1) return false;
            if (grid[(int)position.y][(int)position.z][(int)position.x]) return false;
        }

        return true;
    }


    private Vector3 Rotate(Vector3 piece)
    {
        var z = RotationMatrixZ(cursorRotation.z);
        var y = RotationMatrixY(cursorRotation.y);

        return y.MultiplyVector(z.MultiplyVector(piece));
    }


    private float Sin(float angle)
    {
        if (angle == 90) return 1f;
        else if (angle == 270) return -1f;

        return 0f;
    }


    private float Cos(float angle)
    {
        if (angle == 0) return 1f;
        else if (angle == 180) return -1f;

        return 0f;
    }


    private void PickupCUBE(CUBE cube)
    {
        currentCUBE = cube;
        inventory[currentCUBE.ID]++;
        cursorRotation = currentBuild[currentCUBE].rotation;
        rotation = Quaternion.Euler(cursorRotation);
        cursorOffset = cursorPosition - currentCUBE.transform.position;
        RemoveCUBE(cube);
        cursorStatus = CursorStatuses.Holding;
    }


    private bool PlaceCUBE(int weaponIndex = -1)
    {
        if (currentCUBE == null) return false;
        if (!Fits()) return false;

        if (inventory[currentCUBE.ID] > 0)
        {
            inventory[currentCUBE.ID]--;
        }
        else
        {
            return false;
        }

        if (currentCUBE.type == CUBE.Types.Weapon)
        {
            if (weaponIndex == -1)
            {
                weaponIndex = weaponsIndex;
            }
            weapons[weaponIndex] = currentCUBE.GetComponent<Weapon>();
            currentBuild.Add(currentCUBE, new CUBEGridInfo(cursor - cursorOffset, cursorRotation, weaponIndex));
        }
        else
        {
            currentBuild.Add(currentCUBE, new CUBEGridInfo(cursor - cursorOffset, cursorRotation, -1));
        }

        // add all pieces
        foreach (var piece in currentCUBE.pieces)
        {
            Vector3 rotatedPiece = cursor - cursorOffset + Rotate(piece);
            grid[(int)rotatedPiece.y][(int)rotatedPiece.z][(int)rotatedPiece.x] = currentCUBE;
            //cells[(int)rotatedPiece.y][(int)rotatedPiece.z][(int)rotatedPiece.x].renderer.material = CellClosed_Mat;
        }

        // stats
        shipHealth += currentCUBE.health;
        shipShield += currentCUBE.shield;
        shipSpeed += currentCUBE.speed;

        currentCUBE.transform.parent = ship.transform;
        currentCUBE = null;
        cursorOffset = Vector3.zero;
        cursorStatus = CursorStatuses.Hover;

        return true;
    }


    private bool PlaceCUBE(bool loadAnother, int weaponIndex = -1)
    {
        if (currentCUBE == null) return false;
        int id = currentCUBE.ID;
        if (PlaceCUBE(weaponIndex))
        {
            if (loadAnother)
            {
                CreateCUBE(id);
            }
            return true;
        }
        return false;
    }


    private void RemoveCUBE(CUBE cube)
    {
        // pivot 
        Vector3 pivot = cursor;
        var rotationZ = Matrix4x4.identity;
        var rotationY = Matrix4x4.identity;
        if (currentBuild.ContainsKey(cube))
        {
            pivot = currentBuild[cube].position;
            rotationZ = RotationMatrixZ(currentBuild[cube].rotation.z);
            rotationY = RotationMatrixY(currentBuild[cube].rotation.y);
        }

        // pieces
        foreach (var piece in cube.pieces)
        {
            Vector3 rotatedPiece = pivot + rotationZ.MultiplyVector(rotationY.MultiplyVector(piece));
            grid[(int)rotatedPiece.y][(int)rotatedPiece.z][(int)rotatedPiece.x] = null;
            cells[(int)rotatedPiece.y][(int)rotatedPiece.z][(int)rotatedPiece.x].renderer.material = CellOpen_Mat;
        }

        if (currentCUBE.type == CUBE.Types.Weapon && weapons.Contains(currentCUBE.GetComponent<Weapon>()))
        {
            weapons[Array.IndexOf(weapons, currentCUBE.GetComponent<Weapon>())] = null;
        }
        currentBuild.Remove(cube);

        // stats
        shipHealth -= cube.health;
        shipShield -= cube.shield;
        shipSpeed -= cube.speed;

        cells[(int)cursor.y][(int)cursor.z][(int)cursor.x].renderer.material = CellCursor_Mat;
    }


    private Matrix4x4 RotationMatrixZ(float angle)
    {
        return new Matrix4x4
        {
            m00 = Cos(angle), m01 = -Sin(angle), m02 = 0, m03 = 0,
            m10 = Sin(angle), m11 = Cos(angle), m12 = 0, m13 = 0,
            m20 = 0, m21 = 0, m22 = 1, m23 = 0,
            m30 = 0, m31 = 0, m32 = 0, m33 = 1
        };
    }


    private Matrix4x4 RotationMatrixX(float angle)
    {
        return new Matrix4x4
        {
            m00 = 1, m01 = 0, m02 = 0, m03 = 0,
            m10 = 0, m11 = Cos(angle), m12 = -Sin(angle), m13 = 0,
            m20 = 0, m21 = Sin(angle), m22 = Cos(angle), m23 = 0,
            m30 = 0, m31 = 0, m32 = 0, m33 = 1
        };
    }


    private Matrix4x4 RotationMatrixY(float angle)
    {
        return new Matrix4x4
        {
            m00 = Cos(angle), m01 = 0, m02 = Sin(angle), m03 = 0,
            m10 = 0, m11 = 1, m12 = 0, m13 = 0, 
            m20 = -Sin(angle), m21 = 0, m22 = Cos(angle), m23 = 0,
            m30 = 0, m31 = 0, m32 = 0, m33 = 1
        };
    }


    private BuildInfo LoadFromData(string buildName)
    {
        currentBuild.Clear();
        weapons = new Weapon[6];

        this.buildName = buildName;
        string path;
        #if DEVMODE
        path = DEVBUILDSPATH;
        #else  
        path = USERBUILDSPATH;
        #endif
        string build = PlayerPrefs.GetString(path + buildName, "NA");
        if (build == "NA")
        {
            return null;
        }
        BuildInfo buildInfo = new BuildInfo();
        buildInfo.partList = new List<KeyValuePair<int, CUBEGridInfo>>();

        Log(build, true, Debugger.LogTypes.Data);

        using (StringReader str = new StringReader(build))
        using (XmlTextReader xml = new XmlTextReader(str))
        {
            int pieceID = -1;
            Vector3 pieceP = Vector3.zero;
            Vector3 pieceR = Vector3.zero;
            int weaponMap = -1;
            while (xml.Read())
            {
                if (xml.IsStartElement())
                {
                    switch (xml.Name)
                    {
                        case "Name":
                        case "Ship":
                        case "Info":
                            break;

                        case "Health":
                            buildInfo.health = float.Parse(xml.ReadString());
                            break;
                        case "Shield":
                            buildInfo.shield = float.Parse(xml.ReadString());
                            break;
                        case "Speed":
                            buildInfo.speed = float.Parse(xml.ReadString());
                            break;

                        case "Piece":
                            break;

                        case "ID":
                            pieceID = int.Parse(xml.ReadString());
                            break;
                        case "Position":
                            pieceP = Utility.ParseV3(xml.ReadString());
                            break;
                        case "Rotation":
                            pieceR = Utility.ParseV3(xml.ReadString());
                            break;
                        case "WeaponMap":
                            weaponMap = int.Parse(xml.ReadString());
                            buildInfo.partList.Add(new KeyValuePair<int, CUBEGridInfo>(pieceID, new CUBEGridInfo(pieceP, pieceR, weaponMap)));
                            break;

                        default:
                            Debugger.LogWarning("Incorrect XML Element in build: " + xml.Name);
                            break;
                    }
                }
            }
        }

        return buildInfo;
    }


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
        var colorVertices = ship.GetComponentsInChildren<ColorVertices>();
        foreach (var color in colorVertices)
        {
            color.Bake();
        }
        
        // create one mesh
        var compactor = ship.AddComponent<ShipCompactor>();
        var compShip = compactor.Compact(typeof(PoolObject));

        yield return new WaitForEndOfFrame();
        #if UNITY_EDITOR
        
        UnityEditor.AssetDatabase.CreateAsset(compShip.GetComponent<MeshFilter>().mesh, MESHPATH + buildName + "_Mesh.asset");
        UnityEditor.PrefabUtility.CreatePrefab(PREFABPATH + buildName + ".prefab", compShip.gameObject); // keeping children

        #endif
    }

    #endregion
}