// Steve Yeager
// 1.15.2014

using UnityEngine;
using System.Collections;

// Runs all methods needed at game start.
public class GameStart : MonoBehaviour
{
    private void Awake()
    {
        int CUBECount = CUBE.LoadAllCUBEInfo().Length;

        #if UNITY_ANDROID

        int[] inventory = new int[CUBECount];
        for (int i = 0; i < CUBECount; i++)
        {
            inventory[i] = 10;
        }

        CUBE.SetInventory(inventory);

        #endif
    }
}