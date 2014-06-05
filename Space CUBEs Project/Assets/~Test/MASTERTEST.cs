// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.01.14
// Edited: 2014.06.02

using System.Collections.Generic;
using LittleByte.Data;
using UnityEngine;

//
public class MASTERTEST : MonoBehaviour
{
    public int tests;
    public Vector3 position;
    public Transform myTransform;
    public UILabel label;
    public List<int> testList; 

    private void Awake()
    {
        //position = SaveData.Load<Vector3>("Test Position", "Default", Vector3.one);
        //label.text = "Test: " + position;
        //SaveData.Save("Test Position", position + Vector3.one);


        testList = SaveData.Load<List<int>>("list", "Test");
        //using (var profiler = new LittleByte.Debug.Profiler("Saving"))
        //{
        //    SaveData.Save("list", testList, "Test");
        //    Debug.Log(SaveData.DeleteKey("list", "Test"));
        //    SaveData.Save("test", position, "Test");    
        //}


        //Debug.Log(SaveData.DeleteFile("Test"));
        //Debug.Log(SaveData.DeleteFile("Test"));
        //SaveData.DeleteFile("Default");
    }
}