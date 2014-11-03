// Little Byte Games

using System.Collections;
using Annotations;
using LittleByte.Audio;
using UnityEngine;

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

    private enum BarrelRollStatuses
    {
        Ready,
        Rolling,
        Waiting
    }

    private BarrelRollStatuses barrelRollStatus = BarrelRollStatuses.Ready;

    /// <summary>Velocity to be applied to the ship next step.</summary>
    private Vector3 velocity;

    [SerializeField, UsedImplicitly]
    private AudioPlayer barrelRollClip;

    #endregion

    #region Properties

    public bool CanBarrelRoll
    {
        get { return barrelRollStatus == BarrelRollStatuses.Ready; }
    }

    #endregion

    #region Const Fields

    private const float SpeedModifier = 0.8f;
    private const float Barrelrollmodifier = 2f;

    #endregion

    #region MonoBehaviour Overrides

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
        myRigidbody.MovePosition(myRigidbody.position + velocity * deltaTime);
        velocity = Vector3.zero;
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

        velocity = input.normalized * speed;
    }

    public void Move(Vector3 vector)
    {
        velocity = vector;
    }

    /// <summary>
    /// Sets velocity to zero.
    /// </summary>
    public void Stop()
    {
        velocity = Vector3.zero;
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

        AudioManager.Play(barrelRollClip);

        float timer = barrelRollTime;
        while (timer > 0f)
        {
            // test bounds
            TestBoundaries(ref direction);
            {
                Move((Vector3)direction * barrelRollMoveSpeed);
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
        Vector3 screenPosition = levelCamera.WorldToViewportPoint(myTransform.position + (Vector3)input * speed * Time.fixedDeltaTime);

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