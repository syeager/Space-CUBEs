// Steve Yeager
// 

using UnityEngine;
using Profiler = LittleByte.Debug.Profiler;

//
public class MASTERTEST : MonoBehaviour
{
    public int tests;
    public Vector3 position;
    public Transform myTransform;

    void Awake()
    {
        using (var timer = new Profiler("generic"))
        {
            for (int i = 0; i < tests; i++)
                myTransform = GetComponent<Transform>();
        }

        using (var timer = new Profiler("type"))
        {
            for (int i = 0; i < tests; i++)
                myTransform = ((Transform)GetComponent(typeof(Transform)));
        }
    }
}