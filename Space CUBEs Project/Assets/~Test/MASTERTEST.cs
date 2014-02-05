// Steve Yeager
// 

using UnityEngine;
using System.Collections;

//
public class MASTERTEST : MonoBehaviour
{
    public float angle;



    void Start()
    {
        {
            Debug.Log("Sin\n0: " + Utility.SinZero(0) + "\n90: " + Utility.SinZero(90) + "\n180: " + Utility.SinZero(180) + "\n270: " + Utility.SinZero(270) + "\n360: " + Utility.SinZero(360));
        }
        {
            Debug.Log("Cos\n0: " + Utility.CosZero(0) + "\n90: " + Utility.CosZero(90) + "\n180: " + Utility.CosZero(180) + "\n270: " + Utility.CosZero(270) + "\n360: " + Utility.CosZero(360));
        }
    }
}