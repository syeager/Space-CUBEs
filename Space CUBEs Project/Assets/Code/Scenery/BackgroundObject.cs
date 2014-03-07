// Steve Yeager
// 3.3.2014

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class BackgroundObject : MonoBase
{
    #region Private Fields

    private float speed = 0f;
    private Transform myTransform;

    #endregion

    
    public void Initialize(float scale, float speed)
    {
        this.speed = speed;
        myTransform.localScale = Vector3.one*scale;
        myTransform.Rotate(Vector3.forward, Random.Range(0f, 360f));
    }


    private void Awake()
    {
        myTransform = transform;
    }


    private void Update()
    {
        myTransform.position += Vector3.left*speed*deltaTime;
    }
}