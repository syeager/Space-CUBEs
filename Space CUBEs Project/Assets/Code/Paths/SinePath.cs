// Steve Yeager
// 1.26.2014

using UnityEngine;
using System;

namespace Paths
{
    public class SinePath : Path
    {
        public float amplitude = 0.5f;
        public float frequency = 1f;
        private float time = 0f;


        public override Vector3 Direction(float deltaTime)
        {
            time += deltaTime;
            return Vector3.left + Vector3.up * Mathf.Sin(time * frequency) * amplitude;
        }
    }
}