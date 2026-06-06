using System;

using UnityEngine;

namespace Assets._GAME.Scripts.Car
{
    [Serializable]
    public struct Wheel
    {
        public LayerMask driveable;
        public Transform springRoot;
        public Transform tire;
        [Range(.2f, 1)] public float damperZeta;
        public float springStiffness;
        public float springRestLength;
        public float springMaxTravel;
        public float radius;
        public bool grounded;
        public float rpm; // TODO; calc from radius & car speed at point

        private readonly float GetDamperStiffness(Rigidbody car)
        {
            // z = [.2, 1]
            // d = damperStiffness
            // k = springStiffness
            // m = mass
            // z = d / (2 * sqrt(k * m))
            // z * 2 * sqrt(k * m) = d
            return (float)(damperZeta * 2 * Math.Sqrt(springStiffness * car.mass));
        }

        public float GetSpringForce(Rigidbody car, float input)
        {
            // assumes linear spring compression
            Vector3 position = springRoot.position;
            Vector3 up = springRoot.up;
            Vector3 down = -up;
            float springMaxLength = springRestLength + springMaxTravel;
            float totalMaxLength = springMaxLength + radius;
            // only visual
            tire.Rotate(Vector3.right, rpm * input * Time.deltaTime, Space.Self);
            if (!Physics.Raycast(position, down, out RaycastHit hit, totalMaxLength, driveable))
            {
                // no hit
                Debug.DrawLine(position, position + totalMaxLength * down, Color.green);
                // only visual
                tire.transform.position = position - up * springRestLength;
                grounded = false;
                return 0;
            }
            // hit
            Debug.DrawLine(position, hit.point, Color.red);
            // only visual
            tire.transform.position = hit.point + up * radius;
            grounded = true;
            // spring
            float springLength = hit.distance - radius;
            float springCompression = (springRestLength - springLength) / springMaxTravel;
            // damper
            float springVelocity = Vector3.Dot(car.GetPointVelocity(position), up);
            float damperForce = GetDamperStiffness(car) * springVelocity;
            // force
            float springForce = springStiffness * springCompression;
            float netForce = springForce - damperForce;
            return netForce;
        }
    }
}
