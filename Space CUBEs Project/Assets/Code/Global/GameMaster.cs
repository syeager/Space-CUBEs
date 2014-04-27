// Steve Yeager
// 11.26.2013

using Annotations;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    #region References

    private static GameMaster Main;

    #endregion

    #region Public Fields

    public int targetFPS = 60;

    #endregion


    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Awake()
    {
        if (Main == null)
        {
            Main = this;
            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = targetFPS;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion
}