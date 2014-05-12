// Steve Yeager
// 1.12.2014

using UnityEngine;
using System.Linq;

public class Formation : MonoBehaviour
{
    #region Public Fields

    public Vector3[] positions;

    #endregion

    #region Const Fields

    public const string FormationPath = "Assets/Formations/";

    #endregion


    #region Static Methods

    #if UNITY_EDITOR
    public static Formation[] AllFormations()
    {
        return Utility.LoadObjects<Formation>(FormationPath, false).ToArray();
    }
    #endif

    #endregion
}