// Steve Yeager
// 11.25.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Controls movement for a ship.
/// </summary>
public class ShipMotor : MonoBase
{
    #region References

    private Transform myTransform;
    
    #endregion

    #region Public Fields

    /// <summary></summary>
    public float speed;
    /// <summary></summary>
    public float barrelRollMoveSpeed;
    /// <summary></summary>
    public float barrelRollTime;
    /// <summary></summary>
    public float barrelRollBuffer = 0.5f;    

    #endregion

    #region Private Fields

    private enum BarrelRollStatuses { Ready, Rolling, Waiting }
    private BarrelRollStatuses barrelRollStatus = BarrelRollStatuses.Ready;

    #endregion


    #region Unity Overrides

    private void Awake()
    {
        // get references
        myTransform = transform;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="direction"></param>
    public void Move(float input, Vector3 direction)
    {
        if (barrelRollStatus == BarrelRollStatuses.Rolling) return;

        myTransform.Translate(input * speed * direction * deltaTime);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="bounds"></param>
    public IEnumerator BarrelRoll(int direction, float bounds)
    {
        if (barrelRollStatus == BarrelRollStatuses.Rolling) yield break;
        
        barrelRollStatus = BarrelRollStatuses.Rolling;
        float rollingSpeed = 360f / barrelRollTime;
        Vector3 eulerAngles = myTransform.eulerAngles;
        Vector3 moveDir = myTransform.right * direction;

        var timer = barrelRollTime;
        float ypos;
        while (timer > 0f)
        {
            // test bounds
            ypos = Camera.main.WorldToViewportPoint(myTransform.position).y;
            if (!(direction > 0f && ypos <= bounds) && !(direction < 0f && ypos >= 1 - bounds))
            {
                myTransform.Translate(moveDir * barrelRollMoveSpeed * deltaTime, Space.World);
            }

            myTransform.Rotate(Vector3.back, rollingSpeed * direction * deltaTime);

            timer -= deltaTime;
            yield return null;
        }

        // reset rotation and position
        myTransform.position = new Vector3(myTransform.position.x, myTransform.position.y, 0f);
        myTransform.eulerAngles = eulerAngles;

        barrelRollStatus = BarrelRollStatuses.Waiting;
        InvokeAction(() => barrelRollStatus = BarrelRollStatuses.Ready, barrelRollBuffer);
    }

    #endregion
}