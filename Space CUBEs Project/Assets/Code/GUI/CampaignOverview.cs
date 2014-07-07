// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.07.06
// Edited: 2014.07.06

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
    public UISprite[] ranks;

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

    private bool completed;

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

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Update()
    {
        if (completed) return;

        if (Skip())
        {
            Complete();
        }
    }

    #endregion

    #region State Methods

    private void InitializingEnter(Dictionary<string, object> info = null)
    {
        gameObject.SetActive(true);

        // score
        StartCoroutine(Rollup(playerScore, scoreRollup, scoreLabel));

        // rank
        StartCoroutine(IncreaseRank());

        states.SetState(LootState);
    }


    private void LootEnter(Dictionary<string, object> info = null)
    {
        states.SetUpdate(Rollup(playerLoot, lootRollup, lootLabel)).JobCompleteEvent += killed => states.SetState(SalvageState);
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
    }


    private void IdleEnter(Dictionary<string, object> info = null)
    {
        // show buttons
        mainMenuButton.gameObject.SetActive(true);
        garageButton.gameObject.SetActive(true);
        storeButton.gameObject.SetActive(true);
        replayButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        nextButton.isEnabled = !lastLevel;
    }

    #endregion

    #region Public Methods

    public void Initialize(float score, int[] ranks, int rank, float loot, int[] salvage)
    {
        // cache data
        playerScore = score;
        playerRank = rank;
        rankThresholds = ranks;
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
        states.CreateState(LootState, LootEnter, info => { });
        states.CreateState(SalvageState, info => states.SetUpdate(SalvageUpdate()), info => { });
        states.CreateState(IdleState, IdleEnter, info => { });
        states.Start();
    }

    #endregion

    #region Private Methods

    private IEnumerator Rollup(float target, float speed, UILabel label)
    {
        float cursor = 0f;
        while (cursor < target)
        {
            cursor += speed * deltaTime;
            label.text = Mathf.FloorToInt(cursor).ToString();

            yield return null;
        }
        cursor = target;
        label.text = Mathf.FloorToInt(cursor).ToString();
    }


    private IEnumerator IncreaseRank()
    {
        yield return null;
    }


    private static bool Skip()
    {
#if UNITY_STANDALONE
        return Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Escape);
#else
        return Input.GetKeyUp(KeyCode.Escape) || (Input.touchCount > 0 && Input.GetTouch(0).tapCount >= 2);
#endif
    }


    private void Complete()
    {
        completed = true;

        // loot


        // salvage


        // score


        // rank


        if (states.currentState != IdleState)
        {
            states.SetState(IdleState);
        }
    }

    #endregion
}