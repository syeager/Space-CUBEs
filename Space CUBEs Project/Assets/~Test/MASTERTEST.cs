// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.01.14
// Edited: 2014.06.14

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using GameSaveData;
using LittleByte.Data;
using LittleByte.Debug.Attributes;
using UnityClasses;

//
using UnityEngine;

[InstanceCount]
public class MASTERTEST : MonoBehaviour
{
    public bool save;
    public bool load;

    public Dictionary<string, int> numbers;


    private void Start()
    {
        numbers = new Dictionary<string, int>
                  {
                      {"Sup", 7},
                      {"Stud", 10},
                  };
    }

    private void Update()
    {
        if (save)
        {
            save = false;
            SaveData.Save("Testing Dict", numbers);
        }

        if (load)
        {
            load = false;
            numbers = SaveData.Load<Dictionary<string, int>>("Testing Dict");
        }
    }
}