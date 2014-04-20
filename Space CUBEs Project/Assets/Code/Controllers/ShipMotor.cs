﻿// Steve Yeager
// 11.25.2013

using Annotations;
using UnityEngine;
using System.Collections;

/// <summary>
/// Controls movement for a ship.
/// </summary>
public class ShipMotor : MonoBase
{
    #region References

    private Transform myTransform;
    private Rigidbody myRigidbody;
    private Camera levelCamera;
    
    #endregion

    #region Public Fields

    /// <summary>Max movement speed.</summary>
    public float speed;
    /// <summary>Movement speed during barrel roll.</summary>
    public float barrelRollMoveSpeed;
    /// <summary>How long the barrel roll lasts.</summary>
    public float barrelRollTime = 0.3f;
    /// <summary>Time between allowed barrel rolls.</summary>
    public float barrelRollBuffer = 0.5f;

    public bool hasVerticalBounds = true;
    /// <summary>Screen percentage for screen top/bottom boundary.</summary>
    public float verticalBounds = 0.05f;
    public bool hasHorizontalBounds = true;
    public float leftBound = 0.01f;
    public float rightBound = 0.01f;

    #endregion

    #region Private Fields

    private enum BarrelRollStatuses { Ready, Rolling, Waiting }
    private BarrelRollStatuses barrelRollStatus = BarrelRollStatuses.Ready;

    /// <summary>Velocity to be applied to the ship next step.</summary>
    private Vector3 velocity;

    #endregion

    #region Const Fields

    private const float SpeedModifier = 0.8f;
    private const float Barrelrollmodifier = 1.5f;

    #endregion


    #region Unity Overrides

    [UsedImplicitly]
    private void Awake()
    {
        // get references
        myTransform = transform;
        myRigidbody = rigidbody;
    }


    [UsedImplicitly]
    private void Start()
    {
        levelCamera = Camera.main;
    }


    [UsedImplicitly]
    private void FixedUpdate()
    {
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.deltaTime);
    }

    #endregion

    #region Public Methods
    
    public void Initialize(float speed)
    {
        myRigidbody = rigidbody;

        this.speed = speed * SpeedModifier;
        barrelRollMoveSpeed = Barrelrollmodifier * this.speed;

        enabled = true;
    }


    public void Move(Vector2 input)
    {
        if (hasVerticalBounds || hasHorizontalBounds)
        {
            TestBoundaries(ref input);
        }

        velocity = input * speed;
    }


    public void Move(Vector3 vector)
    {
        velocity = vector;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public IEnumerator BarrelRoll(Vector2 direction, float bounds)
    {
        if (barrelRollStatus != BarrelRollStatuses.Ready) yield break;
        
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
        Vector3 screenPosition = levelCamera.WorldToViewportPoint(myTransform.position);

        // vertical
        if (hasVerticalBounds)
        {
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
        }
        // horizontal
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