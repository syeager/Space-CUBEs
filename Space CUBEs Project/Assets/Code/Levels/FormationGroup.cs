// Little Byte Games

using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpaceCUBEs
{
    [Serializable]
    public class FormationGroup
    {
        public Formation formation;
        public Vector3 position;
        public float rotation;
        public Enemy.Classes[] enemies = new Enemy.Classes[1];
        public Path[] paths = new Path[1];
        public bool needsClearing;
        public float spawnTime;

#if UNITY_EDITOR

        public object Clone(FormationGroupContainer container, bool asset)
        {
            FormationGroup clone = new FormationGroup {formation = formation, position = position, rotation = rotation, needsClearing = needsClearing, spawnTime = spawnTime};

            // enemies
            clone.enemies = new Enemy.Classes[enemies.Length];
            Array.Copy(enemies, clone.enemies, enemies.Length);

            // paths
            clone.paths = new Path[paths.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i] == null) continue;

                Type pathType = paths[i].GetType();
                Path path = (Path)ScriptableObject.CreateInstance(pathType.Name);

                if (asset)
                {
                    AssetDatabase.AddObjectToAsset(path, container);
                }

                clone.paths[i] = path;
                FieldInfo[] fields = pathType.GetFields();
                foreach (FieldInfo field in fields)
                {
                    field.SetValue(clone.paths[i], field.GetValue(paths[i]));
                }
            }

            return clone;
        }
#endif
    }
}