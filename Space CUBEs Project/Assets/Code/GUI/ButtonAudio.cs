// Little Byte Games

using UnityEngine;

namespace SpaceCUBEs
{
    public class ButtonAudio : MonoBehaviour
    {
        public GUIAudio.GUIType guiType;

        public void OnClick()
        {
            GUIAudio.Main.Play(guiType);
        }
    }
}