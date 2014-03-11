// Steve Yeager
// 1.26.2014

using UnityEngine;
using System;

namespace Paths
{
    public class CirclePath : Path
    {
        public float angle;
        public Vector3 center;
        public float angularSpeed = 1f;

        private Vector3 angularPosition;
        private Vector3 direction;
        private Vector3 lastTarget;


        public override void Initialize(Transform transform)
        {
            base.Initialize(transform);
            direction = Utility.RotateVector(Vector3.left, Quaternion.AngleAxis(angle, Vector3.back))*speed;
            angularPosition = -center;
            center += myTransform.position;
            lastTarget = myTransform.position;
        }


        public override Vector3 Direction(float deltaTime)
        {
            center += direction*deltaTime;
            angularPosition = Utility.RotateVector(angularPosition, Quaternion.AngleAxis(angularSpeed, Vector3.back));
            Vector3 target = center + angularPosition;
            Vector3 move = target - lastTarget;
            lastTarget = target;
            return move;  // can't use position
        }
    }
}