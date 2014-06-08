// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.01.14
// Edited: 2014.06.07

using System.Collections.Generic;
using GameSaveData;
using LittleByte.Data;
using UnityEngine;
using Profiler = LittleByte.Debug.Profiler;

//
public class MASTERTEST : MonoBehaviour
{
    public bool save;
    public bool load;
    public bool delete;

    public int tests;
    public SVector3 position;
    public int health;
    public Transform myTransform;
    public UILabel label;
    public List<int> testList;
    public BuildInfo buildInfo;
    public ConstructionGrid grid;

    private const string TestFolder = @"Tests\";


    private void Start()
    {
        using (var profiler = new Profiler("Saving PB"))
        {
            for (int i = 0; i < tests; i++)
            {
                SaveData.Save("health", health, TestFolder);
            }
        }

        using (var profiler = new Profiler("Loading PB"))
        {
            for (int i = 0; i < tests; i++)
            {
                health = SaveData.Load<int>("health", TestFolder);
            }
        }


        using (var profiler = new Profiler("Saving PP"))
        {
            for (int i = 0; i < tests; i++)
            {
                PlayerPrefs.SetInt("health", health);
            }
        }

        using (var profiler = new Profiler("Loading PP"))
        {
            for (int i = 0; i < tests; i++)
            {
                health = PlayerPrefs.GetInt("health");
            }
        }
    }


    private void Update()
    {
        if (save)
        {
            save = false;
            Save();
        }

        if (load)
        {
            load = false;
            Load();
        }

        if (delete)
        {
            delete = false;
            Delete();
        }
    }


    private void Delete()
    {
        SaveData.Delete("Number List", @"Bank\");
    }


    private void Load()
    {
        tests = SaveData.Load("Test", @"Default\", 10);
    }


    private void Save()
    {
        
    }
}