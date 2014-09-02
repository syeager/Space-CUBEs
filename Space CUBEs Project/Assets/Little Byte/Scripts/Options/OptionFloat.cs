// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.01
// Edited: 2014.09.01

using System;
using UnityEngine;

namespace LittleByte.Options
{
    [Serializable]
    public class OptionFloat : Option<float>
    {
        #region Public Fields

        public float min;
        public float max;

        #endregion

        #region Constructors

        public OptionFloat(float startingValue)
        {
            this.startingValue = startingValue;
        }

        #endregion

        #region Option Overrides

        private float value;

        public override float Value
        {
            get { return value; }
            set { this.value = Mathf.Clamp(value, min, max); }
        }


        public override float Random()
        {
            return UnityEngine.Random.Range(min, max);
        }

        #endregion
    }
}