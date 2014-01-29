// Steve Yeager
// 1.26.2014

using UnityEngine;
using System;

namespace Paths
{
    public class StraightPath : Path
    {
        public override Vector3 Direction(float deltaTime)
        {
            return Vector3.left;
        }
    }
}