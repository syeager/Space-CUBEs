// Little Byte Games

using System.Collections;
using Annotations;
using UnityEngine;

namespace SpaceCUBEs
{
    // Space CUBEs Project-csharp
    // Author: Steve Yeager
    // Created: 2014.03.26
    // Edited: 2014.06.11
    
    /// <summary>
    /// 
    /// </summary>
    public class SidewinderMissile : Hitbox
    {
        #region References

        private Rigidbody myRigidbody;

        #endregion

        #region Public Fields

        public float allowedDist = 10f;
        public float dummyRotation = 70f;
        public float dummyDistancePercentage = 0.4f;
        public float angularAccelerationPercentage = 0.1f;
        public float dummyRotationModifier = 0.5f;

        #endregion

        #region Private Fields

        private float speed;
        private float angularSpeed;
        private float homingTime;
        private int dummyTargets;
        private Transform target;
        private Vector3 dummyTarget;
        private Vector3 velocity;

        #endregion

        #region MonoBehaviour Overrides

        [UsedImplicitly]
        private void Start()
        {
            myRigidbody = rigidbody;
            GetComponent<Health>().DieEvent += OnDieHandler;
        }

        [UsedImplicitly]
        private void FixedUpdate()
        {
            myRigidbody.MovePosition(myRigidbody.position + velocity * deltaTime);
        }

        #endregion

        #region Hitbox Overrides

        public void Initialize(Ship sender, float damage, float speed, float angularSpeed, float homingTime, int dummyTargets, Transform target)
        {
            Initialize(sender, damage, Vector3.zero);

            this.speed = speed;
            this.angularSpeed = angularSpeed;
            this.homingTime = homingTime;
            this.dummyTargets = dummyTargets;
            this.target = target;

            GetComponent<Health>().Initialize();

            StartCoroutine(Fire());
        }

        #endregion

        #region Private Methods

        private IEnumerator Fire()
        {
            // find initial target position
            Vector3 targetPosition = target.position;

            // reset dummy rotation
            float rotation = dummyRotation;
            Quaternion rotationTarget = Quaternion.identity;

            // dummy targets
            for (int i = 1; i <= dummyTargets; i++)
            {
                // select dummy target
                Vector3 difference = targetPosition - myTransform.position;
                difference *= dummyDistancePercentage;
                difference = Utility.RotateVector(difference, Quaternion.AngleAxis(Random.Range(-rotation, rotation), Vector3.forward));
                rotation *= dummyRotationModifier;
                dummyTarget = myTransform.position + difference;

                // reset angular speed
                float rotatingSpeed = angularSpeed;

                while (Vector3.Distance(myTransform.position, dummyTarget) > allowedDist)
                {
                    // move
                    velocity = myTransform.forward * speed;

                    // rotate
                    rotationTarget = Quaternion.LookRotation(dummyTarget - myTransform.position, Vector3.back);
                    myTransform.rotation = Quaternion.Slerp(myTransform.rotation, rotationTarget, rotatingSpeed * deltaTime);

                    // increase rotation speed
                    rotatingSpeed += angularSpeed * angularAccelerationPercentage * deltaTime;

                    yield return null;
                }
            }

            // homing timer
            float timer = homingTime;

            while (true)
            {
                // move
                velocity = myTransform.forward * speed;

                // rotate
                if (timer > 0f)
                {
                    timer -= deltaTime;

                    targetPosition = target.position;
                    rotationTarget = Quaternion.LookRotation((targetPosition - myTransform.position).normalized, Vector3.back);
                }
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation, rotationTarget, angularSpeed * deltaTime);

                yield return null;
            }
        }

        #endregion

        #region Event Handlers

        private void OnDieHandler(object sender, DieArgs args)
        {
            myPoolObject.Disable();
        }

        #endregion
    }
}