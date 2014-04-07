// Steve Yeager
// 11.26.2013

using Annotations;
using UnityEngine;

public class GameResources : MonoBehaviour
{
    #region References

    public static GameResources Main;

    #endregion

    #region Public Fields

    public CUBE[] CUBE_Prefabs;
    public GameObject ConstructionGrid_Prefab;
    public Material VertexColor_Mat;
    public Material VertexColorLerp_Mat;
    public Material ShieldHit_Mat;
    public Material HealthHit_Mat;
    public GameObject Player_TR;

    #endregion


    #region Unity Overrides

    [UsedImplicitly]
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