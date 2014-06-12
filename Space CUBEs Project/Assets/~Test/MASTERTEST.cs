// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.01.14
// Edited: 2014.06.08

using System.Collections;
using System.Collections.Generic;
using GameSaveData;
using LittleByte.Data;
using UnityClasses;
using UnityEngine;

//
public class MASTERTEST : MonoBehaviour
{
    public bool save;
    public bool load;
    public bool delete;

    public int tests;
    public Vector3 position;
    public int health;
    public Transform myTransform;
    public UILabel label;
    public List<Vector3> testList;
    public BuildInfo buildInfo;
    public CUBEGridInfo cubeInfo;


    private const string TestFolder = @"Tests\";


    private void Start()
    {
        buildInfo = ConstructionGrid.Load("Avenger");
    }


    private void Update()
    {
        if (save)
        {
            save = false;
            using (var profiler = new LittleByte.Debug.Profiler("Save"))
            {
                for (int i = 0; i < tests; i++) Save();
            }
        }

        if (load)
        {
            load = false;
            using (var profiler = new LittleByte.Debug.Profiler("Load"))
            {
                for (int i = 0; i < tests; i++) Load();
            }
        }

        if (delete)
        {
            delete = false;
            Delete();
        }
    }


    private void Delete()
    {
        //bool test = testList is IList;
    }


    private void Load()
    {
        //buildInfo = SaveData.Load<BuildInfo>("BuildInfo");
        //cubeInfo = SaveData.Load<CUBEGridInfo>("CUBEInfo");
        //position = SaveData.Load<Vector3>("position");
        testList = SaveData.Load<List<Vector3>>("testList");
        //health = SaveData.Load<int>("health");
    }


    private void Save()
    {
        //SaveData.Save("BuildInfo", buildInfo);
        //SaveData.Save("CUBEInfo", cubeInfo);
        //SaveData.Save("position", position);
        SaveData.Save("testList", testList);
        //Protobuf.Save("testList", testList);
        //SaveData.Save("int", 5);
        //SaveData.Save("health", health);
    }
}