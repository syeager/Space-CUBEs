// Little Byte Games
// Author: Steve Yeager
// Created: 2014.10.26
// Edited: 2014.10.26

using Annotations;
using UnityEngine;

namespace SpaceCUBEs
{
    public class ButtonWhite : MonoBehaviour
    {
        #region Private Fields

        [SerializeField, UsedImplicitly]
        private UISprite white;

        #endregion

        #region Public Methods

        public void SetColor(Color color)
        {
            white.color = color;
        }

        #endregion
    }
}