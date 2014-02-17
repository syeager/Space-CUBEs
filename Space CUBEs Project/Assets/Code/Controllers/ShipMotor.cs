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
    public float barrelRollTime = 0.2f;
    /// <summary>Time between allowed barrel rolls.</summary>
    public float barrelRollBuffer = 0.5f;
    
    /// <summary>Screen percentage for screen top/bottom boundary.</summary>
    public float verticalBounds = 0.05f; // should be set by initialize
    public bool hasHorizontalBounds = true;
    public float leftBound = 0.01f;
    public float rightBound = 0.5f;

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
    
    public void Initialize(float speed)
    {
        this.speed = speed;
        barrelRollMoveSpeed = 2*speed;
    }


    public void Move(Vector2 input)
    {
        TestBoundaries(ref input);
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

        var timer = barrelRollTime;
        while (timer > 0f)
        {
            // test bounds
            TestBoundaries(ref direction);
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

    #region Private Methods

    public void TestBoundaries(ref Vector2 input)
    {
        Vector3 screenPosition = LevelCamera.WorldToViewportPoint(myTransform.position);

        // top
        if (input.y > 0f)
        {
            if (screenPosition.y >= (1 - verticalBounds))
            {
                input.y = 0f;
            }
        }
        // bottom
        else if (input.y < 0f)
        {
            if (screenPosition.y <= verticalBounds)
            {
                input.y = 0f;
            }
        }
        if (hasHorizontalBounds)
        {
            // left
            if (input.x < 0f)
            {
                if (screenPosition.x <= leftBound)
                {
                    input.x = 0f;
                }
            }
            // right
            else if (input.x > 0f)
            {
                if (screenPosition.x >= 1 - rightBound)
                {
                    input.x = 0f;
                }
            }
        }
    }

    #endregion
}