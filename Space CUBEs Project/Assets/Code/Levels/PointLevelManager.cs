// Steve Yeager
// 12.17.2013

using System.Collections;
using UnityEngine;

/// <summary>
/// Creates random waves based on points.
/// </summary>
public class PointLevelManager : LevelManager
{
    #region Public Fields

    public int startingPoints;
    public float wavePointMultiplier;

    public float minDelay;
    public float maxDelay;
    public float screenBufferPer;
    public float HUDHeight;
    public float xStart = 50f;

    #endregion

    #region Private Fields

    private int currentWave;
    private Job spawnCycle;
    private float screenTop;
    private float screenBottom;

    #endregion

    #region Properties

    private int wavePoints { get { return Mathf.CeilToInt(startingPoints * currentWave * wavePointMultiplier); } }

    #endregion


    #region MonoBehaviour Overrides

    protected override void Start()
    {
        base.Start();
        
        UpdateScreen();
        spawnCycle = new Job(SpawnCycle());
    }


    private void OnDestroy()
    {
        spawnCycle.Kill();
    }

    #endregion

    #region Private Methods

    private void UpdateScreen()
    {
        float bottom = Screen.height*screenBufferPer;
        float height = Screen.height * HUDHeight;
        screenBottom = Camera.main.ScreenToWorldPoint(new Vector3(0f, bottom+height, 0f)).y;
        screenTop = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height - bottom, 0f)).y;
    }


    private IEnumerator SpawnCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

            var enemy = PoolManager.Pop("Grunt", 13f);
            enemy.transform.SetPosRot(xStart, Random.Range(screenBottom, screenTop), 0f, 0f, 270f, 90f);
            enemy.GetComponent<Enemy>().Spawn();
        }
    }

    #endregion
}