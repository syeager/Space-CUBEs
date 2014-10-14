// Little Byte Games
// Author: Steve Yeager
// Created: 2014.10.12
// Edited: 2014.10.12

using SpaceCUBEs;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FormationGroupContainer : MonoBehaviour
{
    public FormationGroup formationGroup;


    public void Set(FormationGroup group)
    {
        formationGroup = Copy(group);
    }


    public FormationGroup Get()
    {
        return Copy(formationGroup);
    }


    private FormationGroup Copy(FormationGroup group)
    {
        var newGroup = new FormationGroup();
        newGroup.formation = group.formation;
        //EditorUtility.CopySerialized(group.formation, newGroup.formation);
        
        // formation positions
        //newGroup.formation.positions = new Vector3[group.formation.positions.Length];
        //for (int i = 0; i < newGroup.formation.positions.Length; i++)
        //{
        //    newGroup.formation.positions[i] = group.formation.positions[i];
        //}

        // position
        newGroup.position = group.position;

        // rotation
        newGroup.rotation = group.rotation;

        // enemies
        newGroup.enemies = new Enemy.Classes[group.enemies.Length];
        for (int i = 0; i < newGroup.enemies.Length; i++)
        {
            newGroup.enemies[i] = group.enemies[i];
        }

        // paths
        newGroup.paths = new Path[group.paths.Length];
        for (int i = 0; i < newGroup.paths.Length; i++)
        {
            Debug.Log(group.paths[i].GetType().Name);
            newGroup.paths[i] = (Path)ScriptableObject.CreateInstance(group.paths[i].GetType().Name);
            Debugger.NullCheck(newGroup.paths[i], "new");
            Debugger.NullCheck(group.paths[i], "source");
#if UNITY_EDITOR
            EditorUtility.CopySerialized(group.paths[i], newGroup.paths[i]);
#endif
            //newGroup.paths[i] = group.paths[i];
        }

        // needs clearing
        newGroup.needsClearing = group.needsClearing;

        // spawn time
        newGroup.spawnTime = group.spawnTime;

        return newGroup;
    }
}