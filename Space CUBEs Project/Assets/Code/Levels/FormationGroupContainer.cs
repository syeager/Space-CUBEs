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
#if UNITY_EDITOR
            formationGroup = (FormationGroup)group.Clone(this, true);
#endif
        }

        public FormationGroup Get()
        {
#if UNITY_EDITOR
            return (FormationGroup)formationGroup.Clone(this, false);
#endif
            return null;
        }
    }
}