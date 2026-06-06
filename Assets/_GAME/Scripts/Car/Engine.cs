using System;

using UnityEngine;

namespace Assets._GAME.Scripts.Car
{
    [Serializable]
    public struct Engine
    {
        public Rigidbody car;
        public Transform location;
        public float acceleration;
        public float maxSpeed;
        public float deceleration;
        public Vector3 velocity;
        public float velocityRatio;
        public float brakeDeceleration;

        private readonly void Accelerate(float input)
        {
            if (velocity.z > maxSpeed) return;
            car.AddForceAtPosition(acceleration * input * location.forward, location.position, ForceMode.Acceleration);
        }

        private readonly void Decelerate() => car.AddForceAtPosition((Input.GetKey(KeyCode.Space) ? brakeDeceleration : deceleration) * Mathf.Abs(velocityRatio) * -location.forward, location.position, ForceMode.Acceleration);

        private void SetVelocityRatio()
        {
            velocity = car.transform.InverseTransformDirection(car.linearVelocity);
            velocityRatio = velocity.z / maxSpeed;
        }

        public void Run(float input)
        {
            Accelerate(input);
            Decelerate();
            SetVelocityRatio();
        }
    }
}
