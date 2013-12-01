// Steve Yeager
// 11.26.2013

using UnityEngine;

public class GameMaster : MonoBehaviour
{
    #region References

    private static GameMaster Main;

    #endregion


    #region Unity Overrides

    private void Awake()
    {
        if (Main == null)
        {
            Main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion
}