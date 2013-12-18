// Steve Yeager
// 12.17.2013

using UnityEngine;

/// <summary>
/// Creates random waves based on points.
/// </summary>
public class PointLevelManager : MonoBehaviour
{
    #region Public Fields

    public int startingPoints;
    public float wavePointMultiplier;

    #endregion

    #region Private Fields

    private int currentWave;

    #endregion

    #region Properties

    private int wavePoints { get { return Mathf.CeilToInt(startingPoints * currentWave * wavePointMultiplier); } }

    #endregion


    #region Private Methods

    private void CreateRandomWave(int points)
    {

    }

    #endregion
}