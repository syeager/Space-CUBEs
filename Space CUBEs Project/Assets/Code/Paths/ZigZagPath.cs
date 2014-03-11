// Steve Yeager
// 1.26.2014

using UnityEngine;
using System;

namespace Paths
{
    public class ZigZagPath : Path
    {
        public Vector3 angle = new Vector3(-1f, 1f, 0f);
        public float time = 1.5f;
        private float lapTime;


        public override Vector3 Direction(float deltaTime)
        {
            lapTime += deltaTime;
            if (lapTime >= time)
            {
                lapTime = 0f;
                angle.y *= -1f;
            }

            return angle.normalized*speed*deltaTime;
        }
    }
}