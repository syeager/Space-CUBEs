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


    protected virtual void Awake()
    {
        if (Main != null)
        {
            Debugger.LogWarning("Multiple instances of " + typeof(T).Name);
            Destroy(gameObject);
        }
        else
        {
            Main = GetComponent<T>();
        }
    }
}