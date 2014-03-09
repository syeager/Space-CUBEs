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

    public Material back_Mat;
    public float backgroundSpeed;
    public Material star_Mat;
    public float starSpeed;
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

    private Transform myTransform;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        myTransform = transform;
        back_Mat.mainTextureOffset = Vector2.zero;
    }


    private void Start()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            StartCoroutine(Spawn(i));
        }
    }


    private void Update()
    {
        // background
        back_Mat.mainTextureOffset += Vector2.right * backgroundSpeed * deltaTime;

        // stars
        star_Mat.mainTextureOffset += Vector2.right * starSpeed * deltaTime;
    }

    #endregion

    #region Private Methods

    private IEnumerator Spawn(int layer)
    {
        while (true)
        {
            PoolManager.Pop(objects[Random.Range(0, objects.Length)],
                            new Vector3(start, Random.Range(-vertical, vertical), positions[layer]),
                            Quaternion.identity)
                                .GetComponent<BackgroundObject>().Initialize(Random.Range(minSizes[layer], maxSizes[layer]), speeds[layer]);

            yield return new WaitForSeconds(Random.Range(minDelays[layer], maxDelays[layer]));
        }
    }

    #endregion
}