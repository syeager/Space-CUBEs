// Steve Yeager
// 11.26.2013

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameResources : MonoBehaviour
{
    #region References

    public static GameResources Main;

    #endregion

    #region Public Fields

    public CUBE[] CUBE_Prefabs;
    public GameObject ConstructionGrid_Prefab;
    public Material VertexColor_Mat;
    public Material VertexOverlay_Mat;
    public Material ShieldHit_Mat;
    public Material HealthHit_Mat;

    public GameObject[] EnemyPrefabs;
    public Enemy.Classes[] EnemyClasses;

    #endregion


    #region Unity Overrides

    private void Awake()
    {
        if (Main == null)
        {
            Main = this;
        }
    }

    #endregion

    #region Static Methods

    public static CUBE GetCUBE(int ID)
    {
        return Main.CUBE_Prefabs[ID];
    }

    #endregion
}