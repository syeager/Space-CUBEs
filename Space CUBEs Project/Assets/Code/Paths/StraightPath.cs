// Steve Yeager
// 1.26.2014

using UnityEngine;
using System;

namespace Paths
{
    public class StraightPath : Path
    {
        public float angle;
        public float rotation;

        private Vector3 direction;


        public override void Initialize(Transform transform)
        {
            base.Initialize(transform);

            myTransform.Rotate(Vector3.back, rotation, Space.World);
            direction = Utility.RotateVector(Vector3.left, Quaternion.AngleAxis(angle, Vector3.back))*speed;
        }

        public override Vector3 Direction(float deltaTime)
        {
            return direction*deltaTime;
        }
    }
}