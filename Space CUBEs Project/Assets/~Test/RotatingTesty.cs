// Steve Yeager
// 

using UnityEngine;

public class RotatingTesty : MonoBehaviour
{
    public Quaternion rotation;



    void Update()
    {
        transform.rotation = rotation;
        Debug.Log(transform.rotation);
    }
}