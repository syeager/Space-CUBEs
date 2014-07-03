// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.01
// Edited: 2014.06.01

using Annotations;
using UnityEngine;

/// <summary>
/// Hitbox for Plasma Lasers.
/// </summary>
public class PlasmaLaser : Hitbox, IBlackHoleListener
{
    #region References

    [SerializeField, HideInInspector]
    private Rigidbody myRigidbody;

    #endregion

    #region Public Fields

    /// <summary>How fast the laser rotates to align with the Black Hole's pull.</summary>
    public float pullSpeed;

    #endregion

    #region Private Fields

    /// <summary>Speed in m/s to move forward.</summary>
    private float speed;

    #endregion

    #region MonoBehaviour Overrides

    protected override void Reset()
    {
        base.Reset();
        myRigidbody = rigidbody;
    }


    [UsedImplicitly]
    private void FixedUpdate()
    {
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.MovePosition(myTransform.forward * speed * deltaTime + myRigidbody.position);
    }

    #endregion

    #region Hitbox Overrides

    // <inherit/>
    public override void Initialize(Ship sender, float damage, Vector3 moveVec)
    {
        Initialize(sender, damage);
        speed = moveVec.magnitude;
        myRigidbody.velocity = Vector3.zero;
    }

    #endregion

    #region IBlackHoleListener

    // <inherit/>
    public void Interact(Vector3 position, Vector3 pull)
    {
        Quaternion from = Quaternion.LookRotation(myTransform.forward, Vector3.back);
        Quaternion to = Quaternion.LookRotation(Vector3.Cross(pull.normalized, pull.y > 0f ? Vector3.forward : Vector3.back), Vector3.back);
        myTransform.rotation = Quaternion.Slerp(from, to, pullSpeed * deltaTime);
    }

    #endregion
}