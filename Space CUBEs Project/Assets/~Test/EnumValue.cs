// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.16
// Edited: 2014.09.16

using UnityEngine;
using System;

public class EnumValue : MonoBehaviour
{
    public Type enumType;
    public int value;
    public enum EmptyEnum
    {
        
    }


    public void CheckValue()
    {
        if (enumType == null)
        {
            value = 0;
            return;
        }
        int length = Enum.GetNames(enumType).Length;
        if (value > length)
        {
            value = length - 1;
        }
        if (value < 0)
        {
            value = 0;
        }
    }
}