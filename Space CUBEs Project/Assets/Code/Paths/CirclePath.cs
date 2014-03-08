// Steve Yeager
// 1.26.2014

using UnityEngine;
using System;

namespace Paths
{
    public class CirclePath : Path
    {
        public float radius = 5f;
        public float speed = 2f;
        private Vector3 ray = Vector3.up;

        public override void Initialize(Transform transform)
        {
            base.Initialize(transform);
            myTransform.position += Vector3.left * radius;
        }


        public override Vector3 Direction(float deltaTime)
        {

            return (Vector3.left + ray).normalized;
        }
    }
}