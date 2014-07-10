// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.07.06
// Edited: 2014.07.08

using System.Collections;
using System.Collections.Generic;
using Annotations;
using LittleByte.Debug.Attributes;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class CampaignOverview : MonoBase
{
    #region Public Fields

    [NotNull]
    public UILabel scoreLabel;

    /// <summary>Points in seconds for rolling up score.</summary>
    public float scoreRollup;

    [NotEmpty]
    public GameObject[] rankSymbols;

    [NotEmpty]
    public UIWidget[] ranks;

    [NotNull]
    public UILabel lootLabel;

    /// <summary>Points in seconds for rolling up loot.</summary>
    public float lootRollup;

    [NotEmpty]
    public UILabel[] salvageLabels;

    /// <summary>Time in seconds to delay displaying each piece of salvage.</summary>
    public float salvageDelay;

    public ActivateButton mainMenuButton;

    public ActivateButton garageButton;

    public ActivateButton storeButton;

    public ActivateButton replayButton;

    public ActivateButton nextButton;

    #endregion

    #region Private Fields

    private StateMachine states;

    private bool scoreCompleted;

    private bool lastLevel;

    #endregion

    #region Data Fields

    private float playerScore;
    private int playerRank;
    private int[] rankThresholds;
    private float playerLoot;
    private int[] playerSalvage;

    #endregion

    #region State Names

    private const string InitializingState = "Initializing";
    private const string LootState = "Loot";
    private const string SalvageState = "Salvage";
    private const string IdleState = "Idle";
    private const string CompleteState = "Complete";

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Update()
    {
        if (states.IsCurrentState(CompleteState)) return;

        if (Skip())
        {
            states.SetState(CompleteState);
        }
    }

    #endregion

    #region State Methods

    private void InitializingEnter(Dictionary<string, object> info = null)
    {
        gameObject.SetActive(true);

        // score
        StartCoroutine(ScoreRollup());

        states.SetState(LootState);
    }


    private IEnumerator LootUpdate()
    {
        float cursor = 0f;
        while (cursor < playerLoot)
        {
            cursor += lootRollup * deltaTime;
            lootLabel.text = Mathf.FloorToInt(cursor).ToString();

            yield return null;
        }
        cursor = playerLoot;
        lootLabel.text = Mathf.FloorToInt(cursor).ToString();

        states.SetState(SalvageState);
    }


    private IEnumerator SalvageUpdate()
    {
        WaitForSeconds wait = new WaitForSeconds(salvageDelay);
        for (int i = 0; i < playerSalvage.Length; i++)
        {
            yield return wait;
            salvageLabels[i].text = CUBE.AllCUBES[playerSalvage[i]].name;
        }
        yield return wait;

        states.SetState(scoreCompleted ? CompleteState : IdleState);
    }


    private void CompleteEnter(Dictionary<string, object> info = null)
    {
        StopAllCoroutines();

        // show buttons
        mainMenuButton.gameObject.SetActive(true);
        garageButton.gameObject.SetActive(true);
        storeButton.gameObject.SetActive(true);
        replayButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        nextButton.isEnabled = !lastLevel;

        // loot
        lootLabel.text = playerLoot.ToString();

        // salvage
        for (int i = 0; i < playerSalvage.Length; i++)
        {
            salvageLabels[i].text = CUBE.AllCUBES[playerSalvage[i]].name;
        }

        // score
        scoreLabel.text = playerScore.ToString();

        // rank
        for (int i = 0; i < ranks.Length; i++)
        {
            ranks[i].gameObject.SetActive(i == playerRank);
            if (i == playerRank)
            {
                ranks[i].color = Color.white;
            }
        }

        // symbol
        for (int i = 0; i < rankSymbols.Length; i++)
        {
            rankSymbols[i].gameObject.SetActive(i == playerRank);
        }
    }

    #endregion

    #region Public Methods

    public void Initialize(float score, int[] ranks, int rank, float loot, int[] salvage)
    {
        // cache data
        playerScore = score;
        rankThresholds = ranks;
        playerRank = rank;
        playerLoot = loot;
        playerSalvage = salvage;

        // buttons
        mainMenuButton.ActivateEvent += (sender, args) => SceneManager.LoadScene(SceneManager.MainMenu, true, true, true);
        garageButton.ActivateEvent += (sender, args) => SceneManager.LoadScene(SceneManager.Garage, true, true, true);
        storeButton.ActivateEvent += (sender, args) => SceneManager.LoadScene(SceneManager.Store, true, true, true);
        replayButton.ActivateEvent += (sender, args) => SceneManager.ReloadScene();
        int nextLevel = ((FormationLevelManager)FormationLevelManager.Main).levelIndex + 1;
        if (nextLevel < FormationLevelManager.LevelNames.Length)
        {
            nextButton.ActivateEvent += (sender, args) => SceneManager.LoadScene(FormationLevelManager.LevelNames[nextLevel]);
        }
        else
        {
            lastLevel = true;
        }

        states = new StateMachine(this, InitializingState);
        states.CreateState(InitializingState, InitializingEnter, info => { });
        states.CreateState(LootState, info => states.SetUpdate(LootUpdate()), info => { });
        states.CreateState(SalvageState, info => states.SetUpdate(SalvageUpdate()), info => { });
        states.CreateState(IdleState, info => { }, info => { });
        states.CreateState(CompleteState, CompleteEnter, info => { });
        states.Start();
    }

    #endregion

    #region Private Methods

    private IEnumerator ScoreRollup()
    {
        float scoreCursor = 0f;
        int rankCursor = 0;
        while (scoreCursor < playerScore)
        {
            scoreCursor += scoreRollup * deltaTime;

            // label
            scoreLabel.text = Mathf.FloorToInt(scoreCursor).ToString();

            // rank
            if (playerRank > 0 && rankCursor < rankThresholds.Length - 1)
            {
                float scoreProgress = scoreCursor;
                if (rankCursor > 0)
                {
                    scoreProgress -= rankThresholds[rankCursor - 1];
                }
                float rankProgress = rankThresholds[rankCursor];
                if (rankCursor > 0)
                {
                    rankProgress -= rankThresholds[rankCursor - 1];
                }

                float rankPercent = scoreProgress / rankProgress;
                ranks[rankCursor].color = new Color(1f, 1f, 1f, 1f - rankPercent);
                ranks[rankCursor + 1].color = new Color(1f, 1f, 1f, rankPercent);

                // new rank
                if (scoreCursor > rankThresholds[rankCursor])
                {
                    // rank
                    ranks[rankCursor].gameObject.SetActive(false);
                    ranks[rankCursor + 1].color = Color.white;

                    // symbol
                    rankSymbols[rankCursor].SetActive(false);
                    rankCursor++;
                    rankSymbols[rankCursor].SetActive(true);
                }
            }

            yield return null;
        }
        scoreCursor = playerScore;

        // label
        scoreLabel.text = Mathf.FloorToInt(scoreCursor).ToString();

        // rank
        ranks[rankCursor + 1].gameObject.SetActive(false);
        ranks[rankCursor].color = Color.white;

        scoreCompleted = true;
        if (states.IsCurrentState(IdleState))
        {
            states.SetState(CompleteState);
        }
    }


    private static bool Skip()
    {
#if UNITY_STANDALONE
        return Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Escape);
#else
        return Input.GetKeyUp(KeyCode.Escape) || (Input.touchCount > 0 && Input.GetTouch(0).tapCount >= 2);
#endif
    }

    #endregion
}