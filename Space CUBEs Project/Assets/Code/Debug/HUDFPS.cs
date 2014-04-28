using Annotations;
using UnityEngine;

public class HUDFPS : MonoBehaviour
{
    #region References

    private GUIText myGUIText;

    #endregion


    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        myGUIText = guiText;
    }

    #endregion

    #region Public Methods

    public void UpdateFPS(float fps)
    {
        string format = System.String.Format("{0:F2} FPS", fps);
        myGUIText.text = format;

        if (fps < 30 && fps > 10)
        {
            myGUIText.material.color = Color.yellow;
        }
        else if (fps < 10)
        {
            myGUIText.material.color = Color.red;
        }
        else
        {
            myGUIText.material.color = Color.green;
        }
    }

    #endregion
}