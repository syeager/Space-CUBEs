// Steve Yeager
// 12.3.2013

using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Main { get; protected set; }



    private void Awake()
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