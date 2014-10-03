// Little Byte Games
// Author: Steve Yeager
// Created: 2013.11.27
// Edited: 2014.10.02

using UnityEngine;

/// <summary>
/// Holds 
/// </summary>
public class GameResources : Singleton<GameResources>
{
    #region Public Fields

    public CUBE[] CUBE_Prefabs;
    public GameObject ConstructionGrid_Prefab;
    public Material VertexColor_Mat;
    public Material VertexColorLerp_Mat;
    public Material ShieldHit_Mat;
    public Material HealthHit_Mat;
    public GameObject player_Prefab;

    #endregion

    #region Static Methods

    public static CUBE CreateCUBE(int id)
    {
        var cube = (CUBE)Instantiate(Main.CUBE_Prefabs[id]);
        cube.name = CUBE.AllCUBES[id].name;
        return cube;
    }

    #endregion
}