// Steve Yeager
// 3.8.2014

using UnityEngine;
using System.Collections;


public class NormalizationCalculator : MonoBehaviour
{
    #region Public Fields
    
    public Vector3 position;
    public float magnitude = 1;
    
    #endregion


    public void Normalize()
    {
        Debug.Log(position.normalized*magnitude);
    }
}