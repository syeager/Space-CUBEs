// Steve Yeager
// 1.15.2014

using UnityEngine;
using System.Collections;

// Runs all methods needed at game start.
public class GameStart : MonoBehaviour
{
    private void OnEnable()
    {
        CUBE.LoadAllCUBEInfo();
    }
}