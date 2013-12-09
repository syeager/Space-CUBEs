// Steve Yeager
// 12.3.2013

using UnityEngine;

public class Singleton<T> : MonoBase where T : MonoBehaviour
{
    public static T Main { get; protected set; }



    protected virtual void Awake()
    {
        if (Main != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Main = GetComponent<T>();
        }
    }
}