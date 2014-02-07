// Steve Yeager
// 1.12.2014

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class Formation : MonoBehaviour
{
    #region Public Fields

    public Vector3[] positions;

    #endregion

    #region Const Fields

    public const string FORMATIONPATH = "Assets/Formations/";

    #endregion


    #region Static Methods

    #if UNITY_EDITOR
    public static Formation[] AllFormations()
    {
        return Utility.LoadObjects<Formation>(FORMATIONPATH).ToArray();
    }
    #endif

    #endregion
}