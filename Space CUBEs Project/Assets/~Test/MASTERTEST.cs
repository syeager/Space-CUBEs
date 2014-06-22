// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.01.14
// Edited: 2014.06.14

using System.Collections;
using System.Collections.Generic;
using GameSaveData;
using LittleByte.Data;
using LittleByte.Debug.Attributes;
using UnityClasses;

//
using UnityEngine;

public class MASTERTEST : MonoBehaviour
{
    public bool save;
    public bool load;
    public bool delete;
    public AudioPlayer clip;

    [NotNull]
    public GameObject testGameObject;

    [LittleByte.Debug.Attributes.Range(-5f, 5f)]
    public int test = -1;

    public string build;
    private const string TestFolder = @"Tests/";


    private void Start()
    {
        BuildInfo buildInfo = build;
        SaveData.Save(buildInfo.name, buildInfo, TestFolder);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            AudioManager.Play(clip);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(testGameObject);
        }
    }


    private void Delete()
    {
        delete = false;
    }


    private void Load()
    {
        load = false;
    }


    private void Save()
    {
        save = false;
    }
}