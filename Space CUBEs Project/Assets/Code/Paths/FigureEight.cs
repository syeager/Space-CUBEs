// Steve Yeager
// 4.8.2014

using System;
using UnityEngine;

namespace Paths
{
    public class FigureEight : Path
    {
        #region Public Fields

        public float frequencyX = 2f;
        public float frequencyY = 1f;

        #endregion

        #region Private Fields

        private float time;

        #endregion

        #region Const Fields

        private const float Offset = (float)Math.PI/2;
        

        #endregion


        #region Path Overrides

        public void Initialize(Transform transform, float speed, float frequencyX = 2f, float frequencyY = 1f)
        {
            base.Initialize(transform);

            this.speed = speed;
            this.frequencyX = frequencyX;
            this.frequencyY = frequencyY;

            time = Offset;
        }


        public override Vector3 Direction(float deltaTime)
        {
            time += deltaTime;
            return myTransform.TransformDirection(new Vector3((float)Math.Cos(frequencyX * time), 0f, (float)Math.Sin(frequencyY * time)) * speed * deltaTime);
        }

        #endregion
    }
}