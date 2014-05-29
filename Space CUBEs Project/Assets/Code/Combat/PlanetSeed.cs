// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.02.19
// Edited: 2014.05.28

using System.Collections;
using UnityEngine;

/// <summary>
/// Tumbles through space blocking weapons and ramming enemies.
/// </summary>
public class PlanetSeed : Hitbox
{
    #region Public Fields

    /// <summary>How fast the planet grows in m/s.</summary>
    public float growth;

    /// <summary>How fast the planet spins in a random direction in m/s.</summary>
    public float angularSpeed;

    /// <summary>Pieces to break apart into.</summary>
    public GameObject planetPieces;

    /// <summary>Number of pieces to create when destroyed.</summary>
    public int pieceCount;

    /// <summary>How long in seconds the pieces last.</summary>
    public float pieceLife;

    /// <summary>How fast the pieces move in m/s.</summary>
    public float pieceSpeed;

    #endregion

    #region Private Fields

    /// <summary>Rotation to spin while growing.</summary>
    private Vector3 randomRotation;

    /// <summary>Layer of the attacker.</summary>
    private int senderLayer;

    #endregion

    #region MonoBehavoiur Overrides

    protected override void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == senderLayer) return;

        var oppHealth = other.gameObject.GetComponent(typeof(Health)) as Health;
        if (oppHealth != null)
        {
            oppHealth.RecieveHit(sender, damage * deltaTime);
        }
    }

    #endregion

    #region Hitbox Overrides

    public override void Initialize(Ship sender, float damage, float time, Vector3 moveVec)
    {
        base.Initialize(sender, damage, moveVec);

        senderLayer = LayerMask.NameToLayer("PlayerShip");
        gameObject.layer = LayerMask.NameToLayer("Default");

        var health = (Health)GetComponent(typeof(Health));
        health.Initialize();
        health.HealthUpdateEvent += OnDeath;

        myTransform.localScale = Vector3.one;
        randomRotation = Random.rotation.eulerAngles;
        StartCoroutine(Grow(time));
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Increase size and spin.
    /// </summary>
    private IEnumerator Grow(float time)
    {
        while (time >= 0f)
        {
            float dt = deltaTime;
            time -= dt;
            myTransform.localScale += Vector3.one * growth * dt;
            myTransform.Rotate(randomRotation * angularSpeed * dt);
            yield return null;
        }

        OnDeath(this, new HealthUpdateArgs(0f, 0f, 0f));
    }

    #endregion

    #region Event Handlers

    private void OnDeath(object sender, HealthUpdateArgs args)
    {
        if (args.health > 0f) return;

        StopAllCoroutines();
        ((Health)GetComponent(typeof(Health))).HealthUpdateEvent -= OnDeath;

        float angle = 360f / pieceCount;
        for (int i = 0; i < pieceCount; i++)
        {
            GameObject piece = PoolManager.Pop(planetPieces, myTransform.position, myTransform.rotation, pieceLife);
            piece.transform.localScale = myTransform.localScale;
            ((Hitbox)piece.GetComponent(typeof(Hitbox))).Initialize(this.sender, damage / pieceCount, pieceLife, Utility.RotateVector(myTransform.forward, angle * i, Vector3.back) * pieceSpeed);
        }
        myPoolObject.Disable();
    }

    #endregion
}