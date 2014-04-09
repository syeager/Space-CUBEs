// Steve Yeager
// 4.8.2014

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


        #region Path Overrides

        public override Vector3 Direction(float deltaTime)
        {
            time += deltaTime;
            return new Vector3(Mathf.Cos(time), Mathf.Sin(2 * time), 0f) * speed * deltaTime;
        }

        #endregion
    }
}