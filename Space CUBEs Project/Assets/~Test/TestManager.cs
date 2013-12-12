// Steve Yeager
// 

using System.Collections;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PoolManager.Pop("Disco Ball", 2f);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PoolManager.Pop("Super Square", 2f);
        }
    }
}