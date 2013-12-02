// Steve Yeager
// 11.26.2013

using UnityEngine;

public class CUBE : MonoBehaviour
{
    #region Public Fields

    public int ID;
    private Vector3 pivot;
    public Vector3[] pieces = new Vector3[0];

    public enum CUBETypes
    {
        Normal,
        Weapon,
        Cockpit,
        Engine,
    }
    public CUBETypes CUBEType;

    public float health;
    public float shield;

    #endregion

    #region Static Fields

    public static int IDs { get; private set; }

    #endregion
}