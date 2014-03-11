// Steve Yeager
// 3.10.2014

using UnityEngine;
using System;

namespace Paths
{
    public class StopPath : Path
    {
        public Vector3[] stops;
        public float[] delays;

        private int cursor;
        private Vector3 target;
        private float delayTimer = -1f;


        public override void Initialize(Transform transform)
        {
            base.Initialize(transform);

            target = myTransform.position;
        }


        public override Vector3 Direction(float deltaTime)
        {
            if (Vector3.Distance(myTransform.position, stops[cursor]) <= 0.5f)
            {
                delayTimer = delays[cursor];
                cursor++;
            }

            if (delayTimer > 0f)
            {
                delayTimer -= deltaTime;
                return Vector3.zero;
            }
            else
            {
                Vector3 move = (stops[cursor] - target).normalized*speed*deltaTime;
                target += move;
                return move;
            }
        }
    }
}