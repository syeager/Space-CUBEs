﻿// Little Byte Games
// Author: Steve Yeager
// Created: 2014.03.10
// Edited: 2014.09.10

using SpaceCUBEs;
using UnityEngine;

namespace Paths
{
    public class StopPath : Path
    {
        #region Public Fields

        public Vector3[] stops = new Vector3[1];
        public float[] delays = {1f};

        #endregion

        #region Private Fields

        private int cursor;
        private float delayTimer = -1f;

        #endregion

        #region Const Fields

        private const float DistanceBuffer = 0.5f;

        #endregion

        #region Path Overrides

        public override Vector3 Direction(float deltaTime)
        {
            if (cursor >= stops.Length)
            {
                return Vector3.zero;
            }

            if (Vector3.Distance(myTransform.position, stops[cursor]) <= DistanceBuffer)
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
                Vector3 move = (stops[cursor] - myTransform.position).normalized * speed;
                return move;
            }
        }

        #endregion
    }
}