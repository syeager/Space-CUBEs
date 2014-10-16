// Little Byte Games
// Author: Steve Yeager
// Created: 2014.07.06
// Edited: 2014.10.14

using System.Collections;
using System.Collections.Generic;
using Annotations;
using LittleByte;
using LittleByte.Debug.Attributes;
using UnityEngine;

namespace SpaceCUBEs
{
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

        [NotNull]
        public UILabel timeLabel;

        [NotNull]
        public UILabel lootLabel;

        /// <summary>Points in seconds for rolling up loot.</summary>
        public float lootRollup;

        [NotEmpty]
        public UILabel[] salvageLabels;

        /// <summary>Time in seconds to delay displaying each piece of salvage.</summary>
        public float salvageDelay;

        public ActivateButton replayButton;

        public ActivateButton nextButton;

        #endregion

        #region Private Fields

        private StateMachine states;

        private bool scoreCompleted;

        private bool lastLevel;

        [Header("Ranks")]
        [NotEmpty]
        [SerializeField, UsedImplicitly]
        private UISprite[] rankMedals;

        [SerializeField, UsedImplicitly]
        private UITexture rankLetter;

        [SerializeField, UsedImplicitly]
        private float rankHeightStart = 2.01f;

        [SerializeField, UsedImplicitly]
        private float rankHeight = 0.1675f;

        [SerializeField, UsedImplicitly]
        private float rollBackSpeed;

        [SerializeField, UsedImplicitly]
        private float rollBackDelay;

        [SerializeField, UsedImplicitly]
        private AnimationCurve rankIntroCurve;

        [SerializeField, UsedImplicitly]
        private float medalSize;

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
        private const string TimeState = "Time";
        private const string LootState = "Loot";
        private const string SalvageState = "Salvage";
        private const string IdleState = "Idle";
        private const string CompleteState = "Complete";

        #endregion

        #region State Methods

        private void InitializingEnter(Dictionary<string, object> info = null)
        {
            gameObject.SetActive(true);

            // score
            StartCoroutine(ScoreRollup());

            StartCoroutine(Slam());

            states.SetState(TimeState);
        }


        private IEnumerator LootUpdate()
        {
            float cursor = 0f;
            while (cursor < playerLoot)
            {
                cursor += lootRollup * deltaTime;
                lootLabel.text = Mathf.FloorToInt(cursor).ToString("N0");

                yield return null;
            }
            cursor = playerLoot;
            lootLabel.text = Mathf.FloorToInt(cursor).ToString("N0");

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
            replayButton.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(true);
            nextButton.isEnabled = !lastLevel;
            NavigationBar.Show(true);

            // loot
            lootLabel.text = playerLoot.ToString("N0");

            // salvage
            for (int i = 0; i < playerSalvage.Length; i++)
            {
                salvageLabels[i].text = CUBE.AllCUBES[playerSalvage[i]].name;
            }

            // score
            scoreLabel.text = playerScore.ToString("N0");

            // rank
            rankLetter.uvRect = new Rect(0f, rankHeightStart + playerRank * rankHeight, 1f, 0.1459f);

            // symbol
            for (int i = 1; i < rankMedals.Length; i++)
            {
                rankMedals[i].gameObject.SetActive(i == playerRank);
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(Highscore score, int[] ranks, float loot, int[] salvage)
        {
            // cache data
            playerScore = score.score;
            rankThresholds = ranks;
            playerRank = score.rank;
            playerLoot = loot;
            playerSalvage = salvage;

            // buttons
            replayButton.ActivateEvent += (sender, args) => SceneManager.ReloadScene();
            int nextLevel = ((FormationLevelManager)FormationLevelManager.Main).levelIndex + 1;
            if (nextLevel < FormationLevelManager.LevelNames.Length)
            {
                nextButton.ActivateEvent += (sender, args) => SceneManager.ReloadScene();
            }
            else
            {
                lastLevel = true;
            }

            states = new StateMachine(this, InitializingState);
            states.CreateState(InitializingState, InitializingEnter, info => { });
            states.CreateState(TimeState, info =>
                                          {
                                              timeLabel.text = score.TimeString;
                                              states.SetState(LootState);
                                          }, info => { });
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

            // snap to D rank
            if (playerRank > 0)
            {
                UpdateRank(1, rankThresholds[1]); 
                rankLetter.uvRect = new Rect(0f, rankHeightStart + rankHeight, 1f, rankLetter.uvRect.height);
                StartCoroutine(NewRank(1));
            }

            // roll up
            while (scoreCursor < playerScore)
            {
                scoreCursor += scoreRollup * deltaTime;
                scoreLabel.text = Mathf.FloorToInt(scoreCursor).ToString("N0");

                rankCursor = UpdateRank(rankCursor, scoreCursor);

                yield return null;
            }

            // snap
            scoreCursor = playerScore;
            scoreLabel.text = Mathf.FloorToInt(scoreCursor).ToString("N0");

            // roll back
            yield return new WaitForSeconds(rollBackDelay);
            while (scoreCursor > rankThresholds[rankCursor])
            {
                scoreCursor -= rollBackSpeed * deltaTime;

                UpdateRank(rankCursor, scoreCursor);

                yield return null;
            }

            scoreCompleted = true;
            if (states.IsCurrentState(IdleState))
            {
                states.SetState(CompleteState);
            }
        }


        private int UpdateRank(int rankCursor, float scoreCursor)
        {
            // rank
            if (playerRank > 0 && rankCursor < rankThresholds.Length - 1 && scoreCursor > rankThresholds[1])
            {
                float scoreProgress = scoreCursor;
                scoreProgress -= rankThresholds[rankCursor];
                float rankProgress = rankThresholds[rankCursor + 1];
                rankProgress -= rankThresholds[rankCursor];

                float rankPercent = scoreProgress / rankProgress;

                rankLetter.uvRect = new Rect(0f, rankHeightStart + rankCursor * rankHeight + rankHeight * rankPercent, 1f, rankLetter.uvRect.height);

                // new rank
                if (scoreCursor >= rankThresholds[rankCursor + 1])
                {
                    rankCursor++;
                    StartCoroutine(NewRank(rankCursor));
                }
            }

            return rankCursor;
        }


        private IEnumerator NewRank(int rank)
        {
            if (rankMedals[rank].gameObject.activeInHierarchy) yield break;

            rankMedals[rank].width = rankMedals[rank].height = Mathf.RoundToInt(medalSize * rankIntroCurve.Evaluate(0));
            rankMedals[rank].gameObject.SetActive(true);
            //rankMedals[rank].Play();

            float time = rankIntroCurve[rankIntroCurve.length - 1].time;
            float timer = 0f;
            while (timer < time)
            {
                timer += Time.deltaTime;
                rankMedals[rank].width = rankMedals[rank].height = Mathf.RoundToInt(medalSize * rankIntroCurve.Evaluate(timer));
                yield return null;
            }
                rankMedals[rank].width = rankMedals[rank].height = Mathf.RoundToInt(medalSize * rankIntroCurve.Evaluate(time));

            //yield return new WaitForSeconds(rankMedals[rank].clip.length);

            if (rank > 1)
            {
                rankMedals[rank - 1].gameObject.SetActive(false);
            }
        }


        private IEnumerator Slam()
        {
            while (true)
            {
                if (Input.GetKeyUp(KeyCode.Escape) || Input.GetButtonDown("Pause") || Input.touchCount > 0 && Input.GetTouch(0).tapCount >= 2)
                {
                    states.SetState(CompleteState);
                    yield break;
                }

                yield return null;
            }
        }

        #endregion
    }
}