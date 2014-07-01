// Steve Yeager
// 12.7.2013

using UnityEngine;

public class ConsoleLine : Singleton<ConsoleLine>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}