// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.27
// Edited: 2014.06.28

using Annotations;
using UnityEngine;

/// <summary>
/// Plays particle system on enable and disables when particle system is done.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class ParticlePoolObject : PoolObject
{
    #region References

    [SerializeField, HideInInspector]
    private ParticleSystem particles;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Reset()
    {
        particles = particleSystem;
    }


    [UsedImplicitly]
    private void OnEnable()
    {
        particles.Play();
        StartLifeTimer(particles.duration);
    }

    #endregion
}