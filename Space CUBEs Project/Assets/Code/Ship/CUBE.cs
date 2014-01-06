// Steve Yeager
// 11.26.2013

using System;
using UnityEngine;

public class CUBE : MonoBehaviour
{
    #region Public Fields

    public int ID;
    private Vector3 pivot;
    public Vector3[] pieces = new Vector3[0];

    public enum CUBETypes
    {
        Armor = 0,
        Weapon = 1,
        Cockpit = 2,
        Engine = 3,
        Wing = 4,
    }
    public CUBETypes CUBEType;

    public float health;
    public float shield;
    public float speed;

    #endregion

    #region Static Fields

    public static int IDs { get; private set; }

    #endregion
}