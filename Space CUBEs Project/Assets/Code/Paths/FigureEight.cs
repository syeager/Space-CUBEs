// Steve Yeager
// 4.8.2014

using System;
using UnityEngine;

namespace Paths
{
    public class FigureEight : Path
    {
        #region Public Fields

        public float amplitude = 0.5f;
        public float frequency = 1f;

        #endregion

        #region Private Fields

        private float time;

        #endregion

        #region Const Fields

        private const float Offset = (float)Math.PI/2;

        #endregion


        #region Path Overrides

        public override void Initialize(Transform transform)
        {
            base.Initialize(transform);

            time = Offset;
        }


        public override Vector3 Direction(float deltaTime)
        {
            time += deltaTime;
            return new Vector3((float)Math.Cos(2f * time), (float)Math.Sin(time), 0f) * speed * deltaTime;
        }

        #endregion
    }
}