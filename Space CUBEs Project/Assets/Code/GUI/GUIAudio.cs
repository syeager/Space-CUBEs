// Little Byte Games

using Annotations;
using LittleByte.Audio;
using UnityEngine;

namespace SpaceCUBEs
{
    public class GUIAudio : Singleton<GUIAudio>
    {
        #region References

        [SerializeField, UsedImplicitly]
        private Transform myTransform;

        #endregion

        #region Public Fields

        public enum GUIType
        {
            BasicButton,
            Cancel,
            Save,
            Confirm,
        }

        #endregion

        #region Private Fields

        [SerializeField, UsedImplicitly]
        private AudioPlayer[] players;

        #endregion

        #region Public Methods

        public void Play(GUIType guiType)
        {
            AudioPlayer player = AudioManager.Play(players[(int)guiType]);
            player.transform.parent = myTransform;
        }

        #endregion
    }
}