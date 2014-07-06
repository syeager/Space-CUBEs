// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.01.10
// Edited: 2014.07.02

using Annotations;
using UnityEngine;
using System.Collections;
using System;

public class LevelOverviewManager : MonoBehaviour
{
//    #region References

//    public UILabel Score;
//    public UILabel Money;
//    public UILabel Rank;
//    public UILabel[] Awards;

//    #endregion

//    #region Public Fields

//    public float scoreSpeed;
//    public float moneySpeed;
//    public ActivateButton[] navButtons;

//    #endregion

//    #region Private Fields

//    private float score;
//    private float money;

//    private enum States
//    {
//        Score,
//        Money,
//        Rank,
//        Awards,
//        Done
//    }

//    private States state;

//    #endregion

//    #region MonoBehaviour Methods

//    [UsedImplicitly]
//    private void Start()
//    {
//        state = States.Score;
//        StartCoroutine(Roundups(state));

//        // register events
//        foreach (ActivateButton button in navButtons)
//        {
//            button.ActivateEvent += (sender, args) => SceneManager.LoadScene(args.value);
//        }
//    }


//    [UsedImplicitly]
//    private void Update()
//    {
//#if UNITY_EDITOR
//        if (Input.GetKeyUp(KeyCode.Return))
//        {
//            Slam();
//        }
//#else
//        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && Input.GetTouch(0).tapCount >= 2)
//        {
//            Slam();
//        }
//#endif
//    }

//    #endregion

//    #region Public Methods

//    public void LoadGarage()
//    {
//        SceneManager.LoadScene("Garage");
//    }

//    #endregion

//    #region Private Methods

//    private IEnumerator Roundups(States start)
//    {
//        //if (start == States.Score)
//        {
//            yield return StartCoroutine(RoundupScore((int)SceneManager.Main.levelData["Score"]));
//            state = States.Money;
//        }
//        //if (start == States.Money)
//        {
//            yield return StartCoroutine(RoundupMoney((int)SceneManager.Main.levelData["Money"]));
//            state = States.Rank;
//        }
//        //if (start == States.Rank)
//        {
//            yield return StartCoroutine(RoundupRank((char)SceneManager.Main.levelData["Rank"]));
//            state = States.Awards;
//        }
//        //if (start == States.Awards)
//        {
//            yield return StartCoroutine(RoundupAwards((int[])SceneManager.Main.levelData["Awards"]));
//            state = States.Done;
//        }
//    }


//    private IEnumerator RoundupScore(int target)
//    {
//        while (score < target)
//        {
//            score += scoreSpeed * Time.deltaTime;
//            Score.text = String.Format("Score: {0:#,###0}", Mathf.RoundToInt(score));
//            yield return null;
//        }

//        score = target;
//        Score.text = String.Format("Score: {0:#,###0}", Mathf.RoundToInt(score));
//    }


//    private IEnumerator RoundupMoney(int target)
//    {
//        while (money < target)
//        {
//            money += moneySpeed * Time.deltaTime;
//            Money.text = String.Format("Money: {0:#,###0}", Mathf.RoundToInt(money));
//            yield return null;
//        }

//        money = target;
//        Money.text = String.Format("Money: {0:#,###0}", Mathf.RoundToInt(money));
//    }


//    private IEnumerator RoundupRank(char grade)
//    {
//        yield return new WaitForSeconds(1f);
//        Rank.text = "Rank: " + grade.ToString();
//    }


//    private IEnumerator RoundupAwards(int[] IDs)
//    {
//        for (int i = 0; i < IDs.Length; i++)
//        {
//            yield return new WaitForSeconds(0.25f);
//            string grade = " ";
//            for (int j = 0; j < CUBE.allCUBES[IDs[i]].rarity; j++)
//            {
//                grade += "★";
//            }
//            Awards[i].text = CUBE.allCUBES[IDs[i]].name + grade;
//        }
//    }


//    private void Slam()
//    {
//        StopAllCoroutines();

//        //switch (state)
//        //{
//        //    case States.Score:
//        Score.text = String.Format("Score: {0:#,###0}", Mathf.RoundToInt((int)SceneManager.Main.levelData["Score"]));
//        //    break;
//        //case States.Money:
//        Money.text = String.Format("Money: {0:#,###0}", Mathf.RoundToInt((int)SceneManager.Main.levelData["Money"]));
//        //    break;
//        //case States.Rank:
//        Rank.text = "Rank: " + ((char)SceneManager.Main.levelData["Rank"]).ToString();
//        //    break;
//        //case States.Awards:
//        int[] IDs = (int[])SceneManager.Main.levelData["Awards"];
//        for (int i = 0; i < IDs.Length; i++)
//        {
//            string grade = " ";
//            for (int j = 0; j < CUBE.allCUBES[IDs[i]].rarity; j++)
//            {
//                grade += "★";
//            }
//            Awards[i].text = CUBE.allCUBES[IDs[i]].name + grade;
//        }
//        //        break;
//        //}

//        //state++;
//        //if (state == States.Done) return;
//        //StartCoroutine(Roundups(state));
//    }

//    #endregion
}