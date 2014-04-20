// Steve Yeager
// 3.10.2014

using UnityEngine;

namespace Paths
{
    public class StopPath : Path
    {
        #region Public Fields

        public Vector3[] stops = new Vector3[1];
        public float[] delays = new float[1];

        #endregion

        #region Private Fields

        private int cursor;
        private Vector3 target;
        private float delayTimer = -1f;

        #endregion

        #region Const Fields

        private const float DistanceBuffer = 0.5f;
        
        #endregion


        #region Path Overrides

        public override void Initialize(Transform transform)
        {
            base.Initialize(transform);

            target = myTransform.position;
        }


        public override Vector3 Direction(float deltaTime)
        {
            if (cursor >= stops.Length) return Vector3.zero;

            if (Vector3.Distance(myTransform.position, stops[cursor]) <= DistanceBuffer)
            {
                delayTimer = delays[cursor];
                cursor++;

                if (cursor >= stops.Length)
                {
                    return Vector3.zero;
                }
            }

            if (delayTimer > 0f)
            {
                delayTimer -= deltaTime;
                return Vector3.zero;
            }
            else
            {
                Vector3 move = (stops[cursor] - target).normalized * speed * deltaTime;
                target += move;
                return move;
            }
        }

        #endregion
    }
}