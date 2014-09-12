// Little Byte Games
// Author: Steve Yeager
// Created: 2014.01.28
// Edited: 2014.09.10

using SpaceCUBEs;
using UnityEngine;

namespace Paths
{
    public class StraightPath : Path
    {
        #region Public Fields

        public float angle;
        public float rotation;

        #endregion

        #region Private Fields

        private Vector3 direction;

        #endregion

        #region Path Overrides

        public override void Initialize(Transform transform)
        {
            base.Initialize(transform);

            myTransform.Rotate(Vector3.back, rotation, Space.World);
            direction = Utility.RotateVector(Vector3.left, Quaternion.AngleAxis(angle, Vector3.back)) * speed;
        }


        public override Vector3 Direction(float deltaTime)
        {
            return direction;
        }

        #endregion
    }
}