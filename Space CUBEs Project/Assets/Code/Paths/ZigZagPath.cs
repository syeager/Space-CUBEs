// Little Byte Games
// Author: Steve Yeager
// Created: 2014.01.28
// Edited: 2014.09.10

using SpaceCUBEs;
using UnityEngine;

namespace Paths
{
    public class ZigZagPath : Path
    {
        #region Public Fields

        public Vector3 angle = new Vector3(-1f, 1f, 0f);
        public float time = 1.5f;

        #endregion

        #region Private Fields

        private float lapTime;

        #endregion

        #region Path Overrides

        public override void Initialize(Transform transform)
        {
            base.Initialize(transform);

            lapTime += time / 2f;
        }


        public override Vector3 Direction(float deltaTime)
        {
            lapTime += deltaTime;
            if (lapTime >= time)
            {
                lapTime = 0f;
                angle.y *= -1f;
            }

            return angle.normalized * speed;
        }

        #endregion
    }
}