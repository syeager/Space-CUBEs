// Steve Yeager
// 11.25.2013

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Controls movement for a ship.
/// </summary>
public class ShipMotor : MonoBase
{
    #region References

    private Transform myTransform;
    private Camera LevelCamera;
    
    #endregion

    #region Public Fields

    /// <summary>Max movement speed.</summary>
    public float speed;
    /// <summary>Movement speed during barrel roll.</summary>
    public float barrelRollMoveSpeed;
    /// <summary>How long the barrel roll lasts.</summary>
    public float barrelRollTime;
    /// <summary>Time between allowed barrel rolls.</summary>
    public float barrelRollBuffer = 0.5f;
    
    /// <summary>Screen percentage for screen top/bottom boundary.</summary>
    public float verticalBounds = 0.05f;

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


    private void Start()
    {
        LevelCamera = Camera.main;
    }

    #endregion

    #region Public Methods

    public void Move(Vector2 input)
    {
        Vector3 screenPosition = LevelCamera.WorldToScreenPoint(myTransform.position);
        // top
        float H = Screen.height;
        if (input.y > 0f)
        {
            if (screenPosition.y >= (H - H*verticalBounds))
            {
                input.y = 0f;
            }
        }
        else if (input.y < 0f)
        {
            if (screenPosition.y <= (H * verticalBounds))
            {
                input.y = 0f;
            }
        }

        myTransform.Translate(input * speed * deltaTime, Space.World);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public IEnumerator BarrelRoll(Vector2 direction, float bounds)
    {
        if (barrelRollStatus == BarrelRollStatuses.Rolling) yield break;
        
        barrelRollStatus = BarrelRollStatuses.Rolling;
        float rollingSpeed = 360f / barrelRollTime;
        Vector3 eulerAngles = myTransform.eulerAngles;
        //Vector3 moveDir = myTransform.right * direction;

        var timer = barrelRollTime;
        float ypos;
        while (timer > 0f)
        {
            // test bounds
            ypos = Camera.main.WorldToViewportPoint(myTransform.position).y;
            //if (!(direction > 0f && ypos <= bounds) && !(direction < 0f && ypos >= 1 - bounds))
            {
                myTransform.Translate(direction * barrelRollMoveSpeed * deltaTime, Space.World);
            }

            myTransform.Rotate(Vector3.back, rollingSpeed * Mathf.Sign(-direction.y) * deltaTime);

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