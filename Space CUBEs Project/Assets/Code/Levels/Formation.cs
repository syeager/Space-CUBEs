// Little Byte Games
// Author: Steve Yeager
// Created: 2014.01.12
// Edited: 2014.10.12

using System.Linq;
using UnityEngine;

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