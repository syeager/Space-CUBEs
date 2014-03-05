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
    
    public float backgroundSpeed;
    public float start;
    public float vertical;
    public float[] positions;
    public float[] speeds;
    public float[] minDelays;
    public float[] maxDelays;
    public float[] minSizes;
    public float[] maxSizes;
    public GameObject[] objects;
    
    #endregion

    #region Private Fields

    private Material material;
    private Transform myTransform;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        myTransform = transform;
        material = renderer.sharedMaterial;
        material.mainTextureOffset = Vector2.zero;

        for (int i = 0; i < positions.Length; i++)
        {
            StartCoroutine(Spawn(i));
        }
    }


    private void Update()
    {
        // background
        material.mainTextureOffset += Vector2.right * backgroundSpeed * deltaTime;
    }

    #endregion

    #region Private Methods

    private IEnumerator Spawn(int layer)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelays[layer], maxDelays[layer]));
            (Instantiate(objects[Random.Range(0, objects.Length)]) as GameObject).GetComponent<BackgroundObject>().Initialize(myTransform, new Vector3(start, Random.Range(-vertical, vertical), positions[layer]), Random.Range(minSizes[layer], maxSizes[layer]), speeds[layer]);
        }
    }

    #endregion
}