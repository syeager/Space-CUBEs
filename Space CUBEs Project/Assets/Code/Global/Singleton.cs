// Steve Yeager
// 12.3.2013

using UnityEngine;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBase where T : MonoBehaviour
{
    public static T Main { get; protected set; }
    public bool logWarning;


    protected virtual void Awake()
    {
        if (Main != null)
        {
            if (logWarning)
            {
                Debugger.LogWarning("Multiple instances of " + typeof(T).Name);
            }
            Destroy(gameObject);
        }
        else
        {
            Main = GetComponent<T>();
        }
    }
}