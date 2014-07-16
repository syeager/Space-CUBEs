﻿// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.07.13
// Edited: 2014.07.15

using System.Collections;
using System.Collections.Generic;
using Annotations;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Kamikaze : Enemy
{
    #region References

    private Rigidbody myRigidbody;

    #endregion

    #region State Names

    private const string SpawningState = "Spawning";
    private const string HomingState = "Homing";

    #endregion

    #region Public Fields

    public float life = 10f;
    public float speed;

    public PoolObject explosionPrefab;
    public float damage;
    public float explosionTime;

    #endregion

    #region Private Fields

    private Vector3 momentum;

    #endregion

    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // references
        myRigidbody = rigidbody;

        // states
        stateMachine = new StateMachine(this, SpawningState);
        stateMachine.CreateState(SpawningState, SpawningEnter, info => { });
        stateMachine.CreateState(HomingState, info => stateMachine.SetUpdate(HomingUpdate()), info => { });
        stateMachine.CreateState(DyingState, DyingEnter, info => { });
    }


    [UsedImplicitly]
    private void FixedUpdate()
    {
        myRigidbody.AddForce(momentum * speed, ForceMode.VelocityChange);
    }


    [UsedImplicitly]
    private void OnTriggerEnter(Collider other)
    {
        const string playerTag = "Player";
        if (!other.CompareTag(playerTag)) return;

        stateMachine.SetState(DyingState);
    }

    #endregion

    #region State Methods

    private void SpawningEnter(Dictionary<string, object> info)
    {
        MyHealth.Initialize();

        stateMachine.SetState(HomingState);
    }


    private IEnumerator HomingUpdate()
    {
        Transform player = LevelManager.Main.PlayerTransform;
        momentum = myTransform.position.To(player.position);

        for (float timer = 0f; timer < life; timer += deltaTime)
        {
            momentum = (Vector2)myTransform.position.To(player.position);
            yield return null;
        }

        stateMachine.SetState(DyingState);
    }


    private void DyingEnter(Dictionary<string, object> info)
    {
        momentum = Vector3.zero;

        Hitbox explosion = (Hitbox)Prefabs.Pop(explosionPrefab, myTransform.position, myTransform.rotation).GetComponent(typeof(Hitbox));
        explosion.Initialize(this, damage, explosionTime);

        poolObject.Disable();
    }

    #endregion
}