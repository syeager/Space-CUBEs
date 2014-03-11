// Steve Yeager
// 3.8.2014

using UnityEngine;
using System.Collections;


public class NormalizationCalculator : MonoBehaviour
{
    #region Public Fields
    
    public Vector3 position;
    public float magnitude = 1;
    public float angle;
    
    #endregion


    public void Normalize()
    {
        Debug.Log(Utility.RotateVector(position.normalized, Quaternion.AngleAxis(angle, Vector3.back))*magnitude);
    }
}