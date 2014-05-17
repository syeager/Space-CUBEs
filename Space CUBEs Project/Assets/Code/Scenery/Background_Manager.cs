// Steve Yeager
// 3.3.2014

using Annotations;
using UnityEngine;

/// <summary>
/// Creates paralax affect with scrolling backgrounds.
/// </summary>
public class Background_Manager : MonoBase
{
    #region Public Fields

    public Material[] materials;
    public float[] speeds;

    #endregion


    #region MonoBehaviour Overrides

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
        foreach (var material in materials)
        {
            material.mainTextureOffset = Vector2.zero;
        }
    }
#endif

    #endregion
}