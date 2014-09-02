// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.01
// Edited: 2014.09.01

namespace LittleByte.Options
{
    public class OptionBool : Option<bool>
    {
        #region Constructors

        public OptionBool(bool startingValue)
        {
            this.startingValue = startingValue;
        }

        #endregion

        #region Option Overrides

        public override bool Value { get; set; }


        public override bool Random()
        {
            return UnityEngine.Random.Range(0, 2) == 0;
        }

        #endregion
    } 
}