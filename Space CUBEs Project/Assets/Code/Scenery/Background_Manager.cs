// Steve Yeager
// 3.3.2014

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class Background_Manager : MonoBase
{
    #region Public Fields

    public Material[] materials;
    public float[] speeds;

    #endregion


    #region MonoBehaviour Overrides

    private void Update()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].mainTextureOffset += Vector2.right * speeds[i] * deltaTime;
        }
    }


    private void OnApplicationQuit()
    {
        foreach (var material in materials)
        {
            material.mainTextureOffset = Vector2.zero;
        }
    }

    #endregion
}