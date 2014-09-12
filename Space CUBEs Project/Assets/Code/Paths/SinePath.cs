// Little Byte Games
// Author: Steve Yeager
// Created: 2014.01.28
// Edited: 2014.09.10

using System;
using SpaceCUBEs;
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
            return new Vector3(-1f, (float)Math.Cos(time * frequency) * amplitude, 0f) * speed;
        }

        #endregion
    }
}