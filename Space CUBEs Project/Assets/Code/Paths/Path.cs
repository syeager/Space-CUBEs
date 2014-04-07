// Steve Yeager
// 1.26.2014

using UnityEngine;

/// <summary>
/// Flight path for enemy to take. 
/// </summary>
public class Path : ScriptableObject
{
    #region Public Fields

    public float speed = 15f;

    #endregion

    #region Protected Fields

    protected Transform myTransform;

    #endregion


    #region Virtual Methods

    public virtual Vector3 Direction(float deltaTime)
    {
        return Vector3.zero;
    }


    public virtual void Initialize(Transform transform)
    {
        myTransform = transform;
    }

    #endregion
}