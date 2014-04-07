// Steve Yeager
// 1.26.2014

using UnityEngine;

namespace Paths
{
    public class SinePath : Path
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
            return new Vector3(-1f, Mathf.Sin(time * frequency) * amplitude, 0f) * speed * deltaTime;
        }

        #endregion
    }
}