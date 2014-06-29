// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.25
// Edited: 2014.06.28

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns minions for Medic.
/// </summary>
public class MinionSpawner : Weapon
{
    #region Public Fields

    public PoolObject minionPrefab;

    /// <summary>Number of minions to create each spawn.</summary>
    public int minionSpawnCount = 5;

    /// <summary>Max allowed number of minions alive at one time.</summary>
    public int maxMinionCount = 15;

    /// <summary>Time in seconds to delay between minion spawning.</summary>
    public float minionSpawnDelay;

    /// <summary>Time in seconds to deploy.</summary>
    public float deployTime;

    /// <summary>Minion local spawn position.</summary>
    public Vector3 spawnPosition;

    /// <summary>Max distance from spawnPosition.</summary>
    public Vector3 spawnRadius;

    /// <summary>Time in seconds to buff and unbuff enemies.</summary>
    public float buffingTime;

    /// <summary>Health buff particles.</summary>
    public ParticlePoolObject healthParticles;

    /// <summary>Shield buff.</summary>
    public float shieldBuff;

    /// <summary>Shield buff particles.</summary>
    public ParticlePoolObject shieldParticles;

    #endregion

    #region Private Fields

    /// <summary>Currently spawned minions.</summary>
    private readonly List<Health> minions = new List<Health>();

    #endregion

    #region Public Methods

    public Coroutine Spawn()
    {
        int minionsToSpawn = Mathf.Clamp(maxMinionCount - minions.Count, 0, minionSpawnCount);
        return StartCoroutine(Spawn(minionsToSpawn));
    }


    public Coroutine BuffHealth()
    {
        return StartCoroutine(BuffingHealth());
    }


    public Coroutine BuffShield()
    {
        return StartCoroutine(BuffingShield());
    }

    #endregion

    #region Private Methods

    private IEnumerator Spawn(int count)
    {
        if (count == 0)
        {
            yield break;
        }

        yield return new WaitForSeconds(deployTime);

        for (int i = 0; i < count; i++)
        {
            Vector3 spawn = myTransform.position + myTransform.TransformDirection(spawnPosition) + Utility.RotateVector(Random.Range(-1f, 1f) * spawnRadius, Random.Range(0f, 360f), Vector3.back);
            Ship minion = (Ship)Prefabs.Pop(minionPrefab, spawn, myTransform.rotation).GetComponent(typeof(Ship));
            minion.stateMachine.Start();
            minions.Add(minion.MyHealth);
            minion.MyHealth.DieEvent += OnMinionDeath;
        }

        yield return new WaitForSeconds(deployTime);
    }


    private IEnumerator BuffingHealth()
    {
        yield return new WaitForSeconds(buffingTime);
        foreach (Health minion in minions)
        {
            minion.health = minion.maxHealth;
            Prefabs.Pop(healthParticles, minion.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        }
        yield return new WaitForSeconds(buffingTime);
    }


    private IEnumerator BuffingShield()
    {
        yield return new WaitForSeconds(buffingTime);
        foreach (ShieldHealth minion in minions)
        {
            minion.maxShield = shieldBuff;
            minion.shield = shieldBuff;
            Prefabs.Pop(shieldParticles, minion.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        }
        yield return new WaitForSeconds(buffingTime);
        yield return new WaitForSeconds(buffingTime);
        yield return new WaitForSeconds(buffingTime);
        foreach (ShieldHealth minion in minions)
        {
            minion.maxShield = 0f;
            minion.shield = 0f;
            //Prefabs.Pop(shieldParticles, minion.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Called when a minion is killed.
    /// </summary>
    /// <param name="sender">Minion.</param>
    /// <param name="args">Death data.</param>
    private void OnMinionDeath(object sender, DieArgs args)
    {
        Health minion = (Health)sender;
        minions.Remove(minion);
        minion.DieEvent -= OnMinionDeath;
    }

    #endregion
}