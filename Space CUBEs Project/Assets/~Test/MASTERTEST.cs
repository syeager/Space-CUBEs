// Steve Yeager
// 

using UnityEngine;
using System.Collections;

//
public class MASTERTEST : MonoBehaviour
{
    public int tests;
    public Vector3 position;
    public Transform myTransform;

    void Awake()
    {
        using (var timer = new SpeedTimer("generic"))
        {
            for (int i = 0; i < tests; i++)
                myTransform = GetComponent<Transform>();
        }

        using (var timer = new SpeedTimer("type"))
        {
            for (int i = 0; i < tests; i++)
                myTransform = ((Transform)GetComponent(typeof(Transform)));
        }
    }
}