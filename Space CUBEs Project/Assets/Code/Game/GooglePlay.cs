// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.09
// Edited: 2014.10.02

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
            }
            else
            {
                autoSignIn = SaveData.Load(AutoSignInKey, SocialPath, true);

                if (autoSignIn)
                {
                    Social.localUser.Authenticate(SignedIn);
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
                ((PlayGamesPlatform)Social.Active).SignOut();
                button.mainTexture = googlePlus;
                playerImage = null;
            }
            else
            {
                // sign in
                Social.localUser.Authenticate(SignedIn);
            }
        }

        #endregion

        #region Private Methods

        private void SignedIn(bool success)
        {
            Debugger.Log("Signed in: " + success, this, Debugger.LogTypes.Social);

            if (success)
            {
                GoogleProfilePic.LoadProfilePic(Social.localUser.id, size, texture =>
                                                                           {
                                                                               playerImage = texture;
                                                                               button.mainTexture = texture;
                                                                           });
                autoSignIn = true;
            }
            else
            {
                playerImage = null;
                autoSignIn = false;
            }

            SaveData.Save(AutoSignInKey, autoSignIn, SocialPath);
        }

        #endregion
    }
}