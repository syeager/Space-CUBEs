// Little Byte Games

using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpaceCUBEs
{
    public class FormationGroupContainer : MonoBehaviour
    {
        public FormationGroup formationGroup;

        public void Set(FormationGroup group)
        {
            formationGroup = (FormationGroup)group.Clone(this, true);
        }

        public FormationGroup Get()
        {
            return (FormationGroup)formationGroup.Clone(this, false);
        }
    }
}