// Steve Yeager
// 11.25.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Controls movement for any ship.
/// </summary>
public class ShipMotor : MonoBehaviour
{
    #region References

    private Transform myTransform;
    
    #endregion

    #region Public Fields

    public float horizontalSpeed;

    #endregion


    #region Unity Overrides

    private void Awake()
    {
        // get references
        myTransform = transform;
    }

    #endregion

    #region Public Methods

    public void MoveHorizontal(float input)
    {
        myTransform.Translate(input * horizontalSpeed * Vector3.right * Time.deltaTime);
    }

    #endregion
}