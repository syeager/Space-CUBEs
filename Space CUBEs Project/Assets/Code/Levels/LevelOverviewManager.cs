// Steve Yeager
// 1.5.2013

using UnityEngine;
using System.Collections;
using System;

public class LevelOverviewManager : MonoBehaviour
{
    #region References

    public UILabel Score;
    public UILabel Money;
    public UILabel Rank;

    #endregion

    #region Public Fields

    public float scoreSpeed;
    public float moneySpeed;

    #endregion

    #region Private Fields

    private float score;
    private float money;

    #endregion


    #region MonoBehaviour Methods

    private void Start()
    {
        StartCoroutine(Roundups());
    }

    #endregion

    #region Public Methods

    public void LoadGarage()
    {
        GameData.LoadLevel("Garage");
    }

    #endregion

    #region Private Methods

    private IEnumerator Roundups()
    {
        yield return StartCoroutine(RoundupScore((int)GameData.Main.levelData["Score"]));
        yield return StartCoroutine(RoundupMoney((int)GameData.Main.levelData["Money"]));
        StartCoroutine(RoundupRank('A'));
    }


    private IEnumerator RoundupScore(int target)
    {
        while (score < target)
        {
            score += scoreSpeed * Time.deltaTime;
            Score.text = String.Format("Score: {0:#,###0}", Mathf.RoundToInt(score));
            yield return null;
        }

        score = target;
        Score.text = String.Format("Score: {0:#,###0}", Mathf.RoundToInt(score));
    }


    private IEnumerator RoundupMoney(int target)
    {
        while (money < target)
        {
            money += moneySpeed * Time.deltaTime;
            Money.text = String.Format("Money: {0:#,###0}", Mathf.RoundToInt(money));
            yield return null;
        }

        money = target;
        Money.text = String.Format("Money: {0:#,###0}", Mathf.RoundToInt(money));
    }


    private IEnumerator RoundupRank(char grade)
    {
        yield return new WaitForSeconds(1f);
        Rank.text = "Rank: " + GameData.Main.levelData["Rank"].ToString();
    }

    #endregion
}