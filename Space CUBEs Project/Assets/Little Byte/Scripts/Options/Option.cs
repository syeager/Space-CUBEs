// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.01
// Edited: 2014.09.01

using System;

namespace LittleByte.Options
{
    [Serializable]
    public abstract class Option<T>
    {
        public abstract T Value { get; set; }
        public abstract T Random();
        public T startingValue;


        public virtual T Reset()
        {
            Value = startingValue;
            return startingValue;
        }
    }
}