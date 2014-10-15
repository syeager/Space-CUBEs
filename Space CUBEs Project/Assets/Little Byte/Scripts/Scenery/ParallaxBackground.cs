// Little Byte Games
// Author: Steve Yeager
// Created: 2014.03.03
// Edited: 2014.10.14

using Annotations;
using UnityEngine;

/// <summary>
/// Creates paralax affect with scrolling backgrounds.
/// </summary>
public class ParallaxBackground : MonoBase
{
    #region Private Fields

    [SerializeField, UsedImplicitly]
    private Renderer[] renderers;

    private Material[] materials;

    [SerializeField, UsedImplicitly]
    private float[] speeds;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Awake()
    {
        materials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].material;
        }
    }


    [UsedImplicitly]
    private void Update()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].mainTextureOffset += Vector2.right * speeds[i] * deltaTime;
        }
    }


#if UNITY_EDITOR
    [UsedImplicitly]
    private void OnApplicationQuit()
    {
        foreach (Material material in materials)
        {
            material.mainTextureOffset = Vector2.zero;
        }
    }
#endif

    #endregion
}