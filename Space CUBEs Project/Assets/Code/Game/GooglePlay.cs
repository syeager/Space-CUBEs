// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.09
// Edited: 2014.09.09

using Annotations;
using UnityEngine;

public class GooglePlay : MonoBehaviour
{
    #region Private Fields

    [SerializeField, UsedImplicitly]
    private UITexture button;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Awake()
    {
    }

    #endregion

    #region Public Methods

    public void SignIn()
    {
    }

    #endregion

    #region Private Methods

    private void SignedIn(bool success)
    {
        Debug.Log("Signed in: " + success);

        if (success)
        {
            Debug.Log(Social.localUser.image.name);
            button.mainTexture = Social.localUser.image;

        }
    }

    #endregion
}