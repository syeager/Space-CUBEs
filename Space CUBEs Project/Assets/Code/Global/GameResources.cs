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

    #region CUBEs

    public List<CUBE> CUBEs;

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
        return Main.CUBEs.Find(c => c.ID == ID);
    }

    #endregion
}