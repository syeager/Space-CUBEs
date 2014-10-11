// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.09
// Edited: 2014.10.11

using Annotations;
using GooglePlayGames;
using LittleByte.Data;
using UnityEngine;

namespace LittleByte.GooglePlay
{
    public class GooglePlay : MonoBehaviour
    {
        #region Private Fields

        [SerializeField, UsedImplicitly]
        private UITexture button;

        [SerializeField, UsedImplicitly]
        private Texture2D googlePlus;

        [SerializeField, UsedImplicitly]
        private int size;

        [SerializeField, UsedImplicitly]
        private GameObject[] googlePlayButtons;

        #endregion

        #region Static Fields

        private static Texture2D playerImage;
        private static bool autoSignIn;

        #endregion

        #region Const Fields

        private const string SocialPath = @"Social/";
        private const string AutoSignInKey = "Auto Sign In";

        #endregion

        #region MonoBehaviour Overrides

        [UsedImplicitly]
        private void Awake()
        {
            PlayGamesPlatform.Activate();

#if UNITY_ANDROID || UNITY_EDITOR
            if (Social.localUser.authenticated)
            {
                button.mainTexture = playerImage;

                foreach (GameObject playButton in googlePlayButtons)
                {
                    playButton.SetActive(true);
                }
            }
            else
            {
                autoSignIn = SaveData.Load(AutoSignInKey, SocialPath, true);

                if (autoSignIn)
                {
                    Social.localUser.Authenticate(SignedIn);
                }
                else
                {
                    SignInFail();
                }
            }
#else
            Destroy(gameObject);
#endif
        }

        #endregion

        #region Public Methods

        public void SignIn()
        {
            if (Social.localUser.authenticated)
            {
                // sign out
                GoogleCloud.Main.Save();
                ((PlayGamesPlatform)Social.Active).SignOut();
                SignedIn(false);
            }
            else
            {
                // sign in
                Social.localUser.Authenticate(SignedIn);
            }
        }


        public void Achievements()
        {
            Social.ShowAchievementsUI();
        }


        public void Leaderboards()
        {
            Social.ShowLeaderboardUI();
        }

        #endregion

        #region Private Methods

        private void SignedIn(bool success)
        {
            Debugger.Log("Signed in: " + success, this, Debugger.LogTypes.Social);

            if (success)
            {
                SignInSuccess();
            }
            else
            {
                SignInFail();
            }

            SaveData.Save(AutoSignInKey, autoSignIn, SocialPath);
        }


        private void SignInSuccess()
        {
            GoogleProfilePic.LoadProfilePic(Social.localUser.id, size, texture =>
                                                                       {
                                                                           playerImage = texture;
                                                                           button.mainTexture = texture;
                                                                       });

            foreach (GameObject playButton in googlePlayButtons)
            {
                playButton.SetActive(true);
            }

            autoSignIn = true;

            GoogleCloud.Main.Load();
        }


        private void SignInFail()
        {
            button.mainTexture = googlePlus;

            foreach (GameObject playButton in googlePlayButtons)
            {
                playButton.SetActive(false);
            }

            playerImage = null;
            autoSignIn = false;
        }

        #endregion
    }
}