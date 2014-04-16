// Steve Yeager
// 11.26.2013

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
    public GameObject Player_TR;
    public GameObject player_Prefab;

    #endregion


    #region Static Methods

    public static CUBE GetCUBE(int ID)
    {
        return Main.CUBE_Prefabs[ID];
    }

    #endregion
}